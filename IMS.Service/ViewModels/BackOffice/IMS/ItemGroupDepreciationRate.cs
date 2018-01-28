using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class ItemGroupDepreciationRateViewModel : ServiceModel
    {
        public int Id { get; set; }

        [TRequired(ErrorMessageResourceName = "ItemGroupDepreciationRate.Required.FiscalYear", DefaultText = "Fiscal Year is required.")]
        [LocalizedDisplayName("FiscalYear", DefaultText = "Fiscal Year")]
        public int FiscalYearId { get; set; }
        public string FiscalYearName { get; set; }
        public int ItemGroupId { get; set; }
        public string ItemGroupName { get; set; }

        [LocalizedDisplayName("DepreciationRate", DefaultText = "Rate")]
        public decimal DepreciationRate { get; set; }
    }
}
