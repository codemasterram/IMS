using SoftIms.Data.Helper;
using SoftIms.Data.ViewModel;
using SoftIms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftIms.Controllers
{
    public class ImsReportController : BaseController
    {
        public ActionResult ConsumableItemRecord()
        {
            var data = db.ItemRepo.Get(x => x.ItemGroup.ItemType.Alias == "Consumable").OrderBy(x=>x.Name);
            ViewBag.ItemId = new SelectList(data, "Id", "Name");
            return View();
        }

        public PartialViewResult ConsumableItemRecordDetail(int? itemId)
        {
            var currentFiscalYear = Utility.CurrentFiscalYear.FiscalYearId;
            var model = imsReportService.GetItemRecord(itemId, currentFiscalYear);
            return PartialView(model);
        }

        public ActionResult NonConsumableItemRecord()
        {
            var data = db.ItemRepo.Get(x => x.ItemGroup.ItemType.Alias == "Non-Consumable").OrderBy(x => x.Name);
            ViewBag.ItemId = new SelectList(data, "Id", "Name");
            return View();
        }

        public PartialViewResult NonConsumableItemRecordDetail(int? itemId)
        {
            var currentFiscalYear = Utility.CurrentFiscalYear.FiscalYearId;
            var model = imsReportService.GetItemRecord(itemId, currentFiscalYear);
            ViewBag.ItemId = itemId;
            return PartialView(model);
        }

        public ActionResult ItemSubRecord()
        {
            return View();
        }

        public PartialViewResult ItemSubRecordDetail(int? itemId, int? BatchId)
        {
            var currentFiscalYear = Utility.CurrentFiscalYear.FiscalYearId;
            var model = imsReportService.GetItemSubRecord(itemId, currentFiscalYear);
            return PartialView(model);
        }



        #region Pending-to be discussed
        //public ActionResult ItemLedger()
        //{
        //    ViewBag.ItemCategory = imsReportService.ItemCategories();
        //    return View();
        //}
        //public PartialViewResult ItemLedgerDetail(string fiscalYear, string item)
        //{
        //    ViewBag.FiscalYear = fiscalYear;
        //    ViewBag.ItemCode = item;
        //    ViewBag.ItemName = imsReportService.GetItem(item);
        //    var data = imsReportService.ItemLedgerDetail(fiscalYear, item);

        //    return PartialView(data);
        //} 
        #endregion

        public PartialViewResult ItemRequestReport(int id = 0)
        {
            var model = new ItemRequestViewModel();
            if (id > 0)
            {
                model = helper.GetItemRequest(id);
                Session["ItemRequestDetail"] = model.Details;
            }
            if (Session["ItemRequestDetail"] == null)
                Session["ItemRequestDetail"] = new List<ItemRequestDetailViewModel>();

            return PartialView(model);
        }
        public PartialViewResult PurchaseOrderReport(int id = 0)
        {
            var model = new PurchaseOrderViewModel();
            if (id > 0)
            {
                model = helper.GetPurchaseOrder(id);

                Session["PurchaseOrderDetail"] = model.Details;

            }
            if (Session["PurchaseOrderDetail"] == null)
                Session["PurchaseOrderDetail"] = new List<PurchaseOrderDetailViewModel>();

            return PartialView(model);
        }




    }
}