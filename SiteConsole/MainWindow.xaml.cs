using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Threading;
using Chat.Client.ControlLogic;
using Chat.Client.Model;
using SiteConsole;

namespace Chat.Client
{
    public partial class MainWindow 
    {
        private ChatClient _chatClient;
        private readonly ObservableCollection<UserModel> _users = new ObservableCollection<UserModel>();
        private readonly ObservableCollection<RoomModel> _rooms = new ObservableCollection<RoomModel>();

        public ObservableCollection<UserModel> Users { get { return _users;} }
        public ObservableCollection<RoomModel> Rooms { get { return _rooms; } }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            if (!App.IsBencharkMode)
            {
                Loaded += MainWindow_Loaded;
            }
            else
            {
                Hide();
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.ShowDialog();
            App.User = login.User;
            _chatClient = new ChatClient(login.User);

            var users = await _chatClient.GetUsers();
            foreach (var model in users.Select(u => new UserModel {Name = u.Name, IsOnline = u.IsOnline}))
            {
               _users.Add(model);
            }

            _chatClient
                .UserConnectionStatus
                .ObserveOn(new DispatcherSynchronizationContext())
                .Subscribe(notification =>
                {
                    _users.First(u => string.Equals(u.Name, notification.UserName)).IsOnline = notification.IsConnected;
                });

            var rooms = await _chatClient.GetChatRooms();
            foreach (var room in rooms.Select(r => new RoomModel(_chatClient) { Name = r.RoomName }))
            {
                _rooms.Add(room);                
            }
        }
    }
}
