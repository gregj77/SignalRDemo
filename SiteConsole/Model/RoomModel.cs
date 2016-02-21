using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using Chat.Client.ControlLogic;
using DtoModel;
using SiteConsole.Annotations;

namespace Chat.Client.Model
{
    public class RoomModel : INotifyPropertyChanged
    {
        private readonly ChatClient _client;
        private readonly SendMessageCommand _command;
        private readonly ObservableCollection<ChatMessage> _messages = new ObservableCollection<ChatMessage>();
        private readonly SynchronizationContext _current = SynchronizationContext.Current; 

        private string _name;
        private bool _isRoomEnabled;
        private string _inputMessage = string.Empty;
        private readonly SerialDisposable _roomPresence = new SerialDisposable();
        public event PropertyChangedEventHandler PropertyChanged;

        public RoomModel(ChatClient client)
        {
            _client = client;
            _command = new SendMessageCommand(OnSendMessage, () => _inputMessage.Length > 0);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));            
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value; 
                OnPropertyChanged();
            }
        }

        public bool IsRoomEnabled
        {
            get { return _isRoomEnabled; }
            set
            {
                _isRoomEnabled = value;
                if (!_isRoomEnabled)
                {
                    Messages.Clear();
                    InputMessage = string.Empty;
                }
                TogglePresence(_isRoomEnabled);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ChatMessage> Messages
        {
            get { return _messages; }
        }

        public string InputMessage
        {
            get { return _inputMessage; }
            set
            {
                _inputMessage = value;
                _command.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        public ICommand SendMessage
        {
            get { return _command; }   
        }

        private async void OnSendMessage()
        {
            await _client.SendMessage(new ChatMessage
            {
                Message = InputMessage,
                Room = Name
            });
            InputMessage = string.Empty;
        }

        private void TogglePresence(bool shouldEnter)
        {
            if (shouldEnter)
            {
                _roomPresence.Disposable = _client
                    .EnterRoom(Name)
                    .ObserveOn(_current)
                    .Subscribe(msg => _messages.Insert(0, msg));
            }
            else
            {
                _roomPresence.Disposable = Disposable.Empty;
            }            
        }
    }
}
