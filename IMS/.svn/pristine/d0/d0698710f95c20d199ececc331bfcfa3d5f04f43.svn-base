using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftIms.Data.ViewModel
{
   public class LoginHistoryViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(400, MinimumLength = 1)]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed in {0}.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Browser Name")]
        public string Browser { get; set; }
        [Display(Name = "Device Name")]
        public string Device { get; set; }
        [Display(Name = "IP Address")]
        public string IPAddress { get; set; }
        public System.DateTime LogDate { get; set; }
        public bool IsSuccess { get; set; }
    }
    public class LoginHistoryListViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Browser { get; set; }
        public string Device { get; set; }
        public string IPAddress { get; set; }
        public String LogDate { get; set; }
        public bool IsSuccess { get; set; }
    }
}
