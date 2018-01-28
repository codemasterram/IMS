using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SoftIms.Data.ViewModel
{
   public class ItemUnitViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(400, MinimumLength = 1)]
        [Remote("ItemUnitNotExist", "Master", HttpMethod = "Post", AdditionalFields = "Id", ErrorMessage = "{0} already exists.")]
        [Display(Name ="Unit Name")]
        public string Name { get; set; }
        [StringLength(400, MinimumLength = 1)]
        [Display(Name = "Code Name")]
        public string Code { get; set; }
        public int DisplayOrder { get; set; } = 1;
    }
    public class ItemUnitListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int DisplayOrder { get; set; }
    }
}
