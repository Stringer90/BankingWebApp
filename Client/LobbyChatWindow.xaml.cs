using BusinessTier;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for LobbyChatWindow.xaml
    /// </summary>
    public partial class LobbyChatWindow : Window
    {
        private ObservableCollection<string> messages;
        private string Username;
        public LobbyChatWindow(BusinessServerInterface foob, string username)
        {
            InitializeComponent();
            Username = username;
            messages = new ObservableCollection<string>();
            //need to get message list from client
            Messages.ItemsSource = messages;
        }

        private void updateMessages()
        {
            //method for updating messages
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MesageInput.Text))
            {
                messages.Add(MesageInput.Text);
                MesageInput.Clear();
            }
        }
    }
}
