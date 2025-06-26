using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using CGHSBilling.API.HospitalForms.Intefaces;
using CGHSBilling.Areas.HospitalForms.Models;
using CGHSBilling.QueryCollection.HospitalForms;
using CommonDataLayer.DataAccess;
using CommonLayer;
using System.Linq;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.QueryCollection.Masters;
using CommonLayer.Extensions;

namespace CGHSBilling.API.HospitalForms.Repository
{
    public class OPDBillingRepository : IOPDBillingRepository
    {
        private static readonly ILogger Loggger = Logger.Register(typeof(OPDBillingRepository));

        public List<RequestSubmissionOPDModel> GetAllOPDRequest()
        {
            List<RequestSubmissionOPDModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("UserID", Convert.ToInt32(HttpContext.Current.Session["AppUserId"]), DbType.Int32));
                DataTable dtPaper = dbHelper.ExecuteDataTable(OPDFormQueries.GetAllOPDRequest, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new RequestSubmissionOPDModel
                    {
                        RequestId = row.Field<int>("RequestId"),
                        RequestNo = row.Field<string>("RequestNo"),
                        RegistrationNo = row.Field<string>("REgistrationNo"),
                        PatientName = row.Field<string>("PatientName"),
                        PatientAddress = row.Field<string>("PatientAddress"),
                        OPDNo = row.Field<string>("OPDNo"),
                        StrOPDDate = row.Field<string>("OPDDate"),
                        HospitalTypeId = row.Field<int>("HospitalTypeId"),
                        PatientAge = row.Field<double>("PatientAge"),
                        GenderId = row.Field<int>("GenderId"),
                        BillAmount = row.Field<double>("BillAmount"),
                        NameOfDoctor1 = row.Field<string>("NameOfDoctor1"),
                        NameOfDoctor2 = row.Field<string>("NameOfDoctor2"),
                        NameOfDoctor3 = row.Field<string>("NameOfDoctor3"),
                        PatientTypeId = row.Field<int>("PatientTypeId"),
                        Comment = row.Field<string>("Comment"),
                        StateId = row.Field<int>("StateId"),
                        CityId = row.Field<int>("CityId"),
                        ClientId = row.Field<int>("ClientId"),
                        ClientName = row.Field<string>("ClientName"),
                        TypeOfAddmissionId = row.Field<int>("TypeOfAddmissionId"),
                        CompanyName = row.Field<string>("CompanyName"),
                        IsHopePatientBill = row.Field<bool>("IsHopePatientBill")
                    }).ToList();
            }
            return list;
        }

        public List<CommonMasterModel> OPDServicesConsumed()
        {
            List<CommonMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(OPDFormQueries.GetAllOPDServiceType, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        Id = row.Field<int>("ID"),
                        Name = row.Field<string>("NAME")
                    }).ToList();
            }
            return list;
        }

        public List<ServiceMasterModel> GetAllOPDServiceMasterByCategoryId(int categoryId, int userId, int hospitalType, int patientType, int stateId, int cityId, int gender, int roomTypeId = 0, bool loadBillRate = false, string ConnectionString = "DefaultConnection")
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
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAllServiceMasterByCategoryId, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceMasterModel
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        CategoryId = row.Field<int>("categoryId"),
                        BillRate = (loadBillRate ? row.Field<double>("BillRate") : 0.0),
                        RoomTypeId = row.Field<int>("RoomTypeId"),
                        Deactive = row.Field<bool>("Deactive"),
                        Qty = row.Field<int>("Qty"),

                        StateId = stateId,
                        CityId = cityId,
                        HospitalTypeId = hospitalType,
                        PatientTypeId = patientType,
                        GenderId = gender
                    }).ToList();
            }

            return list;
        }

        public List<CommonMasterModel> GetRequestManuallyOPDAddedDetail(int requestId, string ConnectionString)
        {
            List<CommonMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(OPDFormQueries.GetOPDRequestManuallyAddedDetailById,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        TransactionId = row.Field<int>("RequestId"),
                        Name = row.Field<string>("Name"),
                        Qty = row.Field<int>("Qty"),
                        BillRate = row.Field<double>("Amount")
                    }).ToList();
            }

            return list;
        }

        public List<CommonMasterModel> GetOPDRequestDetailById(int requestId, bool isReport = false, string ConnectionString = "DefaultConnection")
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            List<CommonMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(OPDFormQueries.GetOPDRequestDetailById,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        Id = row.Field<int>("DivId"),
                        TransactionId = row.Field<int>("TransactionId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Name = row.Field<string>("Name"),
                        Code = row.Field<string>("ServiceCode"),
                        Qty = row.Field<int>("Qty"),
                        CghsCode = row.Field<string>("CghsCode"),
                        ConsumeDate = row.Field<string>("ConsumeDate"),
                        BillRate = (isReport ? row.Field<double>("BillRate") : 0)
                    }).ToList();
            }

            return list;
        }

        public List<CommonMasterModel> GetDefaultOPDServicesDetail(int requestId, bool isReport = false, string ConnectionString = "DefaultConnection")
        {
            List<CommonMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(OPDFormQueries.GetOPDRequestDefaultServiceDetailById,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        TransactionId = row.Field<int>("RequestId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Qty = row.Field<int>("Qty"),
                        CghsCode = row.Field<string>("CghsCode"),
                        ConsumeDate = row.Field<string>("ConsumeDate"),
                        BillRate = (isReport ? row.Field<double>("BillRate") : 0)
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
                DataTable dtPatient = dbHelper.ExecuteDataTable(OPDFormQueries.GetPatientData,
                    paramCollection, CommandType.StoredProcedure);
                if (dtPatient.Rows.Count > 0)
                {
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
                    }
                }
                else
                {
                    PatientData = null;
                }


            }

            return PatientData;
        }

        public RequestSubmissionOPDModel_Report GetOPDGeneratedRequestById(int requestId, string ConnectionString)
        {
            RequestSubmissionOPDModel_Report result = new RequestSubmissionOPDModel_Report();
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("@requestId", requestId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(OPDFormQueries.GetOPDGeneratedRequestById,
                    paramCollection, CommandType.StoredProcedure);
                var list = dtPaper.AsEnumerable()
                    .Select(row => new RequestSubmissionOPDModel_Report
                    {
                        RequestNo = row.Field<string>("RequestNo"),
                        RegistrationNo = row.Field<string>("REgistrationNo"),
                        PatientName = row.Field<string>("PatientName"),
                        PatientAddress = row.Field<string>("PatientAddress"),
                        OPDNo = row.Field<string>("OPDNo"),
                        HospitalType = row.Field<string>("HospitalType"),
                        PatientAge = row.Field<double>("PatientAge"),
                        Gender = row.Field<string>("Gender"),
                        BillAmount = row.Field<double>("BillAmount"),
                        OPDDate = row.Field<string>("OPDDate"),
                        Comment = row.Field<string>("Comment"),
                        NameOfDoctor1 = row.Field<string>("NameOfDoctor1"),
                        NameOfDoctor2 = row.Field<string>("NameOfDoctor2"),
                        NameOfDoctor3 = row.Field<string>("NameOfDoctor3"),
                        CompanyName = row.Field<string>("CompanyName")
                    }).ToList();

                if (list != null && list.Count > 0)
                    result = list[0];
            }
            return result;
        }

        public List<ViewBillsModel_Report> GetAllgeneratedOPDRequestByDateWise(int Userid, string Fromtime, string Totime, string ConnectionString)
        {
            List<ViewBillsModel_Report> result = new List<ViewBillsModel_Report>();
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("Userid", Userid, DbType.String));
            paramCollection.Add(new DBParameter("Fromtime", Fromtime, DbType.String));
            paramCollection.Add(new DBParameter("Totime", Totime, DbType.String));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(OPDFormQueries.GetAllgeneratedOPDRequestByDateWise,
                paramCollection, CommandType.StoredProcedure);

                result = dtPaper.AsEnumerable()
                  .Select(row => new ViewBillsModel_Report
                  {

                      RequestNo = row.Field<string>("RequestNo"),
                      FileNo = row.Field<string>("CompanyName"),
                      PatientName = row.Field<string>("PatientName"),
                      PatientAddress = row.Field<string>("PatientAddress"),
                      BillNo = row.Field<string>("OPDNo"),
                      BillAmount = row.Field<double>("BillAmount"),
                      ClientName = row.Field<string>("ClientName"),
                      ManagementType = row.Field<string>("ManagementType"),
                      DoctorIncharge = row.Field<string>("DoctorIncharge"),
                      RegistrationNo = row.Field<string>("RegistrationNo"),
                      IsDeactive = row.Field<string>("IsDeactive"),
                      Comment = row.Field<string>("DeactiveComment"),
                      LoginName = row.Field<string>("LoginName"),
                      BillDate = row.Field<string>("StrGeneratedDt"),
                      FromDate = row.Field<string>("FromDate"),
                      ToDate = row.Field<string>("ToDate"),
                      TypeofReport = row.Field<int>("TypeofReport")
                  }).ToList();
            }
            return result;

        }

        #region INsert Update Operations

        #region Create Request
        public RequestSubmissionOPDModel CreateRequest(RequestSubmissionOPDModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DateTime OPDDate = new DateTime();
                if (!DateTime.TryParse(entity.StrOPDDate, out OPDDate))
                    OPDDate = default(DateTime);

                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.RequestId, DbType.Int32, ParameterDirection.Output));
                paramCollection.Add(new DBParameter("RegistrationNo", string.IsNullOrWhiteSpace(entity.RegistrationNo) ? "" : (entity.RegistrationNo), DbType.String));
                paramCollection.Add(new DBParameter("PatientName", entity.PatientName, DbType.String));
                paramCollection.Add(new DBParameter("PatientAddress", entity.PatientAddress, DbType.String));
                paramCollection.Add(new DBParameter("OPDNo", entity.OPDNo, DbType.String));
                paramCollection.Add(new DBParameter("OPDDate", OPDDate, DbType.DateTime));
                paramCollection.Add(new DBParameter("HospitalTypeId", entity.HospitalTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("PatientTypeId", entity.PatientTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("GenderId", entity.GenderId, DbType.Int32));
                paramCollection.Add(new DBParameter("PatientAge", entity.PatientAge, DbType.Double));
                paramCollection.Add(new DBParameter("BillAmount", entity.BillAmount, DbType.Double));
                paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
                paramCollection.Add(new DBParameter("Comment", entity.Comment, DbType.String));
                paramCollection.Add(new DBParameter("NameOfDoctor1", entity.NameOfDoctor1, DbType.String));
                paramCollection.Add(new DBParameter("NameOfDoctor2", entity.NameOfDoctor2, DbType.String));
                paramCollection.Add(new DBParameter("NameOfDoctor3", entity.NameOfDoctor3, DbType.String));
                paramCollection.Add(new DBParameter("StateId", entity.StateId, DbType.Int32));
                paramCollection.Add(new DBParameter("CityId", entity.CityId, DbType.Int32));
                paramCollection.Add(new DBParameter("TypeOfAddmissionId", entity.TypeOfAddmissionId, DbType.Int32));
                paramCollection.Add(new DBParameter("CompanyName", entity.CompanyName, DbType.String));

                var parameterList = dbHelper.ExecuteNonQueryForOutParameter(OPDFormQueries.CreateOPDRequest, paramCollection, CommandType.StoredProcedure);
                entity.RequestId = Convert.ToInt32(parameterList["RequestId"].ToString());

                object obj =
                    dbHelper.ExecuteScalar("select RequestNo from hsp_OPDRequestMaster where requestid = " +
                                           entity.RequestId);
                entity.RequestNo = obj.ToString();
            }).IfNotNull(ex => { throw (ex); });
            return entity;
        }
        #endregion

        #region Update Request
        public RequestSubmissionOPDModel UpdateRequest(RequestSubmissionOPDModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DateTime OPDDate = new DateTime();
                if (!DateTime.TryParse(entity.StrOPDDate, out OPDDate))
                    OPDDate = default(DateTime);

                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.RequestId, DbType.Int32));
                paramCollection.Add(new DBParameter("RegistrationNo", string.IsNullOrWhiteSpace(entity.RegistrationNo) ? "" : (entity.RegistrationNo), DbType.String));
                paramCollection.Add(new DBParameter("PatientName", entity.PatientName, DbType.String));
                paramCollection.Add(new DBParameter("PatientAddress", entity.PatientAddress, DbType.String));
                paramCollection.Add(new DBParameter("OPDNo", entity.OPDNo, DbType.String));
                paramCollection.Add(new DBParameter("OPDDate", OPDDate, DbType.DateTime));
                paramCollection.Add(new DBParameter("HospitalTypeId", entity.HospitalTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("PatientTypeId", entity.PatientTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("GenderId", entity.GenderId, DbType.Int32));
                paramCollection.Add(new DBParameter("PatientAge", entity.PatientAge, DbType.Double));
                paramCollection.Add(new DBParameter("BillAmount", entity.BillAmount, DbType.Double));
                paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
                paramCollection.Add(new DBParameter("Comment", entity.Comment, DbType.String));
                paramCollection.Add(new DBParameter("NameOfDoctor1", entity.NameOfDoctor1, DbType.String));
                paramCollection.Add(new DBParameter("NameOfDoctor2", entity.NameOfDoctor2, DbType.String));
                paramCollection.Add(new DBParameter("NameOfDoctor3", entity.NameOfDoctor3, DbType.String));
                paramCollection.Add(new DBParameter("StateId", entity.StateId, DbType.Int32));
                paramCollection.Add(new DBParameter("CityId", entity.CityId, DbType.Int32));
                paramCollection.Add(new DBParameter("TypeOfAddmissionId", entity.TypeOfAddmissionId, DbType.Int32));
                paramCollection.Add(new DBParameter("CompanyName", entity.CompanyName, DbType.String));
                paramCollection.Add(new DBParameter("IsDeactive", entity.IsDeactive, DbType.Boolean));
                paramCollection.Add(new DBParameter("DeactiveComment", entity.DeactiveComment, DbType.String));
                dbHelper.ExecuteNonQuery(OPDFormQueries.UpdateOPDRequest, paramCollection,
                    CommandType.StoredProcedure);
            }).IfNotNull(ex => { throw (ex); });
            return entity;
        }
        #endregion

        #region Create Services consumed
        public void CreateOPDRequestDetail(CommonMasterModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.TransactionId, DbType.Int32));
                paramCollection.Add(new DBParameter("Id", entity.Id, DbType.Int32));
                paramCollection.Add(new DBParameter("ServiceId", entity.ServiceId, DbType.Int32));
                paramCollection.Add(new DBParameter("Qty", entity.Qty, DbType.Int32));
                dbHelper.ExecuteNonQuery(OPDFormQueries.CreateOPDRequestDetail, paramCollection, CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreateOPDRequestDetail of OPD Bill repository" + ex.Message +
                                 Environment.NewLine + ex.StackTrace);
            });
        }
        #endregion

        #region Default services
        public void CreateOPDRequestDefaultServicesDetail(CommonMasterModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.TransactionId, DbType.Int32));
                paramCollection.Add(new DBParameter("ServiceId", entity.ServiceId, DbType.Int32));
                paramCollection.Add(new DBParameter("Qty", entity.Qty, DbType.Int32));
                dbHelper.ExecuteNonQuery(OPDFormQueries.CreateOPDDefaultServiceDetail, paramCollection, CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreateOPDRequestDetail of OPD Bill repository" + ex.Message +
                                 Environment.NewLine + ex.StackTrace);
            });
        }
        #endregion

        #region Create Manually added OPD services
        public void CreateOPDRequestManullyAddedDetail(CommonMasterModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.TransactionId, DbType.Int32));
                paramCollection.Add(new DBParameter("Name", (string.IsNullOrWhiteSpace(entity.Name) ? string.Empty : entity.Name), DbType.String));
                paramCollection.Add(new DBParameter("Qty", entity.Qty, DbType.Int32));
                paramCollection.Add(new DBParameter("Amount", entity.BillRate, DbType.Double));
                dbHelper.ExecuteNonQuery(OPDFormQueries.CreateOPDRequestManullyAddedDetail, paramCollection,
                    CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreateRequestManullyAddedDetail of RequestSubmision repository" +
                                 ex.Message + Environment.NewLine + ex.StackTrace);
            });
        }
        #endregion

        public void CreatePatientLink(PatientModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", entity.RequestId, DbType.Int32));
                paramCollection.Add(new DBParameter("PatientId", entity.PatientId, DbType.Int32));
                paramCollection.Add(new DBParameter("ParentPatientId", entity.ParentPatientId, DbType.Int32));
                paramCollection.Add(new DBParameter("IsDependent", entity.IsDependent, DbType.Boolean));
                dbHelper.ExecuteNonQuery(OPDFormQueries.CreatePatientLink, paramCollection,
                    CommandType.StoredProcedure);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("error at CreatePatientLink of RequestSubmisionOPD repository" +
                                 ex.Message + Environment.NewLine + ex.StackTrace);
            });
        }
        #endregion

        public List<CommonMasterModel> GetOPDRequestDetailById2(int requestId, int configId, bool isReport = false, string ConnectionString = "DefaultConnection")
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            paramCollection.Add(new DBParameter("ConfigId", configId, DbType.Int32));
            List<CommonMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(OPDFormQueries.GetOPDRequestDetailById2,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        Id = row.Field<int>("DivId"),
                        TransactionId = row.Field<int>("TransactionId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Name = row.Field<string>("Name"),
                        Code = row.Field<string>("ServiceCode"),
                        Qty = row.Field<int>("Qty"),
                        CghsCode = row.Field<string>("CghsCode"),
                        ConsumeDate = row.Field<string>("ConsumeDate"),
                        BillRate = (isReport ? row.Field<double>("BillRate") : 0)
                    }).ToList();
            }

            return list;
        }

        public List<CommonMasterModel> GetDefaultOPDServicesDetail2(int requestId, int configId, bool isReport = false, string ConnectionString = "DefaultConnection")
        {
            List<CommonMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("RequestId", requestId, DbType.Int32));
            paramCollection.Add(new DBParameter("ConfigId", configId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(OPDFormQueries.GetOPDRequestDefaultServiceDetailById2,
                    paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        TransactionId = row.Field<int>("RequestId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Qty = row.Field<int>("Qty"),
                        CghsCode = row.Field<string>("CghsCode"),
                        ConsumeDate = row.Field<string>("ConsumeDate"),
                        BillRate = (isReport ? row.Field<double>("BillRate") : 0)
                    }).ToList();
            }

            return list;
        }

    }
}
