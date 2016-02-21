using System;
using System.Collections.Generic;
using System.Linq;
using DtoModel;

namespace WebApplication.Infrastructure
{
    public class ChatRoomManager
    {
        static readonly ChatRoomManager _instance = new ChatRoomManager();

        public static ChatRoomManager Instance { get { return _instance;} }

        private readonly IDictionary<string, ChatRoom> _rooms = new Dictionary<string, ChatRoom>();

        private ChatRoomManager()
        {
            var name = "SingalR is cool!";
            _rooms.Add(name, new ChatRoom
            {
                Details = new ChatRoomDetails
                {
                    Created = new DateTime(2015, 01, 05, 20, 00, 01),
                    RoomName = name,
                    Description = "For all who love signals..."
                }
            });

            name = "Some random stuff";
            _rooms.Add(name, new ChatRoom
            {
                Details = new ChatRoomDetails
                {
                    Created = DateTime.Now,
                    RoomName = name,
                    Description = "just talk to me..."
                }
            });

            name = "benchmark";
            _rooms.Add(name, new ChatRoom
            {
                Details = new ChatRoomDetails
                {
                    Created = new DateTime(2000, 01, 02),
                    RoomName = name,
                    Description = "room to run some benchmarks"
                }
            });
        }

        public IEnumerable<ChatRoomDetails> GetRooms()
        {
            return _rooms.Values.Select(p => p.Details).OrderByDescending(p => p.Created);
        }

        public ChatMessage OnNewMessage(ChatMessage message, string user)
        {
            message.Sender = user;
            return _rooms[message.Room].PushMessage(message);
        }

        public IObservable<ChatMessage> GetPublishedMessages(string room)
        {
            return _rooms[room].GetOldMessages();
        }
    }
}
