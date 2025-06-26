using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGHSBilling.Areas.Masters.Models;

namespace CGHSBilling.API.Masters.Interfaces
{
    public interface ICommonMasterRepository
    {
        List<CommonMasterModel> GetAllState();
        List<CommonMasterModel> GetAllPatientType();
        List<CommonMasterModel> GetCitybyStateId(int stateId);
        List<CommonMasterModel> GetAllRoomType();
        List<CommonMasterModel> GetAllTypeOfAdmission();
        List<CommonMasterModel> GetAllTypeofManagement();
        List<CommonMasterModel> GetAllActiveGender();
        List<CommonMasterModel> GetAllManagementLinking(int Userid);
    }
}
