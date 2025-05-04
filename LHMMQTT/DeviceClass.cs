using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Serilog;

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

        [Unit("C"), SensorClass("temperature"), ValueFormat("{0:0}")] // °C degree symbol doesn't seem to render correctly and I don't know why
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

        [Unit("GB"), SensorClass("data_size"), ValueFormat("{0:0}")]
        Data,

        [Unit("MB"), SensorClass("data_size"), ValueFormat("{0:0}")]
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
    public class UnitAttribute(string unit) : Attribute {
        public string Unit { get; } = unit;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SensorClassAttribute(string sensorClass) : Attribute {
        public string SensorClass { get; } = sensorClass;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ValueFormatAttribute(string valueFormat) : Attribute {
        public string ValueFormat { get; } = valueFormat;
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
