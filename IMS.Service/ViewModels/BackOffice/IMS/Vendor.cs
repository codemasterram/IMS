using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IMS.Logic.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class VendorViewModel : ServiceModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "बिक्रेताको नाम ")]
        public string Name { get; set; }

        [Display(Name = "भ्याट/प्यान नं.")]
        public string VatPanNo { get; set; }

		[Display(Name = "ठेगाना")]
		public string Address { get; set; }

        [LocalizedDisplayName("City", DefaultText = "City")]
        public string City { get; set; }

        [Display(Name = "फोन नं.")]
        public string PhoneNo { get; set; }

		[Display(Name = "मोबाईल नं.")]
		public string MobileNo { get; set; }

		[Display(Name = "ईमेल")]
		[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Not a valid email address.")]
        public string EmailId { get; set; }

        [LocalizedDisplayName("WebSite", DefaultText = "Web Site")]
        public string WebSite { get; set; }

		[Display(Name = "सम्पर्क व्यक्ति")]
		public string ContactPerson { get; set; }

		[Display(Name = "सम्पर्क व्यक्तिको फोन नं.")]
		public string ContactPersonPhoneNo { get; set; }

		[Display(Name = "सम्पर्क व्यक्तिको मोबाईल नं.")]
		public string ContactPersonMobileNo { get; set; }
		
		[Display(Name = "कैफियात")]
		public string Remarks { get; set; }

        [LocalizedDisplayName("DisplayOrder", DefaultText = "Display Order")]
        public Nullable<int> DisplayOrder { get; set; }
    }

    public class VendorListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VatPanNo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string WebSite { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonPhoneNo { get; set; }
        public string ContactPersonMobileNo { get; set; }
        public string Remarks { get; set; }
    }
}
