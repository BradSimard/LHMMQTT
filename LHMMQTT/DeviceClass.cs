using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LHMMQTT {
    public enum DeviceClass {
        [Unit("V"), SensorClass("voltage"), ValueFormat("{0:F2}")]
        Voltage,

        [Unit("A"), SensorClass("current"), ValueFormat("{0:F2}")]
        Current,

        [Unit("W"), SensorClass("power"), ValueFormat("{0:F2}")]
        Power,

        [Unit("MHz"), SensorClass("frequency"), ValueFormat("{0:0}")]
        Clock,

        [Unit("°C"), SensorClass("temperature"), ValueFormat("{0:0}")]
        Temperature,

        [Unit("%"), ValueFormat("{0:0}")]
        Load,

        [Unit("MHz"), SensorClass("frequency"), ValueFormat("{0:0}")]
        Frequency,

        [Unit("RPM"), SensorClass("speed")]
        Fan,

        [Unit("L/min"), SensorClass("volume_flow_rate")]
        Flow,

        [ValueFormat("{0:0}")]
        Control,

        [ValueFormat("{0:0}")]
        Level,

        [ValueFormat("{0:0}")]
        Factor,

        [Unit("GB"), SensorClass("data_size")]
        Data,

        [Unit("MB"), SensorClass("data_size")]
        SmallData,

        [Unit("bps"), SensorClass("")]
        Throughput,

        [Unit("s"), SensorClass("")]
        TimeSpan,

        [Unit("Wh"), SensorClass("energy")]
        Energy,

        [Unit("dB"), SensorClass("sound_pressure"), ValueFormat("{0:0}")]
        Noise,

        [Unit("S/m"), SensorClass("")]
        Conductivity,

        [Unit("%"), SensorClass("moisture")]
        Humidity
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class UnitAttribute : Attribute {
        public string Unit { get; }
        public UnitAttribute(string unit) => Unit = unit;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SensorClassAttribute : Attribute {
        public string SensorClass { get; }
        public SensorClassAttribute(string sensorClass) => SensorClass = sensorClass;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ValueFormatAttribute : Attribute {
        public string ValueFormat { get; }
        public ValueFormatAttribute(string valueFormat) => ValueFormat = valueFormat;
    }


    public static class DeviceClassExtensions {
        public static string GetUnit(this DeviceClass value) {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            return member?.GetCustomAttribute<UnitAttribute>()?.Unit ?? string.Empty;
        }

        public static string GetSensorClass(this DeviceClass value) {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            return member?.GetCustomAttribute<SensorClassAttribute>()?.SensorClass ?? string.Empty;
        }

        public static string GetValueFormat(this DeviceClass value) {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            return member?.GetCustomAttribute<ValueFormatAttribute>()?.ValueFormat ?? "{0}";
        }
    }
}
