using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LHMMQTT {
    public class AppSettings {
        public MQTT MQTT { get; set; }
        public Updates Updates { get; set; }
    }

    public class MQTT {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Updates {
        public int Delay { get; set; }
    }

    public static class Settings {
        public static AppSettings Current { get; private set; }

        public static void LoadFromConfig() {
            var yaml = File.ReadAllText("config.yaml");

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            var config = deserializer.Deserialize<Dictionary<string, AppSettings>>(yaml);

            Current = config["AppSettings"];
        }
    }
}