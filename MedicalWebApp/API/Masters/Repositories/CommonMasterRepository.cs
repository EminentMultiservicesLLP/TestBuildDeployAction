using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CGHSBilling.API.Masters.Interfaces;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Caching;
using CommonDataLayer.DataAccess;

namespace CGHSBilling.API.Masters.Repositories
{
    public class CommonMasterRepository: ICommonMasterRepository
    {
        public List<CommonMasterModel> GetAllState()
        {
            List<CommonMasterModel> list = null;
           
            if (!MemoryCaching.CacheKeyExist(CachingKeys.State.ToString()))
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    DBParameterCollection paramCollection = new DBParameterCollection();
                    paramCollection.Add(new DBParameter("UserID", Convert.ToInt32(HttpContext.Current.Session["AppUserId"]), DbType.Int32));
                    DataTable dtPaper = dbHelper.ExecuteDataTable("dbsp_mst_GetAllState", paramCollection, CommandType.StoredProcedure);
                    //DataTable dtPaper = dbHelper.ExecuteDataTable("Select StateId,StateCode,StateName,Deactive from StateMaster where isnull(deactive,0)=0  order by StateName asc", CommandType.Text);
                    list = dtPaper.AsEnumerable()
                        .Select(row => new CommonMasterModel
                        {
                            Id = row.Field<int>("StateId"),
                            Name = row.Field<string>("StateName"),
                            Code = row.Field<string>("StateCode"),
                            Deactive = row.Field<bool>("Deactive"),
                            IsClientState = row.Field<bool>("IsClientState")
                        }).ToList();
                }
                MemoryCaching.AddCacheValue(CachingKeys.State.ToString(), list);
            }
            else
            {
                list = (List<CommonMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.State.ToString()));
            }
            return list;
        }

        public List<CommonMasterModel> GetAllPatientType()
        {
            List<CommonMasterModel> list = null;
            if (!MemoryCaching.CacheKeyExist(CachingKeys.PatientType.ToString()))
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    DataTable dtPaper = dbHelper.ExecuteDataTable("Select PatientTypeId,Code,PatientType,isnull(Sequence,0)Sequence,Deactive from PatientTypeMaster where isnull(deactive,0)=0  order by PatientType asc", CommandType.Text);
                    list = dtPaper.AsEnumerable()
                        .Select(row => new CommonMasterModel
                        {
                            Id = row.Field<int>("PatientTypeId"),
                            Name = row.Field<string>("PatientType"),
                            Code = row.Field<string>("Code"),
                            Sequence = row.Field<int>("Sequence"),
                            Deactive = row.Field<bool>("Deactive")
                        }).ToList();
                }
                MemoryCaching.AddCacheValue(CachingKeys.PatientType.ToString(), list);
            }
            else
            {
                list = (List<CommonMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.PatientType.ToString()));
            }
            return list;
        }

        public List<CommonMasterModel> GetCitybyStateId(int stateId)
        {
            List<CommonMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("UserID", Convert.ToInt32(HttpContext.Current.Session["AppUserId"]), DbType.Int32));
                paramCollection.Add(new DBParameter("StateId", stateId, DbType.Int32));
                DataTable dtPaper = dbHelper.ExecuteDataTable("dbsp_mst_GetCitybyStateId", paramCollection, CommandType.StoredProcedure);
                //DataTable dtPaper = dbHelper.ExecuteDataTable("Select CityId,CityCode,CityName,isnull(Deactive,0)Deactive from CityMaster where isnull(deactive,0)=0 and StateId = " + stateId+ " order by CityName asc", CommandType.Text);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        Id = row.Field<int>("CityId"),
                        Name = row.Field<string>("CityName"),
                        Code = row.Field<string>("CityCode"),
                        Deactive = row.Field<bool>("Deactive"),
                        IsClientCity = row.Field<bool>("IsClientCity")
                    }).ToList();
            }

            return list;
        }

        public List<CommonMasterModel> GetAllRoomType()
        {
            List<CommonMasterModel> list = null;
            if (!MemoryCaching.CacheKeyExist(CachingKeys.RoomType.ToString()))
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    DataTable dtPaper = dbHelper.ExecuteDataTable("Select RoomTypeId,Code,RoomType,isnull(deactive,0)Deactive,PriorityLevel, IsValidForEntitlement from RoomTypeMaster where isnull(deactive,0)=0  order by RoomType asc", CommandType.Text);
                    list = dtPaper.AsEnumerable()
                        .Select(row => new CommonMasterModel
                        {
                            Id = row.Field<int>("RoomTypeId"),
                            RoomPriorityLevel = row.Field<int>("PriorityLevel"),
                            Name = row.Field<string>("RoomType"),
                            Code = row.Field<string>("Code"),
                            Deactive = row.Field<bool>("Deactive"),
                            IsValidForEntitlement = row.Field<bool>("IsValidForEntitlement")
                        }).ToList();
                }
                MemoryCaching.AddCacheValue(CachingKeys.RoomType.ToString(), list);
            }
            else
            {
                list = (List<CommonMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.RoomType.ToString()));
            }
            return list;
        }

        public List<CommonMasterModel> GetAllTypeOfAdmission()
        {
            List<CommonMasterModel> list = null;
            if (!MemoryCaching.CacheKeyExist(CachingKeys.AdmissionType.ToString()))
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    DataTable dtPaper = dbHelper.ExecuteDataTable("Select AdmisionTypeId,Code,AdmissionType,isnull(deactive,0)Deactive from TypeofAdmissionMaster where isnull(deactive,0)=0  order by AdmissionType asc", CommandType.Text);
                    list = dtPaper.AsEnumerable()
                        .Select(row => new CommonMasterModel
                        {
                            Id = row.Field<int>("AdmisionTypeId"),
                            Name = row.Field<string>("AdmissionType"),
                            Code = row.Field<string>("Code"),
                            Deactive = row.Field<bool>("Deactive")
                        }).ToList();
                }
                MemoryCaching.AddCacheValue(CachingKeys.AdmissionType.ToString(), list);
            }
            else
            {
                list = (List<CommonMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.AdmissionType.ToString()));
            }
            return list;
        }

        public List<CommonMasterModel> GetAllTypeofManagement()
        {
            List<CommonMasterModel> list = null;
            if (!MemoryCaching.CacheKeyExist(CachingKeys.ManagementType.ToString()))
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    DataTable dtPaper = dbHelper.ExecuteDataTable("Select ManagementTypeId,Code,ManagementType,isnull(deactive,0)Deactive from ManagementTypeMaster where isnull(deactive,0)=0  order by ManagementTypeId asc", CommandType.Text);
                    list = dtPaper.AsEnumerable()
                        .Select(row => new CommonMasterModel
                        {
                            Id = row.Field<int>("ManagementTypeId"),
                            Name = row.Field<string>("ManagementType"),
                            Code = row.Field<string>("Code"),
                            Deactive = row.Field<bool>("Deactive")
                        }).ToList();
                }
                MemoryCaching.AddCacheValue(CachingKeys.ManagementType.ToString(), list);
            }
            else
            {
                list = (List<CommonMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.ManagementType.ToString()));
            }
            
            return list;
        }

        public List<CommonMasterModel> GetAllManagementLinking( int Userid)
        {
            List<CommonMasterModel> list = null;
            if (!MemoryCaching.CacheKeyExist(CachingKeys.ManagementLinking.ToString()))
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    DBParameterCollection paramCollection = new DBParameterCollection();
                    paramCollection.Add(new DBParameter("UserId", Userid, DbType.Int32));
                    DataTable dtPaper = dbHelper.ExecuteDataTable("dbsp_GetAllManagementLinking", paramCollection, CommandType.StoredProcedure);
                    list = dtPaper.AsEnumerable()
                        .Select(row => new CommonMasterModel
                        {
                            Id = row.Field<int>("ManagementTypeId"),
                            Name = row.Field<string>("ManagementType"),
                            Code = row.Field<string>("Code"),
                            Deactive = row.Field<bool>("Deactive")
                        }).ToList();
                }
            }

                return list;
        }


        public List<CommonMasterModel> GetAllActiveGender()
        {
            List<CommonMasterModel> list = null;
            if (!MemoryCaching.CacheKeyExist(CachingKeys.ActiveGender.ToString()))
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    DataTable dtPaper = dbHelper.ExecuteDataTable("Select GenderId,GenderType,isnull(Deactive,0)Deactive " +
                                                                  "from GenderTypeMaster where isnull(Deactive,0)=0", CommandType.Text);
                    list = dtPaper.AsEnumerable()
                        .Select(row => new CommonMasterModel
                        {
                            Id = row.Field<int>("GenderId"),
                            Name = row.Field<string>("GenderType"),
                            Deactive = row.Field<bool>("Deactive")
                        }).ToList();
                }
                MemoryCaching.AddCacheValue(CachingKeys.ActiveGender.ToString(), list);
            }
            else
            {
                list = (List<CommonMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.ActiveGender.ToString()));
            }

            return list;
        }
    }
}