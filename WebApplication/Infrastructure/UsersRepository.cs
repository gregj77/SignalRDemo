using System.Collections.Generic;
using DtoModel;

namespace WebApplication.Infrastructure
{
    public static class UsersRepository
    {
        private static readonly Dictionary<string, ChatUser> _users = new Dictionary<string, ChatUser>
        {
            {"Doe, John", new ChatUser {Name = "Doe, John", IsOnline = false}},
            {"Obama, Barack", new ChatUser {Name = "Obama, Barack", IsOnline = false}},
            {"Merkel, Angela", new ChatUser {Name = "Merkel, Anglea", IsOnline = false}},
            {"Duda, Andrzej", new ChatUser {Name = "Duda, Andrzej", IsOnline = false}},
            {"benchmark", new ChatUser { Name = "benchmark", IsOnline = false}}
        };
        
        public static bool AuthenticateUser(string userName)
        {
            return _users.ContainsKey(userName);
        }

        public static ChatUser UserLoggedIn(string userName)
        {
            _users[userName].IsOnline = true;
            return _users[userName];
        }

        public static ChatUser UserLoggedOff(string userName)
        {
            _users[userName].IsOnline = false;
            return _users[userName];
        }

        public static IEnumerable<ChatUser> GetUsers()
        {
            return _users.Values;
        }
    }
}
