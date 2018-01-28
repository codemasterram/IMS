using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SoftIms.Data.ViewModel
{
   public class ItemViewModel
    {
        public int Id { get; set; }
        [Required]

        [Remote("ItemNotExist", "Master", HttpMethod = "Post", AdditionalFields = "Id,ItemGroupId", ErrorMessage = "{0} already exists.")]
        [StringLength(400, MinimumLength = 1)]
        public string Name { get; set; }
        [Required]
        [StringLength(400, MinimumLength = 1)]
        [Display(Name = "Code")]
        public string Code { get; set; }
        [Required]
        [Display(Name = "Item Group")]
        public int ItemGroupId { get; set; }
        [Required]
        [Display(Name = "Iteem Unit")]
        public int ItemUnitId { get; set; }
        public string ItemUnitName { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; } = 1;
    }
    public class ItemListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ItemGroupName { get; set; }
        public string ItemUnitName { get; set; }
        public int DisplayOrder { get; set; }
    }
}
