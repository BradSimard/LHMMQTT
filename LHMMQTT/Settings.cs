using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LHMMQTT {
    public class AppSettings {
        public required MQTT MQTT { get; set; }
        public required Updates Updates { get; set; }
        public required Sensors Sensors { get; set; }
    }

    public class MQTT {
        public required string Hostname { get; set; }
        public required int Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class Updates {
        public required int Delay { get; set; }
    }

    public class Sensors {
        public required bool CPU { get; set; }
        public required bool GPU { get; set; }
        public required bool Memory { get; set; }
        public required bool Motherboard { get; set; }
        public required bool Controller { get; set; }
        public required bool Networking { get; set; }
        public required bool Storage { get; set; }
    }

    public static class Settings {
        public static AppSettings? Current { get; private set; }

        public static bool LoadFromConfig() {
            // Read contents of config.yaml
            string yaml = String.Empty;
            try {
                yaml = File.ReadAllText("config.yaml");
            } catch (Exception err) {
                Console.WriteLine($"Failed to load config.yaml: {err.Message}");
                return false;
            }

            // Attempt to parse YAML into settings values
            try {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();
                var config = deserializer.Deserialize<Dictionary<string, AppSettings>>(yaml);

                Current = config["AppSettings"];
            } catch (Exception err) {
                Console.WriteLine($"Failed to parse configuration values: {err.Message}");
                return false;
            }

            return true;
        }
    }
}