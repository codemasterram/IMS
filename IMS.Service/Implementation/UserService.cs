using IMS.Logic.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Logic.ViewModels;
using PagedList;
using IMS.Data;
using IMS.Data.Infrastructure;
using System.ComponentModel.DataAnnotations;
using IMS.Logic.Configuration;
using IMS.Logic.Utilities;
using IMS.Logic.ViewModels.BackOffice.User;
using static IMS.Logic.Utilities.Common;

namespace IMS.Logic.Implementation
{
    public class UserService : IUserService
    {
        #region Constructor
        private ILocalizationService localizationService;
        private INotificationService notificationService;
        private IExceptionService exceptionService;

        private IRepository<User> userRepository;
        private IRepository<Employee> employeeRepository;
        private IRepository<UserRole> userRoleRepository;
        private IRepository<LoginHistory> loginHistoryRepository;
        private UnitOfWork db;
        public UserService(
            ILocalizationService localizationSvc,
            INotificationService notificationSvc,
            IExceptionService exceptionSVC,
            IRepository<User> userRepo,
            IRepository<Employee> employeeRepo,
            IRepository<UserRole> userRoleRepo,
            IRepository<LoginHistory> loginHistoryRepo)
        {
            localizationService = localizationSvc;
            exceptionService = exceptionSVC;
            notificationService = notificationSvc;
            userRepository = userRepo;
            employeeRepository = employeeRepo;
            userRoleRepository = userRoleRepo;
            loginHistoryRepository = loginHistoryRepo;
            db = new UnitOfWork();
        }
        #endregion Constructor

        #region User
        public UserViewModel SaveUser(UserViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (!InternetConnectionIsAvailable)
                    model.AddModelError(x => x.Email, "Message.NoInternetConnection", "Internet connection is not available, please try again later.");
                if (model.HasError)
                    return;

                var duplicateUserName = userRepository.Table.Any(x => x.UserId != model.UserId && x.Username == model.Username);
                if (duplicateUserName)
                    model.AddModelError(x => x.Username, "Message.Duplicate", string.Format("{0} already exists, please use another.", model.GetDisplayName(x => x.Username)));
                if (model.HasError)
                    return;

                if (!model.Validate())
                    return;

                var entity = new User();

                if (model.UserId > 0)
                {
                    entity = userRepository.GetById(model.UserId);
                    userRepository.Update(entity);

                    var employee = employeeRepository.GetById(model.EmployeeId);
                    if (employee != null)
                    {
                        employee.UserId = model.UserId;
                        employeeRepository.Update(employee);
                    }

                    var savedUser = employeeRepository.GetById(model.EmployeeId).User;
                    if (savedUser != null)
                    {
                        var savedRoles = savedUser.UserRoles;
                        if (savedRoles != null)
                        {
                            userRoleRepository.DeleteRange(savedRoles);
                        }
                    }

                    if (employee != null && model.Roles != null && model.Roles.Any() && savedUser != null)
                    {
                        var roles = model.Roles.Select(x => x.Value);
                        foreach (var r in roles)
                        {
                            userRoleRepository.Insert(new UserRole { UserId = employee.UserId.Value, RoleId = r, RoleGrantedFrom = DateTime.Now });
                        }
                    }
                }
                else
                {
                    var encServ = new EncryptionService();
                    var generatedPassword = encServ.RandomString(5);

                    model.IsActive = true;
                    model.Salt = encServ.CreateSaltKey(10);
                    model.Password = encServ.CreatePasswordHash(generatedPassword, model.Salt);
                    entity = AutomapperConfig.Mapper.Map<User>(model);
                    userRepository.Insert(entity);
                    model.UserId = entity.UserId;

                    var employee = employeeRepository.GetById(model.EmployeeId);
                    if (employee != null)
                    {
                        employee.UserId = model.UserId;
                        employeeRepository.Update(employee);
                    }

                    if (employee != null && model.Roles != null && model.Roles.Any())
                    {
                        var roles = model.Roles.Select(x => x.Value);
                        foreach (var r in roles)
                        {
                            userRoleRepository.Insert(new UserRole { UserId = employee.UserId.Value, RoleId = r, RoleGrantedFrom = DateTime.Now });
                        }
                    }

                    if (employee != null && employee.Email != null)
                    {
                        var template = notificationService.EmailTemplate(NTAEnum.eEmailTemplateType.UserCreated);

                        var subject = template.Subject;

                        var body = template.TemplateBody.Replace("{EmployeeName}", employee.Name)
                         .Replace("{UserName}", model.Username)
                         .Replace("{Password}", generatedPassword);

                        var messageBody = body;
                        ServiceModel sm = notificationService.SendEmail(employee.Email, null, null, subject, messageBody, null, true, true);
                    }
                }
                if (model.UserId == 0)
                    model.Message = localizationService.GetLocalizedText("Message.RecordSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.RecordUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record update succeeded.");
            }, model);

            return model;
        }

        public UserViewModel GetUser(int UserId)
        {
            var item = userRepository.GetById(UserId);
            var model = AutomapperConfig.Mapper.Map<UserViewModel>(item);
            return model;
        }

        public IList<UserListViewModel> GetUsers(string userName)
        {
            userName = userName == null ? "" : userName.Trim();
            var data = userRepository.Table.Where(x => string.IsNullOrEmpty(userName) || (x.Username.StartsWith(userName) || x.Employees.Any(y => y.Name.Contains(userName)) || x.Employees.Any(y => y.Email.Contains(userName)))).OrderBy(x => x.Username);
            return AutomapperConfig.Mapper.Map<IList<UserListViewModel>>(data);
        }

        public ServiceModel DeleteUser(int id)
        {
            var result = new ServiceModel();
            var data = userRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                var resetLogs = db.PasswordResetLogRepo.Table.Where(x => x.AppUserId == data.UserId);
                db.PasswordResetLogRepo.DeleteRange(resetLogs);
                db.SaveChanges();
                db.UserRepo.Delete(data.UserId);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }

        public ChangePasswordViewModel ChangePassword(int userId, ChangePasswordViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (model.NewPassword != model.ConfirmPassword)
                {
                    model.AddModelError(x => x.NewPassword, "New password and confirm new password not matched.", "New password and confirm new password not matched.");
                    return;
                }

                var oldUser = userRepository.GetById(userId);
                var encSvc = new EncryptionService();

                string pass = encSvc.CreatePasswordHash(model.OldPassword, oldUser.Salt);
                if (oldUser.Password != pass)
                {
                    model.AddModelError(x => x.OldPassword, "Message.OldPasswordNotMatched", string.Format("{0} not matched.", model.GetDisplayName(x => x.OldPassword)));
                    return;
                }

                oldUser.Password = encSvc.CreatePasswordHash(model.ConfirmPassword, oldUser.Salt);
                userRepository.Update(oldUser);
                model.Message = localizationService.GetLocalizedText("Message.RecordUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record update succeeded.");
            }, model);

            return model;
        }
        #endregion User
    }
}
