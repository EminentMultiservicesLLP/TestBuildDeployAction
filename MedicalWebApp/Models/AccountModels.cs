using CommonLayer.EncryptDecrypt;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Security;
using System.Web;
using CGHSBilling.Common;

namespace CGHSBilling.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        [Required(ErrorMessage = "Please Provide Username", AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please provide password", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

    
        public int LoginFor { get; set; }

        /// <summary>
        /// Checks if user with given password exists in the database
        /// </summary>
        /// <param name="_username">User name</param>
        /// <param name="_password">User password</param>
        /// <returns>True if user exist and password is correct</returns>
        public int GetUserId(string _username, string _password, out bool IsAdmin)
        {
            int UserId = 0;
            IsAdmin = false;
            string dbToBeSelected ="";


            //if (HttpContext.Current.Session["DatabaseSeLection"] != null)
            //{
            //   if(Convert.ToString(HttpContext.Current.Session["DatabaseSeLection"]) == "Mumbai") dbToBeSelected = ConnectionString;
            //   else dbToBeSelected = CGHSDelhiConnectionString;

            //}
            ConnectionString _ConnectionString = new ConnectionString();
            dbToBeSelected = _ConnectionString.getFullConnectionString(); 
            using (var cn = new SqlConnection(dbToBeSelected))
            {
                _password = EncryptDecryptDES.EncryptString(_password);
                string _sql = @"SELECT  userid, userCode, UserName, LoginName, ISNULL(IsAdmin,0) IsAdmin FROM [dbo].[Um_Mst_User] WHERE [LoginName] = @u And [Password] = @p and isnull(isdeactive,0)=0";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = _username;
                cmd.Parameters
                    .Add(new SqlParameter("@p", SqlDbType.NVarChar))
                    .Value = _password;
                //cmd.Parameters
                //    .Add(new SqlParameter("@p", SqlDbType.NVarChar))
                //    .Value = Helpers.SHA1.Encode(_password);
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        UserId = Convert.ToInt32(reader["UserID"].ToString());
                        IsAdmin = Convert.ToBoolean(reader["IsAdmin"]);
                        break;
                    }
                    reader.Dispose();
                    cmd.Dispose();
                    return UserId;
                }
                else
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return UserId;
                }
            }
            //return UserId;
        }

        //public static string ConnectionString
        //{
        //    get
        //    {
        //        // return "Server=192.168.2.212;Database=BISNowERP;User Id=sa;Password=optimal$2009;";
        //        return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //    }
        //}

        //public static string CGHSDelhiConnectionString
        //{

        //    get
        //    {
        //        return ConfigurationManager.ConnectionStrings["CghsDelhi"].ConnectionString;
        //    }
        //}
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
    public class EnquiryLogin
    {
        public string Name { get; set; }
        public string HospitalName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Message { get; set; }
    }
    public class SignUpModel
    {
        public string SignUpContactName { get; set; }
        public string SignUpHospitalName { get; set; }
        public string SignUpContactTitle { get; set; }
        public string SignUpemail { get; set; }
        public string SignUpmobile { get; set; }
        public string NameEncrypt { get; set; }
        public int LoginFor { get; set; }
        public DateTime Date { get; set; }
        public DateTime? InsertedON { get; set; }
        public string InsertedMacName { get; set; }
        public string InsertedMacId { get; set; }
        public string InsertedIpAddress { get; set; }
    }
    public class ResetPasswordModel
    {
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public int UserId { get; set; }
    }
}
