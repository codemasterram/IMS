﻿using SoftIms.Data;
using SoftIms.Data.Infrastructure;
using SoftIms.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SoftIms.Utilities;
using static SoftIms.Utilities.Utility;
using static SoftIms.Utilities.ViewModelExtension;
using static SoftIms.Data.Enums;

namespace SoftIms.Controllers
{
    public class MasterController : BaseController
    {
        #region Index
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Department
        public ActionResult Department()
        {
            return View();
        }

        public PartialViewResult DepartmentDetail(string name)
        {
            var data = db.DepartmentRepo.Get(x => (string.IsNullOrEmpty(name.Trim()) || x.Name.StartsWith(name.Trim())))
                        .OrderBy(x => x.DisplayOrder);
            var model = AutomapperConfig.Mapper.Map<IList<DepartmentListViewModel>>(data);
            return PartialView(model);
        }

        public PartialViewResult CreateDepartment(int id = 0)
        {
            var model = new DepartmentViewModel();
            if (id > 0)
                model = AutomapperConfig.Mapper.Map<DepartmentViewModel>(db.DepartmentRepo.GetById(id));
            if (model == null)
                return PartialView(BadRequestView);
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult CreateDepartment(DepartmentViewModel model)
        {
            var errors = new List<string>();
            try
            {
                if (DuplicateDepartment(model.Id, model.Name) != null)
                    ModelState.AddModelError("Name", $"डिपार्टमेन्ट {model.Name} पहिलेनै सुरक्षित छ ।");
                if (ModelState.IsValid)
                {
                    if (model.Id == 0)
                    {
                        var dept = db.DepartmentRepo.Get(x => x.DataStatus == (int)eDataStatus.Active);
                        var order = dept.Any() ? dept.Max(x => x.DisplayOrder) : 0;
                        model.DisplayOrder = order + 1;
                        model.Id = db.DepartmentRepo.Create(AutomapperConfig.Mapper.Map<Department>(model));
                    }
                    else
                    {
                        var oldModel = db.DepartmentRepo.GetById(model.Id);
                        if (oldModel == null)
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            return Json(new { redirecturl = "/error/badrequest" });
                        }
                        oldModel.Name = model.Name;
                    }
                    db.SaveChanges();
                    return Json("");
                }
                foreach (var item in ModelState.Where(x => x.Value.Errors.Any()))
                {
                    errors.Add(item.Value.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.GetExceptionMessages());
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { error = errors, displayError = "Cannot save data." });
        }

        [HttpPost]
        public JsonResult DepartmentNotExist(int id, string name)
        {
            return Json(DuplicateDepartment(id, name) == null);
        }

        private Department DuplicateDepartment(int id, string name)
        {
            var model = db.DepartmentRepo.Get(x => x.Id != id && x.DataStatus == (int)eDataStatus.Active && x.Name.Trim() == name.Trim()).FirstOrDefault();
            return model;
        }

        [HttpPost]
        public JsonResult DeleteDepartment(int id)
        {
            var model = db.DepartmentRepo.GetById(id);
            model.DataStatus = (int)eDataStatus.Deleted;
            return Json("");
        }
        #endregion 

        #region ItemGroup

        public ActionResult ItemGroup()
        {
            return View();
        }

        public PartialViewResult ItemGroupDetail(string name)
        {
            var data = db.ItemGroupRepo.Get(x => (string.IsNullOrEmpty(name.Trim()) || x.Name.StartsWith(name.Trim()))).OrderBy(x => x.DisplayOrder);
            var model = AutomapperConfig.Mapper.Map<IEnumerable<ItemGroup>, IEnumerable<ItemGroupListViewModel>>(data);

            return PartialView(model);
        }

        public PartialViewResult CreateItemGroup(int id = 0)
        {
            var model = new ItemGroupViewModel();
            if (id > 0)
                model = AutomapperConfig.Mapper.Map<ItemGroupViewModel>(db.ItemGroupRepo.GetById(id));
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return PartialView(model);
            }

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult CreateItemGroup(ItemGroupViewModel model)
        {
            List<string> errors = new List<string>();
            try
            {
                if (DuplicateItemGroup(model.Id, model.Name) != null)
                {
                    ModelState.AddModelError("Name", $"Item Group {model.Name} already exists.");
                }

                if (ModelState.IsValid)
                {
                    if (model.Id == 0)
                    {
                        var itemG = db.ItemGroupRepo.Table();
                        var order = itemG.Any() ? itemG.Max(x => x.DisplayOrder) : 0;
                        model.DisplayOrder = order + 1;
                        model.Id = db.ItemGroupRepo.Create(AutomapperConfig.Mapper.Map<ItemGroup>(model));
                    }
                    else
                    {
                        var oldmodel = db.ItemGroupRepo.GetById(model.Id);
                        if (oldmodel == null)
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            return Json(new { redirecturl = "/error/badrequest" });
                        }
                        oldmodel.Name = model.Name;
                        oldmodel.Code = model.Code;
                        oldmodel.ItemTypeId = model.ItemTypeId;

                    }
                    db.SaveChanges();
                    TempData["Success"] = "Item Group Successfully Saved";
                    return RedirectToAction("itemgroup");

                }
                foreach (var item in ModelState.Where(x => x.Value.Errors.Any()))
                {
                    errors.Add(item.Value.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.GetExceptionMessages());
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteItemGroup(int id)
        {
            List<string> errors = new List<string>();
            string fkMessage = string.Empty;
            var model = db.ItemGroupRepo.GetById(id);
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = false, redirecturl = "/error/badrequest" });
            }

            try
            {
                db.ItemGroupRepo.Delete(model);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                if (ex.GetExceptionMessages().Contains("REFERENCE"))
                {
                    fkMessage = ex.GetExceptionMessages();
                    errors.Add("This group is already in use. Used item group cannot be deleted");
                }
                else
                {
                    errors.Add(ex.GetExceptionMessages());
                }
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { success = false, message = fkMessage, error = errors });

        }

