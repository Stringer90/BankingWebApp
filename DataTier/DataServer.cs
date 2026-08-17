using System;
using System.Collections.Generic;
using System.Drawing;
using System.ServiceModel;
//using DBInterface;
using DBLib;

namespace DataTier
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class DataServer : DataServerInterface
    {
        private List<string> LobbyNames = new List<string>();
        private List<string> UserNames = new List<string>();
        private Dictionary<string, Lobby> Lobbies= new Dictionary<string, Lobby> ();

        public List<string> GetUserNames()
        {
            return UserNames;
        }

        public List<string> GetLobbyNames()
        {
            return LobbyNames;
        }

        public void AddUser(string pUsername)
        {
            UserNames.Add(pUsername);
        }

        public void RemoveUser(string pUsername)
        {
            UserNames.Remove(pUsername);
        }

        public void AddLobby(string pLobbyName)
        {
            Lobby NewLobby = new Lobby();
            Lobbies.Add(pLobbyName, NewLobby);
            LobbyNames.Add(pLobbyName);
        }

        public void AddUserToLobby(string pUsername, string pLobbyName)
        {
            Lobbies[pLobbyName].AddUser(pUsername);
        }

        // Also remove dms?
        public void RemoveUserFromLobby(string pUsername, string pLobbyName)
        {
            Lobbies[pLobbyName].RemoveUser(pUsername);
        }

        public List<string> GetLobbyChat(string pLobbyName)
        {
            return Lobbies[pLobbyName].GetChat();
        }

        public List<string> GetLobbyUsers(string pLobbyName)
        {
            return Lobbies[pLobbyName].GetUsers();
        }

        public List<string> GetLobbyFileNames(string pLobbyName)
        {
            return Lobbies[pLobbyName].GetFileNames();
        }

        public List<string> GetLobbyDm(string pLobbyName, string pUser1, string pUser2)
        {
            return Lobbies[pLobbyName].GetDm(pUser1, pUser2);
        }

        public Object GetLobbyFile(string pLobbyName, string pFileName)
        {
            return Lobbies[pLobbyName].GetFile(pFileName);
        }

        public void AddMsg(string pLobbyName, string pMessage)
        {
            Lobbies[pLobbyName].AddMsg(pMessage);
        }

        public void AddDm(string pLobbyName, string pUser1, string pUser2, string pMessage)
        {
            Lobbies[pLobbyName].AddDm(pUser1, pUser2, pMessage);
        }

        public void AddFile(string pLobbyName, string pFileName, Object pFileObject)
        {
            Lobbies[pLobbyName].AddFile(pFileName, pFileObject);
        }

    }
}
