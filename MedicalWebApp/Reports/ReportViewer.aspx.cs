using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CGHSBilling.Areas.HospitalForms.Models;
using CommonLayer;
using Microsoft.Reporting.WebForms;
using ReportDataSource = Microsoft.Reporting.WebForms.ReportDataSource;
using ReportParameter = Microsoft.Reporting.WebForms.ReportParameter;
using System.Threading.Tasks;
using CGHSBilling.API.HospitalForms.Repository;
using CGHSBilling.API.HospitalForms.Intefaces;
using CommonDataLayer.DataAccess;
using CGHSBilling.Common;

namespace CGHSBilling.Reports
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        ReportParameter[] _rparams;
        private ReportDataSource _rds;
        static CommonLayer.ILogger _logger = Logger.Register(typeof(ReportViewer));
        ConnectionString _ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["reportid"] != null)
                {
                    _ConnectionString = new ConnectionString();
                    int reportId = 0, requestId = 0, configId = 0; string Fromtime = "";
                    string Totime = ""; int ServiceId = 0; int CategoryId = 0; int BillTypeId = 0;
                    if (Request.QueryString["reportid"] != null)
                    {
                        if (Request.QueryString["reportid"].ToString() != "")
                            reportId = Convert.ToInt32(Request.QueryString["reportid"].ToString());
                        else
                            reportId = 0;
                    }

                    if (Request.QueryString["requestId"] != null)
                    {
                        if (Request.QueryString["requestId"].ToString() != "")
                            requestId = Convert.ToInt32(Request.QueryString["requestId"].ToString());
                        else
                            requestId = 0;
                    }

                    if (Request.QueryString["configId"] != null)
                    {
                        if (Request.QueryString["configId"].ToString() != "")
                            configId = Convert.ToInt32(Request.QueryString["configId"].ToString());
                        else
                            configId = 0;
                    }

                    if (Request.QueryString["FromTime"] != null)
                        Fromtime = Request.QueryString["FromTime"].ToString();

                    if (Request.QueryString["FromTime"] != null)
                        Totime = Request.QueryString["ToTime"].ToString();

                    if (Request.QueryString["ServiceId"] != null)
                    {
                        if (Request.QueryString["ServiceId"].ToString() != "")
                            ServiceId = Convert.ToInt32(Request.QueryString["ServiceId"].ToString());
                        else
                            ServiceId = 0;
                    }
                    if (Request.QueryString["CategoryId"] != null)
                    {
                        if (Request.QueryString["CategoryId"].ToString() != "")
                            CategoryId = Convert.ToInt32(Request.QueryString["CategoryId"].ToString());
                        else
                            CategoryId = 0;
                    }
                    if (Request.QueryString["BillTypeId"] != null)
                    {
                        if (Request.QueryString["BillTypeId"].ToString() != "")
                            BillTypeId = Convert.ToInt32(Request.QueryString["BillTypeId"].ToString());
                        else
                            BillTypeId = 0;
                    }

                    if (reportId == 1)
                    {
                        GetAllgeneratedRequestById(requestId);
                    }
                    else if (reportId == 2)
                    {
                        GetAllOPDGeneratedRequestById(requestId);
                    }
                    else if (reportId == 3 || reportId == 4)
                    {
                        GetAllgeneratedRequestByDate(Fromtime, Totime, reportId);
                    }
                    else if (reportId == 5)
                    {
                        GetAllServiceWiseBillDtls(Fromtime, Totime, ServiceId, CategoryId, BillTypeId);
                    }
                    else if (reportId == 6)
                    {
                        GetCertificateWise(requestId);
                    }
                    else if (reportId == 7)
                    {
                        GetAllgeneratedRequestById2(requestId, configId);
                    }
                    else if (reportId == 8)
                    {
                        GetAllOPDGeneratedRequestById2(requestId, configId);
                    }
                }


            }
        }
        RequestSubmissionModel_Report requestSubmissionModel = new RequestSubmissionModel_Report();
        private void GetAllgeneratedRequestById(int requestId)
        {
            try
            {
                //RequestSubmissionNewController ctr = new RequestSubmissionNewController();
                requestSubmissionModel = GetAllgeneratedRequestBy_Id(requestId);

                ReportViewer1.LocalReport.DataSources.Clear();
                string reportPath = Server.MapPath("../Reports/HospitalForms/requestSubmissionrpt.rdlc");
                ReportViewer1.LocalReport.EnableExternalImages = true;
                ReportViewer1.LocalReport.ReportPath = reportPath;
                _rds = new ReportDataSource();
                _rds.Name = "RequestSubmissionMaster";
                _rds.Value = new List<RequestSubmissionModel_Report>() { requestSubmissionModel };
                ReportViewer1.LocalReport.DataSources.Add(_rds);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(BillSummary);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(BedCharges);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SurgeryDetails);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(Investigation);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(ManualAddedServiceDetails);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SurgeryManualAddedServiceDetails);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(PharmacyDetail);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(DefaultServices);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(AutoLinkedServices);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(AutoCancerLinkedServices);

                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(ReceiptAgainstFinalBill);
                ReportViewer1.LocalReport.Refresh();

                IHospitalFormsRepository _data = new HospitalFormsRepository();
                _data.BillAmountDeduction(requestId);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private void GetAllgeneratedRequestById2(int requestId, int configId)
        {
            try
            {
                //RequestSubmissionNewController ctr = new RequestSubmissionNewController();
                requestSubmissionModel = GetAllgeneratedRequestBy_Id2(requestId, configId);

                ReportViewer1.LocalReport.DataSources.Clear();
                string reportPath = Server.MapPath("../Reports/HospitalForms/requestSubmissionrpt.rdlc");
                ReportViewer1.LocalReport.EnableExternalImages = true;
                ReportViewer1.LocalReport.ReportPath = reportPath;
                _rds = new ReportDataSource();
                _rds.Name = "RequestSubmissionMaster";
                _rds.Value = new List<RequestSubmissionModel_Report>() { requestSubmissionModel };
                ReportViewer1.LocalReport.DataSources.Add(_rds);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(BillSummary);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(BedCharges);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SurgeryDetails);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(Investigation);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(ManualAddedServiceDetails);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SurgeryManualAddedServiceDetails);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(PharmacyDetail);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(DefaultServices);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(AutoLinkedServices);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(AutoCancerLinkedServices);

                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(ReceiptAgainstFinalBill);
                ReportViewer1.LocalReport.Refresh();

                IHospitalFormsRepository _data = new HospitalFormsRepository();
                _data.BillAmountDeduction(requestId);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        RequestSubmissionOPDModel_Report requestSubmissionOPDModel = new RequestSubmissionOPDModel_Report();
        private void GetAllOPDGeneratedRequestById(int requestId)
        {
            requestSubmissionOPDModel = GetAllOPDGeneratedRequestBy_Id(requestId);

            ReportViewer1.LocalReport.DataSources.Clear();
            string reportPath = Server.MapPath("../Reports/HospitalForms/requestOPDSubmissionrpt.rdlc");
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.ReportPath = reportPath;
            _rds = new ReportDataSource();
            _rds.Name = "OPDRequestSubmission";
            _rds.Value = new List<RequestSubmissionOPDModel_Report>() { requestSubmissionOPDModel };
            ReportViewer1.LocalReport.DataSources.Add(_rds);
            //ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OPDHeader);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OPDBillSummary);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OPDInvestigation);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OPDManualAddedServiceDetails);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OPDDefaultServices);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OPDReceiptAgainstFinalBill);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(RequestOPDSubmissionTotalAmt);
            ReportViewer1.LocalReport.Refresh();
        }
        private void GetAllOPDGeneratedRequestById2(int requestId, int configId)
        {
            requestSubmissionOPDModel = GetAllOPDGeneratedRequestBy_Id2(requestId, configId);

            ReportViewer1.LocalReport.DataSources.Clear();
            string reportPath = Server.MapPath("../Reports/HospitalForms/requestOPDSubmissionrpt.rdlc");
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.ReportPath = reportPath;
            _rds = new ReportDataSource();
            _rds.Name = "OPDRequestSubmission";
            _rds.Value = new List<RequestSubmissionOPDModel_Report>() { requestSubmissionOPDModel };
            ReportViewer1.LocalReport.DataSources.Add(_rds);
            //ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OPDHeader);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OPDBillSummary);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OPDInvestigation);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OPDManualAddedServiceDetails);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OPDDefaultServices);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OPDReceiptAgainstFinalBill);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(RequestOPDSubmissionTotalAmt);
            ReportViewer1.LocalReport.Refresh();
        }

        List<ViewBillsModel_Report> requestSubModelDatewise = new List<ViewBillsModel_Report>();
        private void GetAllgeneratedRequestByDate(string Fromtime, string Totime, int reportid)
        {
            requestSubModelDatewise = reportid == 3 ? GetAllgeneratedRequestByDateWise(Fromtime, Totime) :
                                                   GetAllgeneratedOPDRequestByDateWise(Fromtime, Totime);
            ReportViewer1.LocalReport.DataSources.Clear();
            string reportPath = Server.MapPath("../Reports/HospitalForms/requestSubmissionDateWiserpt.rdlc");
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.ReportPath = reportPath;
            _rds = new ReportDataSource("RequestSubmissionDateWise", requestSubModelDatewise);
            ReportViewer1.LocalReport.DataSources.Add(_rds);
            ReportViewer1.LocalReport.Refresh();
        }

        List<ServiceWiseBillDetailsModel> ServiceWiseBillDtls = new List<ServiceWiseBillDetailsModel>();
        private void GetAllServiceWiseBillDtls(string Fromtime, string Totime, int ServiceId, int CategoryId, int BillTypeId)
        {
            ServiceWiseBillDtls = GetAllServiceWiseBillDtls_Datewise(Fromtime, Totime, ServiceId, CategoryId, BillTypeId);
            ReportViewer1.LocalReport.DataSources.Clear();
            string reportPath = Server.MapPath("../Reports/HospitalForms/requestServiceWiseBillDtlsrpt.rdlc");
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.ReportPath = reportPath;
            _rds = new ReportDataSource("ServiceWiseBillDtls", ServiceWiseBillDtls);
            ReportViewer1.LocalReport.DataSources.Add(_rds);
            ReportViewer1.LocalReport.Refresh();
        }


        #region OPD Data Sources and event


        void OPDBillSummary(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(new ReportDataSource("BillSummaryDS", requestSubmissionOPDModel.BillSummary));
            }
            catch (Exception exception)
            {
                throw;
            }

        }
        void OPDInvestigation(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(new ReportDataSource("RequestDetail", requestSubmissionOPDModel.ConsumeServices));
            }
            catch (Exception exception)
            {

                throw;
            }

        }
        void OPDManualAddedServiceDetails(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(new ReportDataSource("ManualAddedService", requestSubmissionOPDModel.ManullyAddedService));
            }
            catch (Exception exception)
            {
                throw;
            }

        }
        void OPDDefaultServices(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(new ReportDataSource("DefaultServices", requestSubmissionOPDModel.DefaultServices));
            }
            catch (Exception exception)
            {

                throw;
            }

        }

        void OPDReceiptAgainstFinalBill(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(new ReportDataSource("OPDReceiptAgainstFinalBill", new List<RequestSubmissionOPDModel_Report>() { requestSubmissionOPDModel }));
            }

            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }

        void RequestOPDSubmissionTotalAmt(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(new ReportDataSource("RequestOPDSubmissionTotalAmt", new List<RequestSubmissionOPDModel_Report>() { requestSubmissionOPDModel }));
            }

            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
        #endregion

        #region Data Sources for IPD bill and events
        void BedCharges(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(new ReportDataSource("BedChargesSummary", requestSubmissionModel.BedCharges));
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
        void BillSummary(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                //// Dictionary<string, double> summaryData = new Dictionary<string, double>();
                // DataTable dtSummaryData = new DataTable();
                // dtSummaryData.Columns.Add(new DataColumn("ServiceName", typeof(string)));
                // dtSummaryData.Columns.Add(new DataColumn("BillAmount", typeof(double)));

                // if (requestSubmissionModel != null)
                // {
                //     DataRow drNew = dtSummaryData.NewRow();
                //     drNew["ServiceName"] = "Bed Charges";
                //     drNew["BillAmount"] = (requestSubmissionModel.BedCharges != null ? requestSubmissionModel.BedCharges.Sum(s => (s.Qty * s.BillRate)) : 0);
                //     dtSummaryData.Rows.Add(drNew);

                //     drNew = dtSummaryData.NewRow();
                //     drNew["ServiceName"] = "Pharmacy Charges";
                //     drNew["BillAmount"] = (requestSubmissionModel.PharmacyDetails != null ? requestSubmissionModel.PharmacyDetails.Sum(s => (s.BillRate + s.LifeSavingBillRate)) : 0);
                //     dtSummaryData.Rows.Add(drNew);

                //     var services = requestSubmissionModel.Investigation.GroupBy(g => g.Name).Select(s => new { Name = s.Key, BillRate = s.Sum(t => (t.Qty * t.BillRate)) });
                //     foreach (var service in services)
                //     {
                //         drNew = dtSummaryData.NewRow();
                //         drNew["ServiceName"] = service.Name;
                //         drNew["BillAmount"] = service.BillRate;
                //         dtSummaryData.Rows.Add(drNew);
                //     }

                //     drNew = dtSummaryData.NewRow();
                //     drNew["ServiceName"] = "Manually Added Service Charges";
                //     drNew["BillAmount"] = (requestSubmissionModel.ManullyAddedService != null ? requestSubmissionModel.ManullyAddedService.Sum(s => (s.Qty * s.BillRate)) : 0);
                //     dtSummaryData.Rows.Add(drNew);

                //     //dtSummaryData.AcceptChanges();
                //     //summaryData.Add("Bed Charges", (requestSubmissionModel.BedCharges != null ? requestSubmissionModel.BedCharges.Sum(s => (s.Qty * s.BillRate)) : 0));
                //     //summaryData.Add("Pharmacy Charges", (requestSubmissionModel.PharmacyDetails != null ? requestSubmissionModel.PharmacyDetails.Sum(s => (s.BillRate + s.LifeSavingBillRate)) : 0));

                //     //var services = requestSubmissionModel.Investigation.GroupBy(g => g.Name).Select(s => new { Name = s.Key, BillRate = s.Sum(t => (t.Qty * t.BillRate)) });
                //     //foreach (var service in services)
                //     //{
                //     //    summaryData.Add(service.Name, service.BillRate);
                //     //}
                //     //summaryData.Add("Manually Added Service Charges", (requestSubmissionModel.ManullyAddedService != null ? requestSubmissionModel.ManullyAddedService.Sum(s => (s.Qty * s.BillRate)) : 0));
                // }
                e.DataSources.Add(new ReportDataSource("BillSummaryDS", requestSubmissionModel.BillSummary));
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
        void Investigation(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(new ReportDataSource("RequestDetail", requestSubmissionModel.Investigation));
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
        void SurgeryDetails(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                var excludingCancer = requestSubmissionModel.SurgerySummaries.Where(w => w.ServiceTypeId != 7 && w.ServiceTypeId != 13).ToList();
                e.DataSources.Add(new ReportDataSource("SurgeryChargesSummary", excludingCancer));
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        void ManualAddedServiceDetails(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(new ReportDataSource("ManualAddedService", requestSubmissionModel.ManullyAddedService));
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
        void SurgeryManualAddedServiceDetails(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(new ReportDataSource("SurgeryManualAddedService", requestSubmissionModel.SurgeryManullyAddedService));
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
        void PharmacyDetail(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(new ReportDataSource("PharmacyDetail", requestSubmissionModel.PharmacyDetails));
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
        void DefaultServices(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(new ReportDataSource("DefaultServices", requestSubmissionModel.DefaultService));
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
        void AutoLinkedServices(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                var linkedService = requestSubmissionModel.LinkedService.Where(w => w.ServiceTypeId != 7 && w.ServiceTypeId != 13).ToList();
                e.DataSources.Add(new ReportDataSource("AutoLinkedServices", linkedService));
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
        void AutoCancerLinkedServices(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                var CancerlinkedService = requestSubmissionModel.LinkedService.Where(w => w.ServiceTypeId == 7 || w.ServiceTypeId == 13).ToList();
                e.DataSources.Add(new ReportDataSource("AutoCancerLinkedServices", CancerlinkedService));
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
        void ReceiptAgainstFinalBill(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(new ReportDataSource("ReceiptAgainstFinalBill", new List<RequestSubmissionModel_Report>() { requestSubmissionModel }));
            }

            catch (Exception ex)
            {
                _logger.LogInfo("Report render failed:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
        #endregion

        RequestSubmissionModel_Report GetAllgeneratedRequestBy_Id(int requestId)
        {
            _logger.LogInfo("GetAllgeneratedRequestBy_Id Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            RequestSubmissionModel_Report list = new RequestSubmissionModel_Report();
            try
            {
                string ConnectionString = _ConnectionString.getConnectionStringName();
                var _data = new HospitalFormsRepository();
                //var taskAdmissionSummary = Task.Run(() =>
                //{
                //    return _data.AdmissionDetailById(requestId);
                //});
                var taskRequestMaster = Task.Run(() =>
                {
                    return _data.GetAllgeneratedRequestById(requestId, ConnectionString);
                });
                var taskRequestDetail = Task.Run(() =>
                {
                    return _data.GetRequestDetailById(requestId, ConnectionString);
                });
                var taskBedcharges = Task.Run(() =>
                {
                    return _data.GetRequestBedChargeDetails(requestId, ConnectionString);
                });
                var taskSurgerycharges = Task.Run(() =>
                {
                    return _data.GetRequestSurgeryChargeDetails(requestId, ConnectionString);
                });
                var taskPharmacy = Task.Run(() =>
                {
                    return _data.GetRequestPharmacyDetail(requestId, ConnectionString);
                });
                var taskManualServices = Task.Run(() =>
                {
                    return _data.GetRequestManuallyAddedDetail(requestId, ConnectionString);
                });
                var taskSurgeryManualServices = Task.Run(() =>
                {
                    return _data.GetRequestSurgeryManuallyAddedDetail(requestId, ConnectionString);
                });
                var taskDefaultSer = Task.Run(() =>
                {
                    return _data.GetDefaultServicesDetail(requestId, ConnectionString);
                });
                var taskAutoLinkedSer = Task.Run(() =>
                {
                    return _data.GetAutoLinkedServicesDetailByRequestId(requestId, ConnectionString);
                });

                Task.WaitAll(taskRequestMaster, taskRequestDetail, taskBedcharges, taskSurgerycharges, taskSurgeryManualServices, taskPharmacy, taskManualServices, taskDefaultSer, taskAutoLinkedSer);

                list = taskRequestMaster.Result;
                list.PharmacyDetails = taskPharmacy.Result;
                list.BedCharges = taskBedcharges.Result;
                list.ManullyAddedService = taskManualServices.Result;
                list.Investigation = taskRequestDetail.Result;
                list.DefaultService = taskDefaultSer.Result;
                list.LinkedService = taskAutoLinkedSer.Result;
                list.SurgerySummaries = taskSurgerycharges.Result;
                list.SurgeryManullyAddedService = taskSurgeryManualServices.Result;

                if (list != null)
                {
                    list.BillSummary = new List<BillSummary>();
                    if (list.SurgerySummaries.Where(w => w.ServiceTypeId != 7 && w.ServiceTypeId != 13).Any())
                        list.BillSummary.Add(new BillSummary { ServiceName = Common.Constants.SurgerySection, BillAmount = ((list.SurgerySummaries != null ? list.SurgerySummaries.Where(w => w.ServiceTypeId != 7).Sum(s => (s.Qty * s.BillRate)) : 0)) });

                    if (list.SurgerySummaries.Where(w => w.ServiceTypeId == 7 || w.ServiceTypeId == 13).Any())
                    {
                        var cancerSurgCharge = (list.SurgerySummaries != null ? list.SurgerySummaries.Where(w => w.ServiceTypeId == 7 || w.ServiceTypeId == 13).Sum(s => (s.Qty * s.BillRate)) : 0);
                        cancerSurgCharge = cancerSurgCharge + (list.LinkedService != null ? list.LinkedService.Where(w => w.ServiceTypeId == 7 || w.ServiceTypeId == 13).Sum(s => (s.Qty * s.BillRate)) : 0);

                        list.BillSummary.Add(new BillSummary { ServiceName = Common.Constants.CancerSurgerySection, BillAmount = cancerSurgCharge });
                    }

                    list.BillSummary.Add(new BillSummary { ServiceName = Common.Constants.BedSection, BillAmount = ((list.BedCharges != null ? list.BedCharges.Sum(s => (s.Qty * s.BillRate)) : 0)) });
                    list.BillSummary.Add(new BillSummary { ServiceName = Common.Constants.PharmacySection, BillAmount = ((list.PharmacyDetails != null ? list.PharmacyDetails.Sum(s => (s.BillRate + s.LifeSavingBillRate)) : 0)) });

                    var services = list.Investigation.GroupBy(g => g.Name).Select(s => new { Name = s.Key, BillRate = s.Sum(t => (t.Qty * t.BillRate)) });
                    foreach (var service in services)
                    {
                        list.BillSummary.Add(new BillSummary { ServiceName = service.Name, BillAmount = service.BillRate });
                    }
                    list.BillSummary.Add(new BillSummary { ServiceName = Common.Constants.DefaultSection, BillAmount = ((list.DefaultService != null ? list.DefaultService.Sum(s => (s.Qty * s.BillRate)) : 0)) });

                    if (list.LinkedService.Where(w => w.ServiceTypeId != 7 && w.ServiceTypeId != 13).Any())
                        list.BillSummary.Add(new BillSummary { ServiceName = Common.Constants.LinkingSection, BillAmount = ((list.LinkedService != null ? list.LinkedService.Where(w => w.ServiceTypeId != 7 && w.ServiceTypeId != 13).Sum(s => (s.Qty * s.BillRate)) : 0)) });

                    list.BillSummary.Add(new BillSummary { ServiceName = Common.Constants.ManualSection, BillAmount = ((list.ManullyAddedService != null ? list.ManullyAddedService.Sum(s => (s.Qty * s.BillRate)) : 0)) });
                    list.BillSummary.Add(new BillSummary { ServiceName = Common.Constants.SurgeryManualSection, BillAmount = ((list.SurgeryManullyAddedService != null ? list.SurgeryManullyAddedService.Sum(s => (s.OTCharges + s.OtherCharges + s.ExtraCharges + s.AnesthesiaCharges + s.SurgeonCharges)) : 0)) });

                    //Remove all services which are <=0 Amount
                    list.BillSummary.RemoveAll(r => r.BillAmount == 0);
                }

                if (list.Investigation != null && list.Investigation.Count > 0)
                    list.Investigation = list.Investigation.OrderBy(o => o.Name).ThenBy(t => Convert.ToDateTime(t.ConsumeDate)).ToList();
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
                _logger.LogError("Error in GetAllgeneratedRequestBy_Id :", ex);
            }
            //finally
            //{
            //    Loggger.LogInfo("GetAllgeneratedRequestById Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            //}
            return list;
        }
        RequestSubmissionModel_Report GetAllgeneratedRequestBy_Id2(int requestId, int configId)
        {
            _logger.LogInfo("GetAllgeneratedRequestBy_Id2 Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            RequestSubmissionModel_Report list = new RequestSubmissionModel_Report();
            try
            {
                string ConnectionString = _ConnectionString.getConnectionStringName();
                var _data = new HospitalFormsRepository();

                var taskRequestMaster = Task.Run(() => _data.GetAllgeneratedRequestById2(requestId, configId, ConnectionString));
                var taskRequestDetail = Task.Run(() => _data.GetRequestDetailById2(requestId, configId, ConnectionString));
                var taskBedcharges = Task.Run(() => _data.GetRequestBedChargeDetails2(requestId, configId, ConnectionString));
                var taskSurgerycharges = Task.Run(() => _data.GetRequestSurgeryChargeDetails2(requestId, configId, ConnectionString));
                var taskPharmacy = Task.Run(() => _data.GetRequestPharmacyDetail(requestId, ConnectionString));
                var taskManualServices = Task.Run(() => _data.GetRequestManuallyAddedDetail(requestId, ConnectionString));
                var taskSurgeryManualServices = Task.Run(() => _data.GetRequestSurgeryManuallyAddedDetail(requestId, ConnectionString));
                var taskDefaultSer = Task.Run(() => _data.GetDefaultServicesDetail2(requestId, configId, ConnectionString));
                var taskAutoLinkedSer = Task.Run(() => _data.GetAutoLinkedServicesDetailByRequestId2(requestId, configId, ConnectionString));

                Task.WaitAll(taskRequestMaster, taskRequestDetail, taskBedcharges, taskSurgerycharges, taskSurgeryManualServices, taskPharmacy, taskManualServices, taskDefaultSer, taskAutoLinkedSer);

                list = taskRequestMaster.Result;
                list.PharmacyDetails = taskPharmacy.Result;
                list.BedCharges = taskBedcharges.Result;
                list.ManullyAddedService = taskManualServices.Result;
                list.Investigation = taskRequestDetail.Result;
                list.DefaultService = taskDefaultSer.Result;
                list.LinkedService = taskAutoLinkedSer.Result;
                list.SurgerySummaries = taskSurgerycharges.Result;
                list.SurgeryManullyAddedService = taskSurgeryManualServices.Result;

                if (list != null)
                {
                    list.BillSummary = new List<BillSummary>();

                    if (list.SurgerySummaries.Any(w => w.ServiceTypeId != 7 && w.ServiceTypeId != 13))
                    {
                        list.BillSummary.Add(new BillSummary
                        {
                            ServiceName = Common.Constants.SurgerySection,
                            BillAmount = list.SurgerySummaries?.Where(w => w.ServiceTypeId != 7).Sum(s => s.Qty * s.BillRate) ?? 0
                        });
                    }

                    if (list.SurgerySummaries.Any(w => w.ServiceTypeId == 7 || w.ServiceTypeId == 13))
                    {
                        var cancerSurgCharge = list.SurgerySummaries?.Where(w => w.ServiceTypeId == 7 || w.ServiceTypeId == 13).Sum(s => s.Qty * s.BillRate) ?? 0;
                        cancerSurgCharge += list.LinkedService?.Where(w => w.ServiceTypeId == 7 || w.ServiceTypeId == 13).Sum(s => s.Qty * s.BillRate) ?? 0;

                        list.BillSummary.Add(new BillSummary { ServiceName = Common.Constants.CancerSurgerySection, BillAmount = cancerSurgCharge });
                    }

                    list.BillSummary.Add(new BillSummary
                    {
                        ServiceName = Common.Constants.BedSection,
                        BillAmount = list.BedCharges?.Sum(s => s.Qty * s.BillRate) ?? 0
                    });

                    list.BillSummary.Add(new BillSummary
                    {
                        ServiceName = Common.Constants.PharmacySection,
                        BillAmount = list.PharmacyDetails?.Sum(s => s.BillRate + s.LifeSavingBillRate) ?? 0
                    });

                    var services = list.Investigation.GroupBy(g => g.Name)
                        .Select(s => new { Name = s.Key, BillRate = s.Sum(t => t.Qty * t.BillRate) });

                    foreach (var service in services)
                    {
                        list.BillSummary.Add(new BillSummary { ServiceName = service.Name, BillAmount = service.BillRate });
                    }

                    list.BillSummary.Add(new BillSummary
                    {
                        ServiceName = Common.Constants.DefaultSection,
                        BillAmount = list.DefaultService?.Sum(s => s.Qty * s.BillRate) ?? 0
                    });

                    if (list.LinkedService.Any(w => w.ServiceTypeId != 7 && w.ServiceTypeId != 13))
                    {
                        list.BillSummary.Add(new BillSummary
                        {
                            ServiceName = Common.Constants.LinkingSection,
                            BillAmount = list.LinkedService?.Where(w => w.ServiceTypeId != 7 && w.ServiceTypeId != 13).Sum(s => s.Qty * s.BillRate) ?? 0
                        });
                    }

                    list.BillSummary.Add(new BillSummary
                    {
                        ServiceName = Common.Constants.ManualSection,
                        BillAmount = list.ManullyAddedService?.Sum(s => s.Qty * s.BillRate) ?? 0
                    });

                    list.BillSummary.Add(new BillSummary
                    {
                        ServiceName = Common.Constants.SurgeryManualSection,
                        BillAmount = list.SurgeryManullyAddedService?.Sum(s => s.OTCharges + s.OtherCharges + s.ExtraCharges + s.AnesthesiaCharges + s.SurgeonCharges) ?? 0
                    });

                    list.BillSummary.RemoveAll(r => r.BillAmount == 0);
                }

                if (list.Investigation != null && list.Investigation.Count > 0)
                {
                    list.Investigation = list.Investigation.OrderBy(o => o.Name)
                                                             .ThenBy(t => Convert.ToDateTime(t.ConsumeDate))
                                                             .ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllgeneratedRequestBy_Id2 :", ex);
            }
            return list;
        }

        RequestSubmissionOPDModel_Report GetAllOPDGeneratedRequestBy_Id(int requestId)
        {
            _logger.LogInfo("GetAllOPDGeneratedRequestBy_Id Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            RequestSubmissionOPDModel_Report list = new RequestSubmissionOPDModel_Report();
            try
            {
                var _data = new OPDBillingRepository();
                string ConnectionString = _ConnectionString.getConnectionStringName();

                var taskRequestMaster = Task.Run(() =>
                {
                    return _data.GetOPDGeneratedRequestById(requestId, ConnectionString);
                });
                var taskRequestDetail = Task.Run(() =>
                {
                    return _data.GetOPDRequestDetailById(requestId, true, ConnectionString);
                });
                var taskManualServices = Task.Run(() =>
                {
                    return _data.GetRequestManuallyOPDAddedDetail(requestId, ConnectionString);
                });
                var taskDefaultSer = Task.Run(() =>
                {
                    return _data.GetDefaultOPDServicesDetail(requestId, true, ConnectionString);
                });

                Task.WaitAll(taskRequestMaster, taskRequestDetail, taskManualServices, taskDefaultSer);

                list = taskRequestMaster.Result;
                list.ManullyAddedService = taskManualServices.Result;
                list.ConsumeServices = taskRequestDetail.Result;
                list.DefaultServices = taskDefaultSer.Result;

                if (list != null)
                {
                    list.BillSummary = new List<BillSummary>();

                    var services = list.ConsumeServices.GroupBy(g => g.Name).Select(s => new { Name = s.Key, BillRate = s.Sum(t => (t.Qty * t.BillRate)) });
                    foreach (var service in services)
                    {
                        list.BillSummary.Add(new BillSummary { ServiceName = service.Name, BillAmount = service.BillRate });
                    }
                    list.BillSummary.Add(new BillSummary { ServiceName = Common.Constants.DefaultSection, BillAmount = ((list.DefaultServices != null ? list.DefaultServices.Sum(s => (s.Qty * s.BillRate)) : 0)) });

                    list.BillSummary.Add(new BillSummary { ServiceName = Common.Constants.ManualSection, BillAmount = ((list.ManullyAddedService != null ? list.ManullyAddedService.Sum(s => (s.Qty * s.BillRate)) : 0)) });

                    //Remove all services which are <=0 Amount
                    list.BillSummary.RemoveAll(r => r.BillAmount == 0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllgeneratedRequestBy_Id :", ex);
            }
            return list;
        }
        RequestSubmissionOPDModel_Report GetAllOPDGeneratedRequestBy_Id2(int requestId, int configId)
        {
            _logger.LogInfo("GetAllOPDGeneratedRequestBy_Id2 Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            RequestSubmissionOPDModel_Report list = new RequestSubmissionOPDModel_Report();
            try
            {
                var _data = new OPDBillingRepository();
                string ConnectionString = _ConnectionString.getConnectionStringName();

                var taskRequestMaster = Task.Run(() =>
                {
                    return _data.GetOPDGeneratedRequestById(requestId, ConnectionString);
                });
                var taskRequestDetail = Task.Run(() =>
                {
                    return _data.GetOPDRequestDetailById2(requestId, configId, true, ConnectionString);
                });
                var taskManualServices = Task.Run(() =>
                {
                    return _data.GetRequestManuallyOPDAddedDetail(requestId, ConnectionString);
                });
                var taskDefaultSer = Task.Run(() =>
                {
                    return _data.GetDefaultOPDServicesDetail2(requestId, configId, true, ConnectionString);
                });

                Task.WaitAll(taskRequestMaster, taskRequestDetail, taskManualServices, taskDefaultSer);

                list = taskRequestMaster.Result;
                list.ManullyAddedService = taskManualServices.Result;
                list.ConsumeServices = taskRequestDetail.Result;
                list.DefaultServices = taskDefaultSer.Result;

                if (list != null)
                {
                    list.BillSummary = new List<BillSummary>();

                    var services = list.ConsumeServices.GroupBy(g => g.Name)
                        .Select(s => new { Name = s.Key, BillRate = s.Sum(t => (t.Qty * t.BillRate)) });

                    foreach (var service in services)
                    {
                        list.BillSummary.Add(new BillSummary { ServiceName = service.Name, BillAmount = service.BillRate });
                    }

                    list.BillSummary.Add(new BillSummary
                    {
                        ServiceName = Common.Constants.DefaultSection,
                        BillAmount = (list.DefaultServices?.Sum(s => (s.Qty * s.BillRate)) ?? 0)
                    });

                    list.BillSummary.Add(new BillSummary
                    {
                        ServiceName = Common.Constants.ManualSection,
                        BillAmount = (list.ManullyAddedService?.Sum(s => (s.Qty * s.BillRate)) ?? 0)
                    });

                    // Remove all services with 0 bill amount
                    list.BillSummary.RemoveAll(r => r.BillAmount == 0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllOPDGeneratedRequestBy_Id2 :", ex);
            }
            return list;
        }

        List<ViewBillsModel_Report> GetAllgeneratedRequestByDateWise(string Fromtime, string Totime)
        {

            _logger.LogInfo("GetAllgeneratedRequestByDateWise Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<ViewBillsModel_Report> list = new List<ViewBillsModel_Report>();
            try
            {
                string ConnectionString = _ConnectionString.getConnectionStringName();
                var _data = new HospitalFormsRepository();
                int Userid = Convert.ToInt32(Session["AppUserId"]);
                list = _data.GetAllgeneratedRequestByDateWise(Userid, Fromtime, Totime, ConnectionString);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllgeneratedRequestByDate :", ex);
            }

            return list;
        }

        List<ViewBillsModel_Report> GetAllgeneratedOPDRequestByDateWise(string Fromtime, string Totime)
        {
            _logger.LogInfo("GetAllgeneratedOPDRequestByDateWise Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<ViewBillsModel_Report> list = new List<ViewBillsModel_Report>();
            try
            {
                string ConnectionString = _ConnectionString.getConnectionStringName();
                var _data = new OPDBillingRepository();
                int Userid = Convert.ToInt32(Session["AppUserId"]);
                list = _data.GetAllgeneratedOPDRequestByDateWise(Userid, Fromtime, Totime, ConnectionString);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllgeneratedOPDRequestByDateWise :", ex);
            }

            return list;

        }

        List<ServiceWiseBillDetailsModel> GetAllServiceWiseBillDtls_Datewise(string Fromtime, string Totime, int ServiceId, int CategoryId, int BillTypeId)
        {
            var UserId = Convert.ToInt32(Session["AppUserId"]);
            _logger.LogInfo("GetAllServiceWiseBillDtls_Datewise Started for " + UserId + " at :" + DateTime.Now.ToLongTimeString());
            List<ServiceWiseBillDetailsModel> list = new List<ServiceWiseBillDetailsModel>();
            try
            {
                string ConnectionString = _ConnectionString.getConnectionStringName();
                var _data = new ServiceWiseBillDetailsRepository();

                list = _data.GetAllServiceWiseBillDtls_Datewise(Fromtime, Totime, ServiceId, ConnectionString, CategoryId, UserId, BillTypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAllServiceWiseBillDtls_Datewise :", ex);
            }
            return list;
        }
        List<BCertificateModel> Certificate = new List<BCertificateModel>();
        private void GetCertificateWise(int requestId)
        {
            //reportmodel model = new reportmodel();
            //model.strstartdate = strstartdate;
            Certificate = GetCertificate(requestId);
            ReportViewer1.LocalReport.DataSources.Clear();
            string reportPath = Server.MapPath("../Reports/HospitalForms/BCertificate.rdlc");
            //ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.ReportPath = reportPath;
            _rds = new ReportDataSource("DataSet1", Certificate);
            ReportViewer1.LocalReport.DataSources.Add(_rds);
            ReportViewer1.LocalReport.Refresh();
        }

        List<BCertificateModel> GetCertificate(int requestId)
        {
            _logger.LogInfo("GetClientWiseResultData Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            List<BCertificateModel> list = new List<BCertificateModel>();
            try
            {

                var _data = new CertificateRepository();


                list = _data.GetAllCertificate(requestId);


            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetAgentWiseResultById :" + ex);
            }
            return list;
        }


    }
}