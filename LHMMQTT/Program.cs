using System.Text.RegularExpressions;
using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using LibreHardwareMonitor.Hardware;
using Newtonsoft.Json.Linq;
using LHMMQTT;
using System.Diagnostics;
using Serilog;

class Program {
    static async Task Main(string[] args) {
        // Configure logger
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("app.log")
            .CreateLogger();

        // Make sure logger is closed when application exits
        AppDomain.CurrentDomain.ProcessExit += (s, e) => {
            Log.CloseAndFlush();
        };

        // Load app settings for configuration values
        if (!Settings.LoadFromConfig()) {
            Log.Information("Please correct issues with config before running again!");
            return;
        }

        // Run initialization process
        // This is where we figure out the information about the PC itself as well as setup all the sensors
        Device hardware = new Device();
        await hardware.InitializeSensors();

        // Infinitely send updates
        Task backgroundTask = doUpdate(hardware);
        await backgroundTask;
    }

    /// <summary>
    /// Retrieve sensor information on the computer's hardware and send out to the MQTT server
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    static async Task doUpdate(Device device) {
        // Connect to MQTT
        MQTTClient client = new MQTTClient();
        await client.Connect();

        // Used to make delay as close to the configured value as possible
        Stopwatch stopwatch = new Stopwatch();

        while (true) {
            stopwatch.Start();

            // Make sure the MQTT connection is still alive
            if (!client.IsConnected()) {
                await client.Connect();
            }

            // Retrieve updated sensor information from the pc
            IList<IHardware> sensors = device.UpdateSensors();

            // Perform sensor updates in MQTT
            List<Task> updateValueTasks = new List<Task>();
            foreach (IHardware hdw in sensors)
            {
                foreach (ISensor hdwSensor in hdw.Sensors) {
                    // Find the related configured sensor and update its value
                    string uniqueId = Sensor.CalculateUniqueId(device.Name, hdw.Name, hdwSensor.Name, hdwSensor.SensorType);
                    Sensor sensor = device.HaSensors.First(sensor => sensor.UniqueId == uniqueId);
                    if (sensor != null) {
                        updateValueTasks.Add(sensor.SetValue(client, hdwSensor.Value));
                    } else {
                        Log.Information($"Couldn't find sensor for '{uniqueId}'");
                    }
                }
            }

            // Wait for all sensor value updates to finish
            await Task.WhenAll(updateValueTasks);

            stopwatch.Stop();

            // Update interval is configurable (defaults to every 10 seconds)
            // Will subtract out whatever time it took to actually run the update so we can stay as close to the requested delay as possible
            // Quickest update is 1 second
            // TODO: If the updates aren't able to happen in the requested time, temporarily increase the delay and output a warning
            await Task.Delay(Convert.ToInt32(Math.Max((Settings.Current.Updates.Delay * 1000 - stopwatch.ElapsedMilliseconds), 1 * 1000)));
        }
    }
}
