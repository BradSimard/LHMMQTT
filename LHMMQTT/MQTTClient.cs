using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using Serilog;

namespace LHMMQTT {
    internal class MQTTClient {
        private HiveMQClient _client;
        private HiveMQClientOptions _connectionOptions = new HiveMQClientOptions();

        public MQTTClient() {
            _connectionOptions.Host = Settings.Current.MQTT.Hostname;
            _connectionOptions.Port = Settings.Current.MQTT.Port;
            _connectionOptions.UserName = Settings.Current.MQTT.Username;
            _connectionOptions.Password = Settings.Current.MQTT.Password;

            _client = new HiveMQClient(_connectionOptions);
        }

        public bool IsConnected() {
            return _client.IsConnected();
        }

        public async Task Connect() {  
            if (!_client.IsConnected()) {
                await _client.ConnectAsync();
            }
        }

        public async Task Disconnect() {
            if (_client.IsConnected()) {
                await _client.DisconnectAsync();
            }
        }

        public async Task Publish(string topic, string payload) {
            await Publish(topic, payload, HiveMQtt.MQTT5.Types.QualityOfService.ExactlyOnceDelivery);
        }

        public async Task Publish(string topic, string payload, HiveMQtt.MQTT5.Types.QualityOfService qos) {
            await _client.PublishAsync(topic, payload, qos);
        }
    }
}
