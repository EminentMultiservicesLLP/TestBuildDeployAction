using CGHSBilling.API.CommomArea.Interface;
using CGHSBilling.API.CommomArea.Repository;
using CGHSBilling.Areas.CommonArea.Models;
using CGHSBilling.Models;
using CommonLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CGHSBilling.Areas.CommonArea.Controllers
{
    public class RoomEntitlementController : Controller
    {
        //
        // GET: /CommonArea/RoomEntitlement/

        IRoomEntitlement _roomEntitlement;
        private static readonly ILogger _loggger = Logger.Register(typeof(RoomEntitlementController));

        public RoomEntitlementController(IRoomEntitlement roomEntitlement)
        {
            _roomEntitlement = roomEntitlement;
        }
        public RoomEntitlementController()
        {
            _roomEntitlement = new RoomEntitlementRepository();
        }


        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetRoomType(RoomEntitlement model)
        {
            JsonResult jResult;
            try
            {
                var list = _roomEntitlement.GetRoomType(model);
                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                _loggger.LogError("Error in RoomEntitlementRepository GetRoomType  :" + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                return Json("Error");
            }
        }
        [HttpPost]
        public ActionResult SaveMarqueeMessage(MenuUserRightsModel model)
        {
            JsonResult jResult; bool success = false;
            try
            {
                var Save = _roomEntitlement.SaveMarqueeMessage(model);
                jResult = Json(new { Message = "Message Saved successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _loggger.LogError("Error in UserProfileController SaveMarqueeMessage:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jResult;
        }

    }
}
