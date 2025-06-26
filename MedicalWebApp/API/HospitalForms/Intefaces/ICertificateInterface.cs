using CGHSBilling.Areas.HospitalForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CGHSBilling.API.HospitalForms.Intefaces
{
    public interface ICertificateInterface 
    {
        List<BCertificateModel> GetAllDetail(int RequestId);
        int CreateCertificate(BCertificateModel model);

    }
}
