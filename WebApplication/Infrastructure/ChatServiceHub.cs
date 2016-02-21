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
        // 2a Get Users from the UsersRepository.GetUsers();


        // 4a Get rooms from manager ChatRoomManager.Instance.GetRooms();

        // 5a enter and exit room, send old messages var stream = ChatRoomManager.Instance.GetPublishedMessages(roomName);

        // 7a implement sending messages message = ChatRoomManager.Instance.OnNewMessage(message, Context.User.Identity.Name);

        // 3a) log in / log off user, notify clients
        // var user = UsersRepository.UserLoggedIn(Context.User.Identity.Name);
        // var user = UsersRepository.UserLoggedOff(Context.User.Identity.Name);
    }
}
