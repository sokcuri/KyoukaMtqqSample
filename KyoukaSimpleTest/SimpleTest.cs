using System;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KyoukaSimpleTest
{
    public class NoticeMessage
    {
        public string notice;
        public int important;
        public bool sticky;
    }

    public class WhisperMessage
    {
        public string sender;
        public string receiver;
        public string message;
    }

    public class PublicChatMessage
    {
        public string author;
        public string message;
    }

    public delegate void PublicChatEventHandler(PublicChatMessage message);
    public delegate void WhisperEventHandler(WhisperMessage message);
    public delegate void NoticeEventHandler(NoticeMessage message);

    public interface BaseMessageHandler
    {
        PublicChatEventHandler OnPublicChatEvent(PublicChatMessage message);
        WhisperEventHandler OnWhisperEvent(WhisperMessage message);
        NoticeEventHandler OnNoticeEvent(NoticeMessage message);
    }

    public class GeneratedMessage
    {
        public static string[] availableChannels = new string[]
        {
            "chat", "notice"
        };

        public static void Dispatch(BaseMessageHandler handler, string Channel, string Type, string Context)
        {
            if (Channel == "chat" && Type == "PublicChatMessage")
            {
                var msg = JsonConvert.DeserializeObject<PublicChatMessage>(Context);
                handler.OnPublicChatEvent(msg);
            }
            else if (Channel == "chat" && Type == "WhisperMessage")
            {
                var msg = JsonConvert.DeserializeObject<WhisperMessage>(Context);
                handler.OnWhisperEvent(msg);
            }
            else if (Channel == "notice" && Type == "NoticeMessage")
            {
                var msg = JsonConvert.DeserializeObject<NoticeMessage>(Context);
                handler.OnNoticeEvent(msg);
            }
            else throw new ArgumentOutOfRangeException();
        }
    }

    class Sample
    {
        public static void Handle(BaseMessageHandler handler, string channel, string text)
        {
            if (text.IndexOf('|') != -1)
            {
                var type = text.Split("|")[0];
                var context = text.Substring(type.Length + 1);

                GeneratedMessage.Dispatch(handler, channel, type, context);
            }
        }
    }

    class MockHandler : BaseMessageHandler
    {
        public PublicChatMessage publicChatMsg = null;
        public WhisperMessage whisperMsg = null;
        public NoticeMessage noticeMsg = null;

        public PublicChatEventHandler OnPublicChatEvent(PublicChatMessage message)
        {
            publicChatMsg = message;
            return null;
        }

        public WhisperEventHandler OnWhisperEvent(WhisperMessage message)
        {
            whisperMsg = message;
            return null;
        }
        public NoticeEventHandler OnNoticeEvent(NoticeMessage message)
        {
            noticeMsg = message;
            return null;
        }
    }

    [TestClass]
    public class SimpleTest
    {
        [TestMethod]
        public void TestListen()
        {
            var handler = new MockHandler();
            var text = @"PublicChatMessage|{'author': 'Sokcuri', 'message': 'hello world'}";
            Sample.Handle(handler, "chat", text);
            Assert.IsNotNull(handler.publicChatMsg);
        }
    }
}
