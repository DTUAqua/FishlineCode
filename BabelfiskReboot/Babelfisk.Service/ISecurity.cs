using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Babelfisk.Entities.SprattusSecurity;
using Babelfisk.Entities;

namespace Babelfisk.Service
{
    [ServiceContract(SessionMode=SessionMode.NotAllowed)]
    public interface ISecurity
    {
        [OperationContract]
        Users LogonUser();

        [OperationContract]
        Users GetUser(string strUserName);

        [OperationContract]
        List<Users> GetUsers();

        [OperationContract]
        Users GetUserById(int intId);

        [OperationContract]
        List<Role> GetRoles();

        [OperationContract]
        List<FishLineTasks> GetTasks();

        [OperationContract]
        bool CanDeleteUser(Users user);

        [OperationContract]
        DatabaseOperationResult UpdateUser(Users user);

        [OperationContract]
        DatabaseOperationResult UpdateRole(ref Role r);
    }
}
