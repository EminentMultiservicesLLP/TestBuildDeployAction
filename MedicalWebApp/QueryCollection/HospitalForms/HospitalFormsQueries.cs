using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.QueryCollection.HospitalForms
{
    public class HospitalFormsQueries
    {
        public const string CreateRequest = "dbsp_hsp_CreateRequestMaster";
        public const string CreateRequestDetail = "dbsp_hsp_CreateRequestDetail";
        public const string GetAllgeneratedRequest = "dbsp_hsp_GetAllgeneratedRequest";
        public const string GetRequestDetailById = "dbsp_hsp_GetRequestDetailById";
        public const string GetRequestDetailById2 = "dbsp_hsp_GetRequestDetailById2";
        public const string GetAdmisionDetailById = "dbsp_hsp_GetAdmisionDetailById";
        public const string CreateRequestBedChargeDetail = "dbsp_Create_hspBedChargeDetail";
        public const string CreateRequestAdmissionDetail = "dbsp_hsp_CreateRequestAdmissionDetail";
        public const string UpdateRequest = "dbsp_hsp_UpdateRequestMaster";
        public const string GetTariffByUserId = "dbsp_hsp_GetTariffByUserId";
        public const string GetAllgeneratedRequestById = "dbsp_hsp_GetAllgeneratedRequestById";
        public const string GetAllgeneratedRequestById2 = "dbsp_hsp_GetAllgeneratedRequestById2";
        public const string CreateRequestOtDetail = "dbsp_hsp_CreateRequestOtDetail";
        public const string GetRequestOtDetailById = "dbsp_hsp_GetRequestOtDetailById";
        public const string GetRequestSurgeryDetailById = "dbsp_hsp_GetRequestSurgeryDetailById";
        public const string CreateRequestPharmacyDetail = "dbsp_Create_hspRequestSubmissionPharmacyDetail";
        public const string GetRequestPharmacyDetail = "dbsp_get_hspRequestSubmissionPharmacyDetail";
        public const string GetRequestBedChargeDetail = "dbsp_get_hspRequestSubmissionBedChargeDetail";
        public const string GetRequestBedChargeDetail2 = "dbsp_get_hspRequestSubmissionBedChargeDetail2";
        public const string GetRequestManuallyAddedDetail = "dbsp_get_hspRequestSubmissionManuallyAddedDetail";
        public const string GetServiceConsumedInRequest = "dbsp_get_hspGetServiceConsumedInRequest";
        public const string CreateRequestManullyAddedDetail = "dbsp_CreateRequestManullyAddedDetail";
        public const string CreateRequestDefaultServicesDetail = "dbsp_CreateRequestDefaultServicesDetail";
        public const string GetAllServiceTypeByManagementType = "dbsp_hsp_GetAllServiceTypeByManagementType";
        public const string CreateRequestSurgeryDetail = "dbsp_hsp_CreateRequestSurgeryDetail"; 
        public const string GetDefaultServicesDetail = "dbsp_get_hspGetDefaultServicesDetail";
        public const string GetDefaultServicesDetail2 = "dbsp_get_hspGetDefaultServicesDetail2";
        public const string GetRequestSurgeryDetails = "dbsp_get_hspRequestSubmissionSurgeryChargeDetail";
        public const string GetRequestSurgeryDetails2 = "dbsp_get_hspRequestSubmissionSurgeryChargeDetail2";
        public const string GetRequestSurgeryManuallyAddedDetail = "dbsp_get_hspRequestSurgeryManullyAddedDetail";
        public const string CreateRequestSurgeryManullyAddedDetail = "dbsp_CreateRequestSurgeryManullyAddedDetail";

        public const string GetLinkedServiceRatesByCategory_Services = "GetLinkedServiceRatesByCategory_Services";
        public const string CreateRequestAutoLinkedServices = "dbsp_hsp_CreateRequestAutoLinkedServices";
        public const string GetAutoLinkedServicesDetail = "dbsp_get_hspGetAutoLinkedServiceDetail";
        public const string GetAutoLinkedServicesDetail2 = "dbsp_get_hspGetAutoLinkedServiceDetail2";

        public const string BillAmountDeduction = "dbsp_hsp_DeductBillPayment";

        public const string MakePayment = "dbsp_CreatePayment";

        public const string GetAllgeneratedRequestByDateWise = "dbsp_hsp_GetAllgeneratedRequestDatewise";
        public const string GetDeductionType = "dbsp_hsp_GetDeductionType";

        public const string GetServiceMasterList = "dbsp_hsp_GetServiceMasterList";
        public const string GetAllServiceWiseBillDtls_Datewise = "dbsp_hsp_GetAllServiceWiseBillDtls_Datewise";       
        public const string GetAllOPDServiceWiseBillDtls_Datewise = "dbsp_hsp_GetAllOPDServiceWiseBillDtls_Datewise";
        public const string CreatePatientLink = "dbsp_hsp_CreatePatientLink";
        public const string GetPatientData = "dbsp_hsp_GetPatientData";

        public const string GetAllBillNo = "dbsp_hsp_GetAllBillNo";
    }
}