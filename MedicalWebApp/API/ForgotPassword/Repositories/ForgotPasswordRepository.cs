using CGHSBilling.API.ForgotPassword.Interfaces;
using CGHSBilling.Models;
using CGHSBilling.QueryCollection.AdminPanel;
using CommonDataLayer.DataAccess;
using CommonLayer.EncryptDecrypt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CGHSBilling.API.ForgotPassword.Repositories
{
    public class ForgotPasswordRepository : IForgotPasswordInterface
    {
        //public int SaveForgotPassword(ForgotPasswordModel Model)
        //{
        //    int iResult = 0;
        //    using (DBHelper dbhelper = new DBHelper())
        //    {
        //        DBParameterCollection paramcollection = new DBParameterCollection();
        //        paramcollection.Add(new DBParameter("Username", Model.Username, DbType.String));
        //        paramcollection.Add(new DBParameter("NewPassword", EncryptDecryptDES.EncryptString(Model.NewPassword), DbType.String));
        //        paramcollection.Add(new DBParameter("ConfirmPassword", Model.ConfirmPassword, DbType.String));
        //        iResult = dbhelper.ExecuteNonQuery(AdminPanelQueries.SaveForgotPassword, paramcollection, CommandType.StoredProcedure);
        //    }
        //    return iResult;

        //}

        public string UpdatePassword(ForgotPasswordModel model)
        {
            string email  ="";
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramcollection = new DBParameterCollection();              
                paramcollection.Add(new DBParameter("LoginName", model.UserName, DbType.String));
                paramcollection.Add(new DBParameter("Password", model.Password, DbType.String));
                var result = dbHelper.ExecuteNonQuery(AdminPanelQueries.UpdatePassword, paramcollection, CommandType.StoredProcedure);
                
                object obj =
                   dbHelper.ExecuteScalar("select EmailID from UM_MST_User where LoginName=" +
                                          "'" + model.UserName + "'");
                 
                email = obj.ToString(); 
            }
            return email;
        }
    }
}