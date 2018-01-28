using IMS.Data;
using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using static IMS.Data.NTAEnum;

namespace IMS.Logic.ViewModels.BackOffice.Master
{
    public class DesignationViewModel : ServiceModel
    {
        public int Id { get; set; }

        [TRequired(ErrorMessageResourceName = "Master.NameIsRequired", DefaultText = "Please enter {0}.")]
        [LocalizedDisplayName("Master.Name", DefaultText = "Name")]
        [Remote("IsDesignationNameAvailable", "CommonMaster", AdditionalFields = "Id")]
        public string Name { get; set; }

        [TRequired(ErrorMessageResourceName = "Master.Name_EnIsRequired")]
        [LocalizedDisplayName("Master.Name_En")]
        [Remote("IsDesignationNameEnAvailable", "CommonMaster", AdditionalFields = "Id")]
        public string Name_En { get; set; }

        //[TRequired(ErrorMessageResourceName = "Master.AliasIsRequired")]
        [LocalizedDisplayName("Master.Alias")]
        [Remote("IsDesignationAliasAvailable", "CommonMaster", AdditionalFields = "Id")]
        public string Alias { get; set; }

        [LocalizedDisplayName("Master.DisplayOrder")]
        public Nullable<int> DisplayOrder { get; set; }

        [LocalizedDisplayName("Master.Status")]
        public eStatus Status { get; set; }

        public bool IsActive
        {
            get
            {
                return Status == eStatus.Active;
            }
            set
            {
                Status = value ? eStatus.Active : eStatus.InActive;
            }
        }

        public Nullable<int> EntryBy { get; set; }
        public System.DateTime EntryDate { get; set; }
        public Nullable<int> ChangeBy { get; set; }
        public Nullable<System.DateTime> ChangeDate { get; set; }
    }

    public class DesignationList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Name_En { get; set; }
        public string Alias { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
