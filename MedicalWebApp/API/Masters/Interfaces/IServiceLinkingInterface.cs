using CGHSBilling.Areas.Masters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGHSBilling.API.Masters.Interfaces
{
   public  interface IServiceLinkingInterface
    {
        List<HospitalServicelinkingModel> GetAllManagementType();
        HospitalServicelinkingModel SaveServiceTypeManagementtypeLinking(HospitalServicelinkingModel jsonData);
        List<HospitalServicelinkingModel> GetAllLinkedRecordById(int HospitalServicelinkingModel);

    }
}
