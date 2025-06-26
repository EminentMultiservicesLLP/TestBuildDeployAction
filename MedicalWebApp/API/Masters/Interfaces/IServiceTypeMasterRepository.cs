using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Models;

namespace CGHSBilling.API.Masters.Interfaces
{
    public interface IServiceTypeMasterRepository
    {
        bool CheckDuplicateUpdate(CheckDuplicateModel chkmodal);
        ServiceTypeMasterModel SaveServiceType(ServiceTypeMasterModel jsonData);
        ServiceTypeMasterModel UpdateServiceType(ServiceTypeMasterModel jsonData);
        bool CheckDuplicateInsert(CheckDuplicateModel chkmodal);
        List<ServiceTypeMasterModel> GetAllServiceTypeMaster();
        List<CommonMasterModel> GetAllCategory();
        List<ServiceCategoryLinkingModel> GetAllLinkedCategoryByTypeId(int ServiceTypeId);
    }
}
