using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat.Client.Model;
using DtoModel;
using Microsoft.AspNet.SignalR.Client;

namespace Chat.Client.ControlLogic
{
    public class ChatClient : IDisposable
    {
        private readonly HubConnection _hubConnection;
        private readonly IHubProxy _proxy;
        private readonly IObservable<ChatMessage> _messageStream;

        public ChatClient(string user)
        {
            string url = ConfigurationManager.AppSettings["ServiceBaseUrl"];
            _hubConnection = new HubConnection(url);
            // 1. Setup authorization and create hub

            //("Authorization", "Basic " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(user + ":")));
            //url: "ChatService");


            // 3. implement connection status stream
            UserConnectionStatus = Observable.Empty<UserConnectionNotification>();

            // 6. implement receiving messages
            _messageStream = Observable.Never<ChatMessage>();

        }

        public IObservable<UserConnectionNotification> UserConnectionStatus { get; private set; }

        public async Task<IEnumerable<ChatUser>> GetUsers()
        {
            // 2. get users from the proxy
            return new ChatUser[] {new ChatUser() {IsOnline = false, Name = "Kowalski"}};
        }

        public async Task<IEnumerable<ChatRoomDetails>> GetChatRooms()
        {
            // 4. fetch chat rooms
            return new ChatRoomDetails[] {new ChatRoomDetails {Created = DateTime.Now, Description = "", RoomName = "Madagaskar"}};
        }

        public async Task SendMessage(ChatMessage message)
        {
            // 7. implement sending messages
            return;
        }

        public IObservable<ChatMessage> EnterRoom(string room)
        {
            // 5. implement entering and leaving room
            return Observable.Never<ChatMessage>();
        }

        public void Dispose()
        {
            _hubConnection.Dispose();            
        }
    }
}
