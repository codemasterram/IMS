using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SoftIms.Data.ViewModel
{
     public class DesignationViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(400, MinimumLength = 1)]
        [Display(Name ="Desigation Name")]
        [Remote("DesignationNotExist", "Master", HttpMethod = "Post", AdditionalFields = "Id", ErrorMessage = "{0} already exists.")]
        public string Name { get; set; }
        [Required]
        public int DisplayOrder { get; set; } = 1;
    }
    public class DesignationListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
