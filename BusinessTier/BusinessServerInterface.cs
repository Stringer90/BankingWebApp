using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace BusinessTier
{
    [ServiceContract]
    public interface BusinessServerInterface
    {
        // Returns:
        // true : user successfully created
        // false : user name already in use
        [OperationContract]
        bool CreateUser(string pUsername);
        
        [OperationContract]
        void RemoveUser(string pUsername);
        
        // Returns:
        // true : lobby successfully created
        // false : lobby name already in use
        [OperationContract]
        bool CreateLobby(string pLobbyName);
        
        [OperationContract]
        void AddUserToLobby(string pUsername, string pLobbyName);
        
        [OperationContract]
        void RemoveUserFromLobby(string pUsername, string pLobbyName);
        
        // Can return 'null' if no one has sent a message yet.
        // If returns 'null', do nothing
        [OperationContract]
        List<string> GetLobbyChat(string pLobbyName);
        
        [OperationContract]
        List<string> GetLobbyUsers(string pLobbyName);
        
        // Can return 'null' if no one has sent a file yet.
        // If returns 'null', don't update list of files?
        [OperationContract]
        List<string> GetLobbyFileNames(string pLobbyName);
        
        // Returns:
        //           null  : One or more of the users is not in the lobby (grey-out dm section)
        //     empty list  : No dm sent between the users (check if empty: list.Count == 0)
        // populated list  : 
        [OperationContract]
        List<string> GetLobbyDm(string pLobbyName, string pUsername1, string pUsername2);
        
        // If pFileName ends with '.txt', returns string
        // Else, returns BitMap object
        // (Potential for user to save a file like testfile.txt.txt? or image.jpeg.png?)
        [OperationContract]
        Object GetLobbyFile(string pLobbyName, string pFileName);
        
        [OperationContract]
        void SendMsg(string pLobbyName, string pMessage);
        
        [OperationContract]
        void SendDm(string pLobbyName, string pUsername1, string pUsername2, string pMessage);
        
        [OperationContract]
        void SendFile(string pLobbyName, string pFileName, Object pFileObject);

    }
}