        [HttpPost]
        public JsonResult ItemGroupNotExist(int id, string name)
        {
            return Json(DuplicateItemGroup(id, name) == null);
        }

        private ItemGroup DuplicateItemGroup(int id, string name)
        {
            var model = db.ItemGroupRepo.Get(x => x.Id != id && x.Name.Trim() == name.Trim()).FirstOrDefault();
            return model;
        }
        #endregion

        #region Designation

        public ActionResult Designation()
        {
            return View();
        }

        public PartialViewResult DesignationDetail(string name)
        {
            var data = db.DesignationRepo.Get(x => (string.IsNullOrEmpty(name.Trim()) || x.Name.StartsWith(name.Trim()))).OrderBy(x => x.DisplayOrder);
            var model = AutomapperConfig.Mapper.Map<IEnumerable<Designation>, IEnumerable<DesignationListViewModel>>(data);

            return PartialView(model);
        }

        public PartialViewResult CreateDesignation(int id=0)
        {
            var model = new DesignationViewModel();
            if (id > 0)
                model = AutomapperConfig.Mapper.Map<DesignationViewModel>(db.DesignationRepo.GetById(id));
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return PartialView(BadRequestView);
            }
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult CreateDesignation(DesignationViewModel model)
        {
            List<string> errors = new List<string>();
            try
            {
                if (DuplicateDesignation(model.Id, model.Name) != null)
                {
                    ModelState.AddModelError("Name", $"Item Group {model.Name} already exists.");
                }

                if (ModelState.IsValid)
                {
                    if (model.Id == 0)
                    {
                        var itemD = db.DesignationRepo.Table();
                        var order = itemD.Any() ? itemD.Max(x => x.DisplayOrder) : 0;
                        model.DisplayOrder = order + 1;
                        model.Id = db.DesignationRepo.Create(AutomapperConfig.Mapper.Map<Designation>(model));
                    }
                    else
                    {
                        var oldmodel = db.DesignationRepo.GetById(model.Id);
                        if (oldmodel == null)
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            return Json(new { redirecturl = "/error/badrequest" });
                        }
                        oldmodel.Name = model.Name;

                    }
                    db.SaveChanges();
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { message = "Designation successfully created" });

                }
                foreach (var item in ModelState.Where(x => x.Value.Errors.Any()))
                {
                    errors.Add(item.Value.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.GetExceptionMessages());
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { error = errors });
        }

