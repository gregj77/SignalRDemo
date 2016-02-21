using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
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
            _hubConnection.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(user + ":")));
            _proxy = _hubConnection.CreateHubProxy("ChatService");

            UserConnectionStatus = Observable
                .Create<UserConnectionNotification>(o =>
                {
                    var resources = new CompositeDisposable();
                    resources.Add(_proxy.On("NotifyUserConnected", (string userName) =>
                    {
                        o.OnNext(new UserConnectionNotification {UserName = userName, IsConnected = true});
                    }));
                    resources.Add(_proxy.On("NotifyUserDisconnected", (string userName) =>
                    {
                        o.OnNext(new UserConnectionNotification {UserName = userName, IsConnected = false});
                    }));
                    return resources;
                })
                .Publish()
                .RefCount();

            _messageStream = Observable
                .Create<ChatMessage>(o =>
                {
                    return _proxy.On("OnNewChatMessage", (ChatMessage message) => o.OnNext(message));
                })
                .Publish()
                .RefCount();

            _hubConnection.Start().Wait();
        }

        public IObservable<UserConnectionNotification> UserConnectionStatus { get; private set; }

        public async Task<IEnumerable<ChatUser>> GetUsers()
        {
            return await _proxy.Invoke<IEnumerable<ChatUser>>("GetUsers");
        }

        public async Task<IEnumerable<ChatRoomDetails>> GetChatRooms()
        {
            return await _proxy.Invoke<IEnumerable<ChatRoomDetails>>("GetChatRooms");
        }

        public async Task SendMessage(ChatMessage message)
        {
            await _proxy.Invoke<ChatMessage>("SendMessage", message);
        }

        public IObservable<ChatMessage> EnterRoom(string room)
        {
            return Observable.Create<ChatMessage>(async o =>
            {
                var result = _messageStream.Where(p => string.Equals(p.Room, room)).Subscribe(o);
                await _proxy.Invoke<string>("EnterRoom", room);
                return result;
            });
        }

        public void Dispose()
        {
            _hubConnection.Dispose();            
        }
    }
}
