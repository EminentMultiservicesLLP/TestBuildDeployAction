using System.Collections.Generic;
using CGHSBilling.Areas.HospitalForms.Models;
using CGHSBilling.Areas.Masters.Models;
using CommonDataLayer.DataAccess;
using CGHSBilling.API.ScanDoc;

namespace CGHSBilling.API.HospitalForms.Intefaces
{
    public interface IOPDBillingRepository
    {
        List<RequestSubmissionOPDModel> GetAllOPDRequest();
        List<CommonMasterModel> OPDServicesConsumed();
        List<ServiceMasterModel> GetAllOPDServiceMasterByCategoryId(int categoryId, int userId, int hospitalType, int patientType, int stateId, int cityId, int gender, int roomTypeId = 0, bool loadBill = false,string ConnectionString="");
        List<CommonMasterModel> GetOPDRequestDetailById(int requestId, bool isReport = false,string ConnectionString="");
        List<CommonMasterModel> GetDefaultOPDServicesDetail(int requestId, bool isReport = false,string ConnectionString="");
        List<CommonMasterModel> GetRequestManuallyOPDAddedDetail(int requestId,string ConnectionString);
        PatientModel GetPatientData(int requestId, string ConnectionString);

        RequestSubmissionOPDModel CreateRequest(RequestSubmissionOPDModel modelData, DBHelper dbHelper);
        RequestSubmissionOPDModel UpdateRequest(RequestSubmissionOPDModel modelData, DBHelper dbHelper);
        void CreateOPDRequestDetail(CommonMasterModel entity, DBHelper dbHelper);
        void CreateOPDRequestManullyAddedDetail(CommonMasterModel entity, DBHelper dbHelper);
        void CreateOPDRequestDefaultServicesDetail(CommonMasterModel entity, DBHelper dbHelper);
        void CreatePatientLink(PatientModel Patient, DBHelper dbHelper);
    }
}