        [HttpPost]
        public JsonResult DeleteDesignation(int id)
        {
            List<string> errors = new List<string>();
            string fkMessage = string.Empty;
            var model = db.DesignationRepo.GetById(id);
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { redirecturl = "/error/badrequest" });
            }
            try
            {
                db.DesignationRepo.Delete(model);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                if (ex.GetExceptionMessages().Contains("REFERENCE"))
                {
                    fkMessage = ex.GetExceptionMessages();
                    errors.Add("This group is already in use. Used designarion group cannot be deleted");
                }
                else
                {
                    errors.Add(ex.GetExceptionMessages());

                }
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { fkMessage = fkMessage, error = errors });
        }

        [HttpPost]
        public JsonResult DesignationNotExist(int id, string name)
        {
            return Json(DuplicateDesignation(id, name) == null);
        }

        private ItemGroup DuplicateDesignation(int id, string name)
        {
            var model = db.ItemGroupRepo.Get(x => x.Id != id && x.Name.Trim() == name.Trim()).FirstOrDefault();
            return model;
        }
        #endregion

        #region ItemUnit
        public ActionResult ItemUnit()
        {
            return View();
        }

        public PartialViewResult ItemUnitDetail(string name)
        {
            var data = db.ItemUnitRepo.Get(x => (string.IsNullOrEmpty(name.Trim()) || x.Name.StartsWith(name.Trim()))).OrderBy(x => x.DisplayOrder);
            var model = AutomapperConfig.Mapper.Map<IEnumerable<ItemUnit>, IEnumerable<ItemUnitListViewModel>>(data);

            return PartialView(model);
        }

        public PartialViewResult CreateItemUnit(int id = 0)
        {
            var model = new ItemUnitViewModel();
            if (id > 0)
                model = AutomapperConfig.Mapper.Map<ItemUnitViewModel>(db.ItemUnitRepo.GetById(id));
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return PartialView(model);
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult CreateItemUnit(ItemUnitViewModel model)
        {
            List<string> errors = new List<string>();
            try
            {
                if (DuplicateItemUnit(model.Id, model.Name) != null)
                {
                    ModelState.AddModelError("Name", $"Item Unit {model.Name} already exists.");
                }
                if (ModelState.IsValid)
                {
                    if (model.Id == 0)
                    {
                        var itemU = db.ItemUnitRepo.Table();
                        var order = itemU.Any() ? itemU.Max(x => x.DisplayOrder) : 0;
                        model.DisplayOrder = order + 1;
                        model.Id = db.ItemUnitRepo.Create(AutomapperConfig.Mapper.Map<ItemUnit>(model));
                    }
                    else
                    {
                        var oldmodel = db.ItemUnitRepo.GetById(model.Id);
                        if (oldmodel == null)
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            return Json(new { redirecturl = "/error/badrequest" });
                        }
                        oldmodel.Name = model.Name;
                        oldmodel.Code = model.Code;
                    }
                    db.SaveChanges();
                    TempData["Success"] = "Item Unit Successfully Saved";
                    return RedirectToAction("itemunit");
                }
                foreach (var item in ModelState.Where(x => x.Value.Errors.Any()))
                {
                    errors.Add(item.Value.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.GetExceptionMessages());
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteItemUnit(int id)
        {
            List<string> errors = new List<string>();
            string fkMessage = string.Empty;
            var model = db.ItemUnitRepo.GetById(id);
            if (model==null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, message = "/error/badrequest" });
            }
            try
            {
                db.ItemUnitRepo.Delete(model);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                if (ex.GetExceptionMessages().Contains("REFERENCE"))
                {
                    fkMessage = ex.GetExceptionMessages();
                    errors.Add("This Item Unit is already in use. Used Item Unit cannot be deleted");
                }
                else
                {
                    errors.Add(ex.GetExceptionMessages());
                }
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { success = false, message = fkMessage, error = errors });
        }

        [HttpPost]
        public JsonResult ItemUnitNotExist(int id, string name)
        {
            return Json(DuplicateItemUnit(id, name) == null);
        }

        private ItemUnit DuplicateItemUnit(int id, string name)
        {
            var model = db.ItemUnitRepo.Get(x => x.Id != id && x.Name.Trim() == name.Trim()).FirstOrDefault();
            return model;
        }
        #endregion

        #region Item

        public ActionResult Item()
        {
            return View();
        }

        public PartialViewResult ItemDetail(string name, int? itemgroupid)
        {
            var data = db.ItemRepo.Get(x => (string.IsNullOrEmpty(name.Trim()) || x.Name.StartsWith(name.Trim()))&& ( !itemgroupid.HasValue || itemgroupid.Value == 0 || x.ItemGroupId == itemgroupid)).OrderBy(x => x.DisplayOrder);
            var model = AutomapperConfig.Mapper.Map<IEnumerable<Item>, IEnumerable<ItemListViewModel>>(data);

            return PartialView(model);
        }
        public PartialViewResult CreateItem(int id = 0)
        {
            var model = new ItemViewModel();
            if (id > 0)
                model = AutomapperConfig.Mapper.Map<ItemViewModel>(db.ItemRepo.GetById(id));
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return PartialView(BadRequestView);
            }
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult CreateItem(ItemViewModel model)
        {
            List<string> errors = new List<string>();
            try
            {
                if (DuplicateItem(model.Id,model.ItemGroupId, model.Name) != null)
                {
                    ModelState.AddModelError("Name", $"Item  {model.Name} already exists.");
                }

                if (ModelState.IsValid)
                {
                    if (model.Id == 0)
                    {
                        var itemD = db.ItemRepo.Table();
                        var order = itemD.Any() ? itemD.Max(x => x.DisplayOrder) : 0;
                        model.DisplayOrder = order + 1;
                        model.Id = db.ItemRepo.Create(AutomapperConfig.Mapper.Map<Item>(model));
                    }
                    else
                    {
                        var oldmodel = db.ItemRepo.GetById(model.Id);
                        if (oldmodel == null)
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            return Json(new { redirecturl = "/error/badrequest" });
                        }
                        oldmodel.Name = model.Name;
                        oldmodel.Code = model.Code;
                        oldmodel.ItemGroupId = model.ItemGroupId;
                        oldmodel.ItemUnitId = model.ItemUnitId;

                    }
                    db.SaveChanges();
                    TempData["Success"] = "Item Successfully Saved";
                    return RedirectToAction("item");

                }
                foreach (var item in ModelState.Where(x => x.Value.Errors.Any()))
                {
                    errors.Add(item.Value.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.GetExceptionMessages());
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteItem(int id)
        {
            List<string> errors = new List<string>();
            string fkMessage = string.Empty;
            var model = db.ItemRepo.GetById(id);
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = false, message = "invalid request" });
            }
            try
            {
                db.ItemRepo.Delete(model);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                if (ex.GetExceptionMessages().Contains("REFERENCE"))
                {
                    fkMessage = ex.GetExceptionMessages();
                    errors.Add("Item is already in use. Used item  cannot be deleted");
                }
                else
                {
                    errors.Add(ex.GetExceptionMessages());

                }
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { success = false, message = fkMessage, error = errors });
        }


        [HttpPost]
        public JsonResult ItemNotExist(int id, int itemGroupId, string name)
        {
            return Json(DuplicateItem(id, itemGroupId, name) == null);
        }

        private Item DuplicateItem(int id, int itemGroupId, string name)
        {
            var model = db.ItemRepo.Get(x => x.Id != id && x.Name.Trim() == name.Trim() && x.ItemGroupId != itemGroupId).FirstOrDefault();
            return model;
        }

        [HttpGet]
        public JsonResult GetItem(int? itemGroupid, object selectedValue = null)
        {
            var data = ViewHelper.GetItemList(itemGroupid, selectedValue);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region DepreciationRate

        public ActionResult DepreciationRate()
        {
            return View();
        }

        public ActionResult DepreciationRateDetail(int? ItemGroupId)
        {
            var data = db.DepreciationRateRepo.Table().Where(x=>x.ItemGroupId==ItemGroupId);
            var model = AutomapperConfig.Mapper.Map<IEnumerable<DepreciationRate>, IEnumerable<DepreciationRateListViewModel>>(data);
            ViewBag.ItemGroupId = ItemGroupId;
            return View(model);
        }
        public PartialViewResult CreateDepreciationRate(int itemGroupId ,int id = 0)
        {
            var model = new DepreciationRateViewModel()
            {
                ItemGroupId = itemGroupId
            };
            if (id > 0)
                model = AutomapperConfig.Mapper.Map<DepreciationRateViewModel>(db.DepreciationRateRepo.GetById(id));
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return PartialView(BadRequestView);
            }
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult CreateDepreciationRate(DepreciationRateViewModel model)
        {
            List<string> errors = new List<string>();
            try
            {
                if (DuplicateDepreciationRate(model.Id, model.ItemGroupId, model.FiscalYearId) != null)
                {
                    var ficsalyearname = db.FiscalYearRepo.GetById(model.FiscalYearId);
                    ModelState.AddModelError("FiscalYearId", $"Depreciation Rate For  {ficsalyearname.Name} already exists.");
                }

                if (ModelState.IsValid)
                {
                    if (model.Id == 0)
                    {
                        var itemD = db.DepreciationRateRepo.Table();
                        model.Id = db.DepreciationRateRepo.Create(AutomapperConfig.Mapper.Map<DepreciationRate>(model));
                    }
                    else
                    {
                        var oldmodel = db.DepreciationRateRepo.GetById(model.Id);
                        if (oldmodel == null)
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            return Json(new { redirecturl = "/error/badrequest" });
                        }
                        oldmodel.Rate = model.Rate;

                    }
                    db.SaveChanges();
                    TempData["Success"] = "Depreciation Rate Successfully Saved";
                    return RedirectToAction("DepreciationRateDetail",new { ItemGroupId=model.ItemGroupId});

                }
                foreach (var item in ModelState.Where(x => x.Value.Errors.Any()))
                {
                    errors.Add(item.Value.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.GetExceptionMessages());
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteDepreciationRate(int id)
        {
            List<string> errors = new List<string>();
            string fkMessage = string.Empty;
            var model = db.DepreciationRateRepo.GetById(id);
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = false, message = "invalid request" });
            }
            try
            {
                db.DepreciationRateRepo.Delete(model);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                if (ex.GetExceptionMessages().Contains("REFERENCE"))
                {
                    fkMessage = ex.GetExceptionMessages();
                    errors.Add("Depreciation Rate is already in use. Used Depreciation Rate  cannot be deleted");
                }
                else
                {
                    errors.Add(ex.GetExceptionMessages());

                }
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { success = false, message = fkMessage, error = errors });
        }


        [HttpPost]
        public JsonResult DepreciationRateNotExist(int id, int itemGroupId, int FiscalYearId)
        {
            return Json(DuplicateDepreciationRate(id, itemGroupId, FiscalYearId) == null);
        }

        private DepreciationRate DuplicateDepreciationRate(int id, int itemGroupId, int FiscalYearId)
        {
            var model = db.DepreciationRateRepo.Get(x =>x.ItemGroupId == itemGroupId && x.FiscalYearId == FiscalYearId ).FirstOrDefault();
            return model;
        }
        #endregion

        #region ItemType

        public ActionResult ItemType()
        {
            return View();
        }

        public PartialViewResult ItemTypeDetail(string name)
        {
            var data = db.ItemTypeRepo.Get(x => (string.IsNullOrEmpty(name.Trim()) || x.Name.StartsWith(name.Trim()))).OrderBy(x => x.DisplayOrder);
            var model = AutomapperConfig.Mapper.Map<IEnumerable<ItemType>, IEnumerable<ItemTypeListViewModel>>(data);

            return PartialView(model);
        }

        public PartialViewResult CreateItemType(int id)
        {
            var model = new ItemTypeViewModel();
            if (id > 0)
                model = AutomapperConfig.Mapper.Map<ItemTypeViewModel>(db.ItemTypeRepo.GetById(id));
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return PartialView(BadRequestView);
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult CreateItemType(ItemType model)
        {
            List<string> errors = new List<string>();
            try
            {
                if (DuplicateItemType(model.Id, model.Name) != null)
                {
                    ModelState.AddModelError("Name", $"Item Type {model.Name} already exists.");
                }

                if (ModelState.IsValid)
                {
                    if (model.Id == 0)
                    {
                        var itemD = db.ItemTypeRepo.Table();
                        var order = itemD.Any() ? itemD.Max(x => x.DisplayOrder) : 0;
                        model.DisplayOrder = order + 1;
                        model.Id = db.ItemTypeRepo.Create(AutomapperConfig.Mapper.Map<ItemType>(model));
                    }
                    else
                    {
                        var oldmodel = db.ItemTypeRepo.GetById(model.Id);
                        if (oldmodel == null)
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            return Json(new { redirecturl = "/error/badrequest" });
                        }
                        oldmodel.Name = model.Name;
                        oldmodel.Alias = model.Alias;


                    }
                    db.SaveChanges();
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    TempData["Success"] = "Item Type Successfully Saved";
                    return RedirectToAction("itemtype");
                }
                foreach (var item in ModelState.Where(x => x.Value.Errors.Any()))
                {
                    errors.Add(item.Value.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.GetExceptionMessages());
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return View(model);
        }


        [HttpPost]
        public JsonResult DeleteItemType(int id)
        {
            List<string> errors = new List<string>();
            string fkMessage = string.Empty;
            var model = db.ItemTypeRepo.GetById(id);
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = false, message = "Invalid request" });
            }
            try
            {
                db.ItemTypeRepo.Delete(model);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                if (ex.GetExceptionMessages().Contains("REFERENCE"))
                {
                    fkMessage = ex.GetExceptionMessages();
                    errors.Add("Item Type is already in use. Used item type group cannot be deleted");
                }
                else
                {
                    errors.Add(ex.GetExceptionMessages());

                }
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { success = false, message = fkMessage, error = errors });
        }


        [HttpPost]
        public JsonResult ItemTypeNotExist(int id, string name)
        {
            return Json(DuplicateItemType(id, name) == null);
        }

        private ItemType DuplicateItemType(int id, string name)
        {
            var model = db.ItemTypeRepo.Get(x => x.Id != id && x.Name.Trim() == name.Trim()).FirstOrDefault();
            return model;
        }
        #endregion

        #region Role

        [HttpPost]
        public JsonResult RoleNotExist(int id, string name)
        {
            return Json(DuplicateRole(id, name) == null);
        }

        private Role DuplicateRole(int id, string name)
        {
            var model = db.RoleRepo.Get(x => x.Id != id && x.Name.Trim() == name.Trim()).FirstOrDefault();
            return model;
        }
        #endregion

        #region DocumentSetup

        public ActionResult DocumentSetup()
        {
            return View();
        }

        public PartialViewResult DocumentSetupDetail(string name)
        {
            var data = db.DocumentSetupRepo.Get(x => (string.IsNullOrEmpty(name.Trim()) || x.Name.StartsWith(name.Trim())));
            var model = AutomapperConfig.Mapper.Map<IEnumerable<DocumentSetup>, IEnumerable<DocumentSetupViewModel>>(data);

            return PartialView(model);
        }

        public PartialViewResult CreateDocumentSetup(int id)
        {
            var model = new DocumentSetupViewModel();
            if (id > 0)
                model = AutomapperConfig.Mapper.Map<DocumentSetupViewModel>(db.DocumentSetupRepo.GetById(id));
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return PartialView(BadRequestView);
            }
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult CreateDocumentSetup(DocumentSetup model)
        {
            List<string> errors = new List<string>();
            try
            {
                if (DuplicateDocumentSetup(model.Id, model.Name) != null)
                {
                    ModelState.AddModelError("Name", $"Document Setup {model.Name} already exists.");
                }

                if (ModelState.IsValid)
                {
                    if (model.Id == 0)
                    {
                        var itemD = db.DocumentSetupRepo.Table();
                        model.Id = db.DocumentSetupRepo.Create(AutomapperConfig.Mapper.Map<DocumentSetup>(model));
                    }
                    else
                    {
                        var oldmodel = db.DocumentSetupRepo.GetById(model.Id);
                        if (oldmodel == null)
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            return Json(new { redirecturl = "/error/badrequest" });
                        }
                        oldmodel.Name = model.Name;

                    }
                    db.SaveChanges();
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { message = "Document Setup successfully created" });

                }
                foreach (var item in ModelState.Where(x => x.Value.Errors.Any()))
                {
                    errors.Add(item.Value.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.GetExceptionMessages());
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { error = errors });
        }

        [HttpPost]
        public JsonResult DeleteDocumentSetup(int id)
        {
            List<string> errors = new List<string>();
            string fkMessage = string.Empty;
            var model = db.DocumentSetupRepo.GetById(id);
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { redirecturl = "/error/badrequest" });
            }
            try
            {
                db.DocumentSetupRepo.Delete(model);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                if (ex.GetExceptionMessages().Contains("REFERENCE"))
                {
                    fkMessage = ex.GetExceptionMessages();
                    errors.Add("This group is already in use. Used designarion group cannot be deleted");
                }
                else
                {
                    errors.Add(ex.GetExceptionMessages());

                }
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { fkMessage = fkMessage, error = errors });
        }

        [HttpPost]
        public JsonResult DocumentSetupNotExist(int id, string name)
        {
            return Json(DuplicateDocumentSetup(id, name) == null);
        }

        private DocumentSetup DuplicateDocumentSetup(int id, string name)
        {
            var model = db.DocumentSetupRepo.Get(x => x.Id != id && x.Name.Trim() == name.Trim()).FirstOrDefault();
            return model;
        }
        #endregion

        #region Permission

        [HttpPost]
        public JsonResult PermissionNotExist(int id, string name)
        {
            return Json(DuplicatePermission(id, name) == null);
        }

        private Permission DuplicatePermission(int id, string name)
        {
            var model = db.PermissionRepo.Get(x => x.Id != id && x.Name.Trim() == name.Trim()).FirstOrDefault();
            return model;
        }
        #endregion

        #region User

        [HttpPost]
        public JsonResult UserNameNotExist(int? Id, string UserName)
        {
            return Json(DuplicateUser(Id??0, UserName) == null);
        }

        private AppUser DuplicateUser(int id, string name)
        {
            var model = db.AppUserRepo.Get(x => x.Id != id && x.UserName.Trim() == name.Trim()).FirstOrDefault();
            return model;
        }
        #endregion

    }
}