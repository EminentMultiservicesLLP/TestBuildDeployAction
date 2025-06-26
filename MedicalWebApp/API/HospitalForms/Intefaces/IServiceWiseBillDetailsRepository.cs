using CGHSBilling.Areas.HospitalForms.Models;
using CGHSBilling.Areas.Masters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGHSBilling.API.HospitalForms.Intefaces
{
    public interface IServiceWiseBillDetailsRepository
    {
        List<ServiceWiseBillDetailsModel> GetServiceMasterList();
        List<ServiceWiseBillDetailsModel> GetAllServiceWiseBillDtls_Datewise(string Fromtime, string Totime, int ServiceId, string ConnectionString, int CategoryId, int UserID, int BillTypeId);
        List<RequestSubmissionBillNoModel> GetAllBillNo(int userid);
        List<CommonMasterModel> GetAllCategories();
    }
}
