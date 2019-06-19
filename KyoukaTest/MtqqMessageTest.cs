using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using kyouka;

namespace kyoukaTest
{

    [TestClass]
    public class UnitTest1
    {
        private static AutoResetEvent noticeEvent = new AutoResetEvent(false);
        private static AutoResetEvent publicChatEvent = new AutoResetEvent(false);
        private static AutoResetEvent whisperEvent = new AutoResetEvent(false);

        private static NoticeMessage noticeMessage;
        private static PublicChatMessage publicChatMessage;
        private static WhisperMessage whisperMessage;

        public class MessageHandler : BaseMessageHandler
        {
            public NoticeEventHandler OnNoticeEvent(NoticeMessage message)
            {
                noticeMessage = message;
                noticeEvent.Set();
                return null;
            }

            public PublicChatEventHandler OnPublicChatEvent(PublicChatMessage message)
            {
                publicChatMessage = message;
                publicChatEvent.Set();
                return null;
            }

            public WhisperEventHandler OnWhisperEvent(WhisperMessage message)
            {
                whisperMessage = message;
                whisperEvent.Set();
                return null;
            }
        }

        [TestInitialize]
        public async Task InitServer()
        {
            await MQTT.StartServer();
        }

        [TestMethod]
        public async Task MtqqMessageTest()
        {
            MessageHandler messageHandler = new MessageHandler();
            var client = await MQTT.StartClient(messageHandler);

            /**
             * PublicChatMessage|{"author":"sokcuri","message":"hello world"}
             */
            await MQTT.Publish(client, "chat", "PublicChatMessage", "{'author': 'Sokcuri', 'message': 'hello world'}");
            await MQTT.Publish(client, "chat", "WhisperMessage", "{'sender': 'foo', 'receiver': 'bar', 'message': 'message'}");
            await MQTT.Publish(client, "notice", "NoticeMessage", "{'notice': 'notice message', 'important': 2, 'sticky': true}");

            if (!WaitHandle.WaitAll(new WaitHandle[] { noticeEvent, publicChatEvent, whisperEvent }, TimeSpan.FromSeconds(5)))
            {
                Assert.Fail("Timeout");
            }

            Assert.AreEqual("Sokcuri", publicChatMessage.author);
            Assert.AreEqual("hello world", publicChatMessage.message);

            Assert.AreEqual("foo", whisperMessage.sender);
            Assert.AreEqual("bar", whisperMessage.receiver);
            Assert.AreEqual("message", whisperMessage.message);

            Assert.AreEqual("notice message", noticeMessage.notice);
            Assert.AreEqual(2, noticeMessage.important);
            Assert.AreEqual(true, noticeMessage.sticky);

        }
    }
}
