using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftIms.Data.ViewModel
{
    public class FiscalYearViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Alias { get; set; }
        public int FiscalYearId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string ReportYear { get; set; }
        [Required]
        public int DisplayOrder { get; set; }
        [Required]
        public Nullable<System.DateTime> StartDate { get; set; }
        [Required]
        public Nullable<System.DateTime> EndDate { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
    public class FiscalYearListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string ReportYear { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
