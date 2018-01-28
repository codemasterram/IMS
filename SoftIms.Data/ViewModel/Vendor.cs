using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftIms.Data.ViewModel
{
    public class VendorViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Vendor Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Vat | Pan Number")]
        public string VatPanNo { get; set; }
        [Required]
        public string Address { get; set; }
        public string City { get; set; }
        [Display(Name = "Phone Number")]
        [Required]
        public string PhoneNo { get; set; }
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Not a valid email address.")]
        public string EmailId { get; set; }
        [Display(Name = "Web Site Name")]
        public string WebSite { get; set; }
        [Display(Name = "Contact Person Name")]
        public string ContactPerson { get; set; }
        [StringLength(400, MinimumLength = 1)]
        public string Remarks { get; set; }
        public int DisplayOrder { get; set; } = 1;
    }
    public class VendorListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VatPanNo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PhoneNo { get; set; }
        public string EmailId { get; set; }
        public string WebSite { get; set; }
        public string ContactPerson { get; set; }
        public string Remarks { get; set; }
        public int DisplayOrder { get; set; }
    }
}
