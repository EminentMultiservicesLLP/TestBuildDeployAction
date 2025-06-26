using CGHSBilling.Areas.CommonArea.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGHSBilling.API.AdminPanel.Interfaces
{
   public interface IBillingConfigurationRepository
    {
        bool SaveBillingConfiguration(BillConfigurationModel entity,string ConnectionString);
        List<BillConfigurationModel> GetAllBill(int ClientId);
        List<BillConfigurationDtlModel> GetAllDeductedBills(int ClientId);
    }
}
