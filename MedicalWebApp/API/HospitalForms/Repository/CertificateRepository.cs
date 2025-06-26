using CGHSBilling.API.HospitalForms.Intefaces;
using CGHSBilling.Areas.HospitalForms.Models;
using CGHSBilling.QueryCollection.Masters;
using CommonDataLayer.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CGHSBilling.API.HospitalForms.Repository
{
    public class CertificateRepository : ICertificateInterface
    {
        //
        // GET: /CertificateRepository/
        public List<BCertificateModel> GetAllDetail(int RequestId)
        {
            List<BCertificateModel> list = null;

            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("RequestId", RequestId, DbType.Int32));
             
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAllDetail, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new BCertificateModel
                    {
                        RequestId = row.Field<int>("RequestId"),
                        CompanyName = row.Field<string>("FileNo"),
                        Bill_Amount=row.Field<double>("BillAmount"),
                        AdmissionTime = row.Field<string>("AdmissionDate"),
                        DischargeTime=row.Field<string>("DischargeDate"),
                        InvBillRate=row.Field<double>("InvBill_Rate"),
                        PharmacyBillRate=row.Field<double>("PharmacyBill_Rate"),
                        ClientName=row.Field<string>("ClientName"),
                        PatientName=row.Field<string>("PatientName"),
                        LabName=row.Field<string>("LabName"),
                        RelationName = row.Field<string>("RelationName"),
                        Doctor1 = row.Field<string>("Doctor1"),
                        MedicalOfficer = row.Field<string>("MedicalOfficer"),
                        SufferingDate = row.Field<string>("Sufferingdate"),
                        Doctor2 = row.Field<string>("Doctor2"),
                        ChiefMedicalOfficer = row.Field<string>("ChiefMedicalOfficer"),
                        Place = row.Field<string>("Place"),
                        Place2 = row.Field<string>("Place2"),
                    }).ToList();
            }


            return list;
        }
        public int CreateCertificate(BCertificateModel model)
        {
            try
            {


                int iResult = 0;

                using (DBHelper dbHelper = new DBHelper())
                {
                    DBParameterCollection paramCollection = new DBParameterCollection();
                    paramCollection.Add(new DBParameter("RequestId", model.RequestId, DbType.Int32));
                    paramCollection.Add(new DBParameter("RelationName", model.RelationName, DbType.String));
                    paramCollection.Add(new DBParameter("LabName", model.LabName, DbType.String));
                    paramCollection.Add(new DBParameter("CompanyName", model.CompanyName, DbType.String));
                    paramCollection.Add(new DBParameter("Doctor1", model.Doctor1, DbType.String));
                    paramCollection.Add(new DBParameter("ClientName", model.ClientName1, DbType.String));
                    paramCollection.Add(new DBParameter("PatientName", model.PatientName, DbType.String));
                    paramCollection.Add(new DBParameter("AdmissionTime", model.AdmissionDateTime, DbType.DateTime));
                    paramCollection.Add(new DBParameter("DischargeTime", model.DischargeDateTime, DbType.DateTime));
                    paramCollection.Add(new DBParameter("SufferingTime", model.SufferingDate, DbType.String));
                    paramCollection.Add(new DBParameter("MedicalOfficer", model.MedicalOfficer, DbType.String));
                    paramCollection.Add(new DBParameter("ChiefMedicalOfficer", model.ChiefMedicalOfficer, DbType.String));
                    paramCollection.Add(new DBParameter("Doctor2", model.Doctor2, DbType.String));
                    paramCollection.Add(new DBParameter("InvBill_Rate", model.InvBillRate, DbType.Double));
                    paramCollection.Add(new DBParameter("PharmacyBill_Rate", model.PharmacyBillRate, DbType.Double));
                    paramCollection.Add(new DBParameter("BillAmount", model.Bill_Amount, DbType.Double));
                    paramCollection.Add(new DBParameter("Place", model.Place, DbType.String));
                    paramCollection.Add(new DBParameter("Place2", model.Place2, DbType.String));
   





                    iResult = dbHelper.ExecuteNonQuery(MasterQueries.SaveCertificate, paramCollection, CommandType.StoredProcedure);



                    return iResult;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<BCertificateModel> GetAllCertificate(int RequestId)
        {
            List<BCertificateModel> list = null;


            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    


                    DBParameterCollection paramCollection = new DBParameterCollection();
                    paramCollection.Add(new DBParameter("RequestId",RequestId, DbType.Int32));
                //    paramCollection.Add(new DBParameter("ClientId", UserId, DbType.Int32));
           
                    DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAllCertificate, paramCollection, CommandType.StoredProcedure);
                    list = dtPaper.AsEnumerable()
                        .Select(row => new BCertificateModel
                        {
                            PatientName = row.Field<string>("PatientName"),
                            RelationName = row.Field<string>("RelationName"),
                            CompanyName = row.Field<string>("CompanyName"),
                            ClientName = row.Field<string>("ClientName"),
                            AdmissionTime = row.Field<string>("AdmissionTime"),
                            DischargeTime = row.Field<string>("DischargeTime"),
                            Bill_Amount = row.Field<double>("BillAmount"),
                            InvBillRate = row.Field<double>("InvBill_Rate"),
                            PharmacyBillRate= row.Field<double>("PharmacyBill_Rate"),
                            Doctor1 = row.Field<string>("Doctor1"),
                            MedicalOfficer = row.Field<string>("MedicalOfficer"),
                            SufferingDate= row.Field<string>("SufferingDate"),
                            LabName=row.Field<string>("LabName"),
                            Doctor2=row.Field<string>("Doctor2"),
                            ChiefMedicalOfficer = row.Field<string>("ChiefMedicalOfficer"),
                            Place = row.Field<string>("Place"),
                            Place2 = row.Field<string>("place2"),
                        }).ToList();
                }



            }
            catch (Exception ex)
            {

            }
            return list;
        }

    }
}
