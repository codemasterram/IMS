using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftIms.Data
{
    public static class Enums
    {

        #region Ims
        public enum eStockInOutType
        {
            In = 1,
            Out = 2
        }

        public enum eDocumentSetup
        {
            PurchaseOrder = 1,
            ItemRequest = 2,
            StoreEntry = 3,
            Donation = 4,
            DonationReturn = 5,
            StockAdjustment = 6,
            Opening = 7,
            ItemRelease = 8,
        }

        public enum StockEffect
        {
            Add = 1,
            Deduct = 2
        }

        public enum eItemRequestStatus
        {
            Pending = 1,
            OnProcess = 2,
            PartialReleased = 3,
            Released = 4
        }

        public enum ItemTypes
        {
            Consumable = 1,
            NonConsumable = 2,
            Medicine = 5,
        }

        #endregion Ims
        public enum ePermission
        {
            ViewInventoryReport = 1,
            InventoryEntry = 2,
            InventorySetup = 3,
            RequestItemRequest = 4,
            manageEmployee = 5,
            ViewEmployee = 6
        }
        
        public enum eDataStatus
        {
            None = 0,
            Active = 1,
            Deleted = 2
        }
      
        public enum eApplicationStatus
        {
            None = 0,
            Applied = 1,
            Processing = 2,
            Accepted = 3,
            Rejected = 4
        }
        public enum eSelectListType
        {
            Gender = 1,
            MaritalStatus = 2,
            EmployeeType = 3,
            EmployeeStatus = 4,
            ApplicableFor = 5,
            IsActive = 6,
            ApprovedStatus = 7,
            StockInOutType = 10,
            Trimester,
        }
    }
}
