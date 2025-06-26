using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGHSBilling.Areas.AdminPanel.Models;

namespace CGHSBilling.API.AdminPanel.Interfaces
{
   public interface IClientMasterInterface
    {
        int CreateClient(ClientMasterModel clientEntity,string ConnectionString);
        bool UpdateClient(ClientMasterModel clientEntity,string ConnectionString);
        IEnumerable<ClientMasterModel> GetAllClient();
        IEnumerable<ClientMasterModel> GetAllStates();
        IEnumerable<ClientMasterModel> GetHospitalServiceCategory();
        IEnumerable<ClientMasterModel> GetClientType();        
        IEnumerable<ClientMasterModel> GetCityById(int stateId);
        bool CheckDuplicateItem(string typecode, int typeid, string type,string ConnectionString);
        bool CheckDuplicateUpdate(int typeid, string typecode, int relativeid, string type,string CheckDuplicateUpdate);
        int SaveClientConfiguration(ClientConfiguration model, string ConnectionString);
        IEnumerable<ClientConfiguration> GetClientConfiguration(int? LoginId);
        IEnumerable<ClientConfigurationDetails> GetClientConfigDetails(int? ConfigId);
    }
}
