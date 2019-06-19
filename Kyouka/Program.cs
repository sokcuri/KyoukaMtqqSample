using System;
using System.Threading;
using System.Threading.Tasks;

namespace kyouka
{
    public class MessageHandler : BaseMessageHandler
    {
        public NoticeEventHandler OnNoticeEvent(NoticeMessage message)
        {
            throw new NotImplementedException();
        }

        public PublicChatEventHandler OnPublicChatEvent(PublicChatMessage message)
        {
            throw new NotImplementedException();
        }

        public WhisperEventHandler OnWhisperEvent(WhisperMessage message)
        {
            throw new NotImplementedException();
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            await MQTT.StartServer();

            MessageHandler messageHandler = new MessageHandler();

            await MQTT.StartClient(messageHandler);
            Thread.Sleep(1000);
        }
    }
}
