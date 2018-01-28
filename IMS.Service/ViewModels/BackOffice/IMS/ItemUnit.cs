using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IMS.Logic.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class ItemUnitViewModel : ServiceModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "इकाईको नाम")]
        public string Name { get; set; }
		
		[Required]
		[Display(Name = "इकाईको संकेत नं.")]
		public string Code { get; set; }


        [Display(Name = "Display Order")]
        public Nullable<int> DisplayOrder { get; set; }
    }
    public class ItemUnitListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
    }
}
