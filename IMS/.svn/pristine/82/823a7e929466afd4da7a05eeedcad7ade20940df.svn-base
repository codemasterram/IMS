using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SoftIms.Data.ViewModel
{
   public class PermissionViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        [Remote("PermissiontNotExist", "Master", HttpMethod = "Post", AdditionalFields = "Id", ErrorMessage = "{0} already exists.")]
        public string Name { get; set; }
        [StringLength(400, MinimumLength = 1)]
        public string Description { get; set; }
    }
    public class PermissionListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
