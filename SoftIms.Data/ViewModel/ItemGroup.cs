using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SoftIms.Data.ViewModel
{
    public class ItemGroupViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(400, MinimumLength = 1)]
        [Display(Name = "Item Group Name")]
        [Remote("ItemGroupNotExist", "Master", HttpMethod = "Post", AdditionalFields = "Id", ErrorMessage = "{0} already exists.")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Item Group Code")]   
        public string Code { get; set; }
        [Required]
        [Display(Name = "Item Type")]
        public int ItemTypeId { get; set; }
        public string ItemTypeAlias { get; set; }
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; } = 1;
    }
    public class ItemGroupListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ItemTypeName { get; set; }
        public bool IsFixed { get; set; }
        public int DisplayOrder { get; set; }
    }
}
