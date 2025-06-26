using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace CGHSBilling.Areas.AdminPanel.Models
{
    public class UserAccessModel
    {
         public UserAccessModel()
        {
            this.Parent = new List<ParentMenuRights>();
            this.Child = new List<ChildMenuRights>();
           
        }
        public int MenuId { get; set; }
        public List<Menu> Menus { get; set; }
        public int RoleId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public bool Access { get; set; }
        public bool State { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool DeletePerm { get; set; }
        public bool SuperPerm { get; set; }
        public string MenuName { get; set; }
        public string RoleName { get; set; }
        public string PageName { get; set; }
        public int ParentMenuId { get; set; }
        public int SubParentMenuId { get; set; }
        public string UpdatedMacName { get; set; }
        public string UpdatedMacID { get; set; }
        public string UpdatedIPAddress { get; set; }
        public Nullable<int> UpdatedByUserID { get; set; }
        public Nullable<System.DateTime> UpdatedON { get; set; }
        public Nullable<int> InsertedBy { get; set; }
        public Nullable<System.DateTime> InsertedON { get; set; }
        public string InsertedMacName { get; set; }
        public string InsertedMacID { get; set; }
        public string InsertedIPAddress { get; set; }
        public string Message { get; set; }

        public List<ParentMenuRights> Parent { get; set; }
        public List<ChildMenuRights> Child { get; set; }
        
    }

    public class ParentMenuRights
    {
        public int MenuId { get; set; }
        public long UserId { get; set; }
        public bool Access { get; set; }
        public string MenuName { get; set; }
        public string PageName { get; set; }
        public int ParentMenuId { get; set; }
        public bool State { get; set; }
    }

    public class ChildMenuRights
    {
        public int MenuId { get; set; }
        public long UserId { get; set; }
        public bool Access { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }      
        public bool DeletePerm { get; set; }
        public bool SuperPerm { get; set; }
        public string MenuName { get; set; }
        public string PageName { get; set; }
        public int ParentMenuId { get; set; }
    }
  
}
