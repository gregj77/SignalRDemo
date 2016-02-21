using System.ComponentModel;
using System.Runtime.CompilerServices;
using SiteConsole.Annotations;

namespace Chat.Client.Model
{
    public class UserModel : INotifyPropertyChanged
    {
        private string _name;
        private bool _isOnline;
        public event PropertyChangedEventHandler PropertyChanged;

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

        public bool IsOnline
        {
            get { return _isOnline; }
            set
            {
                _isOnline = value;
                OnPropertyChanged();
            }
        }
    }
}
