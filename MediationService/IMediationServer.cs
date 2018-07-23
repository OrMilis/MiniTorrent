using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ClassLibrary;

namespace MediationService
{
    [ServiceContract]
    interface IMediationServer
    {
        [OperationContract]
        bool SignIn(string userDataString);
        [OperationContract]
        bool SignOut(string userDataString);
        [OperationContract]
        string RequestFiles(string fileName);
        [OperationContract]
        string getAllFiles();
        [OperationContract]
        string getAllActiveUsers();
        [OperationContract]
        bool SignUp(String userDataString);
        [OperationContract]
        string SearchUser(string user);
        [OperationContract]
        void setUsers();
        [OperationContract]
        bool deleteUser(string user);
        [OperationContract]
        bool UpdateUser(User user);
        [OperationContract]
        string GetAllUsers();
        [OperationContract]
        void resetAllActiveUsersAndActiveUsers();
    }
}
