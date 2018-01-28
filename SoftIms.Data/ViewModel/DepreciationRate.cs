using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SoftIms.Data.ViewModel
{
    public class DepreciationRateViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name ="Fiscal Year")]
        [Remote("DepreciationRateNotExist", "Master", HttpMethod = "Post", AdditionalFields = "ItemGroupId,Id", ErrorMessage = "{0} already exists.")]
        public int FiscalYearId { get; set; }
        public int ItemGroupId { get; set; }
        [Required]
        [Range(1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public decimal Rate { get; set; }
    }
    public class DepreciationRateListViewModel
    {
        public int Id { get; set; }
        public string FiscalYear { get; set; }
        public string ItemGroupName { get; set; }
        public decimal Rate { get; set; }
    }
}
