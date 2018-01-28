using DateMiti;
using SoftIms.Data;
using SoftIms.Data.Infrastructure;
using SoftIms.Data.ViewModel;
using SoftIms.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static SoftIms.Data.Enums;
using static SoftIms.Utilities.Utility;
using static SoftIms.Utilities.ViewModelExtension;

namespace SoftIms.Controllers
{
    public class ImsEntryController : BaseController
    {
       
        #region OpeningStock

        public ActionResult OpeningStockList()
        {
            Session["OpeningStock"] = null;
            return View();
        }

        public PartialViewResult OpeningStockDetail(string displayDocumentNo, string dateFromBS, string dateToBS, int? sectionId, int? employeeId)
        {
            var dateFrom = dateFromBS.GetDate() == DateTime.MinValue ? (DateTime?)null : dateFromBS.GetDate();
            var dateTo = dateToBS.GetDate() == DateTime.MinValue ? (DateTime?)null : dateFromBS.GetDate();
            var fiscalYearId = CurrentFiscalYear.Id;
            var data = db.StockTransactionRepo.Table().Where(x => x.FiscalYearId == fiscalYearId && x.DocumentSetupId == (int)eDocumentSetup.Opening
                                && (string.IsNullOrEmpty(displayDocumentNo) || x.DisplayDocumentNo == displayDocumentNo)
                                && (!dateFrom.HasValue || x.Date >= dateFrom)
                                && (!dateTo.HasValue || x.Date <= dateTo)
                                && (!sectionId.HasValue || x.StockTransactionDetails.Any(y => y.DepartmentId == sectionId))
                                && (!employeeId.HasValue || x.StockTransactionDetails.Any(y => y.EmployeeId == employeeId))).OrderBy(x => x.Date);
            var model = AutomapperConfig.Mapper.Map<IList<OpeningStockListViewModel>>(data);

            return PartialView(model);
        }

