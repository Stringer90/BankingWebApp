using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using BusinessTier;


namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BusinessServerInterface foob;
        public MainWindow()
        {
            InitializeComponent();
            var tcp = new NetTcpBinding();
            var URL = "net.tcp://localhost:8200/BusinessService";
            var chanFactory = new ChannelFactory<BusinessServerInterface>(tcp, URL);
            foob = chanFactory.CreateChannel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(IsValidInput(UserName.Text))
            {
                if (foob.CreateUser(UserName.Text))
                {
                    StatusText.Text = "Successfully logged in";
                    LobbiesWindow lobbiesWindow = new LobbiesWindow(UserName.Text, foob);
                    lobbiesWindow.Show();
                    this.Close();
                }
                else
                {
                    StatusText.Text = "User already exists.";
                }
            }
        }

        private bool IsValidInput(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Successfully logged in";
            LobbiesWindow lobbiesWindow = new LobbiesWindow(UserName.Text, foob);
            lobbiesWindow.Show();
            this.Close();
        }
    }
}
