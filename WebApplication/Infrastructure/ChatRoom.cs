using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DtoModel;

namespace WebApplication.Infrastructure
{
    public class ChatRoom
    {
        private int _counter = 1;
        private readonly ReplaySubject<ChatMessage> _messages = new ReplaySubject<ChatMessage>(TimeSpan.FromMinutes(5.0));

        public ChatRoomDetails Details { get; set; }

        public IObservable<ChatMessage> GetOldMessages()
        {
            return Observable.Defer(() =>
            {
                var token = DateTime.UtcNow;
                var dummyMessage = new ChatMessage
                {
                    MessageId = -1,
                    MsgTime = token
                };
                _messages.OnNext(dummyMessage);
                return _messages
                    .TakeWhile(msg => msg.MsgTime != token)
                    .Where(msg => msg.MessageId != -1);
            });
        }

        public ChatMessage PushMessage(ChatMessage message)
        {
            message.MessageId = _counter++;
            message.MsgTime = DateTime.Now;
            if (message.Message != "DONE")
                _messages.OnNext(message);
            return message;
        }
    }
}