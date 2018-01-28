using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SoftIms.Data.Enums;

namespace SoftIms.Data.ViewModel
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Department Name")]
        [Remote("DepartmentNotExist", "Master", HttpMethod = "Post", AdditionalFields = "Id", ErrorMessage = "{0} already exists.")]
        [StringLength(400, MinimumLength = 1)]
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public int DataStatus { get; set; } = (int)eDataStatus.Active;
    }

    public class DepartmentListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
