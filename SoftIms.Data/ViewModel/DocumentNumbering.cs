using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftIms.Data.ViewModel
{
    public class DocumentNumberingViewModel : ServiceModel
    {
        public int Id { get; set; }
        public int DocumentSetupId { get; set; }
        public int FiscalYearId { get; set; }
        [Required]
        public int StartNumber { get; set; }
        [StringLength(10, MinimumLength = 1)]
        public string Prefix { get; set; }
        [StringLength(10, MinimumLength = 1)]
        public string Sufix { get; set; }
        [Required]
        public int Length { get; set; }

        public object ValidateModal(object errorList)
        {
            throw new NotImplementedException();
        }
    }
    public class DocumentNumberingListViewModel
    {
        public int Id { get; set; }
        public string DocumentSetupId { get; set; }
        public string DocumentSetupName { get; set; }
        public string FiscalYearId { get; set; }
        public int StartNumber { get; set; }
        public string Prefix { get; set; }
        public string Sufix { get; set; }
        public int Length { get; set; }
    }
}
