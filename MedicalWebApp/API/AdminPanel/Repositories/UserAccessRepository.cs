using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CGHSBilling.API.AdminPanel.Interfaces;
using CGHSBilling.Models;
using CGHSBilling.QueryCollection.AdminPanel;
using CommonDataLayer.DataAccess;
using CommonLayer;
using CommonLayer.Extensions;
namespace CGHSBilling.API.AdminPanel.Repositories
{
    public class UserAccessRepository : IUserAccessInterface
    {
        private static readonly ILogger Loggger = Logger.Register(typeof(UserAccessRepository));
        public override List<ParentMenuRights> GetMenuByUser(int UserId,string ConnectionString)
        {
            List<ParentMenuRights> Access = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("UserId", UserId, DbType.String));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtTable = dbHelper.ExecuteDataTable(AdminPanelQueries.GetMenuByUser, paramCollection, CommandType.StoredProcedure);
                Access = dtTable.AsEnumerable()
                    .Select(row => new ParentMenuRights
                    {
                        MenuId = row.Field<int>("MenuId"),
                        MenuName = row.Field<string>("MenuName"),
                        PageName = row.Field<string>("PageName"),
                        ParentMenuId = row.Field<int>("ParentMenuId"),
                        State = Convert.ToBoolean(row.Field<int?>("State"))

                    }).ToList();
            }
            return Access;

        }
        //public override List<SubjectMasterModel> GetSubAccessData(int UserId)
        //{
        //    List<SubjectMasterModel> Access = null;
        //    DBParameterCollection paramCollection = new DBParameterCollection();
        //    paramCollection.Add(new DBParameter("UserId", UserId, DbType.String));
        //    using (DBHelper dbHelper = new DBHelper())
        //    {
        //        DataTable dtTable = dbHelper.ExecuteDataTable(AdminPanelQueries.GetStdSubjectAccess, paramCollection, CommandType.StoredProcedure);
        //        Access = dtTable.AsEnumerable()
        //            .Select(row => new SubjectMasterModel
        //            {
        //                StandardId = row.Field<int>("StandardId"),
        //                SubjectId = row.Field<int>("SubjectId"),
        //                SubjectName = row.Field<string>("SubjectName"),
        //                StandardName = row.Field<string>("StandardName"),
        //                State = Convert.ToBoolean(row.Field<int?>("State"))

        //            }).ToList();
        //    }
        //    return Access;

        //}
        public override int SaveUserAccess(MenuUserRightsModel accessData,string ConnectionString)
        {
            int iResult = 0;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                IDbTransaction transaction = dbHelper.BeginTransaction();
                 TryCatch.Run(() =>
                {
                if (accessData.UserId > 0)
                { dbHelper.ExecuteNonQuery("DELETE FROM um_menurights WHERE UserId = " + accessData.UserId); }
                foreach (ParentMenuRights detail in accessData.parent)
                {
                    detail.UserId = accessData.UserId;
                    detail.InsertedBy = accessData.InsertedBy;
                    detail.InsertedON = accessData.InsertedON;
                    detail.InsertedIPAddress = accessData.InsertedIPAddress;
                    detail.InsertedMacName = accessData.InsertedMacName;
                    detail.InsertedMacID = accessData.InsertedMacID;
                    SaveUserAccessItem(detail, dbHelper);
                    object tempDetail = dbHelper.ExecuteScalar("select parentmenuid from um_mst_menu where menuid = " + detail.MenuId);
                    if (Convert.ToInt32(tempDetail) > 0)
                    {
                        detail.MenuId = Convert.ToInt32(tempDetail);
                        SaveUserAccessItem(detail, dbHelper);
                    }
                }
                //if (accessData.SubStandardData.Count>0) {
                //    dbHelper.ExecuteNonQuery("DELETE FROM UM_User_StdSubjectAccess WHERE UserId = " + accessData.UserId);
                //    foreach (var SubStdAccessData in accessData.SubStandardData)
                //    {
                //        SubStdAccessData.UserId = accessData.UserId;
                //        SubStdAccessData.InsertedBy = accessData.InsertedBy;
                //        SubStdAccessData.InsertedOn = accessData.InsertedON;
                //        SubStdAccessData.InsertedIpAddress = accessData.InsertedIPAddress;
                //        SubStdAccessData.InsertedMacName = accessData.InsertedMacName;
                //        SubStdAccessData.InsertedMacId = accessData.InsertedMacID;
                //        //SaveSubStandardAccess(SubStdAccessData, dbHelper);
                //    }
                //}
                dbHelper.CommitTransaction(transaction);
                }).IfNotNull(ex =>
                {
                    dbHelper.RollbackTransaction(transaction);
                    Loggger.LogError("Error in Save Access method ofUser Access Repository : parameter :" + Environment.NewLine + ex.StackTrace);
                });
            }
            return iResult;
        }

        //public override int SaveAcademicAccess(List<SubjectMasterModel> AccessData)
        //{
        //    throw new NotImplementedException();
        //}

        public ParentMenuRights SaveUserAccessItem(ParentMenuRights detail, DBHelper dbHelper)
        {
            int iResult = 0;

            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("MenuId", detail.MenuId, DbType.Int32));
            paramCollection.Add(new DBParameter("UserId", detail.UserId, DbType.Int32));
            paramCollection.Add(new DBParameter("InsertedBy", detail.InsertedBy, DbType.Int32));
            paramCollection.Add(new DBParameter("InsertedON", detail.InsertedON, DbType.DateTime));
            paramCollection.Add(new DBParameter("InsertedIPAddress", detail.InsertedIPAddress, DbType.String));
            paramCollection.Add(new DBParameter("InsertedMacName", detail.InsertedMacName, DbType.String));
            paramCollection.Add(new DBParameter("InsertedMacID", detail.InsertedMacID, DbType.String));
            dbHelper.ExecuteNonQuery(AdminPanelQueries.SaveUserAccess, paramCollection, CommandType.StoredProcedure);
            return detail;
        }

        //public SubjectMasterModel SaveSubStandardAccess(SubjectMasterModel detail, DBHelper dbHelper)
        //{
        //    int iResult = 0;

        //    DBParameterCollection paramCollection = new DBParameterCollection();
        //    paramCollection.Add(new DBParameter("SubjectId", detail.SubjectId, DbType.Int32));
        //    paramCollection.Add(new DBParameter("StandardId", detail.StandardId, DbType.Int32));
        //    paramCollection.Add(new DBParameter("UserId", detail.UserId, DbType.Int32));
        //    paramCollection.Add(new DBParameter("InsertedBy", detail.InsertedBy, DbType.Int32));
        //    paramCollection.Add(new DBParameter("InsertedON", detail.InsertedOn, DbType.DateTime));
        //    paramCollection.Add(new DBParameter("InsertedIPAddress", detail.InsertedIpAddress, DbType.String));
        //    paramCollection.Add(new DBParameter("InsertedMacName", detail.InsertedMacName, DbType.String));
        //    paramCollection.Add(new DBParameter("InsertedMacID", detail.InsertedMacId, DbType.String));
        //    dbHelper.ExecuteNonQuery(AdminPanelQueries.SaveSubStandardAccess, paramCollection, CommandType.StoredProcedure);
        //    return detail;
        //}

        

        //public override IEnumerable<SubjectMasterModel> GetAllStdSub()
        //{
        //    List<SubjectMasterModel> list = null;
        //    using (DBHelper dbHelper = new DBHelper())
        //    {
        //        DataTable dtStores = dbHelper.ExecuteDataTable(AdminPanelQueries.GetAllStdSub, CommandType.StoredProcedure);
        //        list = dtStores.AsEnumerable()
        //                    .Select(row => new SubjectMasterModel
        //                    {
        //                        StandardId = row.Field<int>("StandardId"),
        //                        StandardName = row.Field<string>("StandardName"),
        //                        SubjectId = row.Field<int>("SubjectId"),
        //                        SubjectName = row.Field<string>("SubjectName")
        //                    }).ToList();
        //    }
        //    return list;
        //}
    }
}