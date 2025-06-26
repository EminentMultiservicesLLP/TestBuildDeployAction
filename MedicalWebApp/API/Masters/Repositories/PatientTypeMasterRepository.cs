using CGHSBilling.API.Masters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Models;
using CommonDataLayer.DataAccess;
using System.Data;
using CGHSBilling.QueryCollection.Masters;

namespace CGHSBilling.API.Masters.Repositories
{
    public class PatientTypeMasterRepository : IPatientTypeMasterRepository
    {
        public bool CheckDuplicateInsert(CheckDuplicateModel chkmodel)
        {
            bool IsDuplicate = false;
            object result;
            using (DBHelper dbHelper = new DBHelper())
            {
                result = dbHelper.ExecuteScalar("Select 1 from PatientTypeMaster where Code = '" + chkmodel.Code + "' OR PatientType='" + chkmodel.Name + "'", CommandType.Text);
                if (Convert.ToInt32(result) > 0)
                {
                    IsDuplicate = true;
                }
            }
            return IsDuplicate;
        }

        public bool CheckDuplicateUpdate(CheckDuplicateModel chkmodel)
        {

            bool IsDuplicate = false;
            object result;
            using (DBHelper dbHelper = new DBHelper())
            {
                result = dbHelper.ExecuteScalar("Select 1 from PatientTypeMaster where ( Code = '" + chkmodel.Code + "' OR PatientType='" + chkmodel.Name + "') AND PatientTypeId <> " + chkmodel.Id, CommandType.Text);
                if (Convert.ToInt32(result) > 0)
                {
                    IsDuplicate = true;
                }
            }
            return IsDuplicate;
        }

       

        public PatientTypeMasterModel SavePatientTypeMaster(PatientTypeMasterModel entity)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("PatientTypeId", entity.PatientTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("Code", entity.Code, DbType.String));
                paramCollection.Add(new DBParameter("PatientType", entity.PatientType, DbType.String));
                paramCollection.Add(new DBParameter("Sequence", entity.Sequence, DbType.Int32));
                paramCollection.Add(new DBParameter("Deactive", entity.Deactive, DbType.Boolean));
                paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
                paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
                dbHelper.ExecuteNonQuery(MasterQueries.SavePatientTypeMaster, paramCollection, CommandType.StoredProcedure);
            }
            return entity;
        }
        public List<PatientTypeMasterModel> GetAllPatientTypeMaster()
        {
            List<PatientTypeMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable("Select PatientTypeId,Code,PatientType,isnull(Sequence,0)Sequence,Deactive from PatientTypeMaster", CommandType.Text);
                list = dtPaper.AsEnumerable()
                    .Select(row => new PatientTypeMasterModel
                    {
                        PatientTypeId = row.Field<int>("PatientTypeId"),
                        PatientType = row.Field<string>("PatientType"),
                        Code = row.Field<string>("Code"),
                        Sequence = row.Field<int>("Sequence"),
                        Deactive = row.Field<bool>("Deactive")
                    }).ToList();
            }
            return list;
        }
    }
}