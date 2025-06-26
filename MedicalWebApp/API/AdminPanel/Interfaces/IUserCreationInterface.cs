using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGHSBilling.Areas.AdminPanel.Models;

namespace CGHSBilling.API.AdminPanel.Interfaces
{
    public interface IUserCreationInterface
    {
        List<UserCreationModel> GetUserCode(string ConnectionString);
        List<UserCreationModel> GetUserDetails(string ConnectionString);
        int SaveUser(UserCreationModel model, string ConnectionString);
        UserCreationModel GetUserDetailsByUserId(int userId,string ConnectionString);
        bool CheckDuplicateItem(string LoginName, string EmailID, int typeid, string type);
        bool CheckDuplicateUpdate(string EmailID, string LoginName, int typeid, string type);
        List<UserCreationModel> GetClientTracker(string ConnectionString);
    }
}
