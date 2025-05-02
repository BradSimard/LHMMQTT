using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using LibreHardwareMonitor.Hardware;
using Newtonsoft.Json.Linq;

namespace LHMMQTT {
    public class Device {
        private Computer _computer;
        public string Name { get; }
        public string Identifier { get; }
        internal HashSet<Sensor> HaSensors = new HashSet<Sensor>();

        private JObject _haDevice;
        

        public Device() {
            Name = getSafeName();
            Identifier = Name; // TODO: There's probably something better we can use

            // TODO: need a way to make this configurable
            _computer = new Computer {
                IsCpuEnabled = Settings.Current.Sensors.CPU,
                IsGpuEnabled = Settings.Current.Sensors.GPU,
                IsMemoryEnabled = Settings.Current.Sensors.Memory,
                IsMotherboardEnabled = Settings.Current.Sensors.Motherboard,
                IsControllerEnabled = Settings.Current.Sensors.Controller,
                IsNetworkEnabled = Settings.Current.Sensors.Networking,
                IsStorageEnabled = Settings.Current.Sensors.Storage
            };

            _computer.Open();

            // Create an MQTT device (the computer) so that all sensors can be attached to it
            _haDevice = new JObject();
            _haDevice.Add("name", Name);
            _haDevice.Add("identifiers", new JArray { Identifier });
            _haDevice.Add("model", $"{RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture.ToString()})");
            _haDevice.Add("manufacturer", "LHMMQTT");

            // Make sure the sensors are updated on creation or the values will be incorrect and/or missing
            UpdateSensors();
        }

        ~Device() {
            _computer.Close();
        }

        public async Task InitializeSensors() {
            // Connect to MQTT
            MQTTClient client = new MQTTClient();
            await client.Connect();

            // Iterate over hardware sensors and initialize them
            foreach (IHardware hdw in _computer.Hardware) {
                foreach (ISensor hdwSensor in hdw.Sensors) {
                    Sensor sensor = new Sensor(hdw, hdwSensor, _haDevice);
                    await sensor.Configure(client);
                    HaSensors.Add(sensor);
                }
            }

            await client.Disconnect();
        }

        public IList<IHardware> UpdateSensors() {
            // Get updated sensors
            _computer.Accept(new UpdateVisitor());

            return _computer.Hardware;
        }

        string getSafeName() {
            return Regex.Replace(System.Environment.MachineName, @"[^a-zA-Z0-9]", "");
        }
    }
}
