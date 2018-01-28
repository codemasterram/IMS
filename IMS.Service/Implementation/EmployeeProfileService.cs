using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Data;
using IMS.Data.Infrastructure;
using IMS.Logic.Configuration;
using IMS.Logic.Contract;
using IMS.Logic.ViewModels;
using IMS.Logic.ViewModels.BackOffice.Employee;
using static DateMiti.GetDateMiti;
using IMS.Logic.Utilities;
using static IMS.Logic.Utilities.Common;
using IMS.Logic.DataAnnotations;
using IMS.Logic.Extensions;
using PagedList;

namespace IMS.Logic.Implementation
{
    public class EmployeeProfileService : IEmployeeProfileService
    {
        #region Constructor
        private ILocalizationService localizationService;
        private IExceptionService exceptionService;

        private IRepository<Employee> employeeRepository;
        public EmployeeProfileService(
            ILocalizationService localizationSvc,
            IExceptionService exceptionSVC,
            IRepository<Employee> employeeRepo)
        {
            localizationService = localizationSvc;
            exceptionService = exceptionSVC;

            employeeRepository = employeeRepo;
        }
        #endregion Constructor

        #region Employee

        public EmployeeProfileViewModel GetProfile(int id)
        {
            var item = employeeRepository.GetById(id);
            var model = AutomapperConfig.Mapper.Map<EmployeeProfileViewModel>(item);
            return model;
        }

        public EmployeeProfileViewModel SaveProfile(EmployeeProfileViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                var dob = model.DateOfBirthBS.GetDate();
                if (dob.IsFutureDate())
                    model.AddModelError(x => x.DateOfBirthBS, "DobCanNotBeFutureDate", "Date of birth can not be future date.");
                model.DateOfBirth = dob;

                var appointmentDate = model.AppointmentDateBS.GetDate();
                if (appointmentDate.IsFutureDate())
                    model.AddModelError(x => x.AppointmentDateBS, "Message.CanNotBeFutureDate", string.Format("{0} can not be future date.", model.GetDisplayName(x => x.AppointmentDateBS)));
                model.AppointmentDate = appointmentDate;

                var dateTo20YrsofServiceDuration = model.DateTo20YrsofServiceDurationBS.GetDate();
                if (dateTo20YrsofServiceDuration.IsPastDate())
                    model.AddModelError(x => x.DateTo20YrsofServiceDurationBS, "Message.CanNotBeFutureDate", string.Format("{0} can not be past date.", model.GetDisplayName(x => x.DateTo20YrsofServiceDurationBS)));
                model.DateTo20YrsofServiceDuration = dateTo20YrsofServiceDuration;

                var dateTo58YrsOld = model.DateTo58YrsOldBS.GetDate();
                if (dateTo58YrsOld.IsPastDate())
                    model.AddModelError(x => x.DateTo58YrsOldBS, "Message.CanNotBeFutureDate", string.Format("{0} can not be past date.", model.GetDisplayName(x => x.DateTo58YrsOldBS)));
                model.DateTo58YrsOld = dateTo58YrsOld;

                var citizenshipIssuedDate = model.CitizenshipIssuedDateBS.GetDate();
                if (citizenshipIssuedDate.IsFutureDate())
                    model.AddModelError(x => x.CitizenshipIssuedDateBS, "Message.CanNotBeFutureDate", string.Format("{0} can not be future date.", model.GetDisplayName(x => x.CitizenshipIssuedDateBS)));
                model.CitizenshipIssuedDate = citizenshipIssuedDate == DateTime.MinValue ? (DateTime?)null : citizenshipIssuedDate;

                var passportIssuedDate = model.PassportIssuedDateBS.GetDate();
                if (passportIssuedDate.IsFutureDate())
                    model.AddModelError(x => x.PassportIssuedDateBS, "Message.NoFutureDate", string.Format("{0} cannot be future date.", model.GetDisplayName(x => x.PassportIssuedDateBS)));
                model.PassportIssuedDate = passportIssuedDate == DateTime.MinValue ? (DateTime?)null : passportIssuedDate;

                var driverLicenseIssuedDate = model.DriverLicenseIssuedDateBS.GetDate();
                if (driverLicenseIssuedDate.IsFutureDate())
                    model.AddModelError(x => x.DriverLicenseIssuedDateBS, "Message.NoFutureDate", string.Format("{0} cannot be future date.", model.GetDisplayName(x => x.DriverLicenseIssuedDateBS)));
                model.DriverLicenseIssuedDate = driverLicenseIssuedDate == DateTime.MinValue ? (DateTime?)null : driverLicenseIssuedDate;

                if (!model.Email.IsValidEmail())
                    model.AddModelError(x => x.Email, "Message.NoFutureDate", string.Format("{0} not a valid format.", model.GetDisplayName(x => x.DriverLicenseIssuedDateBS)));

                if (model.HasError)
                    return;

                if (!model.Validate())
                    return;

                var entity = employeeRepository.GetById(model.Id);
                entity.Name = model.Name;
                entity.FullNameNP = model.FullNameNP;
                entity.DateOfBirth = model.DateOfBirth;
                entity.Gender = model.Gender;
                entity.AppointmentDate = model.AppointmentDate;
                entity.DateTo20YrsofServiceDuration = model.DateTo20YrsofServiceDuration;
                entity.DateTo58YrsOld = model.DateTo58YrsOld;
                entity.FatherName = model.FatherName;
                entity.MotherName = model.MotherName;
                entity.GrandFatherName = model.GrandFatherName;
                entity.SectionId = model.SectionId;
                entity.DesignationId = model.DesignationId;
                entity.HomeTelephone = model.HomeTelephone;
                entity.OfficeTelephone = model.OfficeTelephone;
                entity.TelExtenstionNumber = model.TelExtenstionNumber;
                entity.Mobile = model.Mobile;
                entity.Email = model.Email;
                entity.PANNumber = model.PANNumber;
                entity.CitizenshipNumber = model.CitizenshipNumber;
                //entity.CitizenshipIssuedDistrictId = model.CitizenshipIssuedDistrictId;
                entity.CitizenshipIssuedDate = model.CitizenshipIssuedDate;
                entity.PassportNumber = model.PassportNumber;
                entity.PassportIssuedDate = model.PassportIssuedDate;
                entity.DriverLicenseNumber = model.DriverLicenseNumber;
                entity.DriverLicenseIssuedDate = model.DriverLicenseIssuedDate;
                entity.MaritalStatus = model.MaritalStatus;
                entity.SupervisorId = model.SupervisorId;
                entity.EmployeeType = model.EmployeeType;
                entity.EmployeeStatus = model.EmployeeStatus;

                employeeRepository.Update(entity);
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.SavedSuccessfully", defaultText: "Saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.UpdatedSuccessfully", defaultText: "Update succeeded.");
            }, model);

            return model;
        }
        #endregion Employee
    }
}
