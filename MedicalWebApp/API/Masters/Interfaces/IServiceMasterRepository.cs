using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Models;

namespace CGHSBilling.API.Masters.Interfaces
{
   public interface IServiceMasterRepository
    {
        bool CheckDuplicateUpdate(CheckDuplicateModel chkmodal);
        ServiceMasterModel SaveService(ServiceMasterModel jsonData);
        ServiceMasterModel UpdateService(ServiceMasterModel jsonData);
        bool CheckDuplicateInsert(CheckDuplicateModel chkmodal);
        List<ServiceMasterModel> GetAllServiceMaster();
        List<ServiceGenderLinking> GetAllLinkedGenderByServiceId(int ServiceId);
        List<ServiceMasterModel> GetServicesByServiceTypeId(int ServiceTypeId);
        List<ServiceMasterModel> GetAllActiveDefaultServiceMaster();

        List<ServiceMasterModel> GetAllActiveDefaultServiceMaster_WithRates(int RoomTypeId, int HospitalTypeId, int PatientTypeId, int GenderId, int StateId, int CityId,string ConnectionString);
    }
}
