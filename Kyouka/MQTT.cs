using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Server;

namespace kyouka
{
    public static class MQTT
    {
        public static async Task<IMqttServer> StartServer()
        {
            var server = new MqttFactory().CreateMqttServer();
            await server.StartAsync(new MqttServerOptions());
            return server;
        }

        public static void printApplicationMessageInfo(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
            Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            Console.WriteLine();
        }

        public static async Task<IMqttClient> StartClient(BaseMessageHandler handler)
        {
            var client = new MqttFactory().CreateMqttClient();

            client.UseApplicationMessageReceivedHandler(e =>
            {
                var Payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                if (Payload.IndexOf('|') != -1)
                {
                    var Channel = e.ApplicationMessage.Topic;
                    var Type = Payload.Split("|")[0];
                    var Context = Payload.Substring(Type.Length + 1);

                    GeneratedMessage.Dispatch(handler, Channel, Type, Context);
                }
            });

            var clientOptions = new MqttClientOptionsBuilder()
                .WithClientId("ClientID")
                .WithTcpServer("127.0.0.1")
                .WithCleanSession()
                .Build();

            await client.ConnectAsync(clientOptions);
            foreach (var channel in GeneratedMessage.channels)
            {
                await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(channel).Build());
            }

            return client;
        }

        public static async Task Publish(IMqttClient client, string topic, string type, string context)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload($"{type}|{context}")
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();
            await client.PublishAsync(message);
        }
    }
}
