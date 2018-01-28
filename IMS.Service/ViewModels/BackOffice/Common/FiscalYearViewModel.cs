using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.Common
{
    public class FiscalYearViewModel : ServiceModel
    {
        public int Id { get; set; }
        public int FiscalYearId { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
