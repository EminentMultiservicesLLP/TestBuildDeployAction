using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Models;

namespace CGHSBilling.API.Masters.Interfaces
{
   public interface IHintNotificationRepository
    {
        bool CreateNotification(HintNotificationModel jsonData);
        bool UpdateNotification(HintNotificationModel jsonData);
        List<HintNotificationModel> GetAllNotification();
        HintNotificationModel GetAllNotificationById(string controlId);
    }
}