        public ActionResult OpeningStockEntry(int id = 0)
        {
            //Session["OpeningStock"] = null;
            var model = new OpeningStockViewModel { DisplayDocumentNo = helper.GetDocumentNo(Enums.eDocumentSetup.Opening), DateBS = DateMiti.GetDateMiti.GetMiti(DateTime.Today) };
            if (id > 0)
            {
                var data = db.StockTransactionRepo.GetById(id);
                model = AutomapperConfig.Mapper.Map<OpeningStockViewModel>(data) ?? new OpeningStockViewModel();

                foreach (var item in data.StockTransactionDetails)
                {
                    model.Details.Add(AutomapperConfig.Mapper.Map<OpeningStockDetailViewModel>(item));
                }

                Session["OpeningStock"] = model.Details;
            }
            if (Session["OpeningStock"] == null)
                Session["OpeningStock"] = new List<OpeningStockDetailViewModel>();
            var itemGroupData = db.ItemGroupRepo.Table().OrderBy(x => x.DisplayOrder);
            var groupModel = AutomapperConfig.Mapper.Map<IList<ItemGroupListViewModel>>(itemGroupData);
            ViewBag.ItemGroups = groupModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult OpeningStockEntry(OpeningStockViewModel model)
        {
            model.Details = (List<OpeningStockDetailViewModel>)Session["OpeningStock"];
            model.FiscalYearId = CurrentFiscalYear.Id;

            if (model.Details.Count == 0)
            {
                ModelState.AddModelError("", "Please enter detail.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Errors"] = ModelState.Values.FirstOrDefault().Errors.Select(x => x.ErrorMessage).ToList();
                RefreshModelState(model.Errors);
                return View(model);
            }

            var entity = new StockTransaction();

            if (model.Id > 0)
            {
                entity = db.StockTransactionRepo.GetById(model.Id);
                entity.Date = model.DateBS.GetDate();
                entity.Remarks = model.Remarks;
                entity.ModifiedDate = DateTime.Now;
                entity.ModifiedBy = CurrentUser.Id;
                db.StockTransactionDetailRepo.DeleteRange(entity.StockTransactionDetails);
            }
            else
            {
                model.DocumentSetupId = (int)eDocumentSetup.Opening;
                model.Date = model.DateBS.GetDate();
                entity = AutomapperConfig.Mapper.Map<StockTransaction>(model);
                entity.CreatedDate = DateTime.Now;
                entity.CreatedBy = CurrentUser.Id;
                db.StockTransactionRepo.Attach(entity);
            }

            foreach (var item in model.Details)
            {
                var detail = AutomapperConfig.Mapper.Map<StockTransactionDetail>(item);
                detail.StockTransaction = entity;
                db.StockTransactionDetailRepo.Attach(detail);
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Add(ex.Message);
            }

            if (model.HasError)
            {
                TempData["Errors"] = model.ErrorMessages;
                TempData["Exception"] = model.Exception;
                RefreshModelState(model.Errors);
                return View(model);
            }
            TempData["SuccessMessage"] = "Opening stock saved successed.";

            model = new OpeningStockViewModel { DisplayDocumentNo = helper.GetDocumentNo(Enums.eDocumentSetup.Opening) };
            Session["OpeningStock"] = null;
            return RedirectToAction("openingstocklist", "imsentry");
        }

        [HttpPost]
        public JsonResult AddOpeningStockDetail(int itemId, string itemSubCodeNo, int qty, decimal? amount, int? sectionId, int? employeeId, string narration, string purchaseDateBS, decimal? purchaseAmount)
        {
            List<string> errors = new List<string>();
            try
            {
                if (ModelState.IsValid)
                {
                    var details = (List<OpeningStockDetailViewModel>)Session["OpeningStock"];
                    var item = db.ItemRepo.GetById(itemId);
                    if (item.ItemGroup.ItemType.Alias == "Non-Consumable")
                    {
                        qty = 1;
                    }
                    var model = new OpeningStockDetailViewModel();
                    var employee = (employeeId.HasValue ? db.EmployeeRepo.GetById(employeeId.Value) : null);
                    var section = (sectionId.HasValue ? db.DepartmentRepo.GetById(sectionId.Value) : null);
                    model.DepartmentId = sectionId;
                    model.ItemId = itemId;
                    model.ItemName = item.Name;
                    model.ItemSubCodeNo = itemSubCodeNo;

                    model.UnitName = item.Name;
                    model.Qty = qty;
                    model.Amount = amount;

                    if (sectionId.HasValue)
                    {
                        model.DepartmentId = sectionId.Value;
                        model.DepartmentName = section.Name;
                    }
                    model.EmployeeId = employeeId;
                    model.EmployeeName = (employee != null) ? employee.Name : "";
                    model.Narration = narration;
                    model.Guid = Guid.NewGuid().ToString();
                    model.PurchaseDateBS = purchaseDateBS;
                    model.PurchaseAmount = purchaseAmount;

                    details.Add(model);

                    Session["OpeningStock"] = details;
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { success = true });
                }
                foreach (var item in ModelState.Where(x => x.Value.Errors.Any()))
                {
                    errors.Add(item.Value.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { error = errors });
        }

        [HttpPost]
        public JsonResult DeleteStockTransaction(int id, bool deleteChildItem = false)
        {
            if (deleteChildItem)
            {
                var models = db.StockTransactionDetailRepo.Table().Where(x => x.StockTransactionId == id);
                db.StockTransactionDetailRepo.DeleteRange(models);
            }

            string errorMsg = null;
            try
            {
                db.StockTransactionRepo.Delete(id);
                return Json("");
            }
            catch (Exception ex)
            {
                if (ex.GetExceptionMessages().Contains("REFERENCE"))
                {
                    errorMsg = "This item is used in another module. Unable to delete this item.";
                }
                Response.StatusCode = (int)HttpStatusCode.SeeOther;
                var confirmChild = errorMsg.Contains("another");
                return Json(new { confirmChild = confirmChild, success = false, errors = new List<string>() { ex.Message }, displayMessage = errorMsg });
            }
        }

        [HttpPost]
        public JsonResult RemoveOpeningStockItemList(string guid)
        {
            var details = (List<OpeningStockDetailViewModel>)Session["OpeningStock"];
            details.Remove(details.FirstOrDefault(x => x.Guid == guid));
            Session["OpeningStock"] = details;
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { success = true });
        }

        public PartialViewResult OpeningStockItemList()
        {
            var model = (List<OpeningStockDetailViewModel>)Session["OpeningStock"];
            return PartialView(model);
        }

        #endregion Opening Stock

        #region ItemRequest

        public ActionResult ItemRequestList()
        {
            Session["ItemRequestDetail"] = null;
            return View();
        }

        public PartialViewResult ItemRequestDetail(string displayDocumentNo, string dateFromBS, string dateToBS, int? sectionId, int? employeeId)
        {
            var dateFrom = dateFromBS.GetDate() == DateTime.MinValue ? (DateTime?)null : dateFromBS.GetDate();
            var dateTo = dateToBS.GetDate() == DateTime.MinValue ? (DateTime?)null : dateFromBS.GetDate();
            var fiscalYearId = CurrentFiscalYear.Id;
            var data = db.ItemRequestRepo.Table().Where(x => x.FiscalYearId == fiscalYearId && x.DocumentSetupId == (int)eDocumentSetup.ItemRequest
                                && (string.IsNullOrEmpty(displayDocumentNo) || x.DisplayDocumentNo == displayDocumentNo)
                                && (!dateFrom.HasValue || x.Date >= dateFrom)
                                && (!dateTo.HasValue || x.Date <= dateTo));

            var model = AutomapperConfig.Mapper.Map<IList<ItemRequestListViewModel>>(data);

            return PartialView(model);
        }

        public ActionResult ItemRequestEntry(int id = 0)
        {
            //Session["ItemRequest"] = null;

            var model = new ItemRequestViewModel { DisplayDocumentNo = helper.GetDocumentNo(Enums.eDocumentSetup.ItemRequest) };
            model.ApplicationStatus = (int)eItemRequestStatus.Pending;
            if (id > 0)
            {
                var data = db.ItemRequestRepo.GetById(id);
                Session["ItemRequestDetail"] = model.Details;
                model = AutomapperConfig.Mapper.Map<ItemRequestViewModel>(data) ?? new ItemRequestViewModel();

                foreach (var item in data.ItemRequestDetails)
                {
                    model.Details.Add(AutomapperConfig.Mapper.Map<ItemRequestDetailViewModel>(item));
                }

                Session["ItemRequestDetail"] = model.Details;
            }


            if (Session["ItemRequestDetail"] == null)
                Session["ItemRequestDetail"] = new List<ItemRequestDetailViewModel>();
            var itemGroupData = db.ItemGroupRepo.Table().OrderBy(x => x.DisplayOrder);
            var groupModel = AutomapperConfig.Mapper.Map<IList<ItemGroupListViewModel>>(itemGroupData);
            model.DepartmentId = CurrentUser.DepartmentId;
            model.RequestedEmployeeId = CurrentUser.DepartmentId;
            model.EmployeeId = CurrentUser.EmployeeId;

            ViewBag.ItemGroups = groupModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult ItemRequestEntry(ItemRequestViewModel model)
        {
            model.Details = (List<ItemRequestDetailViewModel>)Session["ItemRequestDetail"];
            model.FiscalYearId = CurrentFiscalYear.Id;     




            if (model.Details.Count == 0)
            {
                ModelState.AddModelError("", "Please enter detail.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Errors"] = ModelState.Values.FirstOrDefault().Errors.Select(x => x.ErrorMessage).ToList();
                RefreshModelState(model.Errors);
                return View(model);
            }

            var entity = new ItemRequest();

            if (model.Id > 0)
            {
                entity = db.ItemRequestRepo.GetById(model.Id);
                entity.Date = model.DateBS.GetDate();
                entity.Remarks = model.Remarks;
                entity.ModifiedDate = DateTime.Now;
                entity.ModifiedBy = CurrentUser.Id;
                db.ItemRequestDetailRepo.DeleteRange(entity.ItemRequestDetails);
            }
            else
            {
                model.DocumentSetupId = (int)eDocumentSetup.ItemRequest;
                model.Date = model.DateBS.GetDate();
                entity = AutomapperConfig.Mapper.Map<ItemRequest>(model);
                entity.CreatedDate = DateTime.Now;
                entity.CreatedBy = CurrentUser.Id;
                db.ItemRequestRepo.Attach(entity);
            }

            foreach (var item in model.Details)
            {
                var detail = AutomapperConfig.Mapper.Map<ItemRequestDetail>(item);
                detail.ItemRequest = entity;
                db.ItemRequestDetailRepo.Attach(detail);
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Add(ex.Message);
            }

            if (model.HasError)
            {
                TempData["Errors"] = model.ErrorMessages;
                TempData["Exception"] = model.Exception;
                RefreshModelState(model.Errors);
                return View(model);
            }
            TempData["SuccessMessage"] = "Item Request stock saved successed.";

            model = new ItemRequestViewModel { DisplayDocumentNo = helper.GetDocumentNo(Enums.eDocumentSetup.ItemRequest) };
            Session["ItemRequestDetail"] = null;
            return RedirectToAction("ItemRequestlist", "imsentry");
        }

        public JsonResult RemoveItemRequestItemList(Guid guid)
        {
            var details = (List<ItemRequestDetailViewModel>)Session["ItemRequestDetail"];
            details.Remove(details.FirstOrDefault(x => x.Guid == guid));
            Session["ItemRequestDetail"] = details;
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { success = true });
        }

        public PartialViewResult ItemsStatus(int id = 0)
        {
            var model = new ItemRequestItemStatusViewModel();
            if (id > 0)
                model = helper.GetItemRequestItemStatus(id, DateTime.Now, CurrentFiscalYear.FiscalYearId);
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult AddItemRequestDetail(int itemId, int qty, string specification, string narration)
        {
            List<string> errors = new List<string>();
            try
            {
                if (ModelState.IsValid)
                {
                    var details = (List<ItemRequestDetailViewModel>)Session["ItemRequestDetail"] ?? new List<ItemRequestDetailViewModel>();
                    var model = new ItemRequestDetailViewModel();
                    model.Guid = Guid.NewGuid();
                    model.ItemId = itemId;
                    var item = db.ItemRepo.GetById(itemId);
                    model.ItemName = item.Name;
                    model.ItemCode = item.Code;
                    model.UnitName = item.ItemUnit.Name;
                    model.Qty = qty;
                    model.Specification = specification;
                    model.Remarks = narration;
                    details.Add(model);

                    Session["ItemRequestDetail"] = details;
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { success = true });
                }
                foreach (var item in ModelState.Where(x => x.Value.Errors.Any()))
                {
                    errors.Add(item.Value.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { error = errors });
        }

        public JsonResult UpdateRequestItem(Guid guid, int qty, string specification, string narration)
        {
            try
            {
                var details = (List<ItemRequestDetailViewModel>)Session["ItemRequestDetail"];
                var model = details.Where(x => x.Guid == guid).FirstOrDefault();
                model.Qty = qty;
                model.Specification = specification;
                model.Remarks = narration;
                Session["ItemRequestDetail"] = details;
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        qty = model.Qty,
                        narration = model.Remarks
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.SeeOther;
                return Json(new { error = ex.GetExceptionMessages() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetUnProcessedItemRequest(int employeeId = 0, string itemRequestIds = "")
        {
            var query = db.ItemRequestRepo.Table().Where(x => x.FiscalYearId == CurrentFiscalYear.Id && x.DocumentSetupId == (int)eDocumentSetup.ItemRequest
                                && (employeeId == 0 || x.EmployeeId == employeeId)
                                && x.ItemRequestDetails.Any(x1 => !x1.PurchaseOrderId.HasValue)
                                && x.ItemRequestDetails.Any(x1 => !x1.ItemReleaseId.HasValue)).OrderByDescending(x => x.Date).ThenByDescending(x => x.DisplayDocumentNo);

            var data = AutomapperConfig.Mapper.Map<List<ItemRequestViewModel>>(query);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult ItemRequestItemList()
        {
            var model = (List<ItemRequestDetailViewModel>)Session["ItemRequestDetail"];
            if (model == null)
            {
                model = new List<ItemRequestDetailViewModel>();
            }
            return PartialView(model);
        }

        #endregion ItemRequest

        #region Purchase Order
        public ActionResult PurchaseOrder(int id)
        {
            var data = db.PurchaseOrderRepo.GetById(id);
            var model = AutomapperConfig.Mapper.Map<PurchaseOrderListViewModel>(data);
            return View(model);
        }

        public ActionResult PurchaseOrderList()
        {
            Session["PurchaseOrderDetail"] = null;
            return View();
        }

        public PartialViewResult PurchaseOrderDetail(string DisplayDocumentNo, string dateFromBS, string dateToBS)
        {
            DateTime? dateFrom = string.IsNullOrEmpty(dateFromBS) ? (DateTime?)null : dateFromBS.GetDate();
            DateTime? dateTo = string.IsNullOrEmpty(dateToBS) ? (DateTime?)null : dateToBS.GetDate();
            var data = db.PurchaseOrderRepo.Table().Where(x => x.FiscalYearId == CurrentFiscalYear.FiscalYearId && x.DocumentSetupId == (int)eDocumentSetup.PurchaseOrder
                               && (string.IsNullOrEmpty(DisplayDocumentNo) || x.DisplayDocumentNo == DisplayDocumentNo)
                               && (!dateFrom.HasValue || x.Date >= dateFrom)
                               && (!dateTo.HasValue || x.Date <= dateTo)).OrderByDescending(x => x.DisplayDocumentNo);
            var model= AutomapperConfig.Mapper.Map<IList<PurchaseOrderListViewModel>>(data);
            return PartialView(model);
        }

        public ActionResult PurchasePrint(int id = 0)
        {
            var model = new PurchaseOrderViewModel { DisplayDocumentNo = helper.GetDocumentNo(Enums.eDocumentSetup.Opening)};


            if (id > 0)
            {
                var data = db.PurchaseOrderRepo.GetById(id);

                model = AutomapperConfig.Mapper.Map<PurchaseOrderViewModel>(data);

                Session["PurchaseOrderDetail"] = model.Details;

            }
            if (Session["PurchaseOrderDetail"] == null)
                Session["PurchaseOrderDetail"] = new List<PurchaseOrderDetailViewModel>();
            return View(model);
        }

        public ActionResult PurchaseOrderEntry(int id = 0, int itemRequestId = 0)
        {
            var model = new PurchaseOrderViewModel { DisplayDocumentNo = helper.GetDocumentNo(Enums.eDocumentSetup.PurchaseOrder) };
            TempData["ItemRequestIds"] = "";
            Session["PurchaseOrderDetail"] = null;
            if (id > 0)
            {
                var data = db.PurchaseOrderRepo.GetById(id);
                model = AutomapperConfig.Mapper.Map<PurchaseOrderViewModel>(data) ?? new PurchaseOrderViewModel();
                string itemRequestIds = string.Join(",", model.Details.Select(x => x.ItemRequestIdCSV).Distinct());
                TempData["ItemRequestIds"] = itemRequestIds;
                Session["PurchaseOrderDetail"] = model.Details;
            }
            if (id == 0 && itemRequestId > 0)
            {
                var data = db.ItemRequestRepo.GetById(itemRequestId);
                var selectedRequestItem = new List<PurchaseOrderDetailViewModel>();
                if (data != null)
                {
                    var a = AutomapperConfig.Mapper.Map<IList<ItemRequestDetailViewModel>>(data.ItemRequestDetails.Where(x => !x.ItemReleaseId.HasValue && !x.PurchaseOrderId.HasValue));
                    foreach (var item in a)
                    {
                        var i = AutomapperConfig.Mapper.Map<PurchaseOrderDetailViewModel>(item);
                        i.ItemRequestDetailIdCSV = item.Id.ToString();
                        i.ItemRequestIdCSV = item.ItemRequestId.ToString();
                        i.InStockQty = helper.GetItemStatusByItemId(i.ItemId, DateTime.Now, CurrentFiscalYear.FiscalYearId).InStockQuantity;
                        selectedRequestItem.Add(i);
                    }
                }
                Session["PurchaseOrderDetail"] = selectedRequestItem;
            }
            if (Session["PurchaseOrderDetail"] == null)
                Session["PurchaseOrderDetail"] = new List<PurchaseOrderDetailViewModel>();

            //var wf = workflowService.GetWorkflowProcess(eWorkFlowType.PurchaseOrder, 1);
            //ViewBag.ApplicationReceivers = workflowService.GetMembers(wf.Id, Utility.CurrentEmployee != null ? Utility.CurrentEmployee.Id : 0, false);
            return View(model);
        }

        [HttpPost]
        public ActionResult PurchaseOrderEntry(PurchaseOrderViewModel model)
        {
            model.EmployeeId = CurrentUser.Id;
            model.Details = (List<PurchaseOrderDetailViewModel>)Session["PurchaseOrderDetail"];
            model.CurrentUserId = CurrentUser.Id;
            model.FiscalYearId = CurrentFiscalYear.FiscalYearId;

            model.DocumentSetupId = (int)eDocumentSetup.PurchaseOrder;

            if (model.Details.Count == 0)
            {
                model.Errors.Add(new ValidationResult("Please enter detail.", new string[] { "" }));
            }

            if (model.Errors.Any())
            {
                RefreshModelState(model.Errors);
                return View(model);
            }


            if (!model.Validate())
            {
                RefreshModelState(model.Errors);
                return View(model);
            }

            var entity = new PurchaseOrder();

            if (model.Id > 0)
            {
                entity = db.PurchaseOrderRepo.GetById(model.Id);
                entity.Date = model.DateBS.GetDate();
                entity.DueDate = string.IsNullOrEmpty(model.DueDateBS) ? (DateTime?)null : model.DueDateBS.GetDate();
                entity.Remarks = model.Remarks;
                db.PurchaseOrderDetailRepo.DeleteRange(entity.PurchaseOrderDetails);
            }
            else
            {
                model.Date = model.DateBS.GetDate();
                model.EmployeeId = CurrentUser.EmployeeId;
                model.DueDate = string.IsNullOrEmpty(model.DueDateBS) ? (DateTime?)null : model.DueDateBS.GetDate();
                entity = AutomapperConfig.Mapper.Map<PurchaseOrder>(model);
                entity.CreatedDate = DateTime.Now;

                db.PurchaseOrderRepo.Create(entity);
            }

            List<int> selectedItemRequestId = new List<int>();
            foreach (var item in model.Details)
            {
                var detail = AutomapperConfig.Mapper.Map<PurchaseOrderDetail>(item);
                detail.PurchaseOrderId = entity.Id;
                db.PurchaseOrderDetailRepo.Create(detail);

                if (!string.IsNullOrEmpty(item.ItemRequestDetailIdCSV))
                {
                    selectedItemRequestId.AddRange(item.ItemRequestDetailIdCSV.Split(',').Select(x => Convert.ToInt32(x)));
                }
            }

            var selectedItemRequest = db.ItemRequestDetailRepo.Table().Where(x => selectedItemRequestId.Contains(x.Id));
            foreach (var req in selectedItemRequest)
            {
                if (db.PurchaseOrderDetailRepo.Table().Any(x => x.PurchaseOrderId == entity.Id && x.ItemId == req.ItemId))
                    req.PurchaseOrderId = entity.Id;
                else
                    req.PurchaseOrderId = null;
            }
            db.SaveChanges();

            if (model.Id == 0)
                model.Message = "Data saved successfully.";
            else
                model.Message = "Save changes successfully.";

            

            TempData["SuccessMessage"] = "Purchase Order saved successfully.";
            model = new PurchaseOrderViewModel { DisplayDocumentNo = helper.GetDocumentNo(Enums.eDocumentSetup.PurchaseOrder) };
            Session["PurchaseOrderDetail"] = null;
            return RedirectToAction("purchaseorderlist");
        }

        [HttpPost]
        public JsonResult AddPurchaseOrderDetail(int itemId, int qty, decimal? rate, decimal totalAmount, string specification)
        {
            List<string> errors = new List<string>();
            try
            {
                if (ModelState.IsValid)
                {
                    var details = (List<PurchaseOrderDetailViewModel>)Session["PurchaseOrderDetail"];
                    var model = new PurchaseOrderDetailViewModel();
                    model.Guid = Guid.NewGuid();
                    model.ItemId = itemId;
                    var item = db.ItemRepo.GetById(itemId);
                    model.ItemName = item.Name;
                    model.ItemCode = item.Code;
                    model.UnitName = item.ItemUnit.Name;
                    model.Qty = qty;
                    model.Rate = rate.HasValue ? Convert.ToDecimal(rate) : 0;
                    model.TotalAmount = totalAmount;
                    model.Specification = specification;
                    details.Add(model);
                    Session["PurchaseOrderDetail"] = details;
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { success = true });
                }
                foreach (var item in ModelState.Where(x => x.Value.Errors.Any()))
                {
                    errors.Add(item.Value.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { error = errors });
        }

        [HttpPost]
        public JsonResult RemovePurchaseOrderItemList(Guid guid)
        {
            var details = (List<PurchaseOrderDetailViewModel>)Session["PurchaseOrderDetail"];
            details.Remove(details.FirstOrDefault(x => x.Guid == guid));
            Session["PurchaseEntry"] = details;
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { success = true });
        }

        public PartialViewResult PurchaseOrderItemList()
        {
            var model = (List<PurchaseOrderDetailViewModel>)Session["PurchaseOrderDetail"];
            return PartialView(model);
        }


        public PartialViewResult PrintOrderItemList()
        {
            var model = (List<PurchaseOrderDetailViewModel>)Session["PurchaseOrderDetail"];
            return PartialView(model);
        }

        public JsonResult UpdateOrderItem(Guid guid, int qty, decimal? rate, decimal totalAmount, string specification)
        {
            try
            {
                var details = (List<PurchaseOrderDetailViewModel>)Session["PurchaseOrderDetail"];
                var model = details.Where(x => x.Guid == guid).FirstOrDefault();
                model.Qty = qty;
                model.Rate = rate.HasValue ? Convert.ToDecimal(rate) : 0;
                model.TotalAmount = totalAmount;
                model.Specification = specification;
                Session["PurchaseOrderDetail"] = details;
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        qty = model.Qty,
                        rate = model.Rate,
                        totalamount = model.TotalAmount,
                        specification = model.Specification,
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.SeeOther;
                return Json(new { error = ex.GetExceptionMessages() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteItemRequest(int id, bool deleteChildItem = false)
        {
            if (deleteChildItem)
            {
                var models = db.ItemRequestDetailRepo.Table().Where(x => x.ItemRequestId == id);
                db.ItemRequestDetailRepo.DeleteRange(models);
            }
            string errorMsg = null;
            var errors = db.ItemRequestRepo.TryDelete(id, out errorMsg);
            if (errors.Any())
            {
                Response.StatusCode = (int)HttpStatusCode.SeeOther;
                var confirmChild = errorMsg.Contains("another");
                return Json(new { confirmChild = confirmChild, success = false, errors = errors, displayMessage = errorMsg });
            }
            return Json("");
        }

        [HttpPost]
        public JsonResult DeletePurchaseOrder(int id, bool deleteChildItem = false)
        {
            if (deleteChildItem)
            {
                var models = db.PurchaseOrderDetailRepo.Table().Where(x => x.PurchaseOrderId == id);
                db.PurchaseOrderDetailRepo.DeleteRange(models);
            }
            string errorMsg = null;
            var errors = db.PurchaseOrderRepo.TryDelete(id, out errorMsg);
            if (errors.Any())
            {
                Response.StatusCode = (int)HttpStatusCode.SeeOther;
                var confirmChild = errorMsg.Contains("another");
                return Json(new { confirmChild = confirmChild, success = false, errors = errors, displayMessage = errorMsg });
            }
            return Json("");
        }

        public ActionResult AddItemFromRequestReference(int[] requestId)
        {
            string requestIds = string.Join(",", requestId);

            var query = db.ItemRequestRepo.Table().Where(x => x.FiscalYearId == CurrentFiscalYear.Id && x.DocumentSetupId == (int)eDocumentSetup.ItemRequest
                                && x.ItemRequestDetails.Any(x1 => !x1.PurchaseOrderId.HasValue)).OrderByDescending(x => x.Date).ThenByDescending(x => x.DisplayDocumentNo);
            var data = AutomapperConfig.Mapper.Map<List<ItemRequestViewModel>>(query);
            var model = (List<PurchaseOrderDetailViewModel>)Session["PurchaseOrderDetail"];
            var selectedRequestItem = new List<PurchaseOrderDetailViewModel>();
            var filteredData = data.Where(x => requestId.Contains(x.Id));
            foreach (var req in filteredData)
            {
                foreach (var item in req.Details)
                {
                    if (selectedRequestItem.Any(x => x.ItemId == item.ItemId))
                    {
                        selectedRequestItem.FirstOrDefault(x => x.ItemId == item.ItemId).Qty += item.Qty;
                        selectedRequestItem.FirstOrDefault(x => x.ItemId == item.ItemId).ItemRequestIdCSV += "," + item.ItemRequestId;
                        selectedRequestItem.FirstOrDefault(x => x.ItemId == item.ItemId).ItemRequestDetailIdCSV += "," + item.Id;
                    }
                    else
                    {
                        if (!item.ItemReleaseId.HasValue && !item.PurchaseOrderId.HasValue)
                        {
                            var i = AutomapperConfig.Mapper.Map<PurchaseOrderDetailViewModel>(item);
                            i.ItemRequestDetailIdCSV = item.Id.ToString();
                            i.ItemRequestIdCSV = item.ItemRequestId.ToString();
                            i.InStockQty = helper.GetItemStatusByItemId(i.ItemId, DateTime.Now, CurrentFiscalYear.FiscalYearId).InStockQuantity;
                            selectedRequestItem.Add(i);
                        }

                    }
                }
            }

            var selectedItems = selectedRequestItem.Select(x => x.ItemId);
            var a = selectedItems.ToList();
            model.RemoveAll(x => selectedItems.Contains(x.ItemId));
            model.AddRange(selectedRequestItem);

            Session["PurchaseOrderDetail"] = model;
            return RedirectToAction("PurchaseOrderItemList");
        }
        #endregion Purchase Order

        #region PurchaseEntry
        public ActionResult PurchaseEntryList()
        {
            Session.Remove("PurchaseEntry");
            return View();
        }

        public PartialViewResult PurchaseEntryDetail(string displayDocumentNo, string dateFromBS, string dateToBS, int? vendorId)
        {
            DateTime? dateFrom = string.IsNullOrEmpty(dateFromBS) ? (DateTime?)null : dateFromBS.GetDate();
            DateTime? dateTo = string.IsNullOrEmpty(dateToBS) ? (DateTime?)null : dateToBS.GetDate();

            var query = db.StockTransactionRepo.Table().Where(x => x.FiscalYearId == CurrentFiscalYear.Id && x.DocumentSetupId == (int)eDocumentSetup.StoreEntry
                                && (string.IsNullOrEmpty(displayDocumentNo) || x.DisplayDocumentNo == displayDocumentNo)
                                && (!dateFrom.HasValue || x.Date >= dateFrom)
                                && (!vendorId.HasValue || x.VendorId == vendorId)
                                && (!dateTo.HasValue || x.Date <= dateTo)).OrderByDescending(x => x.Date).ThenBy(x => x.DisplayDocumentNo);
            var model= AutomapperConfig.Mapper.Map<IList<PurchaseEntryListViewModel>>(query);
            return PartialView(model);
        }

        public ActionResult PurchaseEntryEntry(int id = 0, int purchaseOrderId = 0)
        {
            var model = new PurchaseEntryViewModel { DisplayDocumentNo = helper.GetDocumentNo(Enums.eDocumentSetup.StoreEntry) };
            var data = db.PurchaseOrderRepo.Table().Where(x => x.FiscalYearId == CurrentFiscalYear.Id && x.DocumentSetupId == (int)eDocumentSetup.PurchaseOrder
                                &&  (!x.StockTransactions.Any() || x.PurchaseOrderDetails.Sum(x1 => x1.Qty) > x.StockTransactions.Sum(x1 => x1.StockTransactionDetails.Sum(x2 => x2.Qty)) || x.StockTransactions.Any(y => y.Id == id))
                                ).OrderByDescending(x => x.DisplayDocumentNo);
            var purchaseOrders= AutomapperConfig.Mapper.Map<IList<PurchaseOrderListViewModel>>(data);
            Session["PurchaseEntry"] = null;
            if (id > 0)
            {
                var query = db.StockTransactionRepo.GetById(id);
             model = AutomapperConfig.Mapper.Map<PurchaseEntryViewModel>(query);
            
            foreach (var item in query.StockTransactionDetails)
            {
                model.Details.Add(AutomapperConfig.Mapper.Map<PurchaseEntryDetailViewModel>(item));
            }

            if (model.PurchaseOrderId.HasValue)
            {
                var _purchaseOrder = db.PurchaseOrderRepo.GetById(Convert.ToInt32(model.PurchaseOrderId));
                if (_purchaseOrder.VendorId.HasValue)
                {
                    var vendorDetail = db.VendorRepo.GetById(_purchaseOrder.VendorId.Value);
                    model.VendorDetail = AutomapperConfig.Mapper.Map<VendorViewModel>(vendorDetail);
                }
            }
                Session["PurchaseEntry"] = model.Details;
            }
            if (id == 0 && purchaseOrderId > 0)
            {
                var query = db.PurchaseOrderRepo.GetById(purchaseOrderId);
                var model1 = AutomapperConfig.Mapper.Map<PurchaseOrderViewModel>(query);

                var vendor = (model.VendorId > 0) ? db.VendorRepo.GetById(model.VendorId) : new Vendor();
                model1.VendorDetails = AutomapperConfig.Mapper.Map<VendorViewModel>(vendor);

                foreach (var item in query.PurchaseOrderDetails)
                {
                    model1.Details.Add(AutomapperConfig.Mapper.Map<PurchaseOrderDetailViewModel>(item));
                }
                var selectedPurchaseOrderItem = new List<PurchaseEntryDetailViewModel>();
                model.PurchaseOrderId = purchaseOrderId;
                model.VendorId = model1.VendorId;
                foreach (var req in model1.Details)
                {
                    var item = db.ItemRepo.GetById(req.ItemId);
                    
                    var itemGroup = AutomapperConfig.Mapper.Map<ItemGroupViewModel>(item.ItemGroup);
                    //var itemGroup = imsMasterService.GetItemGroup(item.ItemGroupId);
                    var subCode = 0;
                    var transactionDetail = db.StockTransactionDetailRepo.Table().Where(x => x.ItemId == item.Id && !string.IsNullOrEmpty(x.ItemSubCodeNo)).Select(y => y.ItemSubCodeNo).ToList();
                    if (transactionDetail.Any())
                    {
                        subCode = transactionDetail.Where(x => x.Split('.').LastOrDefault().IsNumeric()).Select(x => Convert.ToInt32(x.Split('.').LastOrDefault())).Max();
                    }

                    var alreadyStoreEntryOfSamePOQty = db.StockTransactionDetailRepo.Table().Any(x => x.ItemId == req.ItemId && x.StockTransaction.PurchaseOrderId == purchaseOrderId) ? db.StockTransactionDetailRepo.Table().Where(x => x.ItemId == req.ItemId && x.StockTransaction.PurchaseOrderId == purchaseOrderId).Sum(x => x.Qty) : 0;
                    var remainingQty = req.Qty - alreadyStoreEntryOfSamePOQty;

                    for (int i = itemGroup.ItemTypeAlias.ToLower() == "non-consumable" ? remainingQty : 1; i >= 1; i--)
                    {
                        var _items = new PurchaseEntryDetailViewModel();
                        _items.ItemId = req.ItemId;
                        if (itemGroup.ItemTypeAlias.ToLower() == "non-consumable")
                        {
                            subCode++;
                            _items.ItemSubCodeNo = $"{item.Code}.{subCode}";
                        }
                        _items.ItemName = req.ItemName;
                        _items.ItemCode = req.ItemCode;
                        _items.UnitName = item.ItemUnit.Name;
                        _items.Qty = itemGroup.ItemTypeAlias.ToLower() == "non-consumable" ? 1 : req.Qty;
                        _items.Rate = req.Rate;
                        _items.VatPerQty = ((_items.Qty * _items.Rate) * 13 / 100);

                        _items.BasicAmount = req.BasicAmount / (itemGroup.ItemTypeAlias.ToLower() == "non-consumable" ? req.Qty : 1);
                        _items.NetAmount = (req.TotalAmount / (itemGroup.ItemTypeAlias.ToLower() == "non-consumable" ? req.Qty : 1)) + _items.VatPerQty ?? 0;
                        _items.Narration = req.Specification;
                        _items.DepartmentId = Convert.ToInt32(SessionData.GetSystemConfigurationValue("StoreSection"));
                        _items.Guid = Guid.NewGuid().ToString();

                        selectedPurchaseOrderItem.Add(_items);
                    }
                }
                Session["PurchaseEntry"] = selectedPurchaseOrderItem;
            }
            if (Session["PurchaseEntry"] == null)
                Session["PurchaseEntry"] = new List<PurchaseEntryDetailViewModel>();
            ViewBag.PurchaseOrderId = new SelectList(purchaseOrders, "Id", "DisplayDocumentNo", model.PurchaseOrderId);
            return View(model);
        }

        [HttpPost]
        public ActionResult PurchaseEntryEntry(PurchaseEntryViewModel model)
        {
            model.Details = (List<PurchaseEntryDetailViewModel>)Session["PurchaseEntry"];
            model.FiscalYearId = CurrentFiscalYear.FiscalYearId;
            model = helper.SavePurchaseEntry(model);
            
            if (model.HasError)
            {
                TempData["Errors"] = model.ErrorMessages;
                TempData["Exception"] = model.Exception;
                var purchaseOrders = helper.GetPurchaseOrders(CurrentFiscalYear.FiscalYearId, null, null, null, model.Id, true).Select(x => new { Id = x.Id, DisplayDocumentNo = x.DisplayDocumentNo }).ToList();
                ViewBag.PurchaseOrderId = new SelectList(purchaseOrders, "Id", "DisplayDocumentNo", model.PurchaseOrderId);
                model.DisplayDocumentNo = helper.GetDocumentNo(Enums.eDocumentSetup.StoreEntry);
                
                RefreshModelState(model.Errors);
                return View(model);
            }
            TempData["Success"] = "Data saved successfully.";
            model = new PurchaseEntryViewModel { DisplayDocumentNo = helper.GetDocumentNo(Enums.eDocumentSetup.Opening) };
            Session["PurchaseEntry"] = null;
            return RedirectToAction("purchaseentryentry", "imsentry", new { id = 0 });
        }

        public PartialViewResult StoreEntryReport(int id = 0)
        {
            var model = new PurchaseEntryViewModel();
            decimal total = 0;
            if (id > 0)
            {
                model = helper.GetPurchaseEntry(id);
                Session["PurchaseEntry"] = model.Details;
                if (model.Details.Count > 0)
                    total = model.Details.Sum(x => x.NetAmount);
            }
            if (Session["PurchaseEntry"] == null)
                Session["PurchaseEntry"] = new List<PurchaseEntryDetailViewModel>();
            ViewBag.NetAmount = total;
            return PartialView(model);
        }


        [HttpPost]
        public JsonResult AddPurchaseEntryDetail(int itemId, int qty, decimal rate, string narration)
        {
            List<string> errors = new List<string>();
            try
            {
                var item = db.ItemRepo.GetById(itemId);
                int subCode = 0;
                var transactionDetail = db.StockTransactionDetailRepo.Table().Where(x => x.ItemId == item.Id && !string.IsNullOrEmpty(x.ItemSubCodeNo)).Select(y => y.ItemSubCodeNo).ToList();
                if (transactionDetail.Any())
                {
                    subCode = transactionDetail.Where(x => x.Split('.').LastOrDefault().IsNumeric()).Select(x => Convert.ToInt32(x.Split('.').LastOrDefault())).Max();
                }
                var details = (List<PurchaseEntryDetailViewModel>)Session["PurchaseEntry"];
                if (details == null)
                    details = new List<PurchaseEntryDetailViewModel>();
                var loopTo = item.ItemGroup.ItemType.Alias.ToLower() == "non-consumable" ? qty : 1;

                for (int i = 0; i < loopTo; i++)
                {
                    var _items = new PurchaseEntryDetailViewModel();
                    _items.ItemId = item.Id;

                    if (item.ItemGroup.ItemType.Alias.ToLower() == "non-consumable")
                    {
                        subCode++;
                        _items.ItemSubCodeNo = $"{item.Code}.{subCode}";
                    }
                    _items.ItemName = item.Name;
                    _items.ItemCode = item.Code;
                    _items.UnitName = item.ItemUnit.Name;
                    _items.Qty = item.ItemGroup.ItemType.Alias.ToLower() == "non-consumable" ? 1 : qty;
                    _items.Rate = rate;
                    _items.VatPerQty = ((_items.Qty * _items.Rate) * 13 / 100);

                    _items.BasicAmount = _items.Rate * _items.Qty / (item.ItemGroup.ItemType.Alias.ToLower() == "non-consumable" ? 1 : qty);
                    _items.NetAmount = _items.BasicAmount + _items.VatPerQty ?? 0;
                    _items.Narration = narration;
                    _items.DepartmentId = Convert.ToInt32(SessionData.GetSystemConfigurationValue("StoreSection"));
                    _items.Guid = Guid.NewGuid().ToString();

                    details.Add(_items);
                }

                Session["PurchaseEntry"] = details;
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { error = errors });
        }

        [HttpPost]
        public JsonResult RemovePurchaseEntryItemList(string guid)
        {
            var details = (List<PurchaseEntryDetailViewModel>)Session["PurchaseEntry"];
            details.Remove(details.FirstOrDefault(x => x.Guid == guid));
            Session["PurchaseEntry"] = details;
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { success = true });
        }

        public PartialViewResult PurchaseEntryData()
        {
            var model = (List<PurchaseEntryDetailViewModel>)Session["PurchaseEntry"];
            return PartialView(model);
        }

        public ActionResult AddStockItemFromPurchaseOrderReference(int purchaseOrderId, int? stockTransactionId)
        {
            var data = helper.GetPurchaseOrder(purchaseOrderId);

            if (stockTransactionId.HasValue)
            {
                var model = helper.GetPurchaseEntry((int)stockTransactionId);
                if (model.PurchaseOrderId == purchaseOrderId)
                {
                    Session["PurchaseEntry"] = model.Details;
                    return RedirectToAction("PurchaseEntryItemList", "ImsEntry");
                }
            }
            var selectedPurchaseOrderItem = new List<PurchaseEntryDetailViewModel>();

            foreach (var req in data.Details)
            {
                var model = new PurchaseEntryDetailViewModel();
                model.ItemId = req.ItemId;
                var item = db.ItemRepo.GetById(req.ItemId);
                var itemGroup = db.ItemGroupRepo.GetById(item.ItemGroupId);
                model.ItemName = req.ItemName;
                model.ItemCode = req.ItemCode;
                model.UnitName = item.ItemUnit.Name;
                model.Qty = req.Qty;
                model.Rate = req.Rate;
                model.VatPerQty = 0;
                model.BasicAmount = req.BasicAmount;
                model.NetAmount = req.TotalAmount;
                model.Narration = req.Specification;
                model.Guid = Guid.NewGuid().ToString();
                selectedPurchaseOrderItem.Add(model);
            }

            Session["PurchaseEntry"] = selectedPurchaseOrderItem;
            return RedirectToAction("purchaseentryitemlist");
        }

        [HttpGet]
        public JsonResult UpdatePurchaseItem(string guid, string itemSubCodeNo, int DepartmentId, string narration, int qty, decimal rate, decimal? vatPerQty, decimal basicAmount, decimal netAmount, string ledgerpageno)
        {
            try
            {
                var details = (List<PurchaseEntryDetailViewModel>)Session["PurchaseEntry"];
                var model = details.Where(x => x.Guid == guid).FirstOrDefault();
                model.ItemSubCodeNo = itemSubCodeNo;
                model.DepartmentId = DepartmentId;
                model.Narration = narration;
                model.Qty = qty;
                model.Rate = rate;
                model.VatPerQty = vatPerQty;
                model.BasicAmount = basicAmount;
                model.NetAmount = netAmount;
                model.LedgerPageNo = ledgerpageno;
                model.Guid = Guid.NewGuid().ToString();
                Session["PurchaseEntry"] = details;
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.SeeOther;
                return Json(new { error = ex.GetExceptionMessages() }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult StockEntryPrint(int id = 0)
        {
            var model = new PurchaseEntryViewModel();

            if (id > 0)
            {
                model = helper.GetPurchaseEntry(id);
                Session["PurchaseEntry"] = model.Details;
            }
            if (Session["PurchaseEntry"] == null)
                Session["PurchaseEntry"] = new List<PurchaseEntryDetailViewModel>();

            return View(model);
        }

        public PartialViewResult PrintStockEntryList()
        {
            var model = (List<PurchaseEntryDetailViewModel>)Session["PurchaseEntry"];
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult GetVendorByPurchaseOrder(int? purchaseOrderId)
        {
            var data = db.PurchaseOrderRepo.GetById(purchaseOrderId ?? 0);
            return Json(data == null ? 0 : data.VendorId);
        }

        #endregion PurchaseEntry

        #region Item Release
        public ActionResult ItemReleaseList()
        {
            Session["ItemReleaseDetail"] = null;
            return View();
        }

        public PartialViewResult ItemReleaseDetail(string displayReleaseNo, string dateFromBS, string dateToBS)
        {
            var dateFrom = dateFromBS.GetDate();
            var dateTo = dateToBS.GetDate();

            var model = helper.GetItemReleases(CurrentFiscalYear.FiscalYearId, displayReleaseNo, dateFrom: dateFrom == DateTime.MinValue ? (DateTime?)null : dateFrom, dateTo: dateTo == DateTime.MinValue ? (DateTime?)null : dateTo);
            return PartialView(model);
        }

        public ActionResult ItemReleaseEntry(int itemRequestId, int id = 0)
        {
            var model = new ItemReleaseViewModel { DisplayReleaseNo = helper.GetDocumentNo(Enums.eDocumentSetup.ItemRelease) };
            ViewBag.ItemRequestIds = "";
            //Session["ItemReleaseDetail"] = null;
            //var data1 = db.ItemReleaseRepo.Get(x => x.ItemRequestId == itemRequestId);
            //if (data1.Any())
            //{
            //    TempData["Errors"] = "Item Request is already released";
            //    return Redirect("itemrequestlist");
            //}

            ViewBag.ItemRequestList = new SelectList(db.ItemRequestRepo.Get(x => x.Id == itemRequestId), "Id", "DisplayDocumentNo", itemRequestId);
            if (id > 0)
            {
                model = helper.GetItemRelease(id);
                Session["ItemReleaseDetail"] = model.Details;
                string itemRequestIds = string.Join(",", model.Details.Select(x => x.ItemRequestIdCSV).Distinct());
                ViewBag.ItemRequestIds = itemRequestIds;
            }
            if (id == 0 && itemRequestId > 0)
            {
                
                var data = helper.GetItemRequest(itemRequestId);
                var selectedRequestItem = new List<ItemReleaseDetailViewModel>();       
                ViewBag.ItemRequestIds = itemRequestId;
                if (data != null)
                {
                    
                    model.EmployeeId = data.EmployeeId;
                    model.DepartmentId = data.DepartmentId;
                    //Get with FIFO
                    var requestedItems = db.ItemRequestRepo.GetById(itemRequestId).ItemRequestDetails.Where(x=>!x.ItemReleaseId.HasValue);
                    foreach (var d in requestedItems.Select(x => x.Item).Distinct())
                    {
                       
                        
                        var item = new ItemReleaseDetailViewModel();
                       
                        var qty = requestedItems.Where(x => x.ItemId == d.Id).Sum(y => y.Qty);
                        var releasedData = db.ItemReleaseDetailRepo.Table().Where(x => x.ItemId == d.Id && x.ItemRelease.ItemRequestId == itemRequestId);
                        var releasedQty = releasedData.Any() ? releasedData.Sum(y => y.Qty) : 0;
                        qty = qty - releasedQty;
                        if (d.ItemGroup.ItemType.Alias == "Non-Consumable")
                        {
                            var releasedSubcode = d.ItemReleaseDetails.Where(x => x.ItemRelease.FiscalYearId == CurrentFiscalYear.FiscalYearId).
                                Select(x => x.SubCode).ToList();
                            //opening of another section
                            var storeSectionId = Convert.ToInt32(SessionData.GetSystemConfigurationValue("StoreSection"));
                            releasedSubcode.AddRange(d.StockTransactionDetails.Where(x => x.StockTransaction.FiscalYearId == CurrentFiscalYear.FiscalYearId && x.StockTransaction.DocumentSetup.Alias == "Opening" && x.DepartmentId != storeSectionId)
                                .Select(x => x.ItemSubCodeNo));

                            var itemData = db.StockTransactionDetailRepo.Table().Where(x => x.ItemId == d.Id && x.StockTransaction.FiscalYearId == CurrentFiscalYear.FiscalYearId &&
                                (x.StockTransaction.DocumentSetup.Alias == "StoreEntry" ||
                                x.StockTransaction.DocumentSetup.Alias == "Opening") &&
                                !releasedSubcode.Contains(x.ItemSubCodeNo)).OrderBy(x => x.Id).Take(qty).Select(x => x.ItemSubCodeNo).ToList();

                            foreach (var ln in itemData)
                            {
                                
                                item = new ItemReleaseDetailViewModel();
                                item.Qty = 1;
                                item.ItemId = d.Id;
                                item.ItemCode = d.Code;
                                item.ItemName = d.Name;
                                item.UnitName = d.ItemUnit.Name;
                                item.InStockQty = d.Id.ItemInStock();
                                item.SubCode = ln;                               
                                selectedRequestItem.Add(item);
                            }

                        }
                        else
                        {
                            item = new ItemReleaseDetailViewModel();
                            item.Qty = 1;
                            item.ItemId = d.Id;
                            item.ItemCode = d.Code;
                            item.ItemName = d.Name;
                            item.UnitName = d.ItemUnit.Name;
                            item.InStockQty = d.Id.ItemInStock();
                            selectedRequestItem.Add(item);
                        }
                    }
                }
                ViewBag.employeeList = new SelectList(db.EmployeeRepo.Table().Where(x => x.Id == data.EmployeeId),"Id","Name",data.EmployeeId);
    



                    Session["ItemReleaseDetail"] = selectedRequestItem;
            }

            if (Session["ItemReleaseDetail"] == null)
                Session["ItemReleaseDetail"] = new List<ItemReleaseDetailViewModel>();
            return View(model);
        }

        [HttpPost]
        public ActionResult ItemReleaseEntry(ItemReleaseViewModel model)
        {
            model.Details = (List<ItemReleaseDetailViewModel>)Session["ItemReleaseDetail"];
            foreach (var item in model.Details)
            {
                if (item.Qty> item.InStockQty)
                {
                    TempData["Errors"] = "Request item is greater than the item in stock!!";
                    return Redirect(Request.UrlReferrer.ToString());
                }
            }
            model.CurrentEmployeeId = CurrentUser.EmployeeId;            
            model.FiscalYearId = CurrentFiscalYear.FiscalYearId;
            model.DepartmentId = CurrentUser.DepartmentId;
          
            model = helper.SaveItemRelease(model);
            if (model.HasError)
            {
                TempData["Errors"] = model.ErrorMessages;
                TempData["Exception"] = model.Exception;
                RefreshModelState(model.Errors);
                return View(model);
            }
            TempData["SuccessMessage"] = "Item Release saved successfully.";
            model = new ItemReleaseViewModel { DisplayReleaseNo = helper.GetDocumentNo(Enums.eDocumentSetup.ItemRelease) };
            Session["ItemReleaseDetail"] = null;
            return RedirectToAction("itemreleaselist");
        }

        [HttpPost]
        public JsonResult AddItemReleaseDetail(int itemId, string subCode, int qty, string narration)
        {
            List<string> errors = new List<string>();
            try
            {
                if (ModelState.IsValid)
                {
                    var details = (List<ItemReleaseDetailViewModel>)Session["ItemReleaseDetail"];
                    var model = new ItemReleaseDetailViewModel();
                    model.Guid = Guid.NewGuid();
                    model.ItemId = itemId;
                    var item = db.ItemRepo.GetById(itemId);
                    model.ItemName = item.Name;
                    model.SubCode = subCode;
                    model.ItemCode = item.Code;
                    var itemGroup = db.ItemGroupRepo.GetById(item.ItemGroupId);
                    model.UnitName = item.ItemUnit.Name;
                    model.Qty = qty;
                    model.Narration = narration;
                    model.InStockQty = helper.GetItemStatusByItemId(itemId, DateTime.Now, CurrentFiscalYear.FiscalYearId).InStockQuantity;

                    details.Add(model);
                    Session["ItemReleaseDetail"] = details;
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { success = true });
                }
                foreach (var item in ModelState.Where(x => x.Value.Errors.Any()))
                {
                    errors.Add(item.Value.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { error = errors });
        }

        [HttpPost]
        public JsonResult RemoveItemReleaseItemList(Guid guid)
        {
            var details = (List<ItemReleaseDetailViewModel>)Session["ItemReleaseDetail"];
            details.Remove(details.FirstOrDefault(x => x.Guid == guid));
            Session["ItemReleaseDetail"] = details;
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { success = true });
        }

        public PartialViewResult ItemReleaseItemList()
        {
            var model = (List<ItemReleaseDetailViewModel>)Session["ItemReleaseDetail"];
            return PartialView(model);
        }

        public JsonResult UpdateReleaseItem(Guid guid, int qty, string subCode, string narration)
        {
            try
            {
                var details = (List<ItemReleaseDetailViewModel>)Session["ItemReleaseDetail"];
                var model = details.Where(x => x.Guid == guid).FirstOrDefault();
                model.Qty = qty;
                model.Narration = narration;
                model.SubCode = subCode;
                Session["ItemReleaseDetail"] = details;
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.SeeOther;
                return Json(new { error = ex.GetExceptionMessages() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteItemRelease(int id, bool deleteChildItem = false)
        {
            if (deleteChildItem)
            {
                var models = db.ItemReleaseDetailRepo.Table().Where(x => x.ItemReleaseId == id);
                db.ItemReleaseDetailRepo.DeleteRange(models);
            }
            string errorMsg = null;
            var errors = db.ItemReleaseRepo.TryDelete(id, out errorMsg);
            if (errors.Any())
            {
                Response.StatusCode = (int)HttpStatusCode.SeeOther;
                var confirmChild = errorMsg.Contains("another");
                return Json(new { confirmChild = confirmChild, success = false, errors = errors, displayMessage = errorMsg });
            }
            return Json("");
        }

        public ActionResult AddItemFromRequestReferenceForItemRelease(int employeeId, int[] requestId)
        {
            var data = helper.GetItemRequests(CurrentFiscalYear.FiscalYearId, employeeId, "", null, null, false, true);
            var model = (List<ItemReleaseDetailViewModel>)Session["ItemReleaseDetail"];

            var selectedRequestItem = new List<ItemReleaseDetailViewModel>();
            var filteredData = data.Where(x => requestId.Contains(x.Id));
            foreach (var req in filteredData)
            {
                foreach (var item in req.Details)
                {
                    if (selectedRequestItem.Any(x => x.ItemId == item.ItemId))
                    {
                        selectedRequestItem.FirstOrDefault(x => x.ItemId == item.ItemId).Qty += item.Qty;
                        selectedRequestItem.FirstOrDefault(x => x.ItemId == item.ItemId).ItemRequestIdCSV += "," + item.ItemRequestId;
                        selectedRequestItem.FirstOrDefault(x => x.ItemId == item.ItemId).ItemRequestDetailIdCSV += "," + item.Id;
                    }
                    else
                    {
                        var i = AutomapperConfig.Mapper.Map<ItemReleaseDetailViewModel>(item);
                        i.ItemRequestIdCSV = item.ItemRequestId.ToString();
                        i.ItemRequestDetailIdCSV = item.Id.ToString();
                        i.InStockQty = helper.GetItemStatusByItemId(i.ItemId, DateTime.Now, CurrentFiscalYear.FiscalYearId).InStockQuantity;
                        selectedRequestItem.Add(i);
                    }
                }
            }

            var selectedItems = selectedRequestItem.Select(x => x.ItemId);
            model.RemoveAll(x => selectedItems.Contains(x.ItemId));
            model.AddRange(selectedRequestItem);

            Session["ItemReleaseDetail"] = model;
            return RedirectToAction("ItemReleaseItemList");
        }
        #endregion Item Release

        [HttpPost]
        public PartialViewResult ReleasedItemDetail(int? itemReleaseId)
        {
            var data = db.ItemReleaseDetailRepo.Table().Where(x => x.ItemReleaseId == itemReleaseId);
            var model = AutomapperConfig.Mapper.Map<IList<TransactionItemViewModel>>(data);
            return PartialView(model);
        }

        [HttpPost]
        public PartialViewResult DemandItemDetail(int? itemRequestId)
        {
            var data = db.ItemRequestDetailRepo.Table().Where(x => x.ItemRequestId == itemRequestId);
            var model = AutomapperConfig.Mapper.Map<IList<TransactionItemViewModel>>(data);
            return PartialView(model);
        }


    }
}