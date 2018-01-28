using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SoftIms.Data.ViewModel
{
   public class ItemTypeViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(400, MinimumLength = 1)]
        [Display(Name = "Item Type Name")]
        [Remote("ItemTypeNotExist", "Master", HttpMethod = "Post", AdditionalFields = "Id", ErrorMessage = "{0} already exists.")]
        public string Name { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Alias { get; set; }
        [Required]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; } = 1;
    }
    public class ItemTypeListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public int DisplayOrder { get; set; }

    }
}
