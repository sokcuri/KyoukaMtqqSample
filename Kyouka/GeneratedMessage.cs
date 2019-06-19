using System;
using Newtonsoft.Json;

namespace kyouka
{
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
        public static string[] channels = new string[]
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
}
