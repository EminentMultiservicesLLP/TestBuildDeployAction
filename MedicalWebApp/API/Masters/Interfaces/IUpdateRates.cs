using CGHSBilling.Areas.Masters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGHSBilling.API.Masters.Interfaces
{
    public interface IUpdateRates
    {
        List<UpdateRatesModel> GetRates(int StateId, int CityId);
        List<UpdateRatesModel> GetServiceTariff(int? ServiceId);
        int UpdateRates(List<UpdateRatesModel> UpdateRates);
    }
}
