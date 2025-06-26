using CommonLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CommonLayer.Extensions;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using CGHSBilling.API.ScanDoc;
using CGHSBilling.QueryCollection.AdminPanel;
using CGHSBilling.QueryCollection.HospitalForms;
using CommonDataLayer.DataAccess;

namespace CGHSBilling.Controllers
{
    public class FileUploadDownloadController : Controller
    {
        HttpClient client;
        static ILogger _logger = Logger.Register(typeof(FileUploadDownloadController));

        public FileUploadDownloadController()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
        }


        [HttpPost]
        public JsonResult Upload(int NewId, string AreaLocation, string SubAreaLocation)
        {
            string saveAgainstId = ""; //string extraFolderName = "";            
            saveAgainstId = NewId.ToString();
            if (HttpContext.Request.Files.AllKeys.Any())
            {
                for (int i = 0; i <= HttpContext.Request.Files.Count; i++)
                {
                    TryCatch.Run(() =>
                    {
                        var file = HttpContext.Request.Files["files" + i];
                        if (file != null)
                        {
                            _logger.LogInfo("   (((((Upload => Starting file upload for speacialId :" + saveAgainstId + ", AreaLocation:" + AreaLocation);
                            string ScanDocLocation = AreaLocation.ToString()
                                                                    + (string.IsNullOrWhiteSpace(SubAreaLocation) == false ? "\\" + SubAreaLocation : "")
                                                                    + (string.IsNullOrWhiteSpace(saveAgainstId) == false ? "\\" + saveAgainstId : "");

                            var fileSavePath = Path.Combine(Server.MapPath("~/Content/UploadFile/") + ScanDocLocation);
                            if (!Directory.Exists(fileSavePath))
                            {
                                Directory.CreateDirectory(fileSavePath);
                            }
                            fileSavePath = fileSavePath + "\\" + file.FileName;
                            file.SaveAs(fileSavePath);
                            _logger.LogInfo("   File uploaded successfully for speacialId :" + saveAgainstId + ", AreaLocation:" + AreaLocation + " <= Upload)))))");

                            _logger.LogInfo("   (((((File upload DatabaseUpdate for speacialId :" + saveAgainstId + ", AreaLocation:" + AreaLocation);
                            UpdateDatabase(AreaLocation, SubAreaLocation, saveAgainstId, ScanDocLocation + "\\" + file.FileName, file.FileName);
                            _logger.LogInfo("   Successful File upload DatabaseUpdate for speacialId :" + saveAgainstId + ", AreaLocation:" + AreaLocation + "  <= File upload DatabaseUpdate)))))");
                        }
                    }).IfNotNull(ex =>
                    {
                        _logger.LogError("Error :- Issue while loading File, specialId :" + saveAgainstId + ", AreaLocation:" + AreaLocation);
                    });
                }
            }
            return Json(new { success = true });
        }
        public ActionResult Download()
        {
            string[] files = Directory.GetFiles(Server.MapPath("/Files"));
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            ViewBag.Files = files;
            return View();
        }

        public FileResult DownloadFile(string fileName)
        {
            var filepath = System.IO.Path.Combine(Server.MapPath("/Files/"), fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
        }


        private void UpdateDatabase(string AreaLocation, string SubAreaLocation, string againstId, string filePath, string FileName)
        {
            int InsertedBy = Convert.ToInt32(Session["AppUserId"]);
            int agnstId = Convert.ToInt32(againstId);
            TryCatch.Run(() =>
                {
                using (DBHelper dbHelper = new DBHelper())
                {
                    DBParameterCollection paramCollection = new DBParameterCollection();
                    paramCollection.Add(new DBParameter("AreaLocation", AreaLocation, DbType.String));
                    paramCollection.Add(new DBParameter("SubAreaLocation", SubAreaLocation, DbType.String));
                    paramCollection.Add(new DBParameter("againstId", agnstId, DbType.Int32));
                    paramCollection.Add(new DBParameter("filePath", filePath, DbType.String));
                    paramCollection.Add(new DBParameter("FileName", FileName, DbType.String));
                    paramCollection.Add(new DBParameter("InsertedBy", InsertedBy, DbType.Int32));
                        dbHelper.ExecuteNonQuery(AdminPanelQueries.CreateScandoc, paramCollection, CommandType.StoredProcedure);
                 }
            }).IfNotNull(ex =>
            {
                _logger.LogError("Error :- Issue while UpdateDatabase File, specialId :" + againstId + ", AreaLocation:" + AreaLocation);
            });
        }

        
    }
}
