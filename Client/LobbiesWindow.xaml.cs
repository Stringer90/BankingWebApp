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
    /// Interaction logic for LobbiesWindow.xaml
    /// </summary>
    public partial class LobbiesWindow : Window
    {
        private string Username;
        //private List<string> Lobbies;
        private ObservableCollection<string> LobbiesList;
        private BusinessServerInterface foob;
        public LobbiesWindow(string InUsername, BusinessServerInterface InFoob)
        {
            InitializeComponent();
            Username = InUsername;
            foob = InFoob;
            LobbiesList = new ObservableCollection<string>();
            for (int i = 0; i < 10; i++)
            {
                LobbiesList.Add(i.ToString());
            }
            ShowLobbies();

        }

        private void ShowLobbies()
        {
            LobbyList.ItemsSource = LobbiesList;
        }

        private void CreateLobbyBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(LobbyName.Text))
            {
                /*
                if(foob.CreateLobby(LobbyName.Text))
                {
                    //add to the list of lobbies and update the list on screen.
                    Lobbies.Add($"{LobbyName.Text}");
                    ShowLobbies();
                    LobbyStatusText.Text = "Lobby Created!";
                }
                else
                {
                    LobbyStatusText.Text = "Lobby already exists";
                }
                */
                LobbiesList.Add(LobbyName.Text);
                ShowLobbies();
                LobbyStatusText.Text = "Lobby Created!";
            }
        }

        private void JoinLobbyBtn_Click(object sender, RoutedEventArgs e)
        {
            if(LobbyList.SelectedItem != null)
            {
                //get the name of the lobby

                //foob.AddUserToLobby(Username, LobbyList.SelectedItem.ToString());
                LobbyChatWindow lobbyChatWindow = new LobbyChatWindow(foob, Username);
                this.Close();
                lobbyChatWindow.Show();

            }
        }
    }
}
