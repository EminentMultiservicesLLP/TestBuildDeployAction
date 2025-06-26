using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.QueryCollection.Masters
{
    public class MasterQueries
    {
        public const string CheckDuplicateItem = "dbsp_mst_CheckDuplicateItem";
        public const string CheckDuplicateUpdate = "dbsp_mst_CheckDuplicateUpdate";
        public const string GetMasterCode = "dbsp_mst_GetMasterCode";

        public const string SaveServiceType = "dbsp_mst_SaveServiceType";
        public const string SaveServiceMaster = "dbsp_mst_SaveServiceMaster";
        public const string CreateTariffMaster = "dbsp_mst_CreatetariffMaster";
        public const string UpdateTariffMaster = "dbsp_mst_UpdatetariffMaster";
        public const string CreateTariffDetails = "dbsp_mst_CreatetariffDetails";
        public const string GetTariffMaster = "dbsp_mst_gettariffMaster";
        public const string GetTariffDetailById = "dbsp_mst_getTariffDetailById";
        public const string GetTariffMasterforCopy = "dbsp_mst_getTariffDetailforCopy";
        public const string GetTariff_forCopy = "dbsp_mst_getTariff_forCopy";
        public const string GetTariffDetails_forCopy = "dbsp_mst_getTariffDetails_forCopy";
        public const string GetAllServiceMasterByCategory = "dbsp_mst_GetAllServiceMasterByCategory";
        public const string GetAllServiceMasterByCategoryId = "dbsp_mst_GetAllServiceMasterByCategoryId";

        public const string GetServiceMasterByCategoryId = "dbsp_mst_GetServiceMasterByCategoryId";
        public const string GetServiceMasterByCategoryRoomId = "dbsp_mst_GetServiceMasterByCategoryRoomId";
        public const string CreateNotification = "dbsp_mst_CreateNotification";
        public const string GetAllNotification = "dbsp_mst_GetAllNotification";
        public const string SavePatientTypeMaster = "dbsp_mst_SavePatientType";
        public const string CreateSrviceCategoryLinking = "dbsp_mst_CreateSrviceCategoryLinking";
        public const string GetAllLinkedCategoryByTypeId = "dbsp_mst_GetAllLinkedCategoryByTypeId";
        public const string GetAllServiceMaster = "dbsp_mst_GetAllServiceMaster";
        public const string CreateServiceGenderLinking = "dbsp_mst_CreateServiceGenderLinking";
        public const string UpdateServiceMaster = "dbsp_mst_UpdateServiceMaster";
        public const string GetAllLinkedGenderByServiceId = "dbsp_mst_GetAllLinkedGenderByServiceId";
        public const string GetServicesByServiceTypeId = "dbsp_mst_GetServicesByServiceTypeId";
        public const string CreateAutoServiceAllocationMaster = "dbsp_mst_CreateAutoServiceAllocationMaster";
        public const string CreateAutoServiceAllocationDtl = "dbsp_mst_CreateAutoServiceAllocationDtl";
        public const string UpdateAutoServiceAllocationDtl = "dbsp_mst_UpdateAutoServiceAllocationDtl";
        public const string GetAutoServiceAllocation = "dbsp_mst_GetAutoServiceAllocation";
        public const string GetAutoServiceAllocationDetailById = "dbsp_mst_GetAutoServiceAllocationDetailById";
        public const string GetLinkedServicesByServiceType_ServiceId= "dbsp_mst_GetLinkedServicesByServiceType_ServiceId";
        public const string GetLinkedServicesByServiceTypeServiceId = "dbsp_mst_GetLinkedServicesByServiceTypeServiceId";
        public const string GetAllActiveDefaultServiceMaster = "dbsp_mst_GetAllActiveDefaultServiceMaster";
        public const string GetAllActiveDefaultServiceMaster_Rates = "dbsp_mst_GetAllActiveDefaultServiceMaster_Rates";

        public const string GetAllSurgeryMaster = "dbsp_mst_GethspSurgeryList";

        public const string CreateSurgeryMaster = "dbsp_mst_CreateSurgeryMaster";
        public const string UpdateSurgeryMaster = "dbsp_mst_UpdateSurgeryMaster";
        public const string GetSurgeryMaster = "dbsp_mst_GetSurgeryMaster";
        public const string CreateSurgeryDetail = "dbsp_hsp_CreateRequestSurgeryDetail";

        public const string GetRoomEntitlementList = "dbsp_mst_getRoomEntitlementList";

        public const string GetCancerSurgeryList = "dbsp_mst_GetCancerSurgeryList";
        public const string GetServicesList = "dbsp_mst_GetServicesList";

        public const string SaveServiceCategoryLinking = "dbsp_mst_SaveServiceCategoryLinking";
        public const string SaveServiceTypeManagementtypeLinking = "dbsp_mst_SaveServiceTypeManagementtypeLinking";

        public const string GetAllLinkedRecordById = "dbsp_mst_GetAllLinkedRecordById";

        public const string GetAllDetail = "dbsp_hsp_GetAllDetailPatient";
        public const string SaveCertificate = "dbsp_hsp_mst_SaveCertificate";
        public const string GetAllCertificate = "dbsp_hsp_GetCertificateDetail";

        public const string GetRates = "dbsp_mst_GetRates";
        public const string GetServiceTariff = "dbsp_mst_GetServiceTariff";
        public const string UpdateRates = "dbsp_mst_UpdateRates";


    }
}