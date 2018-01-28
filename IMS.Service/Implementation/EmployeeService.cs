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
    public class EmployeeService : IEmployeeService
    {
        #region Constructor
        private ILocalizationService localizationService;
        private IExceptionService exceptionService;
        private IAuthService authorizationService;

        private IRepository<Employee> employeeRepository;
        private IRepository<Address> addressRepository;
        private IRepository<Family> familyRepository;
        private IRepository<Experience> experienceRepository;
        private IRepository<EducationalQualification> educationalQualificationRepository;
        private IRepository<TrainingSeminarConference> trainingSeminarConferenceRepository;
        private IRepository<AwardDecoration> awardDecorationRepository;
        private IRepository<Punishment> punishmentRepository;
        private IRepository<Transfer> transferRepository;
        private IRepository<Retirement> retirementRepository;
        private IRepository<Substitution> substitutionRepository;
        private UnitOfWork db = new UnitOfWork();
        public EmployeeService(
            ILocalizationService localizationSvc,
            IExceptionService exceptionSVC,
            IAuthService authorizationSVC,
            IRepository<Employee> employeeRepo,
            IRepository<Address> addressRepo,
            IRepository<Family> familyRepo,
            IRepository<Experience> experienceRepo,
            IRepository<EducationalQualification> educationalQualificationRepo,
            IRepository<TrainingSeminarConference> trainingSeminarConferenceRepo,
            IRepository<AwardDecoration> awardDecorationRepo,
            IRepository<Punishment> punishmentRepo,
            IRepository<Transfer> transferRepo,
            IRepository<Retirement> retirementRepo,
            IRepository<Substitution> substitutionRepo
            )
        {
            localizationService = localizationSvc;
            exceptionService = exceptionSVC;
            authorizationService = authorizationSVC;

            employeeRepository = employeeRepo;
            addressRepository = addressRepo;
            familyRepository = familyRepo;
            experienceRepository = experienceRepo;
            educationalQualificationRepository = educationalQualificationRepo;
            trainingSeminarConferenceRepository = trainingSeminarConferenceRepo;
            awardDecorationRepository = awardDecorationRepo;
            punishmentRepository = punishmentRepo;
            transferRepository = transferRepo;
            retirementRepository = retirementRepo;
            substitutionRepository = substitutionRepo;
        }
        #endregion Constructor

        #region Employee
        public ServiceModel DeleteEmployee(int id)
        {
            var result = new ServiceModel();
            var data = employeeRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                data.IsActive = false;
                data.EmployeeStatus = "Passive";
                data.EmployeeStatus = NTAEnum.eStatus.InActive.ToString();
                employeeRepository.Update(data);

                if (data.UserId.HasValue)
                {
                    db.UserRepo.Delete(data.UserId.Value);
                }
            }, result);

            var user = db.UserRepo.GetById(data.UserId);
            if (user != null)
            {
                db.UserRepo.Delete(user);
            }

            if (result.HasError)
            {
                if (result.Exception is System.Data.SqlClient.SqlException)
                {
                    result.ErrorMessages.Clear();
                    result.ErrorMessages.Add("Oops! An error has occured. Please delete the related data first.");
                }
                return result;
            }

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }

        public EmployeeViewModel GetEmployee(string employeeId)
        {
            var item = employeeRepository.Table.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
            var model = AutomapperConfig.Mapper.Map<EmployeeViewModel>(item);
            return model;
        }

        public EmployeeViewModel GetEmployee(int employeeId)
        {
            var item = employeeRepository.GetById(employeeId);
            var model = AutomapperConfig.Mapper.Map<EmployeeViewModel>(item);
            return model;
        }

        public IList<EmployeeListViewModel> GetEmployees(int? employeeId = null, string name = null, string employeeType = null, int? sectionId = null, int? designationId = null, NTAEnum.eStatus status = NTAEnum.eStatus.Active)
        {
            var data = employeeRepository.Table.Where(x =>
                                (!employeeId.HasValue || x.Id == employeeId.Value)
                                && x.EmployeeStatus == status.ToString()
                            && (string.IsNullOrEmpty(name) || x.Name.StartsWith(name))
                            && (string.IsNullOrEmpty(employeeType) || x.EmployeeType == employeeType)
                            && (sectionId == null || x.SectionId == sectionId)
                            && (designationId == null || x.DesignationId == designationId)).OrderBy(x => x.Designation.DisplayOrder).ThenBy(x => x.Section.DisplayOrder);
            return AutomapperConfig.Mapper.Map<IList<EmployeeListViewModel>>(data);
        }

        public EmployeeViewModel SaveEmployee(EmployeeViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                var empId = employeeRepository.Table.Where(x => x.EmployeeNo == model.EmployeeNo && x.Id != model.Id);
                if (empId.Any())
                {
                    model.AddModelError(x => x.EmployeeNo, "Error", $"A employee with employee id {model.EmployeeNo} already exists, please use another.");
                }

                var dob = model.DateOfBirthBS.GetDate();
                if (dob.IsFutureDate())
                    model.AddModelError(x => x.DateOfBirthBS, "DobCanNotBeFutureDate", "Date of birth can not be future date.");
                model.DateOfBirth = dob;

                var age = DateTime.Today - dob;
                if (age.Days < 18 * 365)
                {
                    model.AddModelError(x => x.DateOfBirthBS, "DobMustGreaterThan18Years", "Age can not be less than 18 years.");
                }

                var appointmentDate = model.AppointmentDateBS.GetDate();
                if (appointmentDate.IsFutureDate())
                    model.AddModelError(x => x.AppointmentDateBS, "Message.CanNotBeFutureDate", string.Format("{0} can not be future date.", model.GetDisplayName(x => x.AppointmentDateBS)));
                model.AppointmentDate = appointmentDate;

                model.DateTo20YrsofServiceDuration = model.AppointmentDate.AddYears(20);

                model.DateTo58YrsOld = model.DateOfBirth.AddYears(58);

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

                var entity = new Employee();

                if (model.Id > 0)
                {
                    entity = employeeRepository.GetById(model.Id);
                    entity.EmployeeNo = model.EmployeeNo;
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
                    entity.CitizenshipIssuedDistrictId = model.CitizenshipIssuedDistrictId;
                    entity.CitizenshipIssuedDate = model.CitizenshipIssuedDate;
                    entity.PassportNumber = model.PassportNumber;
                    entity.PassportIssuedDate = model.PassportIssuedDate;
                    entity.DriverLicenseNumber = model.DriverLicenseNumber;
                    entity.DriverLicenseIssuedDate = model.DriverLicenseIssuedDate;
                    entity.MaritalStatus = model.MaritalStatus;
                    entity.SupervisorId = model.SupervisorId;
                    entity.EmployeeType = model.EmployeeType;
                    entity.EmployeeStatus = model.EmployeeStatus;

                    if (entity.Nominees.Any())
                    {
                        var nominee = entity.Nominees.FirstOrDefault();
                        var mn = model.Nominees.FirstOrDefault();

                        nominee.AuthorizedBy = mn.AuthorizedBy;
                        nominee.Name = mn.Name;
                        nominee.RelationshipId = mn.RelationshipId;
                    }
                    else
                    {
                        entity.Nominees.Add(AutomapperConfig.Mapper.Map<Nominee>(model.Nominees.FirstOrDefault()));
                    }

                    if (entity.Addresses.Any())
                    {
                        addressRepository.DeleteRange(entity.Addresses);
                    }

                    foreach (var add in model.Addresses)
                    {
                        entity.Addresses.Add(AutomapperConfig.Mapper.Map<Address>(add));
                    }

                    employeeRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<Employee>(model);
                    employeeRepository.Insert(entity);
                    model.Id = entity.Id;
                    model.Nominees.FirstOrDefault().Id = entity.Nominees.FirstOrDefault().Id;

                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.EmployeeSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Employee saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.EmployeeUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Employee update succeeded.");
            }, model);

            return model;
        }

        public IQueryable<Data.Infrastructure.Employee> GetEmployeeQueryable()
        {
            return employeeRepository.Table;
        }

        public IQueryable<Employee> GetEmployeeQueryableForCurrentUser(string username, int employeeId, bool showEmployeeUnder = false)
        {
            var employees = employeeRepository.Table;
            if (!authorizationService.IsAuthorized(username, new int[] { (int)NTAEnum.ePermission.ManageEmployee }))
            {
                if (showEmployeeUnder)
                {
                    var emp = employeeRepository.Table.FirstOrDefault(x => x.Id == employeeId);
                    int designationOrder = 0;

                    if (emp != null)
                    {
                        designationOrder = emp.Designation.DisplayOrder ?? 0;
                    }

                    employees = employees.Where(x => x.Id == employeeId || x.Designation.DisplayOrder > designationOrder);
                }
                else
                {
                    employees = employees.Where(x => x.Id == employeeId);
                }
            }

            return employees;
        }
        #endregion Employee

        #region Family
        public FamilyViewModel SaveFamily(FamilyViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                var joinDate = model.JoinedDateBS.GetDate();
                if (joinDate.IsFutureDate())
                {
                    model.AddModelError(x => x.JoinedDateBS, "Message.NoFutureDate", string.Format("{0} cannot be future date.", model.GetDisplayName(x => x.JoinedDateBS)));
                    return;
                }
                model.JoinedDate = joinDate;

                var retirementDate = model.RetirementDateBS.GetDate();
                if (retirementDate.IsFutureDate())
                {
                    model.AddModelError(x => x.RetirementDateBS, "Message.NoFutureDate", string.Format("{0} can not be future date.", model.GetDisplayName(x => x.RetirementDateBS)));
                    return;
                }
                model.RetirementDate = retirementDate;

                if (!model.Validate())
                    return;

                var entity = new Family();

                if (model.Id > 0)
                {
                    entity = familyRepository.GetById(model.Id);
                    entity.Name = model.Name;
                    entity.RelationshipId = model.RelationshipId;
                    entity.ProfessionId = model.ProfessionId;
                    entity.DesignationId = model.DesignationId;
                    entity.OfficeName = model.OfficeName;
                    entity.Age = model.Age;
                    entity.JoinedDate = model.JoinedDate;
                    entity.RetirementDate = model.RetirementDate;
                    familyRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<Family>(model);
                    familyRepository.Insert(entity);
                    model.Id = entity.Id;
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.RecordSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.RecordUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record update succeeded.");
            }, model);

            return model;
        }

        public IList<FamilyListViewModel> GetFamilies(int employeeId)
        {
            var data = familyRepository.Table.Where(x => x.EmployeeId == employeeId).OrderBy(x => x.RelationshipId);
            return AutomapperConfig.Mapper.Map<IList<FamilyListViewModel>>(data);
        }

        public FamilyViewModel GetFamily(int familyId)
        {
            var item = familyRepository.GetById(familyId);
            var model = AutomapperConfig.Mapper.Map<FamilyViewModel>(item);
            return model;
        }

        public ServiceModel DeleteFamily(int id)
        {
            var result = new ServiceModel();
            var data = familyRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                familyRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion Family

        #region Experience
        public ExperienceViewModel SaveExperience(ExperienceViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                var appointedDate = model.AppointedDateBS.GetDate();
                if (appointedDate.IsFutureDate())
                {
                    model.AddModelError(x => x.AppointedDateBS, "Message.NoFutureDate", string.Format("{0} cannot be future date.", model.GetDisplayName(x => x.AppointedDateBS)));
                    return;
                }
                model.AppointedDate = appointedDate == DateTime.MinValue ? appointedDate : (DateTime?)null;

                var separationDate = model.SeparationDateBS.GetDate();
                if (separationDate.IsPastDate())
                {
                    model.AddModelError(x => x.SeparationDateBS, "Message.NoPastDate", string.Format("{0} can not be past date.", model.GetDisplayName(x => x.SeparationDateBS)));
                    return;
                }
                model.SeparationDate = separationDate == DateTime.MinValue ? (DateTime?)null : separationDate;

                if (!model.Validate())
                    return;

                var entity = new Experience();

                if (model.Id > 0)
                {
                    entity = experienceRepository.GetById(model.Id);
                    entity.OfficeName = model.OfficeName;
                    entity.DesignationId = model.DesignationId;
                    entity.ServiceClassId = model.ServiceClassId;
                    entity.AppointedDate = model.AppointedDate;
                    entity.SeparationDate = model.SeparationDate;
                    entity.AuthorizedBy = model.AuthorizedBy;
                    experienceRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<Experience>(model);
                    experienceRepository.Insert(entity);
                    model.Id = entity.Id;
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.RecordSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.RecordUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record update succeeded.");
            }, model);

            return model;
        }

        public IList<ExperienceListViewModel> GetExperiences(int employeeId)
        {
            var data = experienceRepository.Table.Where(x => x.EmployeeId == employeeId).OrderBy(x => x.AppointedDate);
            return AutomapperConfig.Mapper.Map<IList<ExperienceListViewModel>>(data);
        }

        public ExperienceViewModel GetExperience(int experienceId)
        {
            var item = experienceRepository.GetById(experienceId);
            var model = AutomapperConfig.Mapper.Map<ExperienceViewModel>(item);
            return model;
        }

        public ServiceModel DeleteExperience(int id)
        {
            var result = new ServiceModel();
            var data = experienceRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                experienceRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion Experience

        #region Educational Qualification
        public EducationalQualificationViewModel SaveEducationalQualification(EducationalQualificationViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (model.DegreeId == 0)
                {
                    model.AddModelError(x => x.DegreeId, "Required.DegreeId", string.Format("Please select {0}.", model.GetDisplayName(x => x.DegreeId)));
                    return;
                }

                if (!model.Validate())
                    return;

                var entity = new EducationalQualification();

                if (model.Id > 0)
                {
                    entity = educationalQualificationRepository.GetById(model.Id);
                    entity.DegreeId = model.DegreeId;
                    entity.MajorSubjects = model.MajorSubjects;
                    entity.StartYear = model.StartYear;
                    entity.EndYear = model.EndYear;
                    entity.Division = model.Division;
                    entity.TeachingInstitutionName = model.TeachingInstitutionName;
                    entity.TeachingInstitutionAddress = model.TeachingInstitutionAddress;
                    entity.Remarks = model.Remarks;
                    educationalQualificationRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<EducationalQualification>(model);
                    educationalQualificationRepository.Insert(entity);
                    model.Id = entity.Id;
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.RecordSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.RecordUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record update succeeded.");
            }, model);

            return model;
        }

        public EducationalQualificationViewModel GetEducationalQualification(int educationalQualificationId)
        {
            var item = educationalQualificationRepository.GetById(educationalQualificationId);
            var model = AutomapperConfig.Mapper.Map<EducationalQualificationViewModel>(item);
            return model;
        }

        public IList<EducationalQualificationListViewModel> GetEducationalQualifications(int employeeId)
        {
            var data = educationalQualificationRepository.Table.Where(x => x.EmployeeId == employeeId).OrderBy(x => x.Degree.DisplayOrder);
            return AutomapperConfig.Mapper.Map<IList<EducationalQualificationListViewModel>>(data);
        }

        public ServiceModel DeleteEducationalQualification(int id)
        {
            var result = new ServiceModel();
            var data = educationalQualificationRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                educationalQualificationRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion Educational Qualification

        #region Training Seminar Conference
        public TrainingSeminarConferenceViewModel SaveTrainingSeminarConference(TrainingSeminarConferenceViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                var startDate = model.StartDateBS.GetDate();
                if (startDate.IsFutureDate())
                {
                    model.AddModelError(x => x.StartDateBS, "Message.NoFutureDate", string.Format("{0} cannot be future date.", model.GetDisplayName(x => x.StartDateBS)));
                    return;
                }
                model.StartDate = startDate == DateTime.MinValue ? (DateTime?)null : startDate;

                var endDate = model.EndDateBS.GetDate();
                if (endDate.IsFutureDate())
                {
                    model.AddModelError(x => x.EndDateBS, "Message.NoFutureDate", string.Format("{0} cannot be future date.", model.GetDisplayName(x => x.EndDateBS)));
                    return;
                }
                model.EndDate = endDate == DateTime.MinValue ? (DateTime?)null : endDate;

                if (!model.Validate())
                    return;

                var entity = new TrainingSeminarConference();

                if (model.Id > 0)
                {
                    entity = trainingSeminarConferenceRepository.GetById(model.Id);
                    entity.Name = model.Name;
                    entity.Type = model.Type;
                    entity.StartDate = model.StartDate;
                    entity.EndDate = model.EndDate;
                    entity.OrganizationName = model.OrganizationName;
                    entity.OrganizationAddress = model.OrganizationAddress;
                    entity.Remarks = model.Remarks;
                    trainingSeminarConferenceRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<TrainingSeminarConference>(model);
                    trainingSeminarConferenceRepository.Insert(entity);
                    model.Id = entity.Id;
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.RecordSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.RecordUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record update succeeded.");
            }, model);

            return model;
        }

        public TrainingSeminarConferenceViewModel GetTrainingSeminarConference(int TrainingSeminarConferenceId)
        {
            var item = trainingSeminarConferenceRepository.GetById(TrainingSeminarConferenceId);
            var model = AutomapperConfig.Mapper.Map<TrainingSeminarConferenceViewModel>(item);
            return model;
        }

        public IList<TrainingSeminarConferenceListViewModel> GetTrainingSeminarConferences(int employeeId, string type = null)
        {
            var data = trainingSeminarConferenceRepository.Table.Where(x => x.EmployeeId == employeeId && (type == null || x.Type == type)).OrderBy(x => x.EndDate);
            return AutomapperConfig.Mapper.Map<IList<TrainingSeminarConferenceListViewModel>>(data);
        }

        public ServiceModel DeleteTrainingSeminarConference(int id)
        {
            var result = new ServiceModel();
            var data = trainingSeminarConferenceRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                trainingSeminarConferenceRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion Training Seminar Conference

        #region AwardDecoration
        public AwardDecorationViewModel SaveAwardDecoration(AwardDecorationViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (model.AwardDecorationTypeId == 0)
                    model.AddModelError(x => x.AwardDecorationTypeId, "Required.AwardDecorationTypeId", string.Format("Please select {0}.", model.GetDisplayName(x => x.AwardDecorationTypeId)));

                var receivedDate = model.ReceivedDateBS.GetDate();
                if (receivedDate.IsFutureDate())
                    model.AddModelError(x => x.ReceivedDateBS, "Message.NoFutureDate", string.Format("{0} cannot be future date.", model.GetDisplayName(x => x.ReceivedDateBS)));
                model.ReceivedDate = receivedDate == DateTime.MinValue ? (DateTime?)null : receivedDate;

                if (model.HasError)
                    return;

                if (!model.Validate())
                    return;

                var entity = new AwardDecoration();

                if (model.Id > 0)
                {
                    entity = awardDecorationRepository.GetById(model.Id);
                    entity.AwardDecorationTypeId = model.AwardDecorationTypeId;
                    entity.Description = model.Description;
                    entity.ReceivedDate = model.ReceivedDate;
                    entity.ReasonToReceiveAward = model.ReasonToReceiveAward;
                    entity.Benefit = model.Benefit;
                    entity.AwardedBy = model.AwardedBy;
                    entity.Remarks = model.Remarks;
                    awardDecorationRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<AwardDecoration>(model);
                    awardDecorationRepository.Insert(entity);
                    model.Id = entity.Id;
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.RecordSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.RecordUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record update succeeded.");
            }, model);

            return model;
        }

        public AwardDecorationViewModel GetAwardDecoration(int AwardDecorationId)
        {
            var item = awardDecorationRepository.GetById(AwardDecorationId);
            var model = AutomapperConfig.Mapper.Map<AwardDecorationViewModel>(item);
            return model;
        }

        public IList<AwardDecorationListViewModel> GetAwardDecorations(int employeeId)
        {
            var data = awardDecorationRepository.Table.Where(x => x.EmployeeId == employeeId).OrderBy(x => x.ReceivedDate);
            return AutomapperConfig.Mapper.Map<IList<AwardDecorationListViewModel>>(data);
        }

        public ServiceModel DeleteAwardDecoration(int id)
        {
            var result = new ServiceModel();
            var data = awardDecorationRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                awardDecorationRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion AwardDecoration

        #region Punishment
        public PunishmentViewModel SavePunishment(PunishmentViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (model.PunishmentTypeId == 0)
                    model.AddModelError(x => x.PunishmentTypeId, "Required.PunishmentTypeId", string.Format("Please select {0}.", model.GetDisplayName(x => x.PunishmentTypeId)));

                var punishmentOrderDate = model.PunishmentOrderDateBS.GetDate();
                if (punishmentOrderDate.IsFutureDate())
                    model.AddModelError(x => x.PunishmentOrderDateBS, "Message.NoFutureDate", string.Format("{0} cannot be future date.", model.GetDisplayName(x => x.PunishmentOrderDateBS)));
                model.PunishmentOrderDate = punishmentOrderDate == DateTime.MinValue ? (DateTime?)null : punishmentOrderDate;

                var courtDecisionDate = model.CourtDecisionDateBS.GetDate();
                if (courtDecisionDate.IsFutureDate())
                    model.AddModelError(x => x.CourtDecisionDateBS, "Message.NoFutureDate", string.Format("{0} cannot be future date.", model.GetDisplayName(x => x.CourtDecisionDateBS)));
                model.CourtDecisionDate = courtDecisionDate == DateTime.MinValue ? (DateTime?)null : courtDecisionDate;

                if (model.HasError)
                    return;

                if (!model.Validate())
                    return;

                var entity = new Punishment();

                if (model.Id > 0)
                {
                    entity = punishmentRepository.GetById(model.Id);
                    entity.PunishmentTypeId = model.PunishmentTypeId;
                    entity.PunishmentOrderDate = model.PunishmentOrderDate;
                    entity.CourtDecisionDate = model.CourtDecisionDate;
                    entity.CourtDecision = model.CourtDecision;
                    entity.Remarks = model.Remarks;
                    punishmentRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<Punishment>(model);
                    punishmentRepository.Insert(entity);
                    model.Id = entity.Id;
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.RecordSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.RecordUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record update succeeded.");
            }, model);

            return model;
        }

        public PunishmentViewModel GetPunishment(int PunishmentId)
        {
            var item = punishmentRepository.GetById(PunishmentId);
            var model = AutomapperConfig.Mapper.Map<PunishmentViewModel>(item);
            return model;
        }

        public IList<PunishmentListViewModel> GetPunishments(int employeeId)
        {
            var data = punishmentRepository.Table.Where(x => x.EmployeeId == employeeId).OrderBy(x => x.CourtDecisionDate);
            return AutomapperConfig.Mapper.Map<IList<PunishmentListViewModel>>(data);
        }

        public ServiceModel DeletePunishment(int id)
        {
            var result = new ServiceModel();
            var data = punishmentRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                punishmentRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion Punishment

        #region Transfer
        public TransferViewModel SaveTransfer(TransferViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (model.TransferReasonId == 0)
                    model.AddModelError(x => x.TransferReasonId, "Required.TransferReasonId", string.Format("Please select {0}.", model.GetDisplayName(x => x.TransferReasonId)));

                var startDate = model.StartDateBS.GetDate();
                if (startDate.IsFutureDate())
                    model.AddModelError(x => x.StartDateBS, "Message.NoFutureDate", string.Format("{0} cannot be future date.", model.GetDisplayName(x => x.StartDateBS)));
                model.StartDate = startDate;

                var nextTransferDate = model.NextTransferDateBS.GetDate();
                model.NextTransferDate = nextTransferDate == DateTime.MinValue ? (DateTime?)null : nextTransferDate;

                var attendedDate = model.AttendedDateBS.GetDate();
                model.AttendedDate = attendedDate == DateTime.MinValue ? (DateTime?)null : attendedDate;

                var preparedDate = model.PreparedDateBS.GetDate();
                if (preparedDate.IsFutureDate())
                    model.AddModelError(x => x.PreparedDateBS, "Message.NoFutureDate", string.Format("{0} cannot be future date.", model.GetDisplayName(x => x.PreparedDateBS)));
                model.PreparedDate = preparedDate == DateTime.MinValue ? (DateTime?)null : preparedDate;

                var authorisedDate = model.AuthorisedDateBS.GetDate();
                if (authorisedDate.IsFutureDate())
                    model.AddModelError(x => x.AuthorisedDateBS, "Message.NoFutureDate", string.Format("{0} cannot be future date.", model.GetDisplayName(x => x.AuthorisedDateBS)));
                model.AuthorisedDate = authorisedDate == DateTime.MinValue ? (DateTime?)null : authorisedDate;

                if (model.HasError)
                    return;

                if (!model.Validate())
                    return;

                var entity = new Transfer();

                if (model.Id > 0)
                {
                    entity = transferRepository.GetById(model.Id);
                    entity.PreviousSection = model.PreviousSection;
                    entity.TransferredSection = model.TransferredSection;
                    entity.StartDate = model.StartDate;
                    entity.EndDate = model.EndDate;
                    entity.NextTransferDate = model.NextTransferDate;
                    entity.AttendedDate = model.AttendedDate;
                    entity.TransferReasonId = model.TransferReasonId;
                    entity.PreparedBy = model.PreparedBy;
                    entity.PreparedDate = model.PreparedDate;
                    entity.AuthorisedBy = model.AuthorisedBy;
                    entity.AuthorisedDate = model.AuthorisedDate;
                    entity.Remarks = model.Remarks;
                    transferRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<Transfer>(model);
                    transferRepository.Insert(entity);
                    model.Id = entity.Id;
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.RecordSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.RecordUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record update succeeded.");
            }, model);

            return model;
        }

        public TransferViewModel GetTransfer(int TransferId)
        {
            var item = transferRepository.GetById(TransferId);
            var model = AutomapperConfig.Mapper.Map<TransferViewModel>(item);
            return model;
        }

        public IList<TransferListViewModel> GetTransfers(int employeeId)
        {
            var data = transferRepository.Table.Where(x => x.EmployeeId == employeeId).OrderBy(x => x.StartDate);
            return AutomapperConfig.Mapper.Map<IList<TransferListViewModel>>(data);
        }

        public ServiceModel DeleteTransfer(int id)
        {
            var result = new ServiceModel();
            var data = transferRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                transferRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion Transfer

        #region Retirement
        public RetirementViewModel SaveRetirement(RetirementViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (model.EmployeeId == 0)
                    model.AddModelError(x => x.EmployeeId, "Required.EmployeeId", string.Format("Please select {0}.", model.GetDisplayName(x => x.EmployeeId)));

                if (model.RetirementReasonId == 0)
                    model.AddModelError(x => x.RetirementReasonId, "Required.RetirementReasonId", string.Format("Please select {0}.", model.GetDisplayName(x => x.RetirementReasonId)));

                var retirementDate = model.RetirementDateBS.GetDate();
                if (retirementDate == DateTime.MinValue)
                    model.AddModelError(x => x.RetirementDateBS, "Required.RetirementDate", string.Format("Please select {0}.", model.GetDisplayName(x => x.RetirementDateBS)));
                else
                    model.RetirementDate = retirementDate;

                if (model.HasError)
                    return;

                if (!model.Validate())
                    return;

                var entity = new Retirement();

                if (model.Id > 0)
                {
                    entity = retirementRepository.GetById(model.Id);
                    entity.EmployeeId = model.EmployeeId;
                    entity.RetirementReasonId = model.RetirementReasonId;
                    entity.RetirementDate = model.RetirementDate;
                    entity.IsHandoverCompleted = model.IsHandoverCompleted;
                    entity.HasPFLetterIssued = model.HasPFLetterIssued;
                    entity.HasCITLetterIssued = model.HasCITLetterIssued;
                    retirementRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<Retirement>(model);
                    retirementRepository.Insert(entity);
                    model.Id = entity.Id;
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.RecordSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.RecordUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record update succeeded.");
            }, model);

            return model;
        }

        public RetirementViewModel GetRetirement(int RetirementId)
        {
            var item = retirementRepository.GetById(RetirementId);
            var model = AutomapperConfig.Mapper.Map<RetirementViewModel>(item);
            return model;
        }

        public IList<RetirementListViewModel> GetRetirements(int employeeId)
        {
            var data = retirementRepository.Table.Where(x => x.EmployeeId == employeeId).OrderBy(x => x.RetirementDate);
            return AutomapperConfig.Mapper.Map<IList<RetirementListViewModel>>(data);
        }

        public ServiceModel DeleteRetirement(int id)
        {
            var result = new ServiceModel();
            var data = retirementRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                retirementRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion Retirement

        #region Substitution
        public SubstitutionViewModel SaveSubstitution(SubstitutionViewModel model)
        {
            exceptionService.Execute((m) =>
            {
                if (model.SubstitutionTypeId == 0)
                    model.AddModelError(x => x.SubstitutionTypeId, "Required.SubstitutionTypeId", string.Format("Please select {0}.", model.GetDisplayName(x => x.SubstitutionTypeId)));

                if (model.EmployeeToSubstitute == 0)
                    model.AddModelError(x => x.EmployeeToSubstitute, "Required.EmployeeToSubstitute", string.Format("Please select {0}.", model.GetDisplayName(x => x.EmployeeToSubstitute)));

                if (model.SubstitutedEmployee == 0)
                    model.AddModelError(x => x.SubstitutedEmployee, "Required.SubstitutedEmployee", string.Format("Please select {0}.", model.GetDisplayName(x => x.SubstitutedEmployee)));

                var effectiveFromDate = model.EffectiveFromDateBS.GetDate();
                if (effectiveFromDate == DateTime.MinValue)
                    model.AddModelError(x => x.EffectiveFromDateBS, "Required.EffectiveFromDate", string.Format("Please select {0}.", model.GetDisplayName(x => x.EffectiveFromDateBS)));
                else
                    model.EffectiveFromDate = effectiveFromDate;

                var effectiveToDate = model.EffectiveToDateBS.GetDate();
                if (effectiveToDate == DateTime.MinValue)
                    model.AddModelError(x => x.EffectiveToDateBS, "Required.EffectiveToDate", string.Format("Please select {0}.", model.GetDisplayName(x => x.EffectiveToDateBS)));
                else
                    model.EffectiveToDate = effectiveToDate;

                if (model.EffectiveToDate < model.EffectiveFromDate)
                    model.AddModelError(x => x.EffectiveFromDateBS, "Error.EffectiveDateToCannotBeGreaterValueThanEffectiveDateFrom", string.Format("{0} cannot be greater value than {1}.", model.GetDisplayName(x => x.EffectiveToDateBS), model.GetDisplayName(x => x.EffectiveFromDateBS)));

                if (model.HasError)
                    return;

                if (!model.Validate())
                    return;

                var entity = new Substitution();

                if (model.Id > 0)
                {
                    entity = substitutionRepository.GetById(model.Id);
                    entity.SubstitutionTypeId = model.SubstitutionTypeId;
                    entity.EmployeeToSubstitute = model.EmployeeToSubstitute;
                    entity.SubstitutedEmployee = model.SubstitutedEmployee;
                    entity.EffectiveFromDate = model.EffectiveFromDate;
                    entity.EffectiveToDate = model.EffectiveToDate;
                    entity.Remarks = model.Remarks;
                    substitutionRepository.Update(entity);
                }
                else
                {
                    entity = AutomapperConfig.Mapper.Map<Substitution>(model);
                    substitutionRepository.Insert(entity);
                    model.Id = entity.Id;
                }
                if (model.Id == 0)
                    model.Message = localizationService.GetLocalizedText("Message.RecordSavedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record saved succeeded.");
                else
                    model.Message = localizationService.GetLocalizedText("Message.RecordUpdatedSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Record update succeeded.");
            }, model);

            return model;
        }

        public SubstitutionViewModel GetSubstitution(int SubstitutionId)
        {
            var item = substitutionRepository.GetById(SubstitutionId);
            var model = AutomapperConfig.Mapper.Map<SubstitutionViewModel>(item);
            return model;
        }

        public IList<SubstitutionListViewModel> GetSubstitutions(int? SubstitutionTypeId)
        {
            var data = substitutionRepository.Table.Where(x => x.SubstitutionTypeId == SubstitutionTypeId || !SubstitutionTypeId.HasValue).OrderBy(x => x.EffectiveFromDate);
            return AutomapperConfig.Mapper.Map<IList<SubstitutionListViewModel>>(data);
        }

        public ServiceModel DeleteSubstitution(int id)
        {
            var result = new ServiceModel();
            var data = substitutionRepository.GetById(id);
            if (data == null)
            {
                result.Errors.Add(new ValidationResult(localizationService.GetLocalizedText("ErrorMessage.InvalidRequest", IMSAppConfig.Instance.CurrentLanguage, "Invalid Request!!!")));
                return result;
            }

            exceptionService.Execute((m) =>
            {
                substitutionRepository.Delete(data);
            }, result);

            if (result.HasError)
                return result;

            result.Message = localizationService.GetLocalizedText("Common.DeleteSuccessfully", IMSAppConfig.Instance.CurrentLanguage, "Deleted successfully.");
            return result;
        }
        #endregion Substitution
    }
}
