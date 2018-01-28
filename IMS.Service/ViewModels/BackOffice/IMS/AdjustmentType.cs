using IMS.Logic.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class AdjustmentTypeViewModel : ServiceModel
    {
        public int Id { get; set; }

        [TRequired(ErrorMessageResourceName = "AdjustmentType.Required.Name", DefaultText = "Please enter name.")]
        [LocalizedDisplayName("Name", DefaultText = "Name")]
        public string Name { get; set; }
        
        [LocalizedDisplayName("InOut", DefaultText = "InOut")]
        public string InOut { get; set; }

        public int DisplayOrder { get; set; }

        [TRequired(ErrorMessageResourceName = "AdjustmentType.Required.StockInOutType", DefaultText = "Please select stock in/out type.")]
        [LocalizedDisplayName("InOut", DefaultText = "In/Out")]
        public int StockInOutType { get; set; } = 0;
    }

    public class AdjustmentTypeListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string InOut { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
    }
}
