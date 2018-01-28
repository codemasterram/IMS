using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class ItemGroupViewModel : ServiceModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "जिन्सी समूहको नाम")]
        public string Name { get; set; }
        [Display(Name = "संकेत नं.")]
        [Required]
        public string Code { get; set; }
        [Required]
        [Display(Name = "प्रकार")]
        public int ItemTypeId { get; set; }
        public string ItemTypeAlias { get; set; }
        [Display(Name = "Display Order")]
        public Nullable<int> DisplayOrder { get; set; }

    }

    public class ItemGroupListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ItemTypeName { get; set; }
        public bool IsFixed { get; set; }
    }
}
