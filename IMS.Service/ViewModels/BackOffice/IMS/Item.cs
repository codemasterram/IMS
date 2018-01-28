using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Logic.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class ItemViewModel : ServiceModel
    {
        public int Id { get; set; }

        [Display(Name = "सामानको नाम")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "	संकेत नं.")]
        [Required]
        public string Code { get; set; }

        [Required]
        [Display(Name = " समूह")]
        public int ItemGroupId { get; set; }

        [Required]
        [Display(Name = "इकाईको नाम")]
        public int ItemUnitId { get; set; }

        public string ItemUnitName { get; set; }

        [Display(Name = "Display Order")]
        public Nullable<int> DisplayOrder { get; set; }
    }

    public class ItemListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int ItemGroupId { get; set; }
        public string ItemGroupName { get; set; }
        public string ItemUnitName { get; set; }
        public string ItemTypeName { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
    }
}
