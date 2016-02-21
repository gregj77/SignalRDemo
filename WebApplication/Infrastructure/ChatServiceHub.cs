using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DtoModel;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;


namespace WebApplication.Infrastructure
{
    [HubName("ChatService")]
    public class ChatServiceHub : Hub
    {
        public IEnumerable<ChatUser> GetUsers()
        {
            return UsersRepository.GetUsers();
        }

        public IEnumerable<ChatRoomDetails> GetChatRooms()
        {
            return ChatRoomManager.Instance.GetRooms();
        }

        public async void EnterRoom(string roomName)
        {
            await Groups.Add(Context.ConnectionId, roomName);
            var stream = ChatRoomManager.Instance.GetPublishedMessages(roomName);
            stream.Subscribe(msg => Clients.Caller.OnNewChatMessage(msg));
        }

        public async void LeaveRoom(string roomName)
        {
            await Groups.Remove(Context.ConnectionId, roomName);
        }

        public async void SendMessage(ChatMessage message)
        {
            message = ChatRoomManager.Instance.OnNewMessage(message, Context.User.Identity.Name);
            await Clients.Group(message.Room).OnNewChatMessage(message);
        }

        public async override Task OnConnected()
        {
            var user = UsersRepository.UserLoggedIn(Context.User.Identity.Name);
            await Clients.AllExcept(Context.ConnectionId).NotifyUserConnected(user.Name);
            await base.OnConnected();
        }

        public async override Task OnDisconnected(bool stopCalled)
        {
            var user = UsersRepository.UserLoggedOff(Context.User.Identity.Name);
            await Clients.AllExcept(Context.ConnectionId).NotifyUserDisconnected(user.Name);
            await base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
    }
}
