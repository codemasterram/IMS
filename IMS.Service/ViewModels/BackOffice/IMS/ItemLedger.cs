using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class ItemLedgerViewModel
    {
        public int Id { get; set; }
        public string DataId { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public string Code { get; set; }
        public string SubCode { get; set; }
        public string PurchasedDate { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> PurchaseAmt { get; set; }
        public Nullable<decimal> Dep { get; set; }
        public Nullable<decimal> Sale { get; set; }
        public Nullable<decimal> Rem { get; set; }

    }
}
