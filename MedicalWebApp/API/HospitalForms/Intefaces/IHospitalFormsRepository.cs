using System.Collections.Generic;
using CGHSBilling.Areas.HospitalForms.Models;
using CGHSBilling.Areas.Masters.Models;
using CommonDataLayer.DataAccess;
using CGHSBilling.API.ScanDoc;

namespace CGHSBilling.API.HospitalForms.Intefaces
{
    public interface IHospitalFormsRepository
    {
        List<CommonMasterModel> ServicesConsumedRightDiv();
        //RequestSubmissionModel CreateRequest(RequestSubmissionModel modelData);
        RequestSubmissionModel CreateRequest(RequestSubmissionModel modelData, DBHelper dbHelper);
        RequestSubmissionModel UpdateRequest(RequestSubmissionModel modelData, DBHelper dbHelper);
        void CreateRequestDetail(CommonMasterModel detail, DBHelper dbHelper);
        List<RequestSubmissionModel> GetAllgeneratedRequest();
        void CreateRequestBedChargeDetail(BedCharges entity, DBHelper dbHelper);
        void CreateRequestAdmissionDetail(AdmissionSummary modelData, DBHelper dbHelper);
        void CreatePatientLink(PatientModel Patient,DBHelper dbHelper);

        List<AdmissionSummary> AdmissionDetailById(int requestId);
        List<CommonMasterModel> GetRequestDetailById(int requestId,string ConnectionString);
        List<ServiceMasterModel> GetAllServiceMasterByCategory(string category, int userId, int hospitalType, int patientType, int stateId, int cityId, int gender, int roomTypeId = 0);
        List<ServiceMasterModel> GetAllServiceMasterByCategoryId(int categoryId, int userId, int hospitalType, int patientType, int stateId, int cityId, int gender, int roomTypeId = 0, bool loadBill = false,string ConnectionString="");
        List<TariffDetailModel> GetTariffByUserId(int userId, int hospitalType);
        List<BedCharges> GetRequestBedChargeDetails(int requestId,string ConnectionString);
        List<CommonMasterModel> ServicesConsumedLeftDiv(int managementTypeId = 0, string ConnectionString ="");
        List<ServiceMasterModel> GetServiceMasterByCategoryId(int category, int userId, int hospitalType, int patientType, int stateId, int cityId, int gender);
        RequestSubmissionModel_Report GetAllgeneratedRequestById(int requestId,string ConnectionString);
        void CreateRequestOtDetail(AdmissionSummary modelData, DBHelper dbHelper, int type);
        void CreateRequestPharmacyDetail(CommonMasterModel modelData, DBHelper dbHelper);
        void CreateRequestManullyAddedDetail(CommonMasterModel modelData, DBHelper dbHelper);
        void CreateRequestSurgeryManullyAddedDetail(SurgeryManualServices modelData, DBHelper dbHelper);
        void CreateRequestDefaultServicesDetail(CommonMasterModel modelData, DBHelper dbHelper);
        List<AdmissionSummary> GetRequestOtDetailById(int requestId);
        List<SurgerySummary> GetRequestSurgeryDetailById(int requestId);
        List<CommonMasterModel> GetRequestPharmacyDetail(int requestId,string ConnectionString);
        List<CommonMasterModel> GetRequestManuallyAddedDetail(int requestId, string ConnectionString);
        List<SurgeryManualServices> GetRequestSurgeryManuallyAddedDetail(int requestId, string ConnectionString);
        List<ServiceMasterModel> GetServiceMasterByCategoryRoomId(int category, int userId, int hospitalType, int patientType, int roomTypeId, int stateId, int cityId, int gender, int requestId = 0);
        List<CommonMasterModel> GetServiceConsumedInRequest(int RequestId);
        List<ScanDocEntity> GetScanDocUrl(int RequestId);
        List<SurgeryMasterModel> GetSurgeryMasterList();
        List<CommonMasterModel> GetDefaultServicesDetail(int requestId, string ConnectionString);
        PatientModel GetPatientData(int requestId, string ConnectionString);
        void CreateRequestSurgeryDetail(SurgerySummary modelData, DBHelper dbHelper);

        List<CommonMasterModel> GetLinkedServiceRatesByCategory_Services(string ServiceTypeList, string ServiceIdList, int RoomTypeId, int HospitalTypeId, int PatientTypeId, int GenderId, int StateId, int CityId,string ConnectionString);
        void CreateRequestAutoLinkedService(int RequestId, DBHelper dbHelper);

        bool BillAmountDeduction(int requestId);

        List<CancerSurgeryModel> GetCancerSurgeryList();
        List<ServiceMasterModel> GetServicesList();
        RequestSubmissionModel GetDeductionType();
        

    }
}
