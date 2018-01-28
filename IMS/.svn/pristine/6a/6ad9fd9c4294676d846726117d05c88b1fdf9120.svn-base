using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SoftIms.Data.ViewModel
{
    public class CompanyViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(400, MinimumLength = 1)]
        [Display(Name = "Name of Company")]
        public string Name { get; set; }
        [StringLength(400, MinimumLength = 1)]
        public string SubTitle1 { get; set; }
        [StringLength(400, MinimumLength = 1)]
        public string SubTitle2 { get; set; }
        [StringLength(400, MinimumLength = 1)]
        public string SubTitle3 { get; set; }
        [StringLength(400, MinimumLength = 1)]
        public string SubTitle4 { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage ="{0} format not matched.")]
        public string EmailAddress { get; set; }
        public string Address { get; set; }
    }
    public class CompanyListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SubTitle1 { get; set; }
        public string SubTitle2 { get; set; }
        public string SubTitle3 { get; set; }
        public string SubTitle4 { get; set; }
        public string Phone { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
    }
}
