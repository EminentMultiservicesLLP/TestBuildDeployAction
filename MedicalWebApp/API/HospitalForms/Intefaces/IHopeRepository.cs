using CGHSBilling.Areas.AdminPanel.Models;
using CGHSBilling.Areas.HospitalForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.API.HospitalForms.Intefaces
{
    public interface IHopeRepository
    {
        List<PatientModel> GetHopePatients(string ConnectionString);
        ClientMasterModel GetClientConfiguration( int UserId);
    }
}