using System.Windows;

namespace Chat.Client
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public string User { get; private set; }

        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string user = users.SelectedValue as string;
            if (!string.IsNullOrEmpty(user))
            {
                User = user;
                Close();
            }
        }
    }
}
