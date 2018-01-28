using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Logic.ViewModels;
using IMS.Logic.ViewModels.BackOffice.Employee;
using PagedList;
using IMS.Data;

namespace IMS.Logic.Contract
{
    public interface IEmployeeService
    {
        EmployeeViewModel SaveEmployee(EmployeeViewModel model);
        IQueryable<Data.Infrastructure.Employee> GetEmployeeQueryable();
        IQueryable<Data.Infrastructure.Employee> GetEmployeeQueryableForCurrentUser(string username, int employeeId, bool showEmployeeUnder = false);
        EmployeeViewModel GetEmployee(string employeeId);
        EmployeeViewModel GetEmployee(int employeeId);
        IList<EmployeeListViewModel> GetEmployees(int? employeeId = null, string name = null, string employeeType = null, int? sectionId = null, int? designationId = null, NTAEnum.eStatus status = NTAEnum.eStatus.Active);
        ServiceModel DeleteEmployee(int id);


        FamilyViewModel SaveFamily(FamilyViewModel model);
        FamilyViewModel GetFamily(int familyId);
        IList<FamilyListViewModel> GetFamilies(int employeeId);
        ServiceModel DeleteFamily(int id);

        ExperienceViewModel SaveExperience(ExperienceViewModel model);
        ExperienceViewModel GetExperience(int experienceId);
        IList<ExperienceListViewModel> GetExperiences(int employeeId);
        ServiceModel DeleteExperience(int id);

        EducationalQualificationViewModel SaveEducationalQualification(EducationalQualificationViewModel model);
        EducationalQualificationViewModel GetEducationalQualification(int educationalQualificationId);
        IList<EducationalQualificationListViewModel> GetEducationalQualifications(int employeeId);
        ServiceModel DeleteEducationalQualification(int id);

        TrainingSeminarConferenceViewModel SaveTrainingSeminarConference(TrainingSeminarConferenceViewModel model);
        TrainingSeminarConferenceViewModel GetTrainingSeminarConference(int TrainingSeminarConferenceId);
        IList<TrainingSeminarConferenceListViewModel> GetTrainingSeminarConferences(int employeeId, string type = null);
        ServiceModel DeleteTrainingSeminarConference(int id);

        AwardDecorationViewModel SaveAwardDecoration(AwardDecorationViewModel model);
        AwardDecorationViewModel GetAwardDecoration(int awardDecorationId);
        IList<AwardDecorationListViewModel> GetAwardDecorations(int employeeId);
        ServiceModel DeleteAwardDecoration(int id);

        PunishmentViewModel SavePunishment(PunishmentViewModel model);
        PunishmentViewModel GetPunishment(int aunishmentId);
        IList<PunishmentListViewModel> GetPunishments(int employeeId);
        ServiceModel DeletePunishment(int id);

        TransferViewModel SaveTransfer(TransferViewModel model);
        TransferViewModel GetTransfer(int transferId);
        IList<TransferListViewModel> GetTransfers(int employeeId);
        ServiceModel DeleteTransfer(int id);

        RetirementViewModel SaveRetirement(RetirementViewModel model);
        RetirementViewModel GetRetirement(int RetirementId);
        IList<RetirementListViewModel> GetRetirements(int employeeId);
        ServiceModel DeleteRetirement(int id);

        SubstitutionViewModel SaveSubstitution(SubstitutionViewModel model);
        SubstitutionViewModel GetSubstitution(int SubstitutionId);
        IList<SubstitutionListViewModel> GetSubstitutions(int? SubstitutionTypeId);
        ServiceModel DeleteSubstitution(int id);
    }
}
