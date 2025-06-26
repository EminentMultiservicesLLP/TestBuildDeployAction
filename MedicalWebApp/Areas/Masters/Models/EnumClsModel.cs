using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.Masters.Models
{
    public class EnumClsModel
    {
    }
    enum HospitalType
    {
        NABH = 1,
        NON_NABH = 2
    }

    enum CalculationDropDown
    {
        IncreasedBy=1,
        DecreasedBy=2,
        Equal=3
    }
    enum Increasedtype
    {
        Percent = 1,
        Amount = 2
    }

    enum Gender
    {
        Male=1,
        Female=2,
        Other=3
    }
    
}