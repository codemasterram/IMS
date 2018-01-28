using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SoftIms.Data.ViewModel
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1)]
        [Display(Name = "Role Name")]
        [Remote("RoleNotExist", "Master", HttpMethod = "Post", AdditionalFields = "Id", ErrorMessage = "{0} already exists.")]
        public string Name { get; set; }
        [StringLength(500, MinimumLength = 1)]
        public string Description { get; set; }
        [Required]
        public int DisplayOrder { get; set; } = 1;
    }
    public class RoleListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
