using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftIms.Data.ViewModel
{
    public class ItemStatusViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ItemGroupId { get; set; }
        public string ItemGroupName { get; set; }
        public int ItemUnitId { get; set; }
        public string UnitName { get; set; }
        public int InStockQuantity { get; set; }
    }
}
