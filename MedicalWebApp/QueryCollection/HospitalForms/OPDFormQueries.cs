using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.QueryCollection.HospitalForms
{
    public class OPDFormQueries
    {
        public const string GetAllOPDRequest = "dbsp_hsp_GetAllOPDRequest";
        public const string GetAllOPDServiceType = "dbsp_hsp_GetAllOPDServiceType";
        public const string GetOPDRequestDetailById = "dbsp_hsp_GetOPDRequestDetailById";
        public const string GetOPDRequestDetailById2 = "dbsp_hsp_GetOPDRequestDetailById2";
        public const string GetOPDRequestManuallyAddedDetailById = "dbsp_get_hspRequestManuallyOPDAddedDetail";
        public const string GetOPDRequestDefaultServiceDetailById = "dbsp_get_hspGetDefaultOPDServicesDetail";
        public const string GetOPDRequestDefaultServiceDetailById2 = "dbsp_get_hspGetDefaultOPDServicesDetail2";
        public const string GetOPDGeneratedRequestById = "dbsp_hsp_GetOPDGeneratedRequestById";
        public const string GetPatientData = "dbsp_hsp_OPDGetPatientData";

        public const string CreateOPDRequest = "dbsp_hsp_CreateOPDRequestMaster";
        public const string UpdateOPDRequest = "dbsp_hsp_UpdateOPDRequestMaster";
        public const string CreateOPDRequestDetail = "dbsp_hsp_CreateOPDRequestDetail";
        public const string CreateOPDDefaultServiceDetail = "dbsp_hsp_CreateOPDRequestDefaultServices";
        public const string CreateOPDRequestManullyAddedDetail = "dbsp_CreateOPDRequestManullyAddedDetail";

        public const string GetAllgeneratedOPDRequestByDateWise = "dbsp_GetAllgeneratedOPDRequestByDateWise";
        public const string CreatePatientLink = "dbsp_hsp_OPDCreatePatientLink";


    }
}