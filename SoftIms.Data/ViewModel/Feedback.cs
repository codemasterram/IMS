using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftIms.Data.ViewModel
{
    public class FeedbackViewModel
    {
        [Required(ErrorMessage = "Please enter detail.")]
        public string Detail { get; set; }
        public int? AttachedFiles { get; set; }
        public bool Screenshot { get; set; }
        public string ScreenshotData { get; set; }
    }
}
