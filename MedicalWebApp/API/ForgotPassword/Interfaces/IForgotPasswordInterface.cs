using CGHSBilling.Models;
using CommonDataLayer.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGHSBilling.API.ForgotPassword.Interfaces
{
    public interface IForgotPasswordInterface   
    {
        //int SaveForgotPassword(ForgotPasswordModel model);
        string UpdatePassword(ForgotPasswordModel model);
    }
}
