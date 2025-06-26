using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGHSBilling.Areas.Masters.Models;

namespace CGHSBilling.API.Masters.Interfaces
{
    public interface ITariffMasterRepository
    {
        TariffMasterModel SaveTariff(TariffMasterModel jsonData);
        TariffMasterModel UpdateTariffMaster(TariffMasterModel jsonData);
        List<TariffMasterModel> GetTariffMaster();
        List<TariffDetailModel> GetTariffDetailById(int tariffMasterId);
        List<TariffDetailModel> GetTariffMasterforCopy(int stateId, int cityId, int patientType, int roomType);
        List<TariffDetailModel> GetTariff_forCopy(int stateId, int cityId, int patientType, int roomType);
        List<TariffDetailModel> GetTariffDetails_forCopy(string TariffMasterIds);
    }
}
