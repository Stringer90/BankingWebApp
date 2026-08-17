using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BusinessTier
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class BusinessServer : BusinessServerInterface
    {
        private DataServerInterface DataServer;

        public BusinessServer()
        {
            ChannelFactory<DataServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            //Set the URL and create the connection!
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            DataServer = foobFactory.CreateChannel();
        }
        
        public bool DoesUserExist(string pUsername)
        {
            List<string> users = DataServer.GetUserNames();
        
            foreach (string user in users) 
            { 
                if (string.Equals(user, pUsername))
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool DoesLobbyExist(string pLobbyName)
        {
            List<string> lobbies = DataServer.GetLobbyNames();
        
            foreach (string user in lobbies)
            {
                if (string.Equals(user, pLobbyName))
                {
                    return true;
                }
            }
            return false;
        }
        
        // Interface
        public bool CreateUser(string pUsername)
        {
            if (!DoesUserExist(pUsername))
            {
                DataServer.AddUser(pUsername);
                return true;
            }
            return false;
        }
        
        // Interface
        public void RemoveUser(string pUsername)
        {
            DataServer.RemoveUser(pUsername);
        }
        
        // Interface
        public bool CreateLobby(string pLobbyName)
        {
            if (!DoesLobbyExist(pLobbyName))
            {
                DataServer.AddLobby(pLobbyName);
                return true;
            }
            return false;
        }
        
        // Interface
        public void AddUserToLobby(string pUsername, string pLobbyName)
        {
            DataServer.AddUserToLobby(pUsername, pLobbyName);
        }
        
        // Interface
        public void RemoveUserFromLobby(string pUsername, string pLobbyName)
        {
            // Remove username from the lobby's list of users
            // Remove all dms that reference the user (to and from)
            DataServer.RemoveUserFromLobby(pUsername, pLobbyName);
        }
        
        // Interface
        public List<string> GetLobbyChat(string pLobbyName)
        {
            return DataServer.GetLobbyChat(pLobbyName);
        }
        
        // Interface
        public List<string> GetLobbyUsers(string pLobbyName)
        {
            return DataServer.GetLobbyUsers(pLobbyName);
        }
        
        // Interface
        public List<string> GetLobbyFileNames(string pLobbyName)
        {
            return DataServer.GetLobbyFileNames(pLobbyName);
        }
        
        // Interface
        public List<string> GetLobbyDm(string pLobbyName, string pUsername1, string pUsername2)
        {
            return DataServer.GetLobbyDm(pLobbyName, pUsername1, pUsername2);
        }
        
        // Interface
        public Object GetLobbyFile(string pLobbyName, string pFileName)
        {
            return DataServer.GetLobbyFile(pLobbyName, pFileName);
        }
        
        // Interface
        public void SendMsg(string pLobbyName, string pMessage)
        {
            DataServer.AddMsg(pLobbyName, pMessage);
        }
        
        // Interface
        public void SendDm(string pLobbyName, string pUsername1, string pUsername2, string pMessage)
        {
            DataServer.AddDm(pLobbyName, pUsername1, pUsername2, pMessage);
        }
        
        //Interface
        public void SendFile(string pLobbyName, string pFileName, Object pFileObject)
        {
            DataServer.AddFile(pLobbyName, pFileName, pFileObject);
        }

    }
}
