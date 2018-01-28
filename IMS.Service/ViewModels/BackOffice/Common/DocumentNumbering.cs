using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Logic.ViewModels.BackOffice.Common
{
    public class DocumentNumberingViewModel : ValidationModal
    {
        public int Id { get; set; }
        [TRequired(ErrorMessageResourceName = "Required.Document", DefaultText = "Please select document")]
        [LocalizedDisplayName("Document", DefaultText = "Document")]
        [Remote("DocumentNumberingNotExist", "Setup", HttpMethod = "Post", AdditionalFields = "Id", ErrorMessage = "{0} numbering already exists.")]
        public int DocumentSetupId { get; set; }
        [TRequired(ErrorMessageResourceName = "Required.FiscalYear", DefaultText = "Please select fiscal year")]
        [LocalizedDisplayName("Fiscalyear", DefaultText = "Fiscal year")]
        public int FiscalYearId { get; set; }
        [TRequired(ErrorMessageResourceName = "Required.StartNumber", DefaultText = "Please enter start number")]
        [LocalizedDisplayName("StartNumber", DefaultText = "Start number")]
        public int StartNumber { get; set; }
        [LocalizedDisplayName("Prefix", DefaultText = "Prefix")]
        public string Prefix { get; set; }
        [LocalizedDisplayName("Sufix", DefaultText = "Sufix")]
        public string Sufix { get; set; }
        [TRequired(ErrorMessageResourceName = "Required.Length", DefaultText = "Please enter length")]
        [LocalizedDisplayName("Length", DefaultText = "Length")]
        [Range(3, 9, ErrorMessage = "{0} must between {1} to {2}.")]
        public int Length { get; set; } = 5;
    }

    public class DocumentNumberingListViewModel
    {
        public int Id { get; set; }
        public string DocumentSetupName { get; set; }
        public string FiscalYearName { get; set; }
        public int StartNumber { get; set; }
        public string Prefix { get; set; }
        public string Sufix { get; set; }
        public int Length { get; set; }
    }
}
