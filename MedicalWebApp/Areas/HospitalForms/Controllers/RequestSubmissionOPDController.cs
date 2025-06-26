using CGHSBilling.API.HospitalForms.Intefaces;
using CGHSBilling.API.HospitalForms.Repository;
using CGHSBilling.API.Masters.Repositories;
using CGHSBilling.Areas.HospitalForms.Models;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Caching;
using CGHSBilling.Common;
using CommonDataLayer.DataAccess;
using CommonLayer;
using CommonLayer.Extensions;
using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using static CGHSBilling.Controllers.HomeController;
using System.Transactions;

namespace CGHSBilling.Areas.HospitalForms.Controllers
{
    [ValidateHeaderAntiForgeryTokenAttribute]
    public class RequestSubmissionOPDController : Controller
    {
        IOPDBillingRepository _dao;
        private static readonly ILogger Loggger = Logger.Register(typeof(RequestSubmissionOPDController));
        ConnectionString _connectionString;

        public RequestSubmissionOPDController()
        {
            _dao = new OPDBillingRepository();
            _connectionString = new ConnectionString();
        }

        #region Create / Update OPD Request details
        [HttpPost]
        public ActionResult CreateRequest(RequestSubmissionOPDModel jsonData)
        {
            Loggger.LogInfo("Create request Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            bool isSuccess = false;
            string requestMsg = string.Empty; bool IsNewEntry = false;

            jsonData.InsertedBy = Convert.ToInt32(Session["AppUserId"]);
            jsonData.InsertedOn = System.DateTime.Now;
            jsonData.InsertedIpAddress = Constants.IpAddress;
            jsonData.InsertedMacId = Constants.MacId;
            jsonData.InsertedMacName = Constants.MacName;
            using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                   // IDbTransaction transaction = dbHelper.BeginTransaction();
                    try
                    {
                        var billAmt = GetOPDCalculatedBill(jsonData);
                        jsonData.BillAmount = billAmt;

                        if (jsonData.RequestId > 0)
                        {
                            IsNewEntry = false;
                            _dao.UpdateRequest(jsonData, dbHelper);
                            Loggger.LogInfo("   OPD Create request Completed for UpdateRequest at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                        }
                        else
                        {
                            IsNewEntry = true;
                            _dao.CreateRequest(jsonData, dbHelper);
                            Loggger.LogInfo("   OPD Create request Completed for CreateRequest at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                        }
                        if (jsonData.RequestId > 0)
                        {

                            if (jsonData.Patient != null && IsNewEntry)
                            {
                                jsonData.Patient.RequestId = jsonData.RequestId;
                                _dao.CreatePatientLink(jsonData.Patient, dbHelper);
                            }

                            //Adding Consumed services to database
                            if (jsonData.ConsumeDiv != null)
                            {
                                int count = jsonData.ConsumeDiv.Count;
                                foreach (var item in jsonData.ConsumeDiv)
                                {
                                    item.TransactionId = jsonData.RequestId;
                                    _dao.CreateOPDRequestDetail(item, dbHelper);
                                }
                                Loggger.LogInfo("   OPD Create request Completed for request details details at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                            }

                            //Adding Manually services to database
                            if (jsonData.ManullyAddedService != null)
                            {
                                foreach (var manully in jsonData.ManullyAddedService)
                                {
                                    manully.TransactionId = jsonData.RequestId;
                                    _dao.CreateOPDRequestManullyAddedDetail(manully, dbHelper);
                                }
                                Loggger.LogInfo("   OPD Create request Completed for Manually added services details details at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                            }

                            //Add Default services to database - always run default service add to database
                            if (jsonData.DefaultServices != null)
                            {
                                foreach (var defaultSer in jsonData.DefaultServices)
                                {
                                    var tempDefaultServ = new CommonMasterModel { TransactionId = jsonData.RequestId, ServiceId = defaultSer.ServiceId, ServiceName = defaultSer.ServiceName, ServiceTypeId = defaultSer.ServiceTypeId, ConsumeDate = defaultSer.ConsumeDate, Qty = defaultSer.Qty };
                                    _dao.CreateOPDRequestDefaultServicesDetail(tempDefaultServ, dbHelper);
                                }
                                Loggger.LogInfo("   OPD Create request Completed for Default services details details at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                            }

                            //dbHelper.CommitTransaction(transaction);
                            txscope.Complete();
                            ClearAllSession();
                        }
                        isSuccess = true;
                        //                    requestMsg = !IsNewEntry ? "Sucessfully updated data for Request no. " + jsonData.RequestNo : "New request " + jsonData.RequestNo + " created successfully";
                        requestMsg = !IsNewEntry ? "OPD Request no. " + jsonData.RequestNo + " updated for Bill Amount <b>Rs." + String.Format("{0:n}", billAmt) + "</b>" : "Sucessfully generated OPD bill for Amount <b>Rs." + String.Format("{0:n}", billAmt) + Environment.NewLine + "</b> Your new request no. is " + jsonData.RequestNo;
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        requestMsg = (!IsNewEntry ? "Failed to updated data for Request no. " + jsonData.RequestNo : "Failed to create new request") + " for error " + ex.Message.ToString();
                        //dbHelper.RollbackTransaction(transaction);
                        txscope.Dispose();
                        Loggger.LogError("Error in CreateRequest :", ex);
                    }
                    finally
                    {
                        Loggger.LogInfo("Create request Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                    }
                }
            }
            return Json(new { requestId = jsonData.RequestId, requestNo = jsonData.RequestNo, success = isSuccess, responseMsg = requestMsg, isDeactive = jsonData.IsDeactive }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get All OPD request details 
        public ActionResult GetAllOPDRequest()
        {
            Loggger.LogInfo("GetAllOPDRequest Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<RequestSubmissionOPDModel> list = new List<RequestSubmissionOPDModel>();
            JsonResult jlResult;
            bool IsHopeClient = false;
            try
            {
                list = _dao.GetAllOPDRequest();             
                jlResult = Json(new { data = list}, JsonRequestBehavior.AllowGet);
                return jlResult;

            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllOPDRequest :", ex);
                return Json("error");
            }
            finally
            {
                Loggger.LogInfo("GetAllOPDRequest Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jlResult;
        }
        #endregion

        #region Load OPD Services for Partial View
        [HttpPost]
        public ActionResult GetOPDServiceView(RequestSubmissionOPDModel jsonData)
        {
            Loggger.LogInfo("GetOPDServiceView Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            string view = string.Empty;
            try
            {
                int userId = Convert.ToInt32(Session["AppUserId"]);
                jsonData.ConsumeDiv = OPDServicesConsumed();

                foreach (var service in jsonData.ConsumeDiv)
                    GetServiceMasterList(userId, service.Id, jsonData.HospitalTypeId, jsonData.PatientTypeId, jsonData.StateId, jsonData.CityId, jsonData.GenderId, 0);

                view = RenderPartialViewToString(this, "~/Areas/HospitalForms/Views/HospitalForms/DateWiseOPDConfigPartialView.cshtml", jsonData);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in CommonMasterController  HospitalTypeDropDown:", ex);
            }
            finally
            {
                Loggger.LogInfo("GetOPDServiceView Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return Json(new { partialview = view}, JsonRequestBehavior.AllowGet);
        }

        private List<CommonMasterModel> OPDServicesConsumed()
        {
            Loggger.LogInfo("OPDServicesConsumed Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<CommonMasterModel> list = new List<CommonMasterModel>();
            TryCatch.Run(() =>
            {
                if (MemoryCaching.CacheKeyExist(CachingKeys.OPDServices.ToString()))
                    list= MemoryCaching.GetCacheValue(CachingKeys.OPDServices.ToString()) as List<CommonMasterModel>;
                else
                    list = _dao.OPDServicesConsumed();
            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in RequestSubmissionOPDController method of OPDServicesConsumed :", ex);
            });

            Loggger.LogInfo("OPDServicesConsumed Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            return list;
        }

        private List<ServiceMasterModel> GetServiceMasterList(int userId, int categoryId, int hospitalType, int patientType, int stateId, int cityId, int gender, int roomTypeId)
        {
            Loggger.LogInfo("GetServiceMasterList for OPD Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());

            var serviceType = new List<ServiceMasterModel>();
            var sessionServiceType = Session["OPDServiceMaster_" + categoryId];
            if (sessionServiceType != null) serviceType = sessionServiceType as List<ServiceMasterModel>;
            string ConnectionString = _connectionString.getConnectionStringName();
            if (serviceType == null || serviceType.Count == 0)
            {
                //var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                //Loggger.LogInfo("   uiScheduler Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                //Task.Factory.StartNew(() =>
                {
                    serviceType = _dao.GetAllOPDServiceMasterByCategoryId(categoryId, userId, hospitalType, patientType, stateId, cityId, gender, roomTypeId,false, ConnectionString);
                    Loggger.LogInfo("   GetAllServiceMasterByCategoryId Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                    if (Session["OPDServiceMaster_" + categoryId] != null)
                        Session["OPDServiceMaster_" + categoryId] = serviceType;
                    else
                        Session.Add("OPDServiceMaster_" + categoryId, serviceType);
                }
                //, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }

            Loggger.LogInfo("GetServiceMasterList Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            return serviceType.Where(w => w.CategoryId == categoryId && w.HospitalTypeId == hospitalType
                                                && w.PatientTypeId == (patientType == 0 ? w.PatientTypeId : patientType) && w.StateId == stateId && w.CityId == cityId 
                                                && w.GenderId == (gender == 0 ? w.GenderId : gender) 
                                                && w.RoomTypeId == (roomTypeId == 0? w.RoomTypeId : roomTypeId)).ToList();
        }

        private string RenderPartialViewToString(Controller thisController, string viewName, object model)
        {
            try
            {
                // assign the model of the controller from which this method was called to the instance of the passed controller (a new instance, by the way)
                thisController.ViewData.Model = model;

                // initialize a string builder
                using (StringWriter sw = new StringWriter())
                {
                    // find and load the view or partial view, pass it through the controller factory
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(thisController.ControllerContext, viewName);
                    ViewContext viewContext = new ViewContext(thisController.ControllerContext, viewResult.View, thisController.ViewData, thisController.TempData, sw);

                    // render it
                    viewResult.View.Render(viewContext, sw);

                    //return the razorized view/partial-view as a string
                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in RequestSubmissionNewController method of RenderPartialViewToString :", ex);
                return "";
            }
        }
        #endregion

        #region Validate data passed from user
        private bool Validate(RequestSubmissionOPDModel data, out string errormessage)
        {
            Loggger.LogInfo("Validate Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            errormessage = "";
            if (data != null)
            {
                DateTime OPDDate = new DateTime();
                //if (string.IsNullOrEmpty(data.FileNo))
                //    errormessage += "File no is missing.";
                if (data.HospitalTypeId.NullToInt() == 0)
                    errormessage += "Hospital Type, ";
                if (data.PatientTypeId.NullToInt() == 0)
                    errormessage += "Patient Type, ";
                if (data.StateId.NullToInt() == 0)
                    errormessage += "State, ";
                if (data.CityId.NullToInt() == 0)
                    errormessage += "City, ";
                
                if (string.IsNullOrEmpty(data.OPDNo))
                    errormessage += "OPD No, ";
                if (DateTime.TryParse(data.StrOPDDate, out OPDDate))
                    errormessage += "OPD Date, ";
                if (string.IsNullOrEmpty(data.PatientName))
                    errormessage += "Patient Name, ";
                if (string.IsNullOrWhiteSpace(Convert.ToString(data.PatientAge)) || data.PatientAge == 0.0)
                    errormessage += "Patient Age, ";
                if (data.GenderId.NullToInt() == 0)
                    errormessage += "Gender, ";
                if (string.IsNullOrEmpty(data.PatientAddress))
                    errormessage += "Patient Address, ";
                if (string.IsNullOrEmpty(data.NameOfDoctor1))
                    errormessage += "First Doctor/Consultant Name, ";

                if (errormessage.Count(c => c == ',') > 0)
                {
                    errormessage = errormessage.Remove(errormessage.LastIndexOf(','), 1);
                    int lastIndexofComma = errormessage.LastIndexOf(',');
                    var FirstPartErrormessage = errormessage.Substring(0, lastIndexofComma);
                    var SecondPartErrormessage = errormessage.Substring(lastIndexofComma, errormessage.Length - lastIndexofComma);
                    errormessage = "Missing " + FirstPartErrormessage + SecondPartErrormessage.Replace(", ", " and ") + "\n Please provide missing data.";
                }
            }
            else
                errormessage = "Data is Null.";

            Loggger.LogInfo("Validate Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            if (string.IsNullOrEmpty(errormessage))
                return true;
            else
                return false;
        }
        #endregion

        #region Show service list in popup by Category clicked by user
        public ActionResult DisplayOPDServicesConsumedSession(int requestId, int categoryId, int hospitalType, int patientType, int stateId, int cityId, int gender)
        {
            Loggger.LogInfo("DisplayOPDServicesConsumedSession Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            JsonResult jsonResult = null;
            int userId = Convert.ToInt32(Session["AppUserId"]);
            string ConnectionString = "";
            try
            {
                ConnectionString = _connectionString.getConnectionStringName();
                var serviceType = GetServiceMasterList(userId, categoryId, hospitalType, patientType, stateId, cityId, gender, 0);
                var taskChoosenServices = Task.Run(() => { return (requestId > 0 ? _dao.GetOPDRequestDetailById(requestId,false, ConnectionString) : null); });
                taskChoosenServices.Wait();

                var choosenService = taskChoosenServices.Result;
                if (choosenService != null && choosenService.Count > 0)
                    serviceType.Join(choosenService, s => s.ServiceId, c => c.ServiceId, (s, c) => { s.State = true; return s; });


                Loggger.LogInfo("   GetServiceMasterList Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                jsonResult = Json(new { sessionRecord = serviceType }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in Method: DisplayOPDServicesConsumedSession class:RequestSubmissionNewController :", ex);
            }
            finally
            {
                Loggger.LogInfo("DisplayOPDServicesConsumedSession Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jsonResult;
        }
        #endregion

        #region Get List of Manually Added OPD details
        public ActionResult GetRequestManuallyOPDAddedDetail(int requestId)
        {
            Loggger.LogInfo("GetRequestManuallyOPDAddedDetail Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<CommonMasterModel> list = new List<CommonMasterModel>();
            JsonResult jlResult;
            try
            {
                list = _dao.GetRequestManuallyOPDAddedDetail(requestId, _connectionString.getConnectionStringName());
                Session["OPDManuallyAddedDetailById"] = list;
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetRequestManuallyOPDAddedDetail :", ex);
                return Json("error");
            }
            finally
            {
                Loggger.LogInfo("GetRequestManuallyOPDAddedDetail Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jlResult;
        }
        #endregion

        #region Get details for Selected OPD request like services selected
        public ActionResult GetOPDRequestDetailById(int requestId)
        {
            Loggger.LogInfo("GetOPDRequestDetailById Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            JsonResult jlResult = null;
            try
            {
                ClearAllSession();
                string Connectionstring = _connectionString.getConnectionStringName();
                var taskServices = Task.Run(() => { return (requestId > 0 ? _dao.GetOPDRequestDetailById(requestId,false, Connectionstring) : null); });
                var taskManualServices = Task.Run(() => { return (requestId > 0 ? _dao.GetRequestManuallyOPDAddedDetail(requestId, Connectionstring) : null); });
                var taskDefaultSer = Task.Run(() => { return (requestId > 0 ? _dao.GetDefaultOPDServicesDetail(requestId, false, Connectionstring) : null); });
                var taskHopePatientData = Task.Run(() => { return (requestId > 0  ? _dao.GetPatientData(requestId, Connectionstring) : null); });
                Task.WaitAll(taskServices, taskManualServices, taskDefaultSer);
                Loggger.LogInfo("   Task.WaitAll Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());

                var ManualServices = taskManualServices.Result;
                var DefaultService = taskDefaultSer.Result;
                var ServiceConsumed = taskServices.Result;
                var Patient = taskHopePatientData.Result;
                if (ServiceConsumed != null && ServiceConsumed.Count > 0)
                {
                    Loggger.LogInfo("   OPD ServiceConsumed Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                    ServiceConsumed.ToList().ForEach(c => c.State = true);
                    foreach (var item in ServiceConsumed)
                    {
                        Session["OPDServiceConsumed_" + item.Id] = ServiceConsumed.Where(m => m.Id == item.Id).ToList();
                        item.State = true;
                    }
                }

                if (DefaultService != null && DefaultService.Count > 0)
                {
                    Loggger.LogInfo("   OPD DefaultService Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                    DefaultService.ToList().ForEach(c => c.State = true);
                }

                jlResult = Json(new { ServiceConsumed = ServiceConsumed, ManualServices = ManualServices, DefaultService = DefaultService, Patient = Patient }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetOPDRequestDetailById :", ex);
            }
            finally
            {
                Loggger.LogInfo("GetOPDRequestDetailById Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jlResult;
        }
        #endregion

        #region Get Default service details 
        private List<ServiceMasterModel> GetOPDDefaultServices(int hospitalType, int patientType, int gender, int stateId, int cityId,string ConnectionString)
        {
            Loggger.LogInfo("GetOPDDefaultServices Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            int userId = Convert.ToInt32(Session["AppUserId"]);
            try
            {
                if (MemoryCaching.CacheKeyExist(CachingKeys.DefaultServiceMasters_Rates.ToString()))
                {
                    var exists = MemoryCaching.GetCacheValue(CachingKeys.DefaultServiceMasters_Rates.ToString()) as List<ServiceMasterModel>;
                    if (exists != null && exists.Any(a => a.HospitalTypeId == hospitalType && a.PatientTypeId == patientType
                                    && a.StateId == stateId && a.CityId == cityId && a.GenderId == gender))
                    {
                        Loggger.LogInfo("   GetCacheValue for GetOPDDefaultServices Started for  " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                        return exists.Where(a => a.HospitalTypeId == hospitalType && a.PatientTypeId == patientType
                                    && a.StateId == stateId && a.CityId == cityId && a.GenderId == gender).ToList();
                    }
                }
                return new ServiceMasterRepository().GetAllActiveDefaultServiceMaster_WithRates(0, hospitalType, patientType, gender, stateId, cityId, ConnectionString);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetOPDDefaultServices :", ex);
                return null;
            }
        }
        #endregion

        #region ClearAllSession
        public void ClearAllSession()
        {
            Loggger.LogInfo("ClearAllSession Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            var SessionKeys = Session.Keys.OfType<string>().Where(w => w.StartsWith("OPD"));
            foreach (string key in SessionKeys.ToArray())
                Session.Remove(key);

            Loggger.LogInfo("ClearAllSession Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
        }
        #endregion

        #region "OPD Calculate Bill
        [HttpPost]
        public JsonResult CalculateOPDBill(RequestSubmissionOPDModel jsonData)
        {
            Loggger.LogInfo("OPD Calculate Bill Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            double BillAmount = 0.0;
            try
            {
                BillAmount = GetOPDCalculatedBill(jsonData);
                return Json(new { success = true, billAmount = BillAmount.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in CalculateOPDBill :", ex);
                return Json(new { success = false, billAmount = BillAmount.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        private double GetOPDCalculatedBill(RequestSubmissionOPDModel jsonData)
        {
            Loggger.LogInfo("OPD Calculate Bill Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            double BillAmount = 0.0; 
            try                
            {
                string ConnectionString = _connectionString.getConnectionStringName();
                var taskCharges = Task.Run(() =>
                {
                    var tempAmount = CalcualteDefaultServicesCharge(jsonData.DefaultServices, jsonData.HospitalTypeId, jsonData.PatientTypeId, jsonData.StateId, jsonData.CityId, jsonData.GenderId, ConnectionString);
                    tempAmount = tempAmount + CalculatedManualServiceCharges(jsonData.ManullyAddedService);
                    return tempAmount;
                });

                //Consumed Services
                var taskConsumeServicesCharges = Task.Run(() =>
                {
                    return CalcualateConsumedServiceCharges(jsonData.ConsumeDiv, jsonData.HospitalTypeId, jsonData.PatientTypeId, jsonData.StateId, jsonData.CityId, jsonData.GenderId, ConnectionString);
                });

                Task.WaitAll(taskCharges, taskConsumeServicesCharges);
                BillAmount = taskCharges.Result + taskConsumeServicesCharges.Result;

                Loggger.LogInfo("OPD Calculated Bill completed for Bill:" + BillAmount.ToString());
                return BillAmount;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in Get OPD Calculate Bill :", ex);
                return BillAmount;
            }
        }

        /// <summary>
        /// Calcualte Default Services Charge
        /// </summary>
        /// <param name="hospitalTypeId"></param>
        /// <param name="patientTypeId"></param>
        /// <param name="stateId"></param>
        /// <param name="cityId"></param>
        /// <param name="genderId"></param>
        /// <param name="roomEntitleTypeId"></param>
        /// <param name="bedCharges">Bed Charges</param>
        /// <returns></returns>
        private double CalcualteDefaultServicesCharge(List<CommonMasterModel> data, int hospitalTypeId, int patientTypeId, int stateId, int cityId, int genderId,string ConnectionString)
        {
            var defaultServiceBillRates = 0.0;
            var services = GetOPDDefaultServices(hospitalTypeId, patientTypeId, genderId, stateId, cityId, ConnectionString);

            //below line for assigning to Default service will help while saving
            if (services != null && services.Count() > 0 && data != null && data.Count > 0)
            {
                defaultServiceBillRates = services.Join(data, l=> l.ServiceId, r=> r.ServiceId, (l,r) => { l.Qty = r.Qty; return l; }).Sum(s=> (s.BillRate * s.Qty));
                //foreach (var serv in defaultServices)
                //    defaultServiceBillRates = defaultServiceBillRates + (serv.BillRate * serv.Qty);
            }
            Loggger.LogInfo("    OPD Calculate Bill - Default Bill for one day: " + defaultServiceBillRates);
            return defaultServiceBillRates;
        }

        /// <summary>
        /// Calculated Manual Service Charges
        /// </summary>
        /// <param name="data"><Manual services</param>
        /// <returns></returns>
        private double CalculatedManualServiceCharges(List<CommonMasterModel> data)
        {
            var tempAmount = 0.0;
            if (data != null)
            {
                foreach (var manully in data)
                    tempAmount = tempAmount + (manully.BillRate * manully.Qty);

                Loggger.LogInfo("    Calculate Bill - Manual Service amount:" + tempAmount.ToString());
            }
            return tempAmount;
        }

        /// <summary>
        /// Calcualate Consumed Service Charges
        /// </summary>
        /// <param name="data">Consumed services</param>
        /// <param name="hospitalTypeId"></param>
        /// <param name="patientTypeId"></param>
        /// <param name="stateId"></param>
        /// <param name="cityId"></param>
        /// <param name="genderId"></param>
        /// <returns></returns>
        private double CalcualateConsumedServiceCharges(List<CommonMasterModel> data, int hospitalTypeId, int patientTypeId, int stateId, int cityId, int genderId,string ConnectionString)
        {
            var tempAmount = 0.0;
            var serviceRates = new List<ServiceMasterModel>();
            if (data != null)
            {
                //foreach (var service in data)
                foreach (var service in data.Select(s => s.Id).Distinct())
                {
                    var result = _dao.GetAllOPDServiceMasterByCategoryId(service, Convert.ToInt32(Session["AppUserId"]), hospitalTypeId, patientTypeId, stateId, cityId, genderId, 0, true, ConnectionString);
                    if (result != null && result.Count() > 0)
                        serviceRates.AddRange(result);
                }
                data.Where(w => w.Qty == 0).ToList().ForEach(a => a.Qty = 1);

                var tempBillRate = 0.0;
                foreach (var service in data)
                {
                    tempBillRate = 0.0;
                    var first = serviceRates.Where(w => w.CategoryId == service.Id && w.ServiceId == service.ServiceId).FirstOrDefault();
                    tempBillRate = first != null ? first.BillRate : 0;
                    tempAmount = tempAmount + (tempBillRate * service.Qty);
                }
                Loggger.LogInfo("    OPD Calculate Bill - Service Consumed amount:" + tempAmount.ToString());
            }

            return tempAmount;
        }
        #endregion

        //private string getConnectionString()
        //{

        //    string ConnectionString = "";
        //    if (HttpContext.Session["DatabaseSeLection"] != null)
        //    {
        //        if (Convert.ToString(Session["DatabaseSeLection"]) == "Mumbai") ConnectionString = "DefaultConnection";
        //        else ConnectionString = "CghsDelhi";
        //    }
        //    else ConnectionString = "DefaultConnection";
        //    return ConnectionString;
        //}

    }


}