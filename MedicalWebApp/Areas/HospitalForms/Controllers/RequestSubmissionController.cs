using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using CGHSBilling.API.HospitalForms.Intefaces;
using CGHSBilling.API.HospitalForms.Repository;
using CGHSBilling.API.ScanDoc;
using CGHSBilling.Areas.HospitalForms.Models;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Caching;
using CGHSBilling.Common;
using CommonDataLayer.DataAccess;
using System.Threading.Tasks;
using CommonLayer;
using CommonLayer.Extensions;
using static CGHSBilling.Controllers.HomeController;

namespace CGHSBilling.Areas.HospitalForms.Controllers
{
    [ValidateHeaderAntiForgeryTokenAttribute]
    public class RequestSubmissionController : Controller
    {
        IHospitalFormsRepository _data;
        private static readonly ILogger Loggger = Logger.Register(typeof(RequestSubmissionController));
        public RequestSubmissionController()
        {
            _data = new HospitalFormsRepository();
        }

        double billAmount;
        public ActionResult CreateRequest(RequestSubmissionModel jsonData)
        {
            JsonResult jlResult; bool isSuccess = false;
            
            jsonData.InsertedBy = Convert.ToInt32(Session["AppUserId"]);
            jsonData.InsertedOn = System.DateTime.Now;
            jsonData.InsertedIpAddress = Constants.IpAddress;
            jsonData.InsertedMacId = Constants.MacId;
            jsonData.InsertedMacName = Constants.MacName;
            using (DBHelper dbHelper = new DBHelper())
            {
                IDbTransaction transaction = dbHelper.BeginTransaction();
                try
                {
                   var billAmt = CalculateGenralBill(jsonData);
                    jsonData.BillAmount = billAmount;
                    if (jsonData.RequestId>0)
                    {
                        _data.UpdateRequest(jsonData, dbHelper);
                    }
                    else
                    {
                        _data.CreateRequest(jsonData, dbHelper);
                    }
                    if (jsonData.RequestId > 0)
                    {
                        List<CommonMasterModel> rightList = _data.ServicesConsumedRightDiv();
                        List<CommonMasterModel> leftList = _data.ServicesConsumedLeftDiv(jsonData.ManagementTypeId);

                        rightList.AddRange(leftList);
                        foreach (var item in rightList)
                        {
                            if (Session[item.Name] != null)
                            {
                                var esesServices = Session[item.Name]  as List<CommonMasterModel>;
                                foreach (var sdetail in esesServices)
                                {
                                    item.TransactionId = jsonData.RequestId;
                                    item.ServiceId = sdetail.ServiceId;
                                    item.RoomTypeId = sdetail.RoomTypeId;
                                    item.Qty = sdetail.Qty;
                                    item.ConsumeDate = sdetail.ConsumeDate;
                                    _data.CreateRequestDetail(item, dbHelper);
                                }
                            }
                        }

                        if (jsonData.AdmissionSummaries != null)
                        {
                            foreach (var admission in jsonData.AdmissionSummaries)
                            {
                               // CalculateDateTimeRate calculate= new CalculateDateTimeRate();
                                admission.RequestId = jsonData.RequestId;
                                admission.Qty = CalculateDateTimeRate.CalculateDays(Convert.ToDateTime(admission.StrAdmissionDateTime), Convert.ToDateTime(admission.StrDischargeDateTime));
                                _data.CreateRequestAdmissionDetail(admission, dbHelper);
                            }
                        }
                        //if (jsonData.LeftOtDetail != null)
                        //{
                        //    foreach (var leftot in jsonData.LeftOtDetail)
                        //    {
                        //        leftot.RequestId = jsonData.RequestId;
                        //        _data.CreateRequestOtDetail(leftot, dbHelper, 1);
                        //    }
                        //}
                        //if (jsonData.RightOtDetail != null)
                        //{
                        //    foreach (var rightot in jsonData.RightOtDetail)
                        //    {
                        //        rightot.RequestId = jsonData.RequestId;
                        //        _data.CreateRequestOtDetail(rightot, dbHelper, 2);
                        //    }
                        //}
                        if (jsonData.PharmacyDetails != null)
                        {
                            foreach (var pharmacy in jsonData.PharmacyDetails)
                            {
                                pharmacy.TransactionId = jsonData.RequestId;
                                _data.CreateRequestPharmacyDetail(pharmacy, dbHelper);
                            }
                        }
                        if (jsonData.ManullyAddedService != null)
                        {
                            foreach (var service in jsonData.ManullyAddedService)
                            {
                                service.TransactionId = jsonData.RequestId;
                                _data.CreateRequestManullyAddedDetail(service, dbHelper);
                            }
                        }
                        dbHelper.CommitTransaction(transaction);
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    dbHelper.RollbackTransaction(transaction);
                    Loggger.LogError("Error in CreateRequest :" + ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
            return Json(new { requestId = jsonData.RequestId, requestNo = jsonData.RequestNo, success = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllgeneratedRequest()
        {
            List< RequestSubmissionModel> list=new List<RequestSubmissionModel>();
            JsonResult jlResult;
            try
            {
                list = _data.GetAllgeneratedRequest();
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllgeneratedRequest :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("error");
            }

            return jlResult;
        }
        public RequestSubmissionModel GetAllgeneratedRequestById(int requestId)
        {
            RequestSubmissionModel list = new RequestSubmissionModel();
            try
            {
                //Task[] pullDatas = new System.Threading.Tasks.Task[6];
                var taskAdmissionSummary = Task.Run(() =>
                {
                    return _data.AdmissionDetailById(requestId);
                });
                var taskRequestMaster = Task.Run(() =>
                {
                    return _data.GetAllgeneratedRequestById(requestId);
                });
                var taskRequestDetail = Task.Run(() =>
                {
                    return _data.GetRequestDetailById(requestId);
                });
                var taskBedcharges = Task.Run(() =>
                {
                    return _data.GetRequestBedChargeDetails(requestId);
                });
                var taskPharmacy = Task.Run(() =>
                {
                    return _data.GetRequestPharmacyDetail(requestId);
                });
                var taskManualServices= Task.Run(() =>
                {
                    return _data.GetRequestManuallyAddedDetail(requestId);
                });

                Task.WaitAll(taskAdmissionSummary, taskRequestMaster, taskRequestDetail, taskBedcharges, taskPharmacy, taskManualServices);
                list = taskRequestMaster.Result;
                list.AdmissionSummaries = taskAdmissionSummary.Result;
                list.PharmacyDetails = taskPharmacy.Result;
                list.BedCharges = taskBedcharges.Result;
                list.ManullyAddedService = taskManualServices.Result;
                list.Investigation = taskRequestDetail.Result;

                if(list.Investigation != null && list.Investigation.Count > 0)
                    list.Investigation = list.Investigation.OrderBy(o => o.Name).ThenBy(t => t.ConsumeDate).ToList();
                //if (list.BedCharges != null)
                //{
                //    var temp = new List<CommonMasterModel>();
                //    foreach (var bedChargeDetail in list.BedCharges)
                //    {
                //        temp.Add(new CommonMasterModel {Name="Bed Charges", ServiceName = bedChargeDetail.ServiceName, ConsumeDate = bedChargeDetail.ConsumeDate, BillRate = bedChargeDetail.BillRate, CghsCode = bedChargeDetail.CghsCode, Qty = 1 });
                //    }
                //    if (temp != null && temp.Count > 0)
                //    {
                //        list.Investigation.AddRange(temp);
                //        list.Investigation = list.Investigation.OrderBy(o => o.ConsumeDate).ToList();
                //    }
                //}

                //list = _data.GetAllgeneratedRequestById(requestId);
                //list.AdmissionSummaries = _data.AdmissionDetailById(requestId);
                //list.PharmacyDetails = _data.GetRequestPharmacyDetail(requestId);
                //list.BedCharges = _data.GetRequestBedChargeDetails(requestId);
                //list.ManullyAddedService = _data.GetRequestManuallyAddedDetail(requestId);
                //list.Investigation = _data.GetRequestDetailById(requestId);//.Where(m=>m.Name.Contains("Left")).ToList();
                //list.LeftOtDetail = _data.GetRequestOtDetailById(requestId).Where(m=>m.OtType==1).ToList();

                //if (list.Investigation != null)
                //{
                //    foreach (var item in list.Investigation)
                //    {
                //        var splt = item.Name.Split('_');
                //        if (splt[0] == "Right")
                //        {
                //            list.Investigation.Remove(item);
                //        }
                //        else
                //        {
                //            item.Name = splt[1];
                //        }

                //    }
                //}

            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllgeneratedRequestById :" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

            return list;
        }
        public ActionResult AdmissionDetailById(int requestId)
        {
            List<AdmissionSummary> list = new List<AdmissionSummary>();
            JsonResult jlResult;
            try
            {
                list = _data.AdmissionDetailById(requestId);
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in AdmissionDetailById :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("error");
            }

            return jlResult;
        }
        //public ActionResult GetRequestPharmacyDetail(int requestId)
        //{
        //    List<CommonMasterModel> list = new List<CommonMasterModel>();
        //    JsonResult jlResult;
        //    try
        //    {
        //        list = _data.GetRequestPharmacyDetail(requestId);
        //        jlResult = Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loggger.LogError("Error in GetRequestPharmacyDetail :" + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return Json("error");
        //    }

        //    return jlResult;
        //}
        public string GetRequestDetailById(int requestId)
        {
            List<CommonMasterModel> list = new List<CommonMasterModel>();
            try
            {
                ClearAllSession();
                list = _data.GetRequestDetailById(requestId);
                foreach (var item in list)
                {
                    Session[item.Name] = list.Where(m => m.Id == item.Id).ToList();// && m.RoomTypeId==item.RoomTypeId
                    var ess= Session[item.Name] as List<CommonMasterModel>;
                    foreach (var det in ess)
                    {
                        det.State = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetRequestDetailById :" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

            return "success";
        }
        public ActionResult GetRequestOtDetailById(int requestId)
        {
            List<AdmissionSummary> list = new List<AdmissionSummary>();
            JsonResult jlResult;
            try
            {
                list = _data.GetRequestOtDetailById(requestId);
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetRequestOtDetailById :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("error");
            }
            return jlResult;
        }
        public void AddServicesConsumedSession(string sessionName, int serviceId,bool state,double rate, int roomType,int qty,string date)
        {
            JsonResult jsonResult = null;
            try
            {
                if (Session[sessionName] == null) Session[sessionName] = new List<CommonMasterModel>();

                var sessionData = Session[sessionName] as List<CommonMasterModel>;
                if(state)
                    sessionData.Add(new CommonMasterModel() {ServiceId = serviceId,State = state, BillRate= rate,RoomTypeId = roomType,Qty = qty,ConsumeDate = date });
                else
                {
                    CommonMasterModel removedata;
                    removedata = sessionData.FirstOrDefault(m => m.ServiceId==serviceId && m.RoomTypeId==roomType);
                    sessionData.Remove(removedata);
                }
                Session[sessionName] = sessionData;
                jsonResult = Json(new { sessionRecord = sessionData }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void UpdateQty(string sessionName, int serviceId, bool state, double rate, int roomType, int qty)
        {
            JsonResult jsonResult = null;
            try
            {
                if (Session[sessionName] == null) Session[sessionName] = new List<CommonMasterModel>();

                var sessionData = Session[sessionName] as List<CommonMasterModel>;
                foreach (var item in sessionData)
                {
                    if (item.ServiceId == serviceId && item.RoomTypeId == roomType)
                    {
                        item.Qty = qty;
                        item.State = state;
                    }
                }
                Session[sessionName] = sessionData;
                jsonResult = Json(new { sessionRecord = sessionData }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public ActionResult DisplayServicesConsumedSession(string sessionName,string categoryName, int hospitalType, int patientType, int stateId, int cityId,int gender)
        {
            JsonResult jsonResult = null;
            List<ServiceMasterModel> service = GetServiceMasterByCategory(categoryName,hospitalType,patientType,stateId,cityId, gender);
            try
            {
                if (Session[sessionName] != null)
                {
                    var sessionData = Session[sessionName] as List<CommonMasterModel>;
                    foreach (var eachitem in sessionData)
                    {
                        var find = service.Where(m => m.ServiceId == eachitem.ServiceId && m.RoomTypeId>0);
                        if (find.Any())
                        {
                            service.Where(m => m.ServiceId == eachitem.ServiceId && m.RoomTypeId == eachitem.RoomTypeId).ToList()[0].State = eachitem.State;
                            service.Where(m => m.ServiceId == eachitem.ServiceId && m.RoomTypeId == eachitem.RoomTypeId).ToList()[0].Qty = eachitem.Qty;
                            service.Where(m => m.ServiceId == eachitem.ServiceId && m.RoomTypeId == eachitem.RoomTypeId).ToList()[0].ConsumeDate = eachitem.ConsumeDate;
                        }
                    }
                }

                foreach (var item in service)
                {
                    if (item.Qty <= 0) item.Qty = 1;
                }
                jsonResult = Json(new { sessionRecord = service }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return jsonResult;
        }
        public List<ServiceMasterModel> GetServiceMasterByCategory(string categoryName, int hospitalType, int patientType, int stateId, int cityId,int gender)
        {
            List<ServiceMasterModel> serviceType = new List<ServiceMasterModel>();
            int userId = 1;//Convert.ToInt32(Session["AppUserId"]);
            try
            {
                var list = _data.GetAllServiceMasterByCategory(categoryName, userId,hospitalType,patientType,stateId,cityId, gender);
//                Caching.MemoryCaching.AddCacheValue("ServiceMaster", list)
                serviceType = list.ToList();
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetServiceMasterByCategory :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return serviceType;
            }
            return serviceType;
        }
        public List<ServiceMasterModel> GetServiceMasterByCategory(int categoryid, int hospitalType, int patientType, int stateId, int cityId, int gender)
        {
            List<ServiceMasterModel> serviceType = new List<ServiceMasterModel>();
            int userId = 1;//Convert.ToInt32(Session["AppUserId"]);
            try
            {
                var list = _data.GetAllServiceMasterByCategoryId(categoryid, userId, hospitalType, patientType, stateId, cityId, gender);
                serviceType = list.ToList();
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetServiceMasterByCategory :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return serviceType;
            }
            return serviceType;
        }
        public  JsonResult GetServiceMasterByCategoryId(int categoryId, int hospitalType,int patientType, int stateId, int cityId, int gender)
        {
            JsonResult serviceType;
            int userId = Convert.ToInt32(Session["AppUserId"]);
            try
            {
                var list = _data.GetServiceMasterByCategoryId(categoryId, userId,hospitalType,patientType, stateId, cityId, gender);
                serviceType =  Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetServiceMasterByCategoryId :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("error");
            }
            return serviceType;
        }
        public JsonResult GetServiceMasterByCategoryRoomId(int categoryId, int hospitalType, int patientType, int roomTypeId, int stateId, int cityId, int gender)
        {
            JsonResult serviceType;
            int userId = Convert.ToInt32(Session["AppUserId"]);
            try
            {
                if (MemoryCaching.CacheKeyExist(CachingKeys.AllBedcharges.ToString()))
                {
                    var existingCache = MemoryCaching.GetCacheValue(CachingKeys.AllBedcharges.ToString()) as List<ServiceMasterModel>;
                    if (existingCache != null && existingCache.Any(a => a.HospitalTypeId == hospitalType && a.PatientTypeId == patientType
                                    && a.RoomTypeId == roomTypeId && a.StateId == stateId && a.CityId == cityId && a.GenderId == gender))
                    {
                        return Json(existingCache, JsonRequestBehavior.AllowGet);
                    }
                }

                var list = _data.GetServiceMasterByCategoryRoomId(categoryId, userId, hospitalType, patientType, roomTypeId, stateId, cityId, gender);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetServiceMasterByCategoryRoomId :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("error");
            }
        }
        public JsonResult CalculateGenralBill(RequestSubmissionModel jsonData)
        {
            double bedCharges=0;
            int userId= Convert.ToInt32(Session["AppUserId"]);
            List<TariffDetailModel> tariff=new List<TariffDetailModel>();
            var list = new List<KeyValuePair<string, int>>();
            List<CommonMasterModel> left = _data.ServicesConsumedLeftDiv(jsonData.ManagementTypeId);
            //List<CommonMasterModel> right = ServicesConsumedRightDiv();
            //right.AddRange(left);
            bool initialCheckPass = true;
            TryCatch.Run(() =>
            {
                if (jsonData.AdmissionSummaries != null)
                {
                    List<CommonMasterModel> tempRoomType = new List<CommonMasterModel>(); List<CommonMasterModel> FinalRoomCharge = new List<CommonMasterModel>();
                    List<ChargeDetailsDayWise> _ChargeDetailsDayWise = new List<ChargeDetailsDayWise>();

                    
                    DateTime admissionTime = Convert.ToDateTime(jsonData.AdmissionSummaries[0].StrAdmissionDateTime);
                    

                    DateTime st = new DateTime(admissionTime.Year, admissionTime.Month, admissionTime.Day, 06, 00, 00);
                    DateTime et = st.AddHours(30); var days = 0;

                    for (int i = 0; i < jsonData.AdmissionSummaries.Count; i++)
                    {
                        var item = jsonData.AdmissionSummaries[i];
                        DateTime dischargeTime = Convert.ToDateTime(jsonData.AdmissionSummaries[i].StrDischargeDateTime);
                        tempRoomType.Add(new CommonMasterModel {RoomPriorityLevel = item.RoomTypeId});
                        if (dischargeTime >= et)
                            {
                                var tempDt = new DateTime(dischargeTime.Year, dischargeTime.Month, dischargeTime.Day, 11, 59, 59);
                                //days = days + tempDt.Subtract(st).Days;
                                // days = CalculateDateTimeRate.CalculateDays(admissionTime, et);
                                var calDays = tempDt.Subtract(st).Days; //CalculateDateTimeRate.CalculateDays(st, tempDt);
                                days = days + calDays;
                                for (int iDay = 0; iDay < calDays; iDay++)
                                {
                                    _ChargeDetailsDayWise.Add(new ChargeDetailsDayWise { Day = iDay, RoomTypeId = tempRoomType.Min(m=>m.RoomPriorityLevel), BillRate =  101});
                                }

                                st = tempDt.AddMinutes(1);
                                et = st.AddHours(24);
                                tempRoomType.Clear();
                            }
                            else if (i == jsonData.AdmissionSummaries.Count - 1)
                            {
                                _ChargeDetailsDayWise.Add(new ChargeDetailsDayWise { Day = i, RoomTypeId = tempRoomType.Min(m => m.RoomPriorityLevel) });
                                days = days + 1;
                            }
                        }

                        //bool roomTypeFailed = false;
                        //for (int i = 0; i < finalRoomCharge.Count(); i++)
                        //{
                        //    if (finalRoomCharge[0] != RoomType.ICU)
                        //        roomTypeFailed = true;
                        //    if (finalRoomCharge[1] != RoomType.Private)
                        //        roomTypeFailed = true;

                        //    if (!roomTypeFailed)
                        //        Console.WriteLine("Failed Room Type, Expected room type:" + RoomType.ICU.ToString() +
                        //                          "  --> but received:" + finalRoomCharge[0].ToString());

                        //}
                }
                //foreach (var item in left)
                //{
                //    if (Session[item.Name] != null)
                //    {
                //        var esesServices = Session[item.Name] as List<CommonMasterModel>;
                //        foreach (var sdetail in esesServices)
                //        {
                //            billAmount = billAmount + (sdetail.BillRate*sdetail.Qty);
                //        }
                //    }
                //}
                foreach (var item in left)
                {
                    if (Session[item.Name] != null)
                    {
                        var esesServices = Session[item.Name] as List<CommonMasterModel>;

                        if (jsonData.ManagementTypeId == 2 && item.Name == "Left_Investigations"|| jsonData.ManagementTypeId == 2 && item.Name == "Left_OtherBedSide")
                        {
                            billAmount = billAmount + 0;
                        }
                        else
                        {
                            foreach (var sdetail in esesServices)
                            {
                                billAmount = billAmount + (sdetail.BillRate * sdetail.Qty);
                            }
                        }
                       
                    }
                }
                if (jsonData.LeftOtDetail != null)
                {
                    foreach (var leftOtDetail in jsonData.LeftOtDetail)
                    {
                        billAmount = billAmount + leftOtDetail.BillRate;
                    }
                }
                if (jsonData.PharmacyDetails != null)
                {
                     List<CommonMasterModel> packages= Session["Left_Package"] as List<CommonMasterModel>;
                    if (packages != null && (jsonData.ManagementTypeId == 3 && packages.Count>0))
                    {
                        foreach (var item in packages)
                        {
                            foreach(var pharmacy in jsonData.PharmacyDetails)
                            {
                                if(Convert.ToDateTime(pharmacy.ConsumeDate)>Convert.ToDateTime(item.ConsumeDate))
                                billAmount = billAmount + pharmacy.BillRate;
                            }
                        }
                    }
                    else if(jsonData.ManagementTypeId==2)billAmount=billAmount+0;
                    else
                    {
                        foreach (var pharmacy in jsonData.PharmacyDetails)
                        {
                            billAmount = billAmount + pharmacy.BillRate;
                        }
                    }
                    

                }
                billAmount = billAmount + bedCharges;
            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in CommonMasterController method of CalculateGenralBill :" + Environment.NewLine + ex.StackTrace);
            });
            if (initialCheckPass == false)
            return Json(new { success = false, Message = "Discharge date cannot be less than Admission date" }, JsonRequestBehavior.AllowGet);
            return Json(new { billAmount=billAmount,success=true},JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult ServicesConsumedPartialLeftDiv()
        {
            List<CommonMasterModel> list = new List<CommonMasterModel>();
            try
            {
                list = _data.ServicesConsumedLeftDiv();
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in CommonMasterController  HospitalTypeDropDown:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return PartialView("~/Areas/HospitalForms/Views/HospitalForms/ServicesConsumedpartialView.cshtml", list);
        }
        public PartialViewResult ServicesConsumedDivRightPane()
        {
            List<CommonMasterModel> list = new List<CommonMasterModel>();
            try
            {
                list = ServicesConsumedRightDiv();
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in CommonMasterController  ServicesConsumedDivRightPane:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return PartialView("~/Areas/HospitalForms/Views/HospitalForms/ServicesConsumedpartialViewRightPart.cshtml", list);
        }
       
        public List<CommonMasterModel> ServicesConsumedRightDiv()
        { JsonResult jlResult;
            List<CommonMasterModel> list = new List<CommonMasterModel>();
            TryCatch.Run(() =>
            {
                list = _data.ServicesConsumedRightDiv();
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in CommonMasterController method of ServicesConsumedRightDiv :" + Environment.NewLine + ex.StackTrace);
            });
            return list;
        }

        public ActionResult GetServiceConsumedInRequest(int RequestId)
        {
            List<CommonMasterModel> list = new List<CommonMasterModel>();
            JsonResult jlResult;
            try
            {
                list = _data.GetServiceConsumedInRequest(RequestId);
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                Loggger.LogError("Error in CommonMasterController method of GetServiceConsumedInRequest :" + Environment.NewLine + ex.StackTrace);
                return Json("error");
            }
            return jlResult;
        }
        public JsonResult GetScanDocUrl(int scanDocId)
        {
            List<ScanDocEntity> list = null;
            try
            {
                list = _data.GetScanDocUrl(scanDocId);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in CommonMasterController  GetScanDocUrl:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public void ClearAllSession()
        {
            List<CommonMasterModel> rightList = ServicesConsumedRightDiv();
            List<CommonMasterModel> leftList = _data.ServicesConsumedLeftDiv();
            rightList.AddRange(leftList);
            foreach (var item in rightList)
            {
                Session[item.Name] = null;
            }
        }


        public PartialViewResult ImedSerbPartialForImagePreview(int scanDocId)
        {
            List<ScanDocEntity> list = null;
            try
            {
                list = _data.GetScanDocUrl(scanDocId);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in CommonMasterController  ImedSerbPartialForImagePreview:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return PartialView("~/Areas/CommonArea/Views/CommonArea/ImedPartialViewForImages.cshtml", list);
        }
    }
}
