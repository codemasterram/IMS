using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftIms.Data.ViewModel
{
    public class SystemConfigurationViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Key { get; set; }
        [StringLength(400,MinimumLength =1)]
        public string DisplayKey { get; set; }
        [Required]
        [StringLength(400,MinimumLength =1)]
        public string Value { get; set; }

    }
    public class SystemConfigurationListViewModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string DisplayKey { get; set; }
        public string Value { get; set; }
    }
}
