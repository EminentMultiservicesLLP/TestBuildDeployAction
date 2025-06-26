using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CGHSBilling.API.HospitalForms.Intefaces;
using CGHSBilling.API.Masters.Repositories;
using CGHSBilling.Areas.HospitalForms.Models;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.QueryCollection.HospitalForms;
using CGHSBilling.QueryCollection.Masters;
using CommonDataLayer.DataAccess;
using CommonLayer;
using CommonLayer.Extensions;
using CGHSBilling.API.ScanDoc;
using CGHSBilling.QueryCollection.AdminPanel;

namespace CGHSBilling.API.HospitalForms.Repository
{
    public class HospitalFormsRepository : IHospitalFormsRepository
    {
        private static readonly ILogger Loggger = Logger.Register(typeof(HospitalFormsRepository));
        public List<CommonMasterModel> ServicesConsumedRightDiv()
        {
            List<CommonMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(
                    "Select  ID,NAME from ServicesConsumedDiv where typeid=2 order by ID asc", CommandType.Text);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        Id = row.Field<int>("ID"),
                        Name = row.Field<string>("NAME"),
                    }).ToList();
            }

            return list;
        }
        public RequestSubmissionModel CreateRequest(RequestSubmissionModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.RequestId, DbType.Int32,
                    ParameterDirection.Output));
                //paramCollection.Add(new DBParameter("RequestNo", entity.RequestNo, DbType.String));
                paramCollection.Add(new DBParameter("FileNo", string.IsNullOrWhiteSpace(entity.FileNo) ? "" : (entity.FileNo), DbType.String));
                paramCollection.Add(new DBParameter("PatientName", entity.PatientName, DbType.String));
                paramCollection.Add(new DBParameter("PatientAddress", entity.PatientAddress, DbType.String));
                paramCollection.Add(new DBParameter("IpdNo", entity.IpdNo, DbType.String));
                paramCollection.Add(new DBParameter("ManagementTypeId", entity.ManagementTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("RoomEntitleTypeId", entity.RoomEntitleTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("HospitalTypeId", entity.HospitalTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("PatientTypeId", entity.PatientTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("GenderId", entity.GenderId, DbType.Int32));
                paramCollection.Add(new DBParameter("PatientAge", entity.PatientAge, DbType.Double));
                //paramCollection.Add(new DBParameter("DrugsAmount", entity.DrugsAmount, DbType.Double));
                paramCollection.Add(new DBParameter("BillAmount", entity.BillAmount, DbType.Double));
                //paramCollection.Add(new DBParameter("LifesavingMdcnAmt", entity.LifesavingMdcnAmt, DbType.Double));
                paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
                //paramCollection.Add(new DBParameter("LeftDcDetail", entity.LeftDcDetail, DbType.String));
                //paramCollection.Add(new DBParameter("RightDcDetail", entity.RightDcDetail, DbType.String));
                paramCollection.Add(new DBParameter("StateId", entity.StateId, DbType.Int32));
                paramCollection.Add(new DBParameter("CityId", entity.CityId, DbType.Int32));
                paramCollection.Add(new DBParameter("TypeOfAddmissionId", entity.TypeOfAddmissionId, DbType.Int32));
                paramCollection.Add(new DBParameter("DoctorIncharge", entity.DoctorIncharge, DbType.String));
                paramCollection.Add(new DBParameter("RegistrationNo", entity.RegistrationNo, DbType.String));
                var parameterList = dbHelper.ExecuteNonQueryForOutParameter(HospitalFormsQueries.CreateRequest,
                    paramCollection, CommandType.StoredProcedure);
                entity.RequestId = Convert.ToInt32(parameterList["RequestId"].ToString());
                //entity.RequestNo = parameterList["RequestNo"].ToString();
                object obj =
                    dbHelper.ExecuteScalar("select RequestNo from hsp_RequestMaster where requestid = " +
                                           entity.RequestId);
                entity.RequestNo = obj.ToString();
            }).IfNotNull(ex => { throw (ex); });
            return entity;
        }
        public RequestSubmissionModel UpdateRequest(RequestSubmissionModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.RequestId, DbType.Int32));
                //paramCollection.Add(new DBParameter("RequestNo", entity.RequestNo, DbType.String));
                paramCollection.Add(new DBParameter("FileNo", entity.FileNo, DbType.String));
                paramCollection.Add(new DBParameter("PatientName", entity.PatientName, DbType.String));
                paramCollection.Add(new DBParameter("PatientAddress", entity.PatientAddress, DbType.String));
                paramCollection.Add(new DBParameter("IpdNo", entity.IpdNo, DbType.String));
                paramCollection.Add(new DBParameter("PatientTypeId", entity.PatientTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("ManagementTypeId", entity.ManagementTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("RoomEntitleTypeId", entity.RoomEntitleTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("HospitalTypeId", entity.HospitalTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("GenderId", entity.GenderId, DbType.Int32));
                paramCollection.Add(new DBParameter("PatientAge", entity.PatientAge, DbType.Double));
                //paramCollection.Add(new DBParameter("DrugsAmount", entity.DrugsAmount, DbType.Double));
                paramCollection.Add(new DBParameter("BillAmount", entity.BillAmount, DbType.Double));
                //paramCollection.Add(new DBParameter("LifesavingMdcnAmt", entity.LifesavingMdcnAmt, DbType.Double));
                paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
                //paramCollection.Add(new DBParameter("LeftDcDetail", entity.LeftDcDetail, DbType.String));
                //paramCollection.Add(new DBParameter("RightDcDetail", entity.RightDcDetail, DbType.String));
                paramCollection.Add(new DBParameter("StateId", entity.StateId, DbType.Int32));
                paramCollection.Add(new DBParameter("CityId", entity.CityId, DbType.Int32));
                paramCollection.Add(new DBParameter("TypeOfAddmissionId", entity.TypeOfAddmissionId, DbType.Int32));
                paramCollection.Add(new DBParameter("DoctorIncharge", entity.DoctorIncharge, DbType.String));
                paramCollection.Add(new DBParameter("RegistrationNo", entity.RegistrationNo, DbType.String));
                paramCollection.Add(new DBParameter("IsDeactive", entity.IsDeactive, DbType.Boolean));
                paramCollection.Add(new DBParameter("Comment", entity.Comment, DbType.String));
                dbHelper.ExecuteNonQuery(HospitalFormsQueries.UpdateRequest, paramCollection,
                    CommandType.StoredProcedure);
            }).IfNotNull(ex => { throw (ex); });
            return entity;
        }
        public void CreateRequestBedChargeDetail(BedCharges entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.RequestId, DbType.Int32));
                paramCollection.Add(new DBParameter("RoomTypeId", entity.RoomTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("ConsumeDateTime", entity.ConsumeDate, DbType.String));
                paramCollection.Add(new DBParameter("Qty", entity.Qty, DbType.Int32));
                dbHelper.ExecuteNonQuery(HospitalFormsQueries.CreateRequestBedChargeDetail, paramCollection, CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreateRequestBedChargeDetail of RequestSubmision repository" +
                                 ex.Message + Environment.NewLine + ex.StackTrace);
            });
        }
        public void CreateRequestAdmissionDetail(AdmissionSummary entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.RequestId, DbType.Int32));
                paramCollection.Add(new DBParameter("ServiceId", entity.ServiceId, DbType.Int32));
                paramCollection.Add(new DBParameter("RoomTypeId", entity.RoomTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("Qty", entity.Qty, DbType.Double));
                paramCollection.Add(new DBParameter("AdmissionDateTime", entity.StrAdmissionDateTime, DbType.DateTime));
                paramCollection.Add(new DBParameter("DischargeDateTime", entity.StrDischargeDateTime, DbType.DateTime));
                //paramCollection.Add(new DBParameter("ReportedAdmissionDateTime", (entity.SurgeryAdmissionDateTime == null ? entity.StrAdmissionDateTime : entity.SurgeryAdmissionDateTime), DbType.DateTime));
                //paramCollection.Add(new DBParameter("ReportedDischargeDateTime", (entity.SurgerydDischargeDateTime == null ? entity.StrDischargeDateTime : entity.SurgerydDischargeDateTime), DbType.DateTime));
                dbHelper.ExecuteNonQuery(HospitalFormsQueries.CreateRequestAdmissionDetail, paramCollection,
                    CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreateRequestAdmissionDetail of RequestSubmision repository" +
                                 ex.Message + Environment.NewLine + ex.StackTrace);
            });
        }
        public void CreateRequestSurgeryDetail(SurgerySummary entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.RequestId, DbType.Int32));
                paramCollection.Add(new DBParameter("SurgeryId", entity.SurgeryID, DbType.Int32));
                paramCollection.Add(new DBParameter("SurgeryDateTime", entity.StrSurgeryDateTime, DbType.DateTime));
                dbHelper.ExecuteNonQuery(HospitalFormsQueries.CreateRequestSurgeryDetail, paramCollection, CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreateRequestSurgeryDetail of RequestSubmision repository" +
                                 ex.Message + Environment.NewLine + ex.StackTrace);
            });
        }
        public void CreateRequestOtDetail(AdmissionSummary entity, DBHelper dbHelper, int type)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.RequestId, DbType.Int32));
                paramCollection.Add(new DBParameter("ServiceId", entity.ServiceId, DbType.Int32));
                paramCollection.Add(new DBParameter("type", type, DbType.Int32)); /* type 1 for left and 2 for right*/
                paramCollection.Add(new DBParameter("AdmissionDateTime", entity.StrAdmissionDateTime, DbType.DateTime));
                paramCollection.Add(new DBParameter("DischargeDateTime", entity.StrDischargeDateTime, DbType.DateTime));
                dbHelper.ExecuteNonQuery(HospitalFormsQueries.CreateRequestOtDetail, paramCollection,
                    CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreateRequestOtDetail of RequestSubmision repository" + ex.Message +
                                 Environment.NewLine + ex.StackTrace);
            });
        }
        public void CreateRequestPharmacyDetail(CommonMasterModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.TransactionId, DbType.Int32));
                paramCollection.Add(new DBParameter("GeneralMedicineAmount", entity.BillRate, DbType.Double));
                paramCollection.Add(new DBParameter("LifeSavingMedicineAmount", entity.LifeSavingBillRate, DbType.Double));
                paramCollection.Add(new DBParameter("ConsumeDateTime", entity.ConsumeDate, DbType.String));
                dbHelper.ExecuteNonQuery(HospitalFormsQueries.CreateRequestPharmacyDetail, paramCollection,
                    CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreateRequestPharmacyDetail of RequestSubmision repository" +
                                 ex.Message + Environment.NewLine + ex.StackTrace);
            });
        }
        public void CreateRequestDetail(CommonMasterModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.TransactionId, DbType.Int32));
                paramCollection.Add(new DBParameter("RoomTypeId", entity.RoomTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("Id", entity.Id, DbType.Int32));
                paramCollection.Add(new DBParameter("ServiceId", entity.ServiceId, DbType.Int32));
                paramCollection.Add(new DBParameter("Qty", entity.Qty, DbType.Int32));
                paramCollection.Add(new DBParameter("ConsumeDate", entity.ConsumeDate, DbType.String));
                dbHelper.ExecuteNonQuery(HospitalFormsQueries.CreateRequestDetail, paramCollection,
                    CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreateRequestDetail of RequestSubmision repository" + ex.Message +
                                 Environment.NewLine + ex.StackTrace);
            });
        }
        public void CreateRequestManullyAddedDetail(CommonMasterModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.TransactionId, DbType.Int32));
                paramCollection.Add(new DBParameter("ConsumeDateTime", entity.ConsumeDate, DbType.String));
                paramCollection.Add(new DBParameter("Name", (string.IsNullOrWhiteSpace(entity.Name) ? string.Empty : entity.Name), DbType.String));
                paramCollection.Add(new DBParameter("Qty", entity.Qty, DbType.Int32));
                paramCollection.Add(new DBParameter("Amount", entity.BillRate, DbType.Double));
                dbHelper.ExecuteNonQuery(HospitalFormsQueries.CreateRequestManullyAddedDetail, paramCollection,
                    CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreateRequestManullyAddedDetail of RequestSubmision repository" +
                                 ex.Message + Environment.NewLine + ex.StackTrace);
            });
        }
        public void CreateRequestSurgeryManullyAddedDetail(SurgeryManualServices entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.RequestId, DbType.Int32));
                paramCollection.Add(new DBParameter("ConsumeDateTime", entity.ConsumeDate, DbType.String));
                paramCollection.Add(new DBParameter("SurgeryName", (string.IsNullOrWhiteSpace(entity.SurgeryName) ? string.Empty : entity.SurgeryName), DbType.String));
                paramCollection.Add(new DBParameter("OTCharges", entity.OTCharges, DbType.Double));
                paramCollection.Add(new DBParameter("AnesthesiaCharges", entity.AnesthesiaCharges, DbType.Double));
                paramCollection.Add(new DBParameter("SurgeonCharges", entity.SurgeonCharges, DbType.Double));
                dbHelper.ExecuteNonQuery(HospitalFormsQueries.CreateRequestSurgeryManullyAddedDetail, paramCollection, CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreateRequestSurgeryManullyAddedDetail of RequestSubmision repository" + ex.Message + Environment.NewLine + ex.StackTrace);
            });
        }
        public void CreateRequestDefaultServicesDetail(CommonMasterModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.TransactionId, DbType.Int32));
                paramCollection.Add(new DBParameter("ServiceId", entity.ServiceId, DbType.Int32));
                paramCollection.Add(new DBParameter("ConsumeDateTime", entity.ConsumeDate, DbType.String));
                paramCollection.Add(new DBParameter("Qty", entity.Qty, DbType.Int32));
                paramCollection.Add(new DBParameter("Amount", entity.BillRate, DbType.Double));
                dbHelper.ExecuteNonQuery(HospitalFormsQueries.CreateRequestDefaultServicesDetail, paramCollection,
                    CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreateRequestManullyAddedDetail of RequestSubmision repository" +
                                 ex.Message + Environment.NewLine + ex.StackTrace);
            });
        }
        public void CreateRequestAutoLinkedService(int requestId, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
                dbHelper.ExecuteNonQuery(HospitalFormsQueries.CreateRequestAutoLinkedServices, paramCollection, CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreateAutoLinkedService of RequestSubmision repository" +
                                 ex.Message + Environment.NewLine + ex.StackTrace);
            });
        }
        public List<RequestSubmissionModel> GetAllgeneratedRequest()
        {
            List<RequestSubmissionModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("UserID", Convert.ToInt32(HttpContext.Current.Session["AppUserId"]), DbType.Int32));
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetAllgeneratedRequest, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new RequestSubmissionModel
                    {
                        RequestId = row.Field<int>("RequestId"),
                        RequestNo = row.Field<string>("RequestNo"),
                        FileNo = row.Field<string>("FileNo"),
                        PatientName = row.Field<string>("PatientName"),
                        PatientAddress = row.Field<string>("PatientAddress"),
                        IpdNo = row.Field<string>("IpdNo"),
                        HospitalTypeId = row.Field<int>("HospitalTypeId"),
                        PatientAge = row.Field<double>("PatientAge"),
                        GenderId = row.Field<int>("GenderId"),
                        BillAmount = row.Field<double>("BillAmount"),
                        ManagementTypeId = row.Field<int>("ManagementTypeId"),
                        PatientTypeId = row.Field<int>("PatientTypeId"),
                        StateId = row.Field<int>("StateId"),
                        CityId = row.Field<int>("CityId"),
                        ClientId = row.Field<int>("ClientId"),
                        ClientName = row.Field<string>("ClientName"),
                        TypeOfAddmissionId = row.Field<int>("TypeOfAddmissionId"),
                        RoomEntitleTypeId = row.Field<int>("RoomEntitleTypeId"),
                        ManagementType = row.Field<string>("ManagementType"),
                        DoctorIncharge = row.Field<string>("DoctorIncharge"),
                        RegistrationNo = row.Field<string>("RegistrationNo"),
                        IsReportPrinted = Convert.ToBoolean(row.Field<int>("IsReportPrinted")),
                        IsLumpsum = row.Field<bool>("IsLumpsum"),
                        IsHopePatientBill = row.Field<bool>("IsHopePatientBill")
                    }).ToList();
            }
            return list;
        }
        public RequestSubmissionModel GetDeductionType()
        {
            RequestSubmissionModel model = new RequestSubmissionModel();
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("UserID", Convert.ToInt32(HttpContext.Current.Session["AppUserId"]), DbType.Int32));
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetDeductionType, paramCollection, CommandType.StoredProcedure);
                model.IsLumpsum = Convert.ToBoolean(dtPaper.Rows[0]["IsLumpsum"]);
            }
            return model;
        }
        public List<CommonMasterModel> GetRequestDetailById(int requestId, string ConnectionString)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            List<CommonMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetRequestDetailById,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        Id = row.Field<int>("DivId"),
                        TransactionId = row.Field<int>("TransactionId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        RoomTypeId = row.Field<int>("RoomTypeId"),
                        Name = row.Field<string>("Name"),
                        Code = row.Field<string>("ServiceCode"),
                        Qty = row.Field<int>("Qty"),
                        CghsCode = row.Field<string>("CghsCode"),
                        BillRate = row.Field<double>("BillRate"),
                        ConsumeDate = row.Field<string>("ConsumeDate"),
                        IsAllowedChangeInSurgery = row.Field<bool>("IsAllowedChangeInSurgery")
                    }).ToList();
            }

            return list;
        }
        public List<AdmissionSummary> AdmissionDetailById(int requestId)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            List<AdmissionSummary> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetAdmisionDetailById,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new AdmissionSummary
                    {
                        RequestId = row.Field<int>("RequestId"),
                        StrAdmissionDateTime = row.Field<string>("StrAdmissionDateTime"),
                        StrDischargeDateTime = row.Field<string>("StrDischargeDateTime"),
                        ServiceId = row.Field<int>("ServiceId"),
                    }).ToList();
            }

            return list;
        }
        public List<AdmissionSummary> GetRequestOtDetailById(int requestId)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            List<AdmissionSummary> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetRequestOtDetailById,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new AdmissionSummary
                    {
                        RequestId = row.Field<int>("RequestId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        ServiceCode = row.Field<string>("ServiceCode"),
                        OtType = row.Field<int>("LeftRightPosition"),
                        BillRate = row.Field<double>("BillRate"),
                        StrAdmissionDateTime = row.Field<string>("StrAdmissionDateTime"),
                        StrDischargeDateTime = row.Field<string>("StrDischargeDateTime"),
                    }).ToList();
            }

            return list;
        }
        public List<SurgerySummary> GetRequestSurgeryDetailById(int requestId)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            List<SurgerySummary> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetRequestSurgeryDetailById, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new SurgerySummary
                    {
                        RequestId = row.Field<int>("RequestId"),
                        SurgeryID = row.Field<int>("SurgeryID"),
                        SurgeryName = row.Field<string>("SurgeryName"),
                        IsCancerSurgery = Convert.ToBoolean(row.Field<int>("IsCancerRelated")),
                        StrSurgeryDateTime = row.Field<string>("SurgeryDateTime"),
                        SurgeryDateTime = row.Field<DateTime>("SurgeryDate"),
                        NoOfDays = row.Field<int>("NoOfDays")
                    }).ToList();
            }

            return list;
        }
        public List<ServiceMasterModel> GetAllServiceMasterByCategoryId(int categoryId, int userId, int hospitalType, int patientType, int stateId, int cityId, int gender, int roomTypeId = 0, bool loadBillRate = false, string ConnectionString = "DefaultConnection")
        {
            List<ServiceMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("categoryId", categoryId, DbType.Int32));
            paramCollection.Add(new DBParameter("stateId", stateId, DbType.Int32));
            paramCollection.Add(new DBParameter("cityId", cityId, DbType.Int32));
            paramCollection.Add(new DBParameter("hospitalType", hospitalType, DbType.Int32));
            paramCollection.Add(new DBParameter("patientType", patientType, DbType.Int32));
            paramCollection.Add(new DBParameter("genderId", gender, DbType.Int32));
            paramCollection.Add(new DBParameter("roomTypeId", roomTypeId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAllServiceMasterByCategoryId,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceMasterModel
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        //ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        //Code = row.Field<string>("Code"),
                        //ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        //Sequence = row.Field<int>("SequenceNo"),
                        CategoryId = row.Field<int>("categoryId"),
                        BillRate = (loadBillRate ? row.Field<double>("BillRate") : 0.0),
                        RoomTypeId = row.Field<int>("RoomTypeId"),
                        //RoomType = row.Field<string>("RoomType"),
                        //CategoryName = row.Field<string>("CategoryName"),
                        Deactive = row.Field<bool>("Deactive"),
                        Qty = row.Field<int>("Qty"),

                        StateId = stateId,
                        CityId = cityId,
                        HospitalTypeId = hospitalType,
                        PatientTypeId = patientType,
                        GenderId = gender,
                        IsAllowedChangeInSurgery = row.Field<bool>("IsAllowedChangeInSurgery")
                    }).ToList();
            }

            return list;
        }
        public List<ServiceMasterModel> GetAllServiceMasterByCategory(string category, int userId, int hospitalType, int patientType, int stateId, int cityId, int gender, int roomTypeId = 0)
        {
            List<ServiceMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("category", category, DbType.String));
            paramCollection.Add(new DBParameter("stateId", stateId, DbType.Int32));
            paramCollection.Add(new DBParameter("cityId", cityId, DbType.Int32));
            paramCollection.Add(new DBParameter("hospitalType", hospitalType, DbType.Int32));
            paramCollection.Add(new DBParameter("patientType", patientType, DbType.Int32));
            paramCollection.Add(new DBParameter("genderId", gender, DbType.Int32));
            paramCollection.Add(new DBParameter("roomTypeId", roomTypeId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAllServiceMasterByCategory,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceMasterModel
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        Code = row.Field<string>("Code"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Sequence = row.Field<int>("SequenceNo"),
                        CategoryId = row.Field<int>("categoryId"),
                        BillRate = row.Field<double>("BillRate"),
                        RoomTypeId = row.Field<int>("RoomTypeId"),
                        RoomType = row.Field<string>("RoomType"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Deactive = row.Field<bool>("Deactive"),

                        StateId = stateId,
                        CityId = cityId,
                        HospitalTypeId = hospitalType,
                        PatientTypeId = patientType,
                        GenderId = gender,
                        IsAllowedChangeInSurgery = row.Field<bool>("IsAllowedChangeInSurgery")
                    }).ToList();
            }

            return list;
        }
        public List<ServiceMasterModel> GetServiceMasterByCategoryId(int category, int userId, int hospitalType, int patientType, int stateId, int cityId, int gender)
        {
            List<ServiceMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("categoryId", category, DbType.Int32));
            paramCollection.Add(new DBParameter("userId", userId, DbType.Int32));
            paramCollection.Add(new DBParameter("hospitalType", hospitalType, DbType.Int32));
            paramCollection.Add(new DBParameter("patientType", patientType, DbType.Int32));
            paramCollection.Add(new DBParameter("StateId", stateId, DbType.Int32));
            paramCollection.Add(new DBParameter("cityId", cityId, DbType.Int32));
            paramCollection.Add(new DBParameter("genderId", gender, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetServiceMasterByCategoryId,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceMasterModel
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        Code = row.Field<string>("Code"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Sequence = row.Field<int>("SequenceNo"),
                        CategoryId = row.Field<int>("categoryId"),
                        CategoryName = row.Field<string>("CategoryName"),
                        BillRate = row.Field<double>("BillRate"),
                        Deactive = row.Field<bool>("Deactive"),
                        IsAllowedChangeInSurgery = row.Field<bool>("IsAllowedChangeInSurgery")
                    }).ToList();
            }

            return list;
        }
        public List<ServiceMasterModel> GetServiceMasterByCategoryRoomId(int category, int userId, int hospitalType, int patientType, int roomTypeId, int stateId, int cityId, int gender, int requestId = 0)
        {

            List<ServiceMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("categoryId", category, DbType.Int32));
            paramCollection.Add(new DBParameter("stateId", stateId, DbType.Int32));
            paramCollection.Add(new DBParameter("cityId", cityId, DbType.Int32));
            paramCollection.Add(new DBParameter("hospitalType", hospitalType, DbType.Int32));
            paramCollection.Add(new DBParameter("patientType", patientType, DbType.Int32));
            paramCollection.Add(new DBParameter("roomTypeId", roomTypeId, DbType.Int32));
            paramCollection.Add(new DBParameter("genderId", gender, DbType.Int32));
            paramCollection.Add(new DBParameter("requestId", requestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetServiceMasterByCategoryRoomId,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceMasterModel
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        Code = row.Field<string>("Code"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Sequence = row.Field<int>("SequenceNo"),
                        CategoryId = row.Field<int>("categoryId"),
                        CategoryName = row.Field<string>("CategoryName"),
                        BillRate = row.Field<double>("BillRate"),
                        Deactive = row.Field<bool>("Deactive"),
                        IsDefaultService = row.Field<bool>("IsDefaultService"),
                        StateId = stateId,
                        CityId = cityId,
                        HospitalTypeId = hospitalType,
                        PatientTypeId = patientType,
                        RoomTypeId = roomTypeId,
                        GenderId = gender,
                        IsAllowedChangeInSurgery = row.Field<bool>("IsAllowedChangeInSurgery")
                    }).ToList();

                if (Caching.MemoryCaching.CacheKeyExist(Caching.CachingKeys.AllBedcharges.ToString()))
                {
                    var existingCache = Caching.MemoryCaching.GetCacheValue(Caching.CachingKeys.AllBedcharges.ToString()) as List<ServiceMasterModel>;
                    list.AddRange(existingCache);
                }
                Caching.MemoryCaching.AddCacheValue(Caching.CachingKeys.AllBedcharges.ToString(), list);
            }

            return list;
        }
        public List<TariffDetailModel> GetTariffByUserId(int userId, int hospitalType)
        {
            List<TariffDetailModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("userId", userId, DbType.Int32));
            paramCollection.Add(new DBParameter("hospitalType", hospitalType, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetTariffByUserId, paramCollection,
                    CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new TariffDetailModel
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        BillRate = row.Field<double>("BillRate"),
                    }).ToList();
            }

            return list;
        }
        public List<CommonMasterModel> ServicesConsumedLeftDiv(int ManagementTypeId = 0, string ConnectionString = "DefaultConnection")
        {
            List<CommonMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("ManagementTypeId", ManagementTypeId, DbType.Int32));
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetAllServiceTypeByManagementType, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        Id = row.Field<int>("ID"),
                        Name = row.Field<string>("NAME"),
                        IsAllowedChangeInSurgery = Convert.ToBoolean(row.Field<int>("IsAllowedChangeInSurgery"))
                    }).ToList();
            }

            return list;
        }
        public List<SurgeryMasterModel> GetSurgeryMasterList()
        {
            List<SurgeryMasterModel> list = null;
            if (Caching.MemoryCaching.CacheKeyExist(Caching.CachingKeys.SurgeryMaster.ToString()))
            {
                list = Caching.MemoryCaching.GetCacheValue(Caching.CachingKeys.SurgeryMaster.ToString()) as List<SurgeryMasterModel>;
                if (list != null && list.Count() > 0)
                    return list;
            }

            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAllSurgeryMaster, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new SurgeryMasterModel
                    {
                        SurgeryID = row.Field<int>("SurgeryID"),
                        SurgeryName = row.Field<string>("SurgeryName"),
                        SurgeryTypeID = row.Field<int>("SurgeryTypeID"),
                        NoOfDays = row.Field<int>("NoOfDays")
                    }).ToList();
            }

            Caching.MemoryCaching.AddCacheValue(Caching.CachingKeys.SurgeryMaster.ToString(), list);
            return list;
        }
        public List<CancerSurgeryModel> GetCancerSurgeryList()
        {
            List<CancerSurgeryModel> list = null;
            if (Caching.MemoryCaching.CacheKeyExist(Caching.CachingKeys.CancerSurgery.ToString()))
            {
                list = Caching.MemoryCaching.GetCacheValue(Caching.CachingKeys.CancerSurgery.ToString()) as List<CancerSurgeryModel>;
                if (list != null && list.Count() > 0)
                    return list;
            }
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetCancerSurgeryList, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CancerSurgeryModel
                    {
                        Category = row.Field<string>("Category"),
                        SurgeryName = row.Field<string>("SurgeryName"),
                        Grade = row.Field<string>("Grade")
                    }).ToList();
            }

            Caching.MemoryCaching.AddCacheValue(Caching.CachingKeys.CancerSurgery.ToString(), list);
            return list;


        }
        public List<ServiceMasterModel> GetServicesList()
        {
            List<ServiceMasterModel> list = null;
            if (Caching.MemoryCaching.CacheKeyExist(Caching.CachingKeys.ServicesList.ToString()))
            {
                list = Caching.MemoryCaching.GetCacheValue(Caching.CachingKeys.ServicesList.ToString()) as List<ServiceMasterModel>;
                if (list != null && list.Count() > 0)
                    return list;
            }
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetServicesList, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceMasterModel
                    {
                        ServiceName = row.Field<string>("ServiceName"),
                        ServiceType = row.Field<string>("ServiceType"),
                    }).ToList();
            }

            Caching.MemoryCaching.AddCacheValue(Caching.CachingKeys.ServicesList.ToString(), list);
            return list;


        }
        public List<CommonMasterModel> GetRequestPharmacyDetail(int requestId, string ConnectionString)
        {
            List<CommonMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetRequestPharmacyDetail,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        TransactionId = row.Field<int>("RequestId"),
                        ConsumeDate = row.Field<string>("ConsumeDateTime"),
                        BillRate = row.Field<double>("GeneralMedicineAmount"),
                        LifeSavingBillRate = row.Field<double>("LifeSavingMedicineAmount"),
                    }).ToList();
            }

            return list;
        }
        public List<BedCharges> GetRequestBedChargeDetails(int requestId, string ConnectionString)
        {
            List<BedCharges> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetRequestBedChargeDetail, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new BedCharges
                    {
                        RequestId = row.Field<int>("RequestId"),
                        ConsumeDate = row.Field<string>("ConsumeDate"),
                        RoomTypeId = row.Field<int>("RoomTypeId"),
                        BillRate = row.Field<double>("BillRate"),
                        CghsCode = row.Field<string>("CghsCode"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Qty = row.Field<int>("Qty"),
                    }).ToList();
            }

            return list;
        }
        public List<SurgerySummary> GetRequestSurgeryChargeDetails(int requestId, string ConnectionString)
        {
            List<SurgerySummary> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetRequestSurgeryDetails, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new SurgerySummary
                    {
                        RequestId = row.Field<int>("RequestId"),
                        StrSurgeryDateTime = row.Field<string>("ConsumeDate"),
                        SurgeryID = row.Field<int>("SurgeryID"),
                        BillRate = row.Field<double>("BillRate"),
                        CghsCode = row.Field<string>("CghsCode"),
                        SurgeryName = row.Field<string>("ServiceName"),
                        Qty = row.Field<int>("Qty"),
                        NoOfDays = row.Field<int>("NoOfDays"),
                        ServiceTypeId = row.Field<int>("ServiceTypeId")
                    }).ToList();
            }

            return list;
        }
        public List<CommonMasterModel> GetRequestManuallyAddedDetail(int requestId, string ConnectionString)
        {
            List<CommonMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetRequestManuallyAddedDetail,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        TransactionId = row.Field<int>("RequestId"),
                        ConsumeDate = row.Field<string>("ConsumeDateTime"),
                        Name = row.Field<string>("Name"),
                        Qty = row.Field<int>("Qty"),
                        BillRate = row.Field<double>("Amount")
                    }).ToList();
            }

            return list;
        }
        public List<SurgeryManualServices> GetRequestSurgeryManuallyAddedDetail(int requestId, string ConnectionString)
        {
            List<SurgeryManualServices> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetRequestSurgeryManuallyAddedDetail,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new SurgeryManualServices
                    {
                        RequestId = row.Field<int>("RequestId"),
                        ConsumeDate = row.Field<string>("ConsumeDateTime"),
                        SurgeryName = row.Field<string>("SurgeryName"),
                        OTCharges = row.Field<double>("OTCharges"),
                        AnesthesiaCharges = row.Field<double>("AnesthesiaCharges"),
                        SurgeonCharges = row.Field<double>("SurgeonCharges")
                    }).ToList();
            }

            return list;
        }
        public RequestSubmissionModel_Report GetAllgeneratedRequestById(int requestId, string ConnectionString)
        {
            RequestSubmissionModel_Report result = new RequestSubmissionModel_Report();
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("@requestId", requestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetAllgeneratedRequestById,
                    paramCollection, CommandType.StoredProcedure);
                var list = dtPaper.AsEnumerable()
                    .Select(row => new RequestSubmissionModel_Report
                    {
                        RequestNo = row.Field<string>("RequestNo"),
                        FileNo = row.Field<string>("FileNo"),
                        PatientName = row.Field<string>("PatientName"),
                        PatientAddress = row.Field<string>("PatientAddress"),
                        IpdNo = row.Field<string>("IpdNo"),
                        HospitalType = row.Field<string>("HospitalType"),
                        PatientAge = row.Field<double>("PatientAge"),
                        Gender = row.Field<string>("Gender"),
                        BillAmount = row.Field<double>("BillAmount"),
                        ManagementType = row.Field<string>("ManagementType"),
                        BillDate = row.Field<string>("StrInsertedOn"),
                        StrAdmissionDate = row.Field<string>("StrAdmissionDate"),
                        StrDischrgeDate = row.Field<string>("StrDischrgeDate"),
                        RoomType = row.Field<string>("RoomEntitleType"),
                        RoomEntitleType = row.Field<string>("RoomEntitleType"),
                        TypeOfAddmission = row.Field<string>("TypeOfAddmission"),
                        DoctorIncharge = row.Field<string>("DoctorIncharge"),
                        RegistrationNo = row.Field<string>("RegistrationNo"),
                        IsReportPrinted = Convert.ToBoolean(row.Field<int>("IsReportPrinted"))
                    }).ToList<RequestSubmissionModel_Report>();

                if (list != null && list.Count > 0)
                    result = list[0];
            }
            return result;
        }
        public List<ViewBillsModel_Report> GetAllgeneratedRequestByDateWise(int Userid, string Fromtime, string Totime, string ConnectionString)
        {
            List<ViewBillsModel_Report> result = new List<ViewBillsModel_Report>();
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("Userid", Userid, DbType.String));
            paramCollection.Add(new DBParameter("Fromtime", Fromtime, DbType.String));
            paramCollection.Add(new DBParameter("Totime", Totime, DbType.String));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetAllgeneratedRequestByDateWise,
                 paramCollection, CommandType.StoredProcedure);

                result = dtPaper.AsEnumerable()
                  .Select(row => new ViewBillsModel_Report
                  {

                      RequestNo = row.Field<string>("RequestNo"),
                      FileNo = row.Field<string>("FileNo"),
                      PatientName = row.Field<string>("PatientName"),
                      PatientAddress = row.Field<string>("PatientAddress"),
                      BillNo = row.Field<string>("IpdNo"),
                      PatientAge = row.Field<double>("PatientAge"),
                      BillAmount = row.Field<double>("BillAmount"),
                      ClientName = row.Field<string>("ClientName"),
                      ManagementType = row.Field<string>("ManagementType"),
                      DoctorIncharge = row.Field<string>("DoctorIncharge"),
                      RegistrationNo = row.Field<string>("RegistrationNo"),
                      IsDeactive = row.Field<string>("IsDeactive"),
                      Comment = row.Field<string>("Comment"),
                      LoginName = row.Field<string>("LoginName"),
                      BillDate = row.Field<string>("StrGeneratedDt"),
                      FromDate = row.Field<string>("FromDate"),
                      ToDate = row.Field<string>("ToDate"),
                      TypeofReport = row.Field<int>("TypeofReport")
                  }).ToList();
            }
            return result;
        }
        public List<CommonMasterModel> GetServiceConsumedInRequest(int RequestId)
        {
            List<CommonMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", RequestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetServiceConsumedInRequest, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        TransactionId = row.Field<int>("RequestId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        Id = row.Field<int>("ServiceTypeId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Name = row.Field<string>("ServiceType"),
                    }).ToList();
            }

            return list;
        }
        public List<ScanDocEntity> GetScanDocUrl(int RequestId)
        {
            List<ScanDocEntity> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            using (DBHelper dbHelper = new DBHelper())
            {
                paramCollection.Add(new DBParameter("scanDocId", RequestId, DbType.Int32));
                DataTable dtPaper = dbHelper.ExecuteDataTable(AdminPanelQueries.GetScanDocUrl, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable().Select(row => new ScanDocEntity
                {
                    ScanDocTypeId = row.Field<int>("ScanDocTypeId"),
                    ScanDocType = row.Field<string>("ScanDocType"),
                    ScanDocSubType = row.Field<string>("ScanDocSubType"),
                    FileNames = row.Field<string>("FileNames"),
                    FilePath = row.Field<string>("FilePath"),
                }).ToList();
            }

            return list;
        }
        public List<CommonMasterModel> GetDefaultServicesDetail(int requestId, string ConnectionString)
        {
            List<CommonMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetDefaultServicesDetail,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        TransactionId = row.Field<int>("RequestId"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Qty = row.Field<int>("Qty"),
                        BillRate = row.Field<double>("BillRate"),
                        ConsumeDate = row.Field<string>("ConsumeDate"),
                        CghsCode = row.Field<string>("CghsCode"),
                    }).ToList();
            }

            return list;
        }
        public PatientModel GetPatientData(int requestId, string ConnectionString)
        {
            PatientModel PatientData = new PatientModel();
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPatient = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetPatientData,
                    paramCollection, CommandType.StoredProcedure);
                DataRow row = dtPatient.Rows[0];
                {
                    PatientData.PatientId = row.Field<int>("PatientId");
                    PatientData.ParentPatientId = row.Field<int>("ParentPatientId");
                    PatientData.IsDependent = row.Field<bool>("IsDependent");
                    PatientData.CompanyName = row.Field<string>("CompanyName");
                    PatientData.PatientName = row.Field<string>("PatientName");
                    PatientData.Age = row.Field<double>("Age");
                    PatientData.GenderId = row.Field<int>("GenderId");
                    PatientData.Address = row.Field<string>("Address");
                    PatientData.RoomTypeId = row.Field<int>("RoomTypeId");

                }


            }

            return PatientData;
        }
        public List<CommonMasterModel> GetAutoLinkedServicesDetailByRequestId(int requestId, string ConnectionString)
        {
            List<CommonMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetAutoLinkedServicesDetail,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        TransactionId = row.Field<int>("RequestId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Qty = row.Field<int>("Qty"),
                        BillRate = row.Field<double>("Rate"),
                        ConsumeDate = row.Field<string>("ConsumeDate"),
                        CghsCode = row.Field<string>("CghsCode"),
                        ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        ReportHeading = row.Field<string>("ReportHeading")
                    }).ToList();
            }

            return list;
        }
        public List<CommonMasterModel> GetLinkedServiceRatesByCategory_Services(string ServiceTypeList, string ServiceIdList, int RoomTypeId, int HospitalTypeId, int PatientTypeId, int GenderId, int StateId, int CityId, string ConnectionString)
        {
            List<CommonMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ServiceTypeIds", ServiceTypeList, DbType.String));
            paramCollection.Add(new DBParameter("ServiceIds", ServiceIdList, DbType.String));
            paramCollection.Add(new DBParameter("RoomTypeId", RoomTypeId, DbType.Int32));
            paramCollection.Add(new DBParameter("HospitalTypeId", HospitalTypeId, DbType.Int32));
            paramCollection.Add(new DBParameter("PatientTypeId", PatientTypeId, DbType.Int32));
            paramCollection.Add(new DBParameter("GenderId", GenderId, DbType.Int32));
            paramCollection.Add(new DBParameter("StateId", StateId, DbType.Int32));
            paramCollection.Add(new DBParameter("CityId", CityId, DbType.Int32));

            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetLinkedServiceRatesByCategory_Services,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        ParentServiceId = row.Field<int>("ParentServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Name = row.Field<string>("ServiceName"),
                        Qty = row.Field<int>("Qty"),
                        BillRate = row.Field<double>("BillRate")
                    }).ToList();
            }

            return list;

        }
        public bool BillAmountDeduction(int requestId)
        {
            TryCatch.Run(() =>
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    DBParameterCollection paramCollection = new DBParameterCollection();
                    paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));

                    dbHelper.ExecuteNonQuery(HospitalFormsQueries.BillAmountDeduction, paramCollection, CommandType.StoredProcedure);
                }
            }).IfNotNull(ex => { return false; });
            return true;

        }
        public void CreatePatientLink(PatientModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.RequestId, DbType.Int32));
                paramCollection.Add(new DBParameter("PatientId", entity.PatientId, DbType.Int32));
                paramCollection.Add(new DBParameter("ParentPatientId", entity.ParentPatientId, DbType.Int32));
                paramCollection.Add(new DBParameter("IsDependent", entity.IsDependent, DbType.Boolean));
                dbHelper.ExecuteNonQuery(HospitalFormsQueries.CreatePatientLink, paramCollection,
                    CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreatePatientLink of RequestSubmision repository" +
                                 ex.Message + Environment.NewLine + ex.StackTrace);
            });
        }
        //=================
        public RequestSubmissionModel_Report GetAllgeneratedRequestById2(int requestId, int configId, string ConnectionString)
        {
            RequestSubmissionModel_Report result = new RequestSubmissionModel_Report();
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("@requestId", requestId, DbType.Int32));
            paramCollection.Add(new DBParameter("@configId", configId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetAllgeneratedRequestById2,
                    paramCollection, CommandType.StoredProcedure);
                var list = dtPaper.AsEnumerable()
                    .Select(row => new RequestSubmissionModel_Report
                    {
                        RequestNo = row.Field<string>("RequestNo"),
                        FileNo = row.Field<string>("FileNo"),
                        PatientName = row.Field<string>("PatientName"),
                        PatientAddress = row.Field<string>("PatientAddress"),
                        IpdNo = row.Field<string>("IpdNo"),
                        HospitalType = row.Field<string>("HospitalType"),
                        PatientAge = row.Field<double>("PatientAge"),
                        Gender = row.Field<string>("Gender"),
                        BillAmount = row.Field<double>("BillAmount"),
                        ManagementType = row.Field<string>("ManagementType"),
                        BillDate = row.Field<string>("StrInsertedOn"),
                        StrAdmissionDate = row.Field<string>("StrAdmissionDate"),
                        StrDischrgeDate = row.Field<string>("StrDischrgeDate"),
                        RoomType = row.Field<string>("RoomEntitleType"),
                        RoomEntitleType = row.Field<string>("RoomEntitleType"),
                        TypeOfAddmission = row.Field<string>("TypeOfAddmission"),
                        DoctorIncharge = row.Field<string>("DoctorIncharge"),
                        RegistrationNo = row.Field<string>("RegistrationNo"),
                        IsReportPrinted = Convert.ToBoolean(row.Field<int>("IsReportPrinted"))
                    }).ToList<RequestSubmissionModel_Report>();

                if (list != null && list.Count > 0)
                    result = list[0];
            }
            return result;
        }
        public List<CommonMasterModel> GetRequestDetailById2(int requestId, int configId, string ConnectionString)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            paramCollection.Add(new DBParameter("ConfigId", configId, DbType.Int32));
            List<CommonMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetRequestDetailById2,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        Id = row.Field<int>("DivId"),
                        TransactionId = row.Field<int>("TransactionId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        RoomTypeId = row.Field<int>("RoomTypeId"),
                        Name = row.Field<string>("Name"),
                        Code = row.Field<string>("ServiceCode"),
                        Qty = row.Field<int>("Qty"),
                        CghsCode = row.Field<string>("CghsCode"),
                        BillRate = row.Field<double>("BillRate"),
                        ConsumeDate = row.Field<string>("ConsumeDate"),
                        IsAllowedChangeInSurgery = row.Field<bool>("IsAllowedChangeInSurgery")
                    }).ToList();
            }

            return list;
        }
        public List<BedCharges> GetRequestBedChargeDetails2(int requestId, int configId, string ConnectionString)
        {
            List<BedCharges> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            paramCollection.Add(new DBParameter("ConfigId", configId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetRequestBedChargeDetail2, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new BedCharges
                    {
                        RequestId = row.Field<int>("RequestId"),
                        ConsumeDate = row.Field<string>("ConsumeDate"),
                        RoomTypeId = row.Field<int>("RoomTypeId"),
                        BillRate = row.Field<double>("BillRate"),
                        CghsCode = row.Field<string>("CghsCode"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Qty = row.Field<int>("Qty"),
                    }).ToList();
            }

            return list;
        }
        public List<SurgerySummary> GetRequestSurgeryChargeDetails2(int requestId, int configId, string ConnectionString)
        {
            List<SurgerySummary> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            paramCollection.Add(new DBParameter("ConfigId", configId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetRequestSurgeryDetails2, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new SurgerySummary
                    {
                        RequestId = row.Field<int>("RequestId"),
                        StrSurgeryDateTime = row.Field<string>("ConsumeDate"),
                        SurgeryID = row.Field<int>("SurgeryID"),
                        BillRate = row.Field<double>("BillRate"),
                        CghsCode = row.Field<string>("CghsCode"),
                        SurgeryName = row.Field<string>("ServiceName"),
                        Qty = row.Field<int>("Qty"),
                        NoOfDays = row.Field<int>("NoOfDays"),
                        ServiceTypeId = row.Field<int>("ServiceTypeId")
                    }).ToList();
            }

            return list;
        }
        public List<CommonMasterModel> GetDefaultServicesDetail2(int requestId, int configId, string ConnectionString)
        {
            List<CommonMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            paramCollection.Add(new DBParameter("ConfigId", configId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetDefaultServicesDetail2,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        TransactionId = row.Field<int>("RequestId"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Qty = row.Field<int>("Qty"),
                        BillRate = row.Field<double>("BillRate"),
                        ConsumeDate = row.Field<string>("ConsumeDate"),
                        CghsCode = row.Field<string>("CghsCode"),
                    }).ToList();
            }

            return list;
        }
        public List<CommonMasterModel> GetAutoLinkedServicesDetailByRequestId2(int requestId, int configId, string ConnectionString)
        {
            List<CommonMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            paramCollection.Add(new DBParameter("ConfigId", configId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetAutoLinkedServicesDetail2,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        TransactionId = row.Field<int>("RequestId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Qty = row.Field<int>("Qty"),
                        BillRate = row.Field<double>("Rate"),
                        ConsumeDate = row.Field<string>("ConsumeDate"),
                        CghsCode = row.Field<string>("CghsCode"),
                        ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        ReportHeading = row.Field<string>("ReportHeading")
                    }).ToList();
            }

            return list;
        }
    }
}
