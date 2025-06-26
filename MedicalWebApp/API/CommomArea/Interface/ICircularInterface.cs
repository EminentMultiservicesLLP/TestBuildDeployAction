using CGHSBilling.Areas.CommonArea.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGHSBilling.API.CommomArea.Interface
{
    public interface ICircularInterface
    {
        List<CircularModel> GetCircularDetails();
        List<CircularModel> GetCircularDownloadFileDetails(int CircularID);
    }
}
