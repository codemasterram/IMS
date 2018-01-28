using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftIms.Data.ViewModel;
using static SoftIms.Utilities.Utility;
using static SoftIms.Utilities.ViewModelExtension;
using SoftIms.Data;
using System.Net;
using SoftIms.Data.Infrastructure;
using SoftIms.Data.Configuration;
using System.Web.Security;
using System.Net.Mail;
using SoftIms.Data.Helper;

namespace SoftIms.Controllers
{

    public class UserController : BaseController
    {
        protected NotificationService NotificationService;
        public UserController()
        {
            NotificationService = new NotificationService();
        }
        [Authorize]
        public ActionResult Index()
        {
            return View();

        }


        #region User
        public PartialViewResult Detail(string name)
        {
            var data = db.AppUserRepo.Get(x => (string.IsNullOrEmpty(name.Trim()) || x.UserName.StartsWith(name.Trim())));
            var model = AutomapperConfig.Mapper.Map<IList<AppUserListViewModel>>(data);
            return PartialView(model);
        }

        public ActionResult Create(int id = 0)
        {
            var model = new AppUserViewModel() { Id = id };
            if (id > 0)
                model = AutomapperConfig.Mapper.Map<AppUserViewModel>(db.AppUserRepo.GetById(id));
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return PartialView(BadRequestView);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(AppUserViewModel model)
        {
            List<string> errors = new List<string>();
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id == 0)
                    {

                        var dbModel = AutomapperConfig.Mapper.Map<AppUser>(model);
                        EncryptionService encsvc = new EncryptionService();

                        dbModel.Salt = encsvc.CreateSaltKey(10);
                        dbModel.Password = encsvc.CreatePasswordHash(model.Password, dbModel.Salt);
                        dbModel.IsActive = true;
                        dbModel.UserRoles.Add(new UserRole { RoleId = model.RoleId });
                        model.Id = db.AppUserRepo.Create(dbModel);

                    }
                    else
                    {
                        var oldmodel = db.AppUserRepo.GetById(model.Id);
                        if (oldmodel == null)
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            return Json(new { redirecturl = "/error/badrequest" });
                        }
                        var role = db.UserRoleRepo.Table().Where(x => x.UserId == model.Id);
                        db.UserRoleRepo.DeleteRange(role);
                        var dbModel = AutomapperConfig.Mapper.Map<AppUser>(model);
                        EncryptionService encsvc = new EncryptionService();
                        oldmodel.UserName = model.UserName;
                        oldmodel.Password = encsvc.CreatePasswordHash(model.Password, dbModel.Salt);
                        oldmodel.EmployeeId = model.EmployeeId;
                        oldmodel.Photo = model.Photo;
                        oldmodel.UserRoles.Add(new UserRole { RoleId = model.RoleId });

                        oldmodel.IsActive = model.IsActive;
                    }
                    db.SaveChanges();
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    TempData["Success"] = "User Successfully Saved";
                    return RedirectToAction("Index");

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
        public JsonResult DeleteUser(int id)
        {
            List<string> errors = new List<string>();
            string fkMessage = string.Empty;
            var model = db.AppUserRepo.GetById(id);
            if (model == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { redirecturl = "/error/badrequest" });
            }
            try
            {
                db.AppUserRepo.Delete(model);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                if (ex.GetExceptionMessages().Contains("REFERENCE"))
                {
                    fkMessage = ex.GetExceptionMessages();
                    errors.Add("User  cannot be deleted.");
                }
                else
                {
                    errors.Add(ex.GetExceptionMessages());

                }
            }
            Response.StatusCode = (int)HttpStatusCode.SeeOther;
            return Json(new { fkMessage = fkMessage, error = errors });
        }

        #endregion

        #region Login
        [AllowAnonymous]
        public ActionResult Login(string returnurl)
        {
            ViewBag.ReturnUrl = returnurl;
            return PartialView();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string userName, string password, string returnUrl, string captcha = "", bool rememberMe = false)
        {
            try
            {
                ViewBag.UserName = userName;

                List<string> errors = new List<string>();
                returnUrl = null;

                var user = db.AppUserRepo.Get(x => x.UserName == userName && x.IsActive == true).FirstOrDefault();

                if (user != null)
                {
                    EncryptionService encsvc = new EncryptionService();

                    password = encsvc.CreatePasswordHash(password, user.Salt);
                    if (user.Password == password)
                    {
                        FormsAuthentication.SetAuthCookie(userName, rememberMe);
                    }

                    //if (user.IsFirstLogin)
                    //    return RedirectToAction("changepassword", "user");
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("index", "home");
                    }
                    return RedirectToLocal(returnUrl);
                }
                TempData["messageError"] = "<strong>Oops! </strong> Invalid username or password!";

            }
            catch (Exception ex)
            {
                TempData["messageError"] = ex.Message;
            }
            return PartialView();
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("index", "home");
        } 
        #endregion

        #region Logoff
        [Authorize]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll();
            return RedirectToAction("login", "user");
        }
        #endregion


        #region FeedBack and error submition
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPassword(ForgetPasswordViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                model.userId = model.userId.Trim();
                try
                {
                    var user = db.AppUserRepo.Table().Where(x => x.UserName == model.userId || x.Employee!=null).FirstOrDefault();
                    
                    if (user == null)
                    {
                        TempData["Errors"] = "Unable to find user account";
                        return RedirectToAction("forgetpassword");
                    }                    
                    var guid = Guid.NewGuid().ToString();
                    db.PasswordResetLogRepo.Create(new PasswordResetLog
                    {
                        AppUserId = user.Id,
                        Date = DateTime.Now,
                        Browser = ClientUserAgent,
                        Device = ClientDevice,
                        IPAddress = ClientIP,
                        Guid = guid,
                    });
                    string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                    var email = user.Employee.Email.Trim();
                    var body = "Dear User,<p>Please goto the following link to reset your password.</p>";
                    body += $"<a href='{domain}/user/resetpassword/{guid}'>{domain}/user/resetpassword/{guid}</a>";
                    body += "<br/><br/>";
                    body += "Thank you. <br/>";
                    body += $"{domain}";
                    var SendMail = NotificationService.SendEmail(new Smtp() ,email, "Forgot password Info", body);
                   

                    
                }
                catch (Exception ex)
                {
                    TempData["Errors"] = ex;
                    return RedirectToAction("login");

                }
                
            }
            return RedirectToAction("forgetpassword");   
        }
        #endregion


    }
}