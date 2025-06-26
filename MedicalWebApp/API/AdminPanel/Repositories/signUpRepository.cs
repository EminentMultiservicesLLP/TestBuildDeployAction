using CGHSBilling.API.AdminPanel.Interfaces;
using CGHSBilling.Models;
using CGHSBilling.QueryCollection.AdminPanel;
using CGHSBilling.QueryCollection.Masters;
using CommonDataLayer.DataAccess;
using CommonLayer.EncryptDecrypt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CGHSBilling.API.AdminPanel.Repositories
{
    public class signUpRepository : IsignUpInterface
    {
        public int SaveSignUpDetails(SignUpModel model)
        {
            int iResult = 0;
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("Name", model.SignUpContactName, DbType.String));
                paramCollection.Add(new DBParameter("NameEncrypt", model.NameEncrypt, DbType.String));
                paramCollection.Add(new DBParameter("HospitalName", model.SignUpHospitalName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedON", model.InsertedON, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedIpAddress", model.InsertedIpAddress, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacName", model.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacId", model.InsertedMacId, DbType.String));
                paramCollection.Add(new DBParameter("EmailID", model.SignUpemail, DbType.String));
                iResult = dbHelper.ExecuteNonQuery(AdminPanelQueries.SaveSignUpDetails, paramCollection, CommandType.StoredProcedure);
            }
                return iResult;
            }

        public bool CheckDuplicateItem(string SignUpContactName, string SignUpHospitalName, string SignUpemail, int typeid, string type)
        {
            bool bResult = false;
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("Type", type, DbType.String));
                paramCollection.Add(new DBParameter("SignUpContactName", SignUpContactName, DbType.String));
                paramCollection.Add(new DBParameter("SignUpHospitalName", SignUpHospitalName, DbType.String));
                paramCollection.Add(new DBParameter("EmailID", SignUpemail, DbType.String));
                paramCollection.Add(new DBParameter("ID", typeid, DbType.Int32));
                paramCollection.Add(new DBParameter("IsExist", true, DbType.Boolean, ParameterDirection.Output));

                bResult = dbHelper.ExecuteNonQueryForOutParameter<bool>(MasterQueries.CheckDuplicateItem, paramCollection, CommandType.StoredProcedure, "IsExist");
            }
            return bResult;
        }


        public int SaveResetPassword(ResetPasswordModel model)
        {
            int iResult = 0;
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("UserId", model.UserId, DbType.Int32));
                paramCollection.Add(new DBParameter("ConfirmPassword", EncryptDecryptDES.EncryptString(model.ConfirmPassword), DbType.String));
                iResult = dbHelper.ExecuteNonQuery(AdminPanelQueries.SaveResetPassword, paramCollection, CommandType.StoredProcedure);
            }
            return iResult;
        }



    }
}