using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Logic.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace IMS.Logic.ViewModels.BackOffice.Common
{
    public class TestLogViewModel : ServiceModel
    {
        public int Id { get; set; }

        [TRequired(ErrorMessageResourceName = "TestLog.Required.ModuleName", DefaultText = "Please enter module name.")]
        [LocalizedDisplayName("TestLog.ModuleName", DefaultText = "Module name")]
        public string Module { get; set; }

        [LocalizedDisplayName("TestLog.FieldName", DefaultText = "Field Name")]
        public string Field { get; set; }

        [LocalizedDisplayName("TestLog.DataUrl", DefaultText = "Form URL")]
        public string DataUrl { get; set; }

        [LocalizedDisplayName("TestLog.Note", DefaultText = "Note")]
        [Required(ErrorMessage = "Please enter note.")]
        public string Note { get; set; }

        [TRequired(ErrorMessageResourceName = "TestLog.Required.CheckedBy", DefaultText = "Please enter checked by.")]
        [LocalizedDisplayName("TestLog.CheckedBy", DefaultText = "Checked by")]
        public string CheckedBy { get; set; }

        [LocalizedDisplayName("TestLog.Date", DefaultText = "Date")]
        public System.DateTime Date { get; set; }

        [LocalizedDisplayName("TestLog.IsResolved", DefaultText = "Is resolved?")]
        public bool IsResolved { get; set; } = false;

        [LocalizedDisplayName("TestLog.ResolvedBy", DefaultText = "Resolved by")]
        public string ResolvedBy { get; set; }

        [LocalizedDisplayName("TestLog.ResolvedDate", DefaultText = "Resolved date")]
        public Nullable<System.DateTime> ResolvedDate { get; set; }

        [LocalizedDisplayName("TestLog.ResolvedDate", DefaultText = "Resolved date")]
        public string ResolvedDateBs { get; set; }

        [LocalizedDisplayName("TestLog.ResolvedRemarks", DefaultText = "Resolved remarks")]
        public string ResolvedRemarks { get; set; }
    }

    public class TestLogListViewModel
    {
        public int TestLogId { get; set; }
        public string Module { get; set; }
        public string Field { get; set; }
        public string DataUrl { get; set; }
        public string Note { get; set; }
        public string CheckedBy { get; set; }
        public System.DateTime Date { get; set; }
        public string DateBs { get; set; }
        public bool IsResolved { get; set; }
        public string ResolvedBy { get; set; }
        public Nullable<System.DateTime> ResolvedDate { get; set; }
        public string ResolvedDateBs { get; set; }
        public string ResolvedRemarks { get; set; }
    }
}
