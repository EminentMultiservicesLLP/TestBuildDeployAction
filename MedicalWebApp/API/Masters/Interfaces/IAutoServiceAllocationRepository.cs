using CGHSBilling.Areas.Masters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGHSBilling.API.Masters.Interfaces
{
  public interface IAutoServiceAllocationRepository
    {
        AutoServiceAllocationModel SaveAutoServiceAllocation(AutoServiceAllocationModel jsonData);
        AutoServiceAllocationModel UpdateAutoServiceAllocation(AutoServiceAllocationModel jsonData);
        List<AutoServiceAllocationModel> GetAutoServiceAllocation();
        List<CommonMasterModel> GetDaysDropdwon();
        List<AutoServiceAllocationDtlModel> GetAutoServiceAllocationDetailById(int AutoAllocationId, int ServiceTypeId);
        List<AutoServiceAllocationDtlModel> GetLinkedServicesByServiceType_ServiceId(int AutoAllocationId, int ServiceTypeId, int ServiceId);
        List<AutoServiceAllocationDtlModel> GetLinkedServicesByServiceTypeServiceId(int ServiceId, int hospitalType, int patientType, int stateId, int cityId, int gender);
    }
}
