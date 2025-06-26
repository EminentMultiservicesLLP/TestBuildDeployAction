using System.Collections.Generic;
using CGHSBilling.Models;

namespace CGHSBilling.API.AdminPanel.Interfaces
{
    public abstract class IUserAccessInterface
    {
        public abstract List<ParentMenuRights> GetMenuByUser(int userId, string ConnectionString);
        public abstract int SaveUserAccess(MenuUserRightsModel accessData, string ConnectionString);
        //public abstract int SaveAcademicAccess(List<SubjectMasterModel> AccessData);
        //public abstract IEnumerable<SubjectMasterModel> GetAllStdSub();
        //public abstract List<SubjectMasterModel> GetSubAccessData(int userId);
    }
}