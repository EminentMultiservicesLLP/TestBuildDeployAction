using CGHSBilling.API.HospitalForms.Intefaces;
using CGHSBilling.API.HospitalForms.Repository;
using CGHSBilling.Areas.HospitalForms.Models;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Common;
using CommonDataLayer.DataAccess;
using CommonLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using CommonLayer.Extensions;
using static CGHSBilling.Controllers.HomeController;
using System.IO;
using CGHSBilling.Caching;
using System.Threading.Tasks;
using CGHSBilling.API.Masters.Repositories;
using System.Threading;
using System.Transactions;

namespace CGHSBilling.Areas.HospitalForms.Controllers
{
    [ValidateHeaderAntiForgeryTokenAttribute]
    public class RequestSubmissionNewController : Controller
    {
        IHospitalFormsRepository _data;
        private static readonly ILogger Loggger = Logger.Register(typeof(RequestSubmissionNewController));
        ConnectionString _connectionString;
       

        public RequestSubmissionNewController()
        {
            _data = new HospitalFormsRepository();
            _connectionString = new ConnectionString();            
        }        

        [HttpPost]
        public ActionResult CreateRequest(RequestSubmissionModel jsonData)
        {
            Loggger.LogInfo("Create request Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :"+ DateTime.Now.ToLongTimeString());
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
                    //IDbTransaction transaction = dbHelper.BeginTransaction();
                    try
                    {

                        //if (jsonData.RequestId > 0 && Session["DeductionMode"] != null &&
                        //    Convert.ToInt32(Session["DeductionMode"]) != 3)
                        //    return Json(new { requestId = jsonData.RequestId, success = true, responseMsg = "You are not allowed to make Changes for this Bill" }, JsonRequestBehavior.AllowGet);

                        if (jsonData.PatientTypeId == 2 && jsonData.RoomEntitleTypeId !=1  )
                            return Json(new { requestId = jsonData.RequestId, success = false, responseMsg = "Room Entitlement should be General ward for ESIC Patients" }, JsonRequestBehavior.AllowGet);

                        var billAmt = GetCalculatedBill(jsonData);
                        jsonData.BillAmount = billAmt;

                        if (jsonData.RequestId > 0)
                        {
                            IsNewEntry = false;
                            _data.UpdateRequest(jsonData, dbHelper);
                            Loggger.LogInfo("   Create request Completed for UpdateRequest at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                        }
                        else
                        {
                            IsNewEntry = true;
                            _data.CreateRequest(jsonData, dbHelper);
                            Loggger.LogInfo("   Create request Completed for CreateRequest at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                        }
                        if (jsonData.RequestId > 0)
                        {
                            //Adding Hope Patient Details if any 
                            if (jsonData.Patient != null && IsNewEntry)
                            {
                                jsonData.Patient.RequestId = jsonData.RequestId;
                                _data.CreatePatientLink(jsonData.Patient , dbHelper);
                            }


                            //Adding Admission details to database
                            if (jsonData.AdmissionSummaries != null)
                            {
                                int count = jsonData.AdmissionSummaries.Count;
                                foreach (var admission in jsonData.AdmissionSummaries)
                                {
                                    //admission.StrDischargeDateTime = jsonData.AdmissionSummaries[count-1].StrAdmissionDateTime;
                                    admission.RequestId = jsonData.RequestId;
                                    _data.CreateRequestAdmissionDetail(admission, dbHelper);
                                    Loggger.LogInfo("   Create request Completed for Admission details at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                                }
                            }
                            
                            if (MemoryCaching.CacheKeyExist(CachingKeys.ManagementType.ToString()) && jsonData.SurgerySummaries != null)
                            {
                                var list = MemoryCaching.GetCacheValue(CachingKeys.ManagementType.ToString()) as List<CommonMasterModel>;
                                if (list.Where(w => w.Id == jsonData.ManagementTypeId && w.Name.ToLower().Contains("surg")).Any())  // chec if management type selected as Surgery
                                {
                                    var surgeryMasterList = MemoryCaching.GetCacheValue(CachingKeys.SurgeryMaster.ToString()) as List<SurgeryMasterModel>;
                                    jsonData.SurgerySummaries.Join(surgeryMasterList, s => s.SurgeryName, c => c.SurgeryName,
                                        (s, c) => { s.SurgeryID = c.SurgeryID; s.SurgeryDateTime = Convert.ToDateTime(s.StrSurgeryDateTime); return s; });
                                    foreach (var surgery in jsonData.SurgerySummaries.OrderByDescending(d => d.SurgeryDateTime))
                                    {
                                        surgery.RequestId = jsonData.RequestId;
                                        _data.CreateRequestSurgeryDetail(surgery, dbHelper);
                                    }
                                }
                                Loggger.LogInfo("   Create request Completed for Surgery details at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                            }
                            //Adding Consumed services to database
                            if (jsonData.ConsumeDiv != null)
                            {
                                int count = jsonData.ConsumeDiv.Count;
                                foreach (var item in jsonData.ConsumeDiv)
                                {
                                    item.TransactionId = jsonData.RequestId;
                                    _data.CreateRequestDetail(item, dbHelper);
                                }
                                Loggger.LogInfo("   Create request Completed for request details details at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                            }
                            //Adding Bed charges to database
                            if (jsonData.BedCharges != null)
                            {

                                foreach (var item in jsonData.BedCharges)
                                {
                                    item.RequestId = jsonData.RequestId;
                                    _data.CreateRequestBedChargeDetail(item, dbHelper);
                                }
                                Loggger.LogInfo("   Create request Completed for Bed charges details details at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                            }
                            //Adding Pharmacy services to database
                            if (jsonData.PharmacyDetails != null)
                            {
                                foreach (var pharmacy in jsonData.PharmacyDetails)
                                {
                                    pharmacy.TransactionId = jsonData.RequestId;
                                    _data.CreateRequestPharmacyDetail(pharmacy, dbHelper);
                                }
                                Loggger.LogInfo("   Create request Completed for Pharmacy details details at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                            }
                            //Adding Manually services to database
                            if (jsonData.ManullyAddedService != null)
                            {
                                foreach (var manully in jsonData.ManullyAddedService)
                                {
                                    manully.TransactionId = jsonData.RequestId;
                                    _data.CreateRequestManullyAddedDetail(manully, dbHelper);
                                }
                                Loggger.LogInfo("   Create request Completed for Manually added services details details at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                            }

                            //Adding Surgery Manually services to database
                            if (jsonData.SurgeryManullyAddedService != null)
                            {
                                foreach (var manully in jsonData.SurgeryManullyAddedService)
                                {
                                    manully.RequestId = jsonData.RequestId;
                                    _data.CreateRequestSurgeryManullyAddedDetail(manully, dbHelper);
                                }
                                Loggger.LogInfo("   Create request Completed for Surgery Manually added services details details at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                            }

                            //Add Default services to database - always run default service add to database
                            {
                                
                                var defaultServices = GetDefaultServices(jsonData.RoomEntitleTypeId, jsonData.HospitalTypeId, jsonData.PatientTypeId, jsonData.GenderId, jsonData.StateId, jsonData.CityId, _connectionString.getConnectionStringName());

                                if (defaultServices != null)
                                {
                                    foreach (var defaultSer in defaultServices)
                                    {
                                        var tempDefaultServ = new CommonMasterModel { TransactionId = jsonData.RequestId, ServiceId = defaultSer.ServiceId, ServiceName = defaultSer.ServiceName, ServiceTypeId = defaultSer.ServiceTypeId, ConsumeDate = defaultSer.ConsumeDate, Qty = defaultSer.Qty };
                                        _data.CreateRequestDefaultServicesDetail(tempDefaultServ, dbHelper);
                                    }
                                }
                                Loggger.LogInfo("   Create request Completed for Default services details details at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                            }

                            //Adding Linking services to database
                            if ((jsonData.BedCharges != null && jsonData.BedCharges.Count > 0) || (jsonData.ConsumeDiv != null && jsonData.ConsumeDiv.Count > 0))
                            {
                                _data.CreateRequestAutoLinkedService(jsonData.RequestId, dbHelper);
                                Loggger.LogInfo("   Create request Completed for Auto linking details details at :" + DateTime.Now.ToLongTimeString() + " for Request no. " + jsonData.RequestNo);
                            }

                           //dbHelper.CommitTransaction(transaction);
                            txscope.Complete();
                            ClearAllSession(jsonData.ManagementTypeId);
                        }
                        isSuccess = true;
                        requestMsg = !IsNewEntry ? "IPD Request no. " + jsonData.RequestNo + " updated for Bill Amount <b>Rs." + String.Format("{0:n}", billAmt) + "</b>" : "Sucessfully generated IPD bill for Amount <b>Rs." + String.Format("{0:n}", billAmt) + Environment.NewLine + "</b> Your new request no. is " + jsonData.RequestNo;
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
            return Json(new { requestId = jsonData.RequestId, requestNo = jsonData.RequestNo, success = isSuccess, responseMsg = requestMsg,isDeactive =jsonData.IsDeactive  }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CalculateBill(RequestSubmissionModel jsonData)
        {
            Loggger.LogInfo("Calculate Bill Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            double BillAmount = 0.0; 
            try
            {
                BillAmount = GetCalculatedBill(jsonData);
                return Json(new { success = true, billAmount = BillAmount.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in CreateRequest :", ex);
                return Json(new { success = false, billAmount = BillAmount.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        private double GetCalculatedBill(RequestSubmissionModel jsonData)
        {

            Loggger.LogInfo("Calculate Bill Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            double BillAmount = 0.0; string ConnectionString = "";
            try
            {
                if (jsonData.RequestId > 0 && Session["DeductionMode"] != null &&
                 Convert.ToInt32(Session["DeductionMode"]) != 3)  return 0 ;
                

                    UpdateAdmissionSugeryDates(jsonData.AdmissionSummaries, jsonData.SurgerySummaries);
                 
                    //Make Call here for Surgery 
                   
                    //Extra Bed charge calculation 
                    DateTime dtAdmisssionThreasHoldTime = Convert.ToDateTime(Constants.AdmissionThresholdTime);
                    DateTime dtDischargeThreasHoldTime = Convert.ToDateTime(Constants.DischargeThresholdTime);
                    if (jsonData.AdmissionSummaries != null)
                    {
                        try
                        {
                            jsonData.AdmissionSummaries.ForEach(f => { f.AdmissionDateTime = Convert.ToDateTime(f.StrAdmissionDateTime); f.DischargeDateTime = Convert.ToDateTime(f.StrDischargeDateTime); });
                            var admissionRow = jsonData.AdmissionSummaries.OrderBy(O => O.AdmissionDateTime).FirstOrDefault();
                            //var dischargeRow = jsonData.AdmissionSummaries.OrderByDescending(O => O.DischargeDateTime).FirstOrDefault();

                            //If Admission time is before 6am then add one more charge for Bed 
                            if (admissionRow.AdmissionDateTime.TimeOfDay <= dtAdmisssionThreasHoldTime.TimeOfDay)
                            {
                                var admisionDate = admissionRow.AdmissionDateTime.ToString("dd-MMM-yyyy");
                                if (jsonData.BedCharges != null && jsonData.BedCharges.Any())
                                {
                                    jsonData.BedCharges.Where(w => w.ConsumeDate == admisionDate).First().Qty = 2;
                                }
                                //BillAmount = BillAmount + (jsonData.BedCharges != null && jsonData.BedCharges.Any() ? jsonData.BedCharges.Where(w => w.ConsumeDate == admisionDate).FirstOrDefault().BillRate : 0);
                                Loggger.LogInfo("    Calculate Bill - Previous dat bedcharge as extra applied ");
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    var dischargeRow = jsonData.AdmissionSummaries.OrderByDescending(O => O.DischargeDateTime).FirstOrDefault();
                    var managementTypeList = new CommonMasterRepository().GetAllTypeofManagement();
                    if (managementTypeList.Where(w => w.Id == jsonData.ManagementTypeId && w.Name.ToUpper() == "SURGICAL").Any())
                    {
                        jsonData.BedCharges.ForEach(f => f.Qty = 0);
                    }
                    else
                    {
                        SetBedChargeQunatity(jsonData.BedCharges, jsonData.SurgerySummaries, dischargeRow.DischargeDateTime);
                    }

                    //Check if Admission Date is Same as Discharge Date
                    // if true it implies Discharge Row and Admission Row are same.  
                    var checkAdmIsEqualToDis = jsonData.AdmissionSummaries.Any(O => O.AdmissionDateTime.Date == O.DischargeDateTime.Date);
                    if (checkAdmIsEqualToDis && (jsonData.ManagementTypeId == 1 || jsonData.ManagementTypeId == 4 || jsonData.ManagementTypeId == 5))
                    {
                        if (jsonData.BedCharges != null && jsonData.BedCharges.Any())
                        {
                            var quantity = dischargeRow.AdmissionDateTime.TimeOfDay <= dtAdmisssionThreasHoldTime.TimeOfDay ?
                                       dischargeRow.DischargeDateTime.TimeOfDay <= dtDischargeThreasHoldTime.TimeOfDay ? 1 : 2
                                       : 1;

                            jsonData.BedCharges.Where(w => w.ConsumeDate == dischargeRow.AdmissionDateTime.Date.ToString("dd-MMM-yyyy"))
                                              .FirstOrDefault().Qty = quantity;
                        }
                    }



                    ConnectionString = _connectionString.getConnectionStringName();
                
                //Added By Ankit Mane for Taking care of unwanted services. Like Edit Mode
                RemoveServices(jsonData);
               

                //Bed + Pharmacy + default + Manual service + Surgery Manual Charges bill calculation
                var taskCharges = Task.Run(() =>
                   {
                       var tempAmount = CalculateBedCharges(jsonData.BedCharges, dischargeRow.DischargeDateTime);
                       tempAmount = tempAmount + CalculatePharmacyCharges(jsonData.PharmacyDetails);
                       tempAmount = tempAmount + CalcualteDefaultServicesCharge(jsonData.HospitalTypeId, jsonData.PatientTypeId, jsonData.StateId, jsonData.CityId, jsonData.GenderId, jsonData.RoomEntitleTypeId, jsonData.BedCharges, ConnectionString);
                       tempAmount = tempAmount + CalculatedManualServiceCharges(jsonData.ManullyAddedService);
                       tempAmount = tempAmount + CalculatedSurgeryManualServiceCharges(jsonData.SurgeryManullyAddedService);
                       return tempAmount;
                   });

                    //Surgery Package details
                    var taskSurgeryPackageCharges = Task.Run(() =>
                    {
                        if (jsonData.SurgerySummaries != null && jsonData.SurgerySummaries.Count > 0)
                        {
                            var serviceCategoryId = (jsonData.ManagementTypeId == 2 ? 7 : (jsonData.ManagementTypeId == 3 ? 7 : 8));
                            return CalculateSurgeryPackage(jsonData.HospitalTypeId, jsonData.PatientTypeId, jsonData.StateId, jsonData.CityId, jsonData.GenderId, jsonData.RoomEntitleTypeId, serviceCategoryId, jsonData.ConsumeDiv, jsonData.SurgerySummaries, ConnectionString);
                        }
                        else
                            return 0.0;
                    });

                    //Consumed Services
                    var taskConsumeServicesCharges = Task.Run(() =>
                    {
                        return CalcualateConsumedServiceCharges(jsonData.ConsumeDiv, jsonData.HospitalTypeId, jsonData.PatientTypeId, jsonData.StateId, jsonData.CityId, jsonData.GenderId, jsonData.RoomEntitleTypeId, ConnectionString);
                    });

                    var taskLinkingServicesCharges = Task.Run(() =>
                    {
                    //return CalculateLinkingServiceCharge(jsonData.BedCharges, jsonData.ConsumeDiv, jsonData.HospitalTypeId, jsonData.PatientTypeId, jsonData.StateId, jsonData.CityId, jsonData.GenderId, jsonData.RoomEntitleTypeId);
                    return CalculateLinkingServiceCharge(jsonData, ConnectionString);
                    });

                    Task.WaitAll(taskCharges, taskSurgeryPackageCharges, taskConsumeServicesCharges, taskLinkingServicesCharges);
                    BillAmount = BillAmount + taskCharges.Result + taskConsumeServicesCharges.Result + taskLinkingServicesCharges.Result + taskSurgeryPackageCharges.Result;

                    Loggger.LogInfo("Calculated Bill completed for Bill:" + BillAmount.ToString());
                    return BillAmount;
                
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in Get Calculate Bill :", ex);
                return BillAmount;
            }
        }

        //Remove Unwanted Service
        private void RemoveServices(RequestSubmissionModel jsonData)
        {
            if (jsonData.AdmissionSummaries != null && jsonData.AdmissionSummaries.Count > 0)
            {
                //Remove All Services which are not in Between admission and Discharge Date                  
                var AdmissionDatetime = Convert.ToDateTime(jsonData.AdmissionSummaries[0].StrAdmissionDateTime).Date;
                var DischargeDatetime = Convert.ToDateTime(jsonData.AdmissionSummaries[0].StrDischargeDateTime).Date;
                bool Consumediv = jsonData.ConsumeDiv != null && jsonData.ConsumeDiv.Count > 0 ? true : false;
                bool Pharmacydetails = jsonData.PharmacyDetails != null && jsonData.PharmacyDetails.Count > 0 ? true : false;
                bool Surgerymanullyaddedservice  = jsonData.SurgeryManullyAddedService != null && jsonData.SurgeryManullyAddedService.Count > 0 ? true : false;
                bool Surgerysummaries = jsonData.SurgerySummaries != null && jsonData.SurgerySummaries.Count > 0 ? true : false;

                if (Consumediv)
                    jsonData.ConsumeDiv.RemoveAll(x => (Convert.ToDateTime(x.ConsumeDate) < AdmissionDatetime || Convert.ToDateTime(x.ConsumeDate) > DischargeDatetime));

                if (Pharmacydetails)
                    jsonData.PharmacyDetails.RemoveAll(x => (Convert.ToDateTime(x.ConsumeDate) < AdmissionDatetime || Convert.ToDateTime(x.ConsumeDate) > DischargeDatetime));

                if (jsonData.ManullyAddedService != null && jsonData.ManullyAddedService.Count > 0)
                    jsonData.ManullyAddedService.RemoveAll(x => (Convert.ToDateTime(x.ConsumeDate) < AdmissionDatetime || Convert.ToDateTime(x.ConsumeDate) > DischargeDatetime));
                
                // No need to handle here, handled for Edit 
                //DefaultService.RemoveAll(x => (Convert.ToDateTime(x.ConsumeDate) < AdmissionDatetime || Convert.ToDateTime(x.ConsumeDate) > DischargeDatetime));

                if (Surgerymanullyaddedservice)
                    jsonData.SurgeryManullyAddedService.RemoveAll(x => (Convert.ToDateTime(x.ConsumeDate) < AdmissionDatetime || Convert.ToDateTime(x.ConsumeDate) > DischargeDatetime));

                //Remove All surgeries which are not in between Admission date and surgery date
                if (Surgerysummaries)
                    jsonData.SurgerySummaries.RemoveAll(x => (Convert.ToDateTime(x.StrSurgeryDateTime) < AdmissionDatetime || Convert.ToDateTime(x.StrSurgeryDateTime) > DischargeDatetime));

                //Remove all Manual Surgeries when it is Medical
                if (jsonData.ManagementTypeId == 1)
                {
                    if (Surgerymanullyaddedservice)
                        jsonData.SurgeryManullyAddedService.RemoveAll(x => x.ConsumeDate == x.ConsumeDate);

                    if (Consumediv)
                        jsonData.ConsumeDiv.RemoveAll(x => x.Id == 9);
                }

                //Remove Services Consumed if Services are in Packages or when its surgical
                if ((jsonData.ManagementTypeId == 2 || jsonData.ManagementTypeId == 3)
                 && (Consumediv || Pharmacydetails) && Surgerysummaries)
                {
                    var ConsumedDates = Consumediv ?  jsonData.ConsumeDiv.Select(x => Convert.ToDateTime(x.ConsumeDate)).Distinct().ToList() : new List<DateTime>();

                    if(Pharmacydetails)
                         ConsumedDates.AddRange(jsonData.PharmacyDetails.Select(x => Convert.ToDateTime(x.ConsumeDate)).Distinct().ToList()) ;

                    foreach (var ConsumedDate in ConsumedDates)
                    {
                        if ((jsonData.SurgerySummaries.Where(w => Convert.ToDateTime(ConsumedDate) >= Convert.ToDateTime(w.StrSurgeryDateTime) && Convert.ToDateTime(ConsumedDate) <= Convert.ToDateTime(w.StrSurgeryDateTime).AddDays((w.NoOfDays == 0 ? 0 : w.NoOfDays - 1))).Any()
                            && jsonData.ManagementTypeId == 3) || jsonData.ManagementTypeId == 2)
                        {
                            if (Consumediv)
                                jsonData.ConsumeDiv.RemoveAll(x => Convert.ToDateTime(x.ConsumeDate) == ConsumedDate && x.IsAllowedChangeInSurgery == false);

                            if (Pharmacydetails)
                                jsonData.PharmacyDetails.RemoveAll(x => Convert.ToDateTime(x.ConsumeDate) == ConsumedDate && x.IsAllowedChangeInSurgery == false);
                        }

                    }
                }

            }

        
        }

        private void UpdateAdmissionSugeryDates(List<AdmissionSummary> admissionDetails, List<SurgerySummary> surgeryDetails)
        {
            if (admissionDetails != null && admissionDetails.Count > 0)
            {
                //admissionDetails.Where(w => w.SurgeryAdmissionDateTime.NullToString() == string.Empty).ToList().ForEach(f => f.SurgeryAdmissionDateTime = f.StrAdmissionDateTime);
                //admissionDetails.Where(w => w.SurgerydDischargeDateTime.NullToString() == string.Empty).ToList().ForEach(f => f.SurgerydDischargeDateTime = f.StrDischargeDateTime);

                admissionDetails.ForEach(f => { f.AdmissionDateTime = Convert.ToDateTime(f.StrAdmissionDateTime); f.DischargeDateTime = Convert.ToDateTime(f.StrAdmissionDateTime); });
            }

            if (surgeryDetails != null && surgeryDetails.Count > 0)
                surgeryDetails.ForEach(f => { f.SurgeryDateTime = Convert.ToDateTime(f.StrSurgeryDateTime); });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bedCharges">Bed charge list</param>
        /// <param name="dischargeTime">Discharge time </param>
        private void SetBedChargeQunatity(List<BedCharges> bedCharges, List<SurgerySummary> surgeryDetails, DateTime dischargeTime)
        {
            Loggger.LogInfo("    Started setting Bed Quantity: function: SetBedChargeQunatity ");

            DateTime dtDischargeThreasHoldTime = Convert.ToDateTime(Constants.DischargeThresholdTime);

            //Bed Charges bill calculation
            if (bedCharges != null)
            {
                bedCharges.Where(w => w.Qty == 0).ToList().ForEach(a => a.Qty = 1);
                if (surgeryDetails != null && surgeryDetails.Count > 0)
                {
                    foreach (var item in bedCharges)
                        item.Qty = surgeryDetails.Where(w => (Convert.ToDateTime(item.ConsumeDate) >= w.SurgeryDateTime.Date && Convert.ToDateTime(item.ConsumeDate) <= w.SurgeryDateTime.AddDays(w.NoOfDays -1).Date)).Any() ? 0 : item.Qty;
                }
                //else
                //    bedCharges.Where(w => w.Qty == 0).ToList().ForEach(a => a.Qty = 1);

                var dischargeDate = dischargeTime.ToString("dd-MMM-yyyy");

                var lastDayBed = bedCharges.Where(w => w.ConsumeDate == dischargeDate).FirstOrDefault();
                if (lastDayBed != null)
                {
                    if (surgeryDetails == null)
                    {
                        if (dischargeTime.TimeOfDay >= dtDischargeThreasHoldTime.TimeOfDay )  //tempSurgeryDetails.Max(m=> m.SurgeryDateTime))
                        {
                            if (bedCharges != null && bedCharges.Any())
                            {
                                lastDayBed.Qty = 1;
                                Loggger.LogInfo("    setting Bed Quantity - Last date bedcharge as extra applied ");
                            }
                        }
                        else
                            lastDayBed.Qty = 0;
                    }
                    else
                    {
                        if (dischargeTime.TimeOfDay >= dtDischargeThreasHoldTime.TimeOfDay
                            && dischargeTime > surgeryDetails.Max(m => m.SurgeryDateTime.AddDays(m.NoOfDays).AddHours(dtDischargeThreasHoldTime.Hour).AddMinutes(dtDischargeThreasHoldTime.Minute).AddSeconds(dtDischargeThreasHoldTime.Second))                            )  //tempSurgeryDetails.Max(m=> m.SurgeryDateTime))
                        {
                            if (bedCharges != null && bedCharges.Any())
                            {
                                lastDayBed.Qty = 1;
                                Loggger.LogInfo("    setting Bed Quantity - Last date bedcharge as extra applied ");
                            }
                        }
                        else
                            lastDayBed.Qty = 0;
                    }
                }
            }
            Loggger.LogInfo("    End setting Bed Quantity: function: SetBedChargeQunatity ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bedCharges">Bed charge list</param>
        /// <param name="dischargeTime">Discharge time from Admission details</param>
        /// <returns></returns>
        private double CalculateBedCharges(List<BedCharges> bedCharges, DateTime dischargeTime)
        {
            double bedBillAmount = 0.0;
            Loggger.LogInfo("    Calculate Bill - Bedcharged " + bedBillAmount.ToString());

            //Bed Charges bill calculation
            if (bedCharges != null)
            {
                foreach (var item in bedCharges)
                    bedBillAmount = bedBillAmount + (item.Qty * Convert.ToDouble(item.BillRate));

                Loggger.LogInfo("    Calculate Bill - Bedcharged " + bedBillAmount.ToString());
            }

            return bedBillAmount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">Pharmacy data collection</param>
        /// <returns></returns>
        private double CalculatePharmacyCharges(List<CommonMasterModel> data)
        {
            var BillAmount = 0.0;
            if (data != null)
            {
                foreach (var pharmacy in data)
                    BillAmount = BillAmount + pharmacy.BillRate + pharmacy.LifeSavingBillRate;

                Loggger.LogInfo("    Calculate Bill - Pharmacy amount:" + BillAmount.ToString());
            }
            return BillAmount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">Consumed services</param>
        /// <param name="hospitalTypeId"></param>
        /// <param name="patientTypeId"></param>
        /// <param name="stateId"></param>
        /// <param name="cityId"></param>
        /// <param name="genderId"></param>
        /// <param name="roomEntitleTypeId"></param>
        /// <returns></returns>
        private double CalcualateConsumedServiceCharges(List<CommonMasterModel> data, int hospitalTypeId, int patientTypeId, int stateId, int cityId, int genderId, int roomEntitleTypeId,string ConnectionString)
        {
            var tempAmount = 0.0;
            var serviceRates = new List<ServiceMasterModel>();
            if (data != null)
            {
                //foreach (var service in data)
                foreach (var service in data.Select(s => s.Id).Distinct())
                {
                    var result = _data.GetAllServiceMasterByCategoryId(service, Convert.ToInt32(Session["AppUserId"]), hospitalTypeId, patientTypeId, stateId, cityId, genderId, roomEntitleTypeId, true, ConnectionString);
                    if (result != null && result.Count() > 0)
                        serviceRates.AddRange(result);
                }
                data.Where(w => w.Qty == 0).ToList().ForEach(a => a.Qty = 1);

                var tempBillRate = 0.0;
                foreach (var service in data)
                {
                    tempBillRate = 0.0;
                    var first = serviceRates.Where(w => w.CategoryId == service.Id && w.ServiceId == service.ServiceId).FirstOrDefault();
                    tempBillRate = first != null ? first.BillRate: 0;
                    tempAmount = tempAmount + (tempBillRate * service.Qty);
                }
                Loggger.LogInfo("    Calculate Bill - Service Consumed amount:" + tempAmount.ToString());
            }

            return tempAmount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hospitalTypeId"></param>
        /// <param name="patientTypeId"></param>
        /// <param name="stateId"></param>
        /// <param name="cityId"></param>
        /// <param name="genderId"></param>
        /// <param name="roomEntitleTypeId"></param>
        /// <param name="bedCharges">Bed Charges</param>
        /// <returns></returns>
        private double CalcualteDefaultServicesCharge(int hospitalTypeId, int patientTypeId, int stateId, int cityId, int genderId, int roomEntitleTypeId, List<BedCharges> bedCharges,string ConnectionString)
        {
            var defaultServiceBillRates = 0.0;
            var defaultServices = GetDefaultServices(roomEntitleTypeId, hospitalTypeId, patientTypeId, genderId, stateId, cityId, ConnectionString);

            //below line for assigning to Default service will help while saving
            if (defaultServices != null && defaultServices.Count() > 0)
            {
                foreach (var serv in defaultServices)
                    defaultServiceBillRates = defaultServiceBillRates + (serv.BillRate * serv.Qty);
            }
            Loggger.LogInfo("    Calculate Bill - Default Bill for one day: " + defaultServiceBillRates);
            Loggger.LogInfo("    Calculate Bill - Default Bill for full bill: " + (defaultServiceBillRates * (bedCharges != null ? bedCharges.Where(w => w.Qty > 0).Count() : 1)));
            defaultServiceBillRates = (defaultServiceBillRates * (bedCharges != null ? bedCharges.Where(w => w.Qty > 0).Count() : 1));
            return defaultServiceBillRates;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="data"><Manual services</param>
        /// <returns></returns>
        private double CalculatedSurgeryManualServiceCharges(List<SurgeryManualServices> data)
        {
            var tempAmount = 0.0;
            if (data != null)
            {
                foreach (var manully in data)
                    tempAmount = tempAmount + manully.OTCharges + manully.SurgeonCharges + manully.AnesthesiaCharges + manully.ExtraCharges + manully.OtherCharges;

                Loggger.LogInfo("    Calculate Bill - Surgery Manual Service amount:" + tempAmount.ToString());
            }
            return tempAmount;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="bedCharges">Bed Charges</param>
        /// <param name="consumeServices">Consumed services</param>
        /// <param name="hospitalTypeId"></param>
        /// <param name="patientTypeId"></param>
        /// <param name="stateId"></param>
        /// <param name="cityId"></param>
        /// <param name="genderId"></param>
        /// <param name="roomEntitleTypeId"></param>
        /// <returns></returns>
        //private double CalculateLinkingServiceCharge(List<BedCharges> bedCharges, List<CommonMasterModel> consumeServices, int hospitalTypeId, int patientTypeId, int stateId, int cityId, int genderId, int roomEntitleTypeId)
        private double CalculateLinkingServiceCharge(RequestSubmissionModel data,string ConnectionString)
        {
            double tempAmount = 0.0;
            List<BedCharges> bedCharges; List<CommonMasterModel> consumeServices; List<SurgerySummary> surgeryServices;
            int hospitalTypeId; int patientTypeId; int stateId; int cityId; int genderId; int roomEntitleTypeId;

            bedCharges = data.BedCharges; consumeServices = data.ConsumeDiv; surgeryServices = data.SurgerySummaries;
            hospitalTypeId = data.HospitalTypeId; patientTypeId = data.PatientTypeId; stateId = data.StateId; cityId = data.CityId; genderId = data.GenderId; roomEntitleTypeId = data.RoomEntitleTypeId;

            if ((bedCharges != null && bedCharges.Count > 0) || (consumeServices != null && consumeServices.Count > 0))
            {
                var selectedServices = (consumeServices != null ?
                                            consumeServices.Select(s => new { ServiceId = s.ServiceId, ConsumeDate = s.ConsumeDate }).Distinct()
                                            : new[] { new { ServiceId = 0, ConsumeDate = "" } });
                
                //select only days where bed charges applied changed after discussion with Ashish on 04-Jan-2019
                var selectedBedCharges = (bedCharges != null ?
                                            bedCharges.Where(w => w.Qty > 0).Select(s => new { ServiceId = s.RoomTypeId, ConsumeDate = s.ConsumeDate }).Distinct()
                                            : new[] { new { ServiceId = 0, ConsumeDate = "" } });
                
                //Ankit Mane(Am) removed Distinct  in order to have multiple Camcer Surgeries in Same day for Same Grade.
                //var selectedSurgeryServices = (surgeryServices != null ?
                //                           surgeryServices.Select(s => new { ServiceId = s.SurgeryID, ConsumeDate = s.StrSurgeryDateTime }).Distinct()
                //                           : new[] { new { ServiceId = 0, ConsumeDate = "" } });
                var selectedSurgeryServices = (surgeryServices != null ?
                                            surgeryServices.Select(s => new { ServiceId = s.SurgeryID, ConsumeDate = s.StrSurgeryDateTime })
                                            : new[] { new { ServiceId = 0, ConsumeDate = "" } });

                //Ankit Mane(Am) replaced union with concat in order to have multiple Cancer Surgeries in Same day for Same Grade.
                //var allServices = selectedServices.Where(w => w.ServiceId > 0).Union(selectedBedCharges.Where(w => w.ServiceId > 0)).Union(selectedSurgeryServices.Where(w => w.ServiceId > 0));
                var allServices = selectedServices.Where(w => w.ServiceId > 0).Union(selectedBedCharges.Where(w => w.ServiceId > 0)).Concat(selectedSurgeryServices.Where(w=> w.ServiceId > 0));
                var serviceIds = string.Join(",", allServices.Select(s => s.ServiceId).Distinct());

                var linkedServices = _data.GetLinkedServiceRatesByCategory_Services(String.Empty, serviceIds, roomEntitleTypeId, hospitalTypeId, patientTypeId, genderId, stateId, cityId, ConnectionString);

                var linkedServicesAmount = (from lk in linkedServices
                                            join ls in allServices on lk.ParentServiceId equals ls.ServiceId
                                            select new { lk.BillRate, lk.Qty }).Sum(s => s.BillRate * s.Qty);

                tempAmount = linkedServicesAmount;
                Loggger.LogInfo("    Calculate Bill - Linked Services amount:" + linkedServicesAmount.ToString());
            }
            return tempAmount;
        }

        private double CalculateSurgeryPackage(int hospitalTypeId, int patientTypeId, int stateId, int cityId, int genderId, int roomEntitleTypeId, int serviceCategoryId, List<CommonMasterModel> consumedServices, List<SurgerySummary> surgeryDetails,string ConnectionString)
        {
            double tempAmount = 0.0; var serviceRates = new List<ServiceMasterModel>();

            //get service rate for Surgery/cancer surgery services
            var result = _data.GetAllServiceMasterByCategoryId(serviceCategoryId, Convert.ToInt32(Session["AppUserId"]), hospitalTypeId, patientTypeId, stateId, cityId, genderId, roomEntitleTypeId, true, ConnectionString);
            if (result != null && result.Count() > 0)
            {
                serviceRates.AddRange(result);
                var surgeryRates = (from a in surgeryDetails
                                    join b in serviceRates.Where(w => w.CategoryId == serviceCategoryId) on a.SurgeryID equals b.ServiceId
                                    select new { a.SurgeryDateTime, b.BillRate }).ToList();

                if (serviceCategoryId == 8) //cancer surgery, sum all surgery charges
                {
                    tempAmount = tempAmount + surgeryRates.Sum(s => s.BillRate);
                }
                else //for Surgery & Manual Surgery discount
                {
                    var grouped = surgeryRates.GroupBy(g => g.SurgeryDateTime).Select(s => new { SurgeryDateTime = s.Key, Count = s.Count() }).ToList();

                    //Sum all surgery dates which are unique
                    tempAmount = tempAmount + (from a in surgeryRates join b in grouped.Where(w => w.Count == 1) on a.SurgeryDateTime equals b.SurgeryDateTime select a).Sum(s => s.BillRate);

                    //Get all surgery which has more than one surgery on same day
                    foreach(var grp in grouped.Where(w => w.Count > 1))
                    {
                        var isFirstRow = true;
                        foreach (var serviceRate in surgeryRates.Where(w => w.SurgeryDateTime == grp.SurgeryDateTime).OrderByDescending(o=> o.BillRate))
                        {
                            var ss = serviceRate.BillRate;
                            tempAmount = tempAmount + (isFirstRow ? serviceRate.BillRate : ((serviceRate.BillRate * Constants.DiscountPercSurgerySecondOnward) / 100)); //first surgery will be full charge & second onward all withh be 50%;
                            isFirstRow = false;
                        }
                    }
                }
            }

            Loggger.LogInfo("    Calculate Bill - Surgeries amount:" + tempAmount.ToString());
            return tempAmount;
        }

        public ActionResult GetAllgeneratedRequest()
        {
            Loggger.LogInfo("GetAllgeneratedRequest Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<RequestSubmissionModel> list = new List<RequestSubmissionModel>();
            JsonResult jlResult;
            try
            {
                list = _data.GetAllgeneratedRequest();
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllgeneratedRequest :", ex);
                return Json("error");
            }
            finally
            {
                Loggger.LogInfo("GetAllgeneratedRequest Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jlResult;
        }


        public ActionResult GetDeductionType()
        {
            Loggger.LogInfo("GetDeductionType Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            RequestSubmissionModel model = new RequestSubmissionModel();
            JsonResult jlResult;
            try
            {
                model = _data.GetDeductionType();            
                jlResult = Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetDeductionType :", ex);
                return Json("error");
            }
            finally
            {
                Loggger.LogInfo("GetDeductionType Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }

            return jlResult;
        }


       

        [HttpPost]
        public ActionResult GetDateWisePartialView(RequestSubmissionModel jsonData)
        {

            Loggger.LogInfo("GetDateWisePartialView Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<BedCharges> BedCharges = new List<BedCharges>();
            string view = string.Empty; var errormessage = ""; bool isSurgeryDateConflicts = false;
            try
            {
                UpdateAdmissionSugeryDates(jsonData.AdmissionSummaries, jsonData.SurgerySummaries);

                if (Validate(jsonData,out errormessage, out isSurgeryDateConflicts))
                {
                    Loggger.LogInfo("   Validate Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                    var list = jsonData.AdmissionSummaries;
                    //var surgeryList = jsonData.SurgerySummaries;
                    int userId = Convert.ToInt32(Session["AppUserId"]);

                    Task<List<string>> taskCreateDates = null;
                    if (list != null && list.Count > 0)
                    {
                        Loggger.LogInfo("   taskCreateDates Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                        taskCreateDates = Task.Run(() =>
                        {
                            var admissionDetails = list.Where(w => w.ServiceName == "Admission Dates" || w.ServiceId == 0).SingleOrDefault();
                            var startdate = Convert.ToDateTime(admissionDetails.StrAdmissionDateTime);

                            var result = new List<string>(); ;
                            DateTime datetoAdd = startdate;
                            double cielValue = (Convert.ToDateTime(admissionDetails.StrDischargeDateTime).Date - startdate.Date).TotalDays;
                            int daycount = Convert.ToInt32(Math.Ceiling(cielValue));
                            for (int i = 0; i <= daycount; i++)
                            {
                                result.Add(datetoAdd.ToString("dd-MMM-yyyy"));
                                datetoAdd = datetoAdd.AddDays(1);
                            }

                            return result;
                        });
                    }
                    ////else
                    //{
                    //    if (surgeryList != null && surgeryList.Count > 0)
                    //    {
                    //        taskCreateDates = Task.Run(() =>
                    //        {
                    //            //surgeryList.ForEach(f => f.SurgeryDateTime = Convert.ToDateTime(f.StrSurgeryDateTime));
                    //            surgeryList = surgeryList.OrderBy(o => o.SurgeryDateTime).ToList();

                    //            var firstDateRow = surgeryList.OrderBy(m => m.SurgeryDateTime).FirstOrDefault();
                    //            DateTime StartDate = firstDateRow.SurgeryDateTime;
                    //            DateTime EndDate = firstDateRow.SurgeryDateTime.AddDays(firstDateRow.NoOfDays-1);//include date of surgery start and hence minus one day.

                    //            foreach (var row in surgeryList)
                    //            {
                    //                if (row.SurgeryDateTime.AddDays(row.NoOfDays) > EndDate)
                    //                    EndDate = row.SurgeryDateTime.AddDays(row.NoOfDays-1); //include date of surgery start and hence minus one day.
                    //            }

                    //            var result = new List<string>();
                    //            DateTime datetoAdd = StartDate;
                    //            double cielValue = (EndDate - StartDate).TotalDays;
                    //            int daycount = Convert.ToInt32(Math.Ceiling(cielValue));
                    //            for (int i = 0; i <= daycount; i++)
                    //            {
                    //                result.Add(datetoAdd.ToString("dd-MMM-yyyy"));
                    //                datetoAdd = datetoAdd.AddDays(1);
                    //            }

                    //            return result;
                    //        });
                    //    }
                    //}

                    //var taskSurgery = Task.Run(() => { return jsonData.RequestId > 0 ? GetSurgerySummaries(jsonData.RequestId) : jsonData.SurgerySummaries; });
                    string ConnectionString = _connectionString.getConnectionStringName();
                    var taskBedCharges = Task.Run(() => { return jsonData.RequestId > 0 ? GetRequestBedChargeDetails(jsonData.RequestId, ConnectionString) : jsonData.BedCharges; });
                    var taskConsumeDiv = Task.Run(() => { return ServicesConsumedLeftDiv(jsonData.ManagementTypeId, ConnectionString); });
                    //var taskCalDefaultBill = new TaskFactory().StartNew((requestid) =>
                    //{
                    //    var defaultServiceBillRates = 0.0;
                    //    if (Convert.ToInt32(jsonData.RequestId) > 0)
                    //    {
                    //        //here we are fetching default services without bill rate because bill rate should not be available to user
                    //        var defaultServices = (!MemoryCaching.CacheKeyExist(CachingKeys.DefaultServiceMasters.ToString())) ? new ServiceMasterRepository().GetAllActiveDefaultServiceMaster() :
                    //            MemoryCaching.GetCacheValue(CachingKeys.DefaultServiceMasters.ToString()) as List<ServiceMasterModel>;

                    //        if (defaultServices != null && defaultServices.Count > 0)
                    //        {
                    //            foreach (var serv in defaultServices)
                    //                defaultServiceBillRates = defaultServiceBillRates + serv.BillRate;
                    //        }
                    //    }
                    //    return defaultServiceBillRates;
                    //}, jsonData.RequestId);

                    Task.WaitAll(taskBedCharges, taskConsumeDiv);
                    Loggger.LogInfo("    Task.WaitAll Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                    jsonData.TotalDates = taskCreateDates != null ? taskCreateDates.Result : null;
                    jsonData.BedCharges = taskBedCharges.Result;
                    jsonData.ConsumeDiv = taskConsumeDiv.Result;
                    //jsonData.SurgerySummaries = ((taskSurgery.Result == null || taskSurgery.Result.Count ==0) && jsonData.SurgerySummaries != null ? jsonData.SurgerySummaries : taskSurgery.Result); // this will be in update case when Sugery udated after request created
                    //jsonData.BillAmount = (taskCalDefaultBill.Result * (jsonData.TotalDates.Count() - (jsonData.SurgerySummaries == null ? 0 : jsonData.SurgerySummaries.Count()))); // Defaut bill will be for numbers of dates - surgery dates
                    //jsonData.BillAmount = jsonData.BillAmount + taskCalLinkedServicesBill.Result * (jsonData.TotalDates.Count() - (jsonData.SurgerySummaries == null ? 0 : jsonData.SurgerySummaries.Count())); // Defaut bill will be for numbers of dates - surgery dates

                    foreach (var service in jsonData.ConsumeDiv)
                    {
                        GetServiceMasterList(userId, service.Id, jsonData.HospitalTypeId, jsonData.PatientTypeId, jsonData.StateId, jsonData.CityId, jsonData.GenderId, jsonData.RoomEntitleTypeId);
                    }
                    view = RenderPartialViewToString(this, "~/Areas/HospitalForms/Views/HospitalForms/DateWiseConfigPartialView.cshtml", jsonData);
                }
                

            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in CommonMasterController  HospitalTypeDropDown:", ex);
            }
            finally
            {
                Loggger.LogInfo("GetDateWisePartialView Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            //return Json(new { data = jsonData.BedCharges, billAmount = jsonData.BillAmount, partialview = view,error=errormessage, IsSurgeryDateConflicts = isSurgeryDateConflicts}, JsonRequestBehavior.AllowGet);
            return Json(new { data = jsonData.BedCharges, partialview = view, error = errormessage, IsSurgeryDateConflicts = isSurgeryDateConflicts }, JsonRequestBehavior.AllowGet);
        }

        //public void AddServicesConsumedSession(string sessionName, int serviceId, bool state, double rate, int roomType, int qty)
        //{
        //    JsonResult jsonResult = null;
        //    try
        //    {
        //        var datevar = sessionName.Split('_');
        //        string date = datevar[1];
        //        if (Session[datevar[0]] == null) Session[datevar[0]] = new List<CommonMasterModel>();

        //        var sessionData = Session[datevar[0]] as List<CommonMasterModel>;
        //        if (state)
        //            sessionData.Add(new CommonMasterModel() { ServiceId = serviceId, State = state, BillRate = rate, RoomTypeId = roomType, Qty = qty, ConsumeDate = date });
        //        else
        //        {
        //            CommonMasterModel removedata;
        //            removedata = sessionData.FirstOrDefault(m => m.ServiceId == serviceId && m.RoomTypeId == roomType);
        //            sessionData.Remove(removedata);
        //        }
        //        ViewBag.ServiceTotalAmount = (state ? ViewBag.ServiceTotalAmount + rate : ViewBag.ServiceTotalAmount - rate);
        //        Session[datevar[0]] = sessionData;
        //        jsonResult = Json(new { sessionRecord = sessionData }, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}

            //Added RoomTypeID as new parameter which will be value from Room Entitled dropdown Added by Diwakar on 19-Dec-2018
        public ActionResult DisplayServicesConsumedSession(string sessionName, int categoryId, int hospitalType, int patientType, int stateId, int cityId, int gender, int roomtypeId = 0)
        {
            Loggger.LogInfo("DisplayServicesConsumedSession Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            JsonResult jsonResult = null;
            int userId = Convert.ToInt32(Session["AppUserId"]);
            try
            {
                var serviceType = GetServiceMasterList(userId, categoryId, hospitalType, patientType, stateId, cityId, gender, roomtypeId);
                Loggger.LogInfo("   GetServiceMasterList Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                jsonResult = Json(new { sessionRecord = serviceType }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in Method: DisplayServicesConsumedSession class:RequestSubmissionNewController :", ex);
            }

            //RequestSubmissionController ctr = new RequestSubmissionController();
            //List<ServiceMasterModel> service = ctr.GetAllServiceMasterByCategory(categoryId, hospitalType, patientType, stateId, cityId, gender);
            //var datevar = sessionName.Split('_');
            //string date = datevar[datevar.Length - 1];
            //try
            //{
            //    if (Session[datevar[0]] != null)
            //    {
            //        var sessionData = Session[datevar[0]] as List<CommonMasterModel>;
            //        foreach (var eachitem in sessionData.Where(w => Convert.ToDateTime(w.ConsumeDate).ToString("dd-MMM-yyyy") == date))
            //        {
            //            var find = service.Where(m => m.ServiceId == eachitem.ServiceId);// && m.RoomTypeId > 0&& Convert.ToDateTime(m.ConsumeDate).ToString("dd-MM-yyyy") == date
            //            if (find.Any())
            //            {
            //                service.Where(m => m.ServiceId == eachitem.ServiceId && m.RoomTypeId == eachitem.RoomTypeId).ToList()[0].State = eachitem.State;
            //                service.Where(m => m.ServiceId == eachitem.ServiceId && m.RoomTypeId == eachitem.RoomTypeId).ToList()[0].Qty = eachitem.Qty;
            //                service.Where(m => m.ServiceId == eachitem.ServiceId && m.RoomTypeId == eachitem.RoomTypeId).ToList()[0].ConsumeDate = Convert.ToDateTime(eachitem.ConsumeDate).ToString("dd-MMM-yyyy");
            //            }
            //        }
            //    }
            //    service.Where(w => w.Qty <= 0).Select(s => s.Qty = 1);

            //    //foreach (var item in service)
            //    //{
            //    //    if (item.Qty <= 0) item.Qty = 1;
            //    //}
            //    jsonResult = Json(new { sessionRecord = service }, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    throw;
            //}
            finally
            {
                Loggger.LogInfo("DisplayServicesConsumedSession Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jsonResult;
        }

        private List<ServiceMasterModel> GetDefaultServices(int roomTypeId, int hospitalType, int patientType, int gender, int stateId, int cityId,string ConnectionString)
        {
            Loggger.LogInfo("GetDefaultServices Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            int userId = Convert.ToInt32(Session["AppUserId"]);
            try
            {
                if (MemoryCaching.CacheKeyExist(CachingKeys.DefaultServiceMasters_Rates.ToString()))
                {
                    var exists = MemoryCaching.GetCacheValue(CachingKeys.DefaultServiceMasters_Rates.ToString()) as List<ServiceMasterModel>;
                    if (exists != null && exists.Any(a => a.HospitalTypeId == hospitalType && a.PatientTypeId == patientType
                                    && a.RoomTypeId == roomTypeId && a.StateId == stateId && a.CityId == cityId && a.GenderId == gender))
                    {
                        Loggger.LogInfo("   GetCacheValue for GetDefaultServices Started for  " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                        return exists.Where(a => a.HospitalTypeId == hospitalType && a.PatientTypeId == patientType
                                    && a.RoomTypeId == roomTypeId && a.StateId == stateId && a.CityId == cityId && a.GenderId == gender).ToList();
                    }
                }
                return new ServiceMasterRepository().GetAllActiveDefaultServiceMaster_WithRates(roomTypeId, hospitalType, patientType, gender, stateId, cityId, ConnectionString);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetServiceMasterByCategoryRoomId :", ex);
                return null;
            }
        }

        public JsonResult GetServiceMasterByCategoryRoomId(int categoryId = 0, int hospitalType = 0, int patientType = 0, int roomTypeId = 0, int stateId = 0, int cityId = 0, int gender = 0, int requestId = 0)
        {
            Loggger.LogInfo("GetServiceMasterByCategoryRoomId Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            int userId = Convert.ToInt32(Session["AppUserId"]);
            try
            {
                if (MemoryCaching.CacheKeyExist(CachingKeys.AllBedcharges.ToString()))
                {
                    var exists = MemoryCaching.GetCacheValue(CachingKeys.AllBedcharges.ToString()) as List<ServiceMasterModel>;
                    if (exists != null && exists.Any(a => a.HospitalTypeId == hospitalType && a.PatientTypeId == patientType
                                    && a.RoomTypeId == roomTypeId && a.StateId == stateId && a.CityId == cityId && a.GenderId == gender))
                    {
                        Loggger.LogInfo("   GetCacheValue Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                        var result = exists.Where(a => a.HospitalTypeId == hospitalType && a.PatientTypeId == patientType
                                    && a.RoomTypeId == roomTypeId && a.StateId == stateId && a.CityId == cityId && a.GenderId == gender);
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                var list = _data.GetServiceMasterByCategoryRoomId(categoryId, userId, hospitalType, patientType, roomTypeId, stateId, cityId, gender, requestId);
                if (list != null && list.Any(a => a.HospitalTypeId == hospitalType && a.PatientTypeId == patientType
                                && a.RoomTypeId == roomTypeId && a.StateId == stateId && a.CityId == cityId && a.GenderId == gender))
                {
                    var result = list.Where(a => a.HospitalTypeId == hospitalType && a.PatientTypeId == patientType
                                    && a.RoomTypeId == roomTypeId && a.StateId == stateId && a.CityId == cityId && a.GenderId == gender);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetServiceMasterByCategoryRoomId :", ex);
                return Json("error");
            }
            finally
            {
                Loggger.LogInfo("GetServiceMasterByCategoryRoomId Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
        }
        [HttpPost]
        public ActionResult AdmissionDetailById(RequestSubmissionModel jsonData)
        {
            Loggger.LogInfo("AdmissionDetailById Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<AdmissionSummary> list = new List<AdmissionSummary>();
            JsonResult jlResult;
            try
            {
                if (jsonData.AdmissionSummaries != null && jsonData.AdmissionSummaries.Count >0)
                list = jsonData.AdmissionSummaries;
                else 
                list = _data.AdmissionDetailById(jsonData.RequestId);

                 
                list.Where(w => w.ServiceId == 0).ToList().ForEach(s => s.ServiceName = "Admission Dates");
                list.Where(w => w.ServiceId == 6).ToList().ForEach(s => s.ServiceName = "Surgery Dates");

                Session["AdmissionDetailById"] = list;
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in AdmissionDetailById :", ex);
                return Json("error");
            }
            finally
            {
                Loggger.LogInfo("AdmissionDetailById Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jlResult;
        }

        public ActionResult GetRequestPharmacyDetail(int requestId)
        {
            Loggger.LogInfo("GetRequestPharmacyDetail Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<CommonMasterModel> list = new List<CommonMasterModel>();
            JsonResult jlResult;
            try
            {
                list = _data.GetRequestPharmacyDetail(requestId, _connectionString.getConnectionStringName());
                Session["PharmacyDetailById"] = list;
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetRequestPharmacyDetail :", ex);
                return Json("error");
            }
            finally
            {
                Loggger.LogInfo("GetRequestPharmacyDetail Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jlResult;
        }

        public ActionResult GetRequestManuallyAddedDetail(int requestId)
        {
            Loggger.LogInfo("GetRequestManuallyAddedDetail Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<CommonMasterModel> list = new List<CommonMasterModel>();
            JsonResult jlResult;
            try
            {
                list = _data.GetRequestManuallyAddedDetail(requestId,_connectionString.getConnectionStringName());
                Session["ManuallyAddedDetailById"] = list;
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetRequestManuallyAddedDetail :", ex);
                return Json("error");
            }
            finally
            {
                Loggger.LogInfo("GetRequestManuallyAddedDetail Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jlResult;
        }

        public ActionResult GetRequestSurgeryManuallyAddedDetail(int requestId)
        {
            Loggger.LogInfo("GetRequestSurgeryManuallyAddedDetail Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<SurgeryManualServices> list = new List<SurgeryManualServices>();
            JsonResult jlResult;
            try
            {
                list = _data.GetRequestSurgeryManuallyAddedDetail(requestId, _connectionString.getConnectionStringName());
                Session["ManuallyAddedDetailById"] = list;
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetRequestSurgeryManuallyAddedDetail :", ex);
                return Json("error");
            }
            finally
            {
                Loggger.LogInfo("GetRequestSurgeryManuallyAddedDetail Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jlResult;
        }

        
        [HttpPost]
        public ActionResult GetRequestDetailById(RequestSubmissionModel jsonData)
        {
            Loggger.LogInfo("GetRequestDetailById Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            JsonResult jlResult = null;
            try
            {
                ClearAllSession();
                string ConnectionString = _connectionString.getConnectionStringName(); 
                var taskServices = Task.Run(() => { return (jsonData.RequestId > 0? _data.GetRequestDetailById(jsonData.RequestId, ConnectionString) : null); });
                var taskPharmacy = Task.Run(() => { return (jsonData.RequestId > 0 ? _data.GetRequestPharmacyDetail(jsonData.RequestId, ConnectionString) :null); });
                var taskManualServices = Task.Run(() => { return (jsonData.RequestId > 0 ? _data.GetRequestManuallyAddedDetail(jsonData.RequestId, ConnectionString) : null); });
                var taskSurgeryManualServices = Task.Run(() => { return (jsonData.RequestId > 0 ? _data.GetRequestSurgeryManuallyAddedDetail(jsonData.RequestId, ConnectionString) : null); });
                var taskDefaultSer = Task.Run(() => { return (jsonData.RequestId > 0 ? _data.GetDefaultServicesDetail(jsonData.RequestId, ConnectionString) : null); });

                var taskHopePatientData =   Task.Run(() => { return (jsonData.RequestId > 0 && jsonData.IsHopePatientBill  ? _data.GetPatientData(jsonData.RequestId, ConnectionString) : null); }) ;

                Task.WaitAll(taskServices, taskPharmacy, taskManualServices, taskDefaultSer, taskHopePatientData);
                Loggger.LogInfo("   Task.WaitAll Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                //var ServiceConsumed = taskServices.Result;
                //var PharmacyDetails = taskPharmacy.Result;                                    
                //var ManualServices = taskManualServices.Result;
                //var DefaultService = taskDefaultSer.Result;
                //var SurgeryManualServices = taskSurgeryManualServices.Result;

                ////Filters for removing services on BtnGo click
                //if (jsonData.AdmissionSummaries != null && jsonData.AdmissionSummaries.Count > 0)
                //    {
                //        //Remove All Services which are not in Between admission and Discharge Date                  
                //        var AdmissionDatetime = Convert.ToDateTime(jsonData.AdmissionSummaries[0].StrAdmissionDateTime).Date;
                //        var DischargeDatetime = Convert.ToDateTime(jsonData.AdmissionSummaries[0].StrDischargeDateTime).Date;
                //        ServiceConsumed.RemoveAll(x => (Convert.ToDateTime(x.ConsumeDate) < AdmissionDatetime || Convert.ToDateTime(x.ConsumeDate) > DischargeDatetime));
                //        PharmacyDetails.RemoveAll(x => (Convert.ToDateTime(x.ConsumeDate) < AdmissionDatetime || Convert.ToDateTime(x.ConsumeDate) > DischargeDatetime));
                //        ManualServices.RemoveAll(x => (Convert.ToDateTime(x.ConsumeDate) < AdmissionDatetime || Convert.ToDateTime(x.ConsumeDate) > DischargeDatetime));
                //        DefaultService.RemoveAll(x => (Convert.ToDateTime(x.ConsumeDate) < AdmissionDatetime || Convert.ToDateTime(x.ConsumeDate) > DischargeDatetime));
                //        SurgeryManualServices.RemoveAll(x => (Convert.ToDateTime(x.ConsumeDate) < AdmissionDatetime || Convert.ToDateTime(x.ConsumeDate) > DischargeDatetime));

                //        //Remove All surgeries which are not in between Admission date and surgery date
                //        if (jsonData.SurgerySummaries != null && jsonData.SurgerySummaries.Count > 0)
                //            jsonData.SurgerySummaries.RemoveAll(x => (Convert.ToDateTime(x.StrSurgeryDateTime) < AdmissionDatetime || Convert.ToDateTime(x.StrSurgeryDateTime) > DischargeDatetime));

                //        //Remove all Manual Surgeries when it is Medical
                //        if (jsonData.ManagementTypeId == 1)
                //        {
                //            SurgeryManualServices.RemoveAll(x => x.ConsumeDate == x.ConsumeDate);
                //            ServiceConsumed.RemoveAll(x => x.Id == 9);
                //        }

                //        //Remove Services Consumed if Services are in Packages or when its surgical
                //        if ((jsonData.ManagementTypeId == 2 || jsonData.ManagementTypeId == 3)
                //         && (ServiceConsumed.Count > 0 || PharmacyDetails.Count > 0)
                //         && jsonData.SurgerySummaries != null && jsonData.SurgerySummaries.Count > 0)
                //        {
                //            var ConsumedDates = ServiceConsumed.Select(x => Convert.ToDateTime(x.ConsumeDate)).Distinct().ToList();
                //            ConsumedDates.AddRange(PharmacyDetails.Select(x => Convert.ToDateTime(x.ConsumeDate)).Distinct().ToList());
                //            foreach (var ConsumedDate in ConsumedDates)
                //            {
                //                if ((jsonData.SurgerySummaries.Where(w => Convert.ToDateTime(ConsumedDate) >= Convert.ToDateTime(w.StrSurgeryDateTime) && Convert.ToDateTime(ConsumedDate) <= Convert.ToDateTime(w.StrSurgeryDateTime).AddDays((w.NoOfDays == 0 ? 0 : w.NoOfDays - 1))).Any()
                //                    && jsonData.ManagementTypeId == 3) || jsonData.ManagementTypeId == 2)
                //                {
                //                    ServiceConsumed.RemoveAll(x => Convert.ToDateTime(x.ConsumeDate) == ConsumedDate && x.IsAllowedChangeInSurgery == false);
                //                    PharmacyDetails.RemoveAll(x => Convert.ToDateTime(x.ConsumeDate) == ConsumedDate && x.IsAllowedChangeInSurgery == false);
                //                }

                //            }
                //        }

                //    }

                jsonData.ConsumeDiv = taskServices.Result;
                jsonData.PharmacyDetails = taskPharmacy.Result;
                jsonData.ManullyAddedService = taskManualServices.Result;
                var DefaultService = taskDefaultSer.Result;
                jsonData.SurgeryManullyAddedService = taskSurgeryManualServices.Result;
                if (jsonData.AdmissionSummaries != null && jsonData.AdmissionSummaries.Count > 0)
                {
                   var AdmissionDatetime = Convert.ToDateTime(jsonData.AdmissionSummaries[0].StrAdmissionDateTime).Date;
                   var DischargeDatetime = Convert.ToDateTime(jsonData.AdmissionSummaries[0].StrDischargeDateTime).Date;
                   DefaultService.RemoveAll(x => (Convert.ToDateTime(x.ConsumeDate) < AdmissionDatetime || Convert.ToDateTime(x.ConsumeDate) > DischargeDatetime));
                   RemoveServices(jsonData);
                }


                if (jsonData.ConsumeDiv != null && jsonData.ConsumeDiv.Count > 0)
                {

                    Loggger.LogInfo("   ServiceConsumed Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                    jsonData.ConsumeDiv.ToList().ForEach(c => c.State = true);
                    foreach (var item in jsonData.ConsumeDiv)
                    {

                        Session["ServiceConsumed_" + item.Id] = jsonData.ConsumeDiv.Where(m => m.Id == item.Id).ToList();// && m.RoomTypeId==item.RoomTypeId
                        item.State = true;
                    }
                }

                jsonData.Patient = taskHopePatientData.Result;

                jlResult = Json(new { ServiceConsumed = jsonData.ConsumeDiv, PharmacyDetails = jsonData.PharmacyDetails, ManualServices = jsonData.ManullyAddedService, DefaultService= DefaultService, SurgeryManualServices = jsonData.SurgeryManullyAddedService, SurgerySummaries = jsonData.SurgerySummaries,Patient = jsonData.Patient }, JsonRequestBehavior.AllowGet);

                //var pharmacyDetails = _data.GetRequestPharmacyDetail(requestId);
                //if (pharmacyDetails != null && pharmacyDetails.Count > 0)
                //    Session["pharmacyDetails"] = pharmacyDetails;

                //var manualServiceDetails = _data.GetRequestManuallyAddedDetail(requestId);
                //if (manualServiceDetails != null && manualServiceDetails.Count > 0)
                //    Session["manualServiceDetails"] = manualServiceDetails;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetRequestDetailById :", ex);
            }
            finally
            {
                Loggger.LogInfo("GetRequestDetailById Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jlResult;
        }

        public ActionResult GetRequestOtDetailById(int requestId)
        {
            Loggger.LogInfo("GetRequestOtDetailById Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<AdmissionSummary> list = new List<AdmissionSummary>();
            JsonResult jlResult;
            try
            {
                list = _data.GetRequestOtDetailById(requestId);
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetRequestOtDetailById :", ex);
                return Json("error");
            }
            finally
            {
                Loggger.LogInfo("GetRequestOtDetailById Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jlResult;
        }

        [HttpPost]
        public ActionResult GetRequestSurgeryDetailById(RequestSubmissionModel jsonData)
        {
            Loggger.LogInfo("GetRequestSurgeryDetailById Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            JsonResult jlResult;
            try
            {
                Loggger.LogInfo("   GetSurgerySummaries Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                List<SurgerySummary> list = new List<SurgerySummary>();

                if (jsonData.SurgerySummaries != null && jsonData.SurgerySummaries.Count > 0)
                    list = jsonData.SurgerySummaries;
                else
                    list = GetSurgerySummaries(jsonData.RequestId);



                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in RequestSubmissionNewController class for GetRequestSurgeryDetailById :", ex);
                return Json("error");
            }
            finally
            {
                Loggger.LogInfo("GetRequestSurgeryDetailById Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jlResult;
        }

        public void UpdateQty(string sessionName, int serviceId, bool state, double rate, int roomType, int qty, string ConsumeDate)
        {
            Loggger.LogInfo("UpdateQty Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            JsonResult jsonResult = null;
            var datevar = sessionName.Split('_');
            try
            {
                if (Session[datevar[0]] == null) Session[datevar[0]] = new List<CommonMasterModel>();

                var sessionData = Session[datevar[0]] as List<CommonMasterModel>;
                foreach (var item in sessionData.Where(w => Convert.ToDateTime(w.ConsumeDate).ToString("dd-MMM-yyyy") == ConsumeDate))
                {
                    if (item.ServiceId == serviceId && item.RoomTypeId == roomType)
                    {
                        item.Qty = qty;
                        item.State = state;
                    }
                }
                Session[datevar[0]] = sessionData;
                jsonResult = Json(new { sessionRecord = sessionData }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in RequestSubmissionNewController class for UpdateQty :", ex);
            }
            finally
            {
                Loggger.LogInfo("UpdateQty Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
        }

        private void ClearAllSession(int managementTypeID = 0)
        {
            Loggger.LogInfo("ClearAllSession Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            Session.Remove("AdmissionDetailById");
            Session.Remove("PharmacyDetailById");
            Session.Remove("ManuallyAddedDetailById");
            var SessionKeys = Session.Keys.OfType<string>().Where(w=> w.StartsWith("ServiceConsumed_") || w.StartsWith("ServiceMaster_"));
            foreach (string key in SessionKeys.ToArray())
            {
                Session.Remove(key);
            }
            //var serviceList = _data.ServicesConsumedLeftDiv(managementTypeID);
            //foreach (var item in serviceList)
            //{
            //    Session[item.Name] = null;
            //}

                Loggger.LogInfo("ClearAllSession Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
        }

        public ActionResult GetSurgeryMasterList()
        {
            Loggger.LogInfo("GetSurgeryMasterList Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            JsonResult jsonResult = null;
            List<SurgeryMasterModel> list = new List<SurgeryMasterModel>();
            TryCatch.Run(() =>
            {
                list = _data.GetSurgeryMasterList();
                jsonResult = Json(new { data = list }, JsonRequestBehavior.AllowGet);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in class:RequestSubmissionNewController method :GetSurgeryMasterList :", ex);
            });
            
            
            Loggger.LogInfo("GetSurgeryMasterList Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            return jsonResult;
        }

        public ActionResult GetCancerSurgeryList()
        {
            Loggger.LogInfo("GetCancerSurgeryList Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            JsonResult jsonResult = null;
            bool IsHopeClient = false;
            List<CancerSurgeryModel> list = new List<CancerSurgeryModel>();
            TryCatch.Run(() =>
            {
              
                list = _data.GetCancerSurgeryList();
                jsonResult = Json(new { data = list }, JsonRequestBehavior.AllowGet);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in class:RequestSubmissionNewController method :GetCancerSurgeryList :", ex);
            });


            Loggger.LogInfo("GetCancerSurgeryList Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            return jsonResult;

        }

        public ActionResult GetServicesList()
        {
            Loggger.LogInfo("GetServicesList Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            JsonResult jsonResult = null;
            bool IsHopeClient = false;
            List<ServiceMasterModel> list = new List<ServiceMasterModel>();
            TryCatch.Run(() =>
            {

                list = _data.GetServicesList();
                jsonResult = Json(new { data = list }, JsonRequestBehavior.AllowGet);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in class:RequestSubmissionNewController method :GetServicesList :", ex);
            });
            return jsonResult;
        }


        private List<ServiceMasterModel> GetServiceMasterList(int userId, int categoryId, int hospitalType, int patientType, int stateId, int cityId, int gender, int roomTypeId)
        {
            Loggger.LogInfo("GetServiceMasterList Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());

            var serviceType = new List<ServiceMasterModel>();
            var sessionServiceType = Session["ServiceMaster_" + categoryId];
            if (sessionServiceType != null) serviceType = sessionServiceType as List<ServiceMasterModel>;
            if (serviceType == null || serviceType.Count == 0)
            {
                var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                Loggger.LogInfo("   uiScheduler Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                string ConnectionString = _connectionString.getConnectionStringName();
                Task.Factory.StartNew(() =>
                {
                    var result = _data.GetAllServiceMasterByCategoryId(categoryId, userId, hospitalType, patientType, stateId, cityId, gender, roomTypeId,false, ConnectionString);
                    Loggger.LogInfo("   GetAllServiceMasterByCategoryId Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
                    if (Session["ServiceMaster_" + categoryId] != null)
                        Session["ServiceMaster_" + categoryId] = result;
                    else
                        Session.Add("ServiceMaster_" + categoryId, result);
                }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
            
             Loggger.LogInfo("GetServiceMasterList Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            return serviceType.Where(w => w.CategoryId == categoryId && w.HospitalTypeId == hospitalType
                                                && w.PatientTypeId == patientType && w.StateId == stateId && w.CityId == cityId && w.GenderId == gender && w.RoomTypeId == roomTypeId).ToList();

            //return serviceType;
        }
        private List<CommonMasterModel> ServicesConsumedLeftDiv(int ManagementTypeId = 0,string ConnectionString= "DefaultConnection")
        {
            Loggger.LogInfo("ServicesConsumedLeftDiv Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<CommonMasterModel> list = new List<CommonMasterModel>();
            TryCatch.Run(() =>
            {
                list = _data.ServicesConsumedLeftDiv(ManagementTypeId, ConnectionString);

            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in RequestSubmissionNewController method of ServicesConsumedLeftDiv :", ex);
            });
            
            Loggger.LogInfo("ServicesConsumedLeftDiv Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            return list;
        }

        private List<ServiceMasterModel> GetBedChargesList(int userId, int categoryId, int hospitalType, int patientType, int roomTypeId, int stateId, int cityId, int gender)
        {
            Loggger.LogInfo("GetBedChargesList Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<ServiceMasterModel> list = new List<ServiceMasterModel>();
            try
            {
                list = _data.GetServiceMasterByCategoryRoomId(categoryId, userId, hospitalType, patientType, roomTypeId, stateId, cityId, gender);
                Loggger.LogInfo("   GetServiceMasterByCategoryRoomId Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetBedChargesList :", ex);
            }
            finally
            {
                Loggger.LogInfo("GetBedChargesList Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return list;
        }

        private List<SurgerySummary> GetSurgerySummaries(int RequestID = 0)
        {
            Loggger.LogInfo("GetSurgerySummaries Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<SurgerySummary> list = new List<SurgerySummary>();
            TryCatch.Run(() =>
            {
                list = _data.GetRequestSurgeryDetailById(RequestID);
                Loggger.LogInfo("   GetRequestSurgeryDetailById Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in RequestSubmissionNewController class for method of GetSurgerySummaries :", ex);
            });
            
            Loggger.LogInfo("GetSurgerySummaries Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            return list;
        }

        private List<BedCharges> GetRequestBedChargeDetails(int requestId = 0,string ConnectionString= "DefaultConnection")
        {
            Loggger.LogInfo("GetRequestBedChargeDetails Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<BedCharges> list = new List<BedCharges>();
            TryCatch.Run(() =>
            {
                list = _data.GetRequestBedChargeDetails(requestId, ConnectionString);

            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in RequestSubmissionNewController method of GetRequestBedChargeDetails :", ex);
            });

            Loggger.LogInfo("GetRequestBedChargeDetails Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            return list;
        }

        public static string RenderPartialViewToString(Controller thisController, string viewName, object model)
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

        private bool Validate(RequestSubmissionModel data, out string errormessage, out bool isSurgeryDateConflicts)
        {
            Loggger.LogInfo("Validate Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            errormessage = ""; isSurgeryDateConflicts = false;
            if (data != null)
            {
                //if (string.IsNullOrEmpty(data.FileNo))
                //    errormessage += "File no is missing.";
                if (data.HospitalTypeId.NullToInt() == 0)
                    errormessage += "HospitalType, ";
                if (data.PatientTypeId.NullToInt() == 0)
                    errormessage += "PatientType, ";
                if (data.StateId.NullToInt() == 0)
                    errormessage += "State, ";
                if (data.CityId.NullToInt() == 0)
                    errormessage += "City, ";
                if (data.TypeOfAddmissionId.NullToInt() == 0)
                    errormessage += "Addmission Type, ";
                if (string.IsNullOrEmpty(data.IpdNo))
                    errormessage += "IpdNo, ";
                if (string.IsNullOrEmpty(data.PatientName))
                    errormessage += "PatientName, ";
                if (string.IsNullOrWhiteSpace(Convert.ToString(data.PatientAge)) || data.PatientAge == 0.0)
                    errormessage += "PatientAge, ";
                if (data.GenderId.NullToInt() == 0)
                    errormessage += "Gender, ";
                if (string.IsNullOrEmpty(data.PatientAddress))
                    errormessage += "PatientAddress, ";
                if (data.RoomEntitleTypeId.NullToInt() == 0)
                    errormessage += "Room Type, ";
                if (data.ManagementTypeId.NullToInt() == 0)
                    errormessage += "Management Type, ";

                if (data.AdmissionSummaries == null || data.AdmissionSummaries.Count == 0)
                    errormessage += "Admission details (Admission date & Discharge date), ";

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

            //if (string.IsNullOrWhiteSpace(errormessage) && data.SurgerySummaries != null && data.SurgerySummaries.Count > 0 && data.RequestId == 0 && !data.IgnoreSurgeryValidation)
            //{
            //    var managementTypeList = new CommonMasterRepository().GetAllTypeofManagement();
            //    if (managementTypeList.Where(w => w.Id == data.ManagementTypeId && w.Name.ToUpper() == "SURGICAL").Any())
            //    {
            //        var admissionRow = data.AdmissionSummaries.OrderBy(O => O.AdmissionDateTime).FirstOrDefault();
            //        var dischargeRow = data.AdmissionSummaries.OrderByDescending(O => O.DischargeDateTime).FirstOrDefault();

            //        if (data.SurgerySummaries != null && data.SurgerySummaries.Count > 0)
            //            data.SurgerySummaries.ForEach(f => { f.SurgeryDateTime = Convert.ToDateTime(f.StrSurgeryDateTime); });

            //        var surgeryStartDate = data.SurgerySummaries.OrderBy(O => O.SurgeryDateTime).FirstOrDefault().SurgeryDateTime;
            //        var endOrder = data.SurgerySummaries.OrderByDescending(O => O.SurgeryDateTime).FirstOrDefault();
            //        var surgeryEndDate = endOrder.SurgeryDateTime.AddDays(endOrder.NoOfDays - 1);//surgery no od days in includng surgery date;

            //        //if either admission date or discharge date is not matching with SUrgery date, pop up message for user
            //        if((admissionRow.AdmissionDateTime.Date != surgeryStartDate ||  dischargeRow.DischargeDateTime.Date != surgeryEndDate))
            //        {
            //            //data.AdmissionSummaries.ForEach(f => { f.SurgeryAdmissionDateTime = f.StrAdmissionDateTime; f.SurgerydDischargeDateTime = f.StrDischargeDateTime; });
            //            errormessage += Constants.SurgeryAlertMessage;
            //            isSurgeryDateConflicts = true;
            //        }
            //    }
            //}

            Loggger.LogInfo("Validate Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            if (string.IsNullOrEmpty(errormessage))
                return true;
            else
                return false;
        }

    


    }

    
}