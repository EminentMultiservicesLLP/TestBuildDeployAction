using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGHSBilling.API.Masters.Interfaces
{
   public interface IPatientTypeMasterRepository
    {
        bool CheckDuplicateUpdate(CheckDuplicateModel chkmodal);
        PatientTypeMasterModel SavePatientTypeMaster(PatientTypeMasterModel jsonData);
        bool CheckDuplicateInsert(CheckDuplicateModel chkmodal);
        List<PatientTypeMasterModel> GetAllPatientTypeMaster();
    }
}
