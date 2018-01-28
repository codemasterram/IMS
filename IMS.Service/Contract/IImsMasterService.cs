using IMS.Logic.ViewModels;
using IMS.Logic.ViewModels.BackOffice.IMS;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.Contract
{
    public interface IImsMasterService
    {
        ItemUnitViewModel SaveItemUnit(ItemUnitViewModel model);
        ItemUnitViewModel GetItemUnit(int itemId);
        IList<ItemUnitListViewModel> GetItemUnits(string name);
        ServiceModel DeleteItemUnit(int id);

        ItemViewModel SaveItem(ItemViewModel model);
        ItemViewModel GetItem(int itemId);
        IList<ItemListViewModel> GetItems(int? itemGroupId, int? itemUnitId, int? itemTypeId, string name);
        ServiceModel DeleteItem(int id);

        ItemGroupViewModel SaveItemGroup(ItemGroupViewModel model);
        ItemGroupViewModel GetItemGroup(int itemGroupId);
        IList<ItemGroupListViewModel> GetItemGroups(string name);
        ServiceModel DeleteItemGroup(int id);

        ItemGroupDepreciationRateViewModel SaveItemGroupDepreciationRate(ItemGroupDepreciationRateViewModel model);
        IList<ItemGroupDepreciationRateViewModel> GetDepreciationRates(int itemGroupId);
        ItemGroupDepreciationRateViewModel GetDepreciationRate(int id);
        ServiceModel DeleteItemGroupDepreciationRate(int id);

        VendorViewModel SaveVendor(VendorViewModel model);
        VendorViewModel GetVendor(int vendorId);
        IList<VendorListViewModel> GetVendors(string name);
        ServiceModel DeleteVendor(int id);

        AdjustmentTypeViewModel SaveAdjustmentType(AdjustmentTypeViewModel model);
        AdjustmentTypeViewModel GetAdjustmentType(int adjustmentTypeId);
        IList<AdjustmentTypeListViewModel> GetAdjustmentTypes(string name, int? stockInOutType);
        ServiceModel DeleteAdjustmentType(int id);

    }
}
