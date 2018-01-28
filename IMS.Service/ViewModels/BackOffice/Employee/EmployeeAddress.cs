using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IMS.Data.NTAEnum;

namespace IMS.Logic.ViewModels.BackOffice.Employee
{
    public class EmployeeAddressViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public eAddressType AddressType { get; set; }

        [Display(Name = "Zone")]
        public int? ZoneId { get; set; }

        [LocalizedDisplayName("Employee.DistrictId", DefaultText = "District")]
        public int? DistrictId { get; set; }

        [LocalizedDisplayName("Employee.VdcId", DefaultText = "Vdc")]
        public int? VdcId { get; set; }

        [LocalizedDisplayName("Employee.WardNo", DefaultText = "Ward no")]
        public string WardNo { get; set; }

        [LocalizedDisplayName("Employee.StreetAddress", DefaultText = "Street")]
        public string StreetAddress { get; set; }

        [LocalizedDisplayName("Employee.HouseNumber", DefaultText = "House no")]
        public string HouseNumber { get; set; }
    }
}
