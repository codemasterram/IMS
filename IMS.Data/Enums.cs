using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Data
{
    public static class NTAEnum
    {
        public enum eUserType
        {
            Employee = 1,
            Customer = 2
        }

        public enum ePBISValueType
        {
            Supervisor = 1,
            Reviewer = 2
        }

        public enum eMonth
        {
            Baishakh = 1,
            Jestha = 2,
            Ashadh = 3,
            Srawan = 4,
            Bhadra = 5,
            Aswin = 6,
            Kratik = 7,
            Mansir = 8,
            Pause = 9,
            Marg = 10,
            Falgun = 11,
            Chaitra = 12
        }

        public enum eDataStatus
        {
            None = 0,
            Active = 1,
            Deleted = 2
        }

        public enum eStatus
        {
            None = 0,
            Active = 1,
            InActive = 2
        }

        public enum eSelectListType
        {
            Gender = 1,
            MaritalStatus = 2,
            EmployeeType = 3,
            EmployeeStatus = 4,
            ApplicableFor = 5,
            CheckType = 6,
            ApprovedStatus = 7,
            AttendanceReportCategory = 8,
            FrequencyUnit = 9,
            StockInOutType = 10,
            IpType,
            BandWidthUnit,
            InternetServiceType,
            Trimester,
        }

        public enum eAddressType
        {
            Permanent = 1,
            Temporary = 2,
            Nominee = 3
        }

        public enum ePermission
        {
            None = 0,
            ManageAccount = 19,
            ManageTypeApproval = 23,
            ManageCustomer = 29,
            ManageFrequency = 17,
            HRSetup = 16,
            ManageEAttendance = 12,
            ManageEmployee = 3,
            PBISFormDistribution = 5,
            PBISReportView = 7,
            PBISValueFill = 6,
            RequestLeaveApplication = 11,
            RequestManualAttendance = 14,
            RequestTravelApplication = 8,
            ResponseLeaveApplication = 13,
            ResponseManualAttendance = 15,
            ResponseTravelApplication = 9,
            ViewEAttendanceData = 10,
            ViewEmployee = 4,
            UploadDocument = 21,
            InventoryEntry = 27,
            InventorySetup = 28,
            ViewInventoryReport = 26,
            ManageLIcense = 18,
            ManageQOS = 22,
            SendNotification = 25,
            ManageShortCode = 20,
            ManageRoles = 2,
            ManageUser = 1,
            ManageWHOISISP = 24,
            SystemSetup = 30
        }

        public enum eModule
        {
            None = 0,
            UserManagement = 1,
            HumanResourceManagement = 2,
            FrequencyManagement = 4,
            LicenseManagement = 5,
            Accounting = 7,
            InventoryManagement = 8,
            ShortCodeAssignmentAndManagement = 9,
            QOSManagement = 11,
            TypeApproval = 12,
            WHOISISPDatabaseManagement = 13,
            CustomerManagement = 15,
            SetupModule = 16
        }

        public enum eType
        {
            Training = 1,
            Seminar = 2,
            Conference = 3
        }

        public enum eDocumentModule
        {
            None = 0,
            Employee = 1,
            TypeApprovalApplication = 2,
            LicenseApplication = 3,
            LicenseSaleOrTransfer = 4,
            SampleImportAuthorization = 5,
            TypeApprovalRenewal = 6,
            ShortCodeApplication = 7
        }

        public enum eDocumentType
        {
            None = 0,
            EmployeeProfilePicture = 1,
            NomineePhoto = 2,
            NomineeForm = 3,
            EmployeeMedicalClearance = 4,
            EmployeeEducationalCertificate = 5,
            EmployeeExperienceVerificationForm = 6,
            EmployeePromotionLetter = 7,
            EmployeeRetirementLetter = 8,
            EmployeeSubstitutionCertificate = 9,
            EmployeeThumbRight = 10,
            EmployeeThumbLeft = 11,
            NomineeThumbRight = 12,
            NomineeThumbLeft = 13,
            EmployeeTrainingSeminarCertificate = 14,
            EmployeeWorkExperience = 15,
            TestLog = 16,
            EmployeeJobApplication = 17,
            EmployeeAdmissionCard = 18,
            EmployeeAppointmentLetter = 19,
            EmployeeOathTakenLetter = 20,
            EmployeeCitizenshipCertificate = 21,
            EmployeePassportCertificate = 22,
            TypeApprovalApplicationPaymentVoucher = 23,
            TypeApprovalCircuitDiagramBlockDiagram = 24,
            TypeApprovalProductBrochures = 25,
            TypeApprovalOperationInstallationUserManual = 26,
            TypeApprovalInternalAndExternalPhotographOfEquipment = 27,
            TypeApprovalEquipmentTestReport = 28,
            TypeApprovalDeclarationOfConformityDoc = 29,
            TypeApprovalLocalRepresentativeCompanyRegistrationCertificate = 30,
            TypeApprovalLocalRepresentativePANCertificate = 31,
            TypeApprovalLetterOfAuthoriztion = 32,
            TypeApprovalOtherDocument = 33,
            CustomerProfilePhoto = 34,
            TypeApprovalCompanySeal = 35,
            EmployeeAwardOrDecoration = 36,
            EmployeePunishment = 37,
            EmployeeTransfer = 38,
            EmployeeLeaveApplication = 39,
            OLMCitizenship = 40,
            OLMLegalDocument = 41,
            OLMTechnicalDocument = 42,
            TASampleRequestLetter = 43,
            TASampleInvoice = 44,
            TASampleCompanyRegistrationCertificate = 45,
            TASamplePANCertificate = 46,
            TARenewalApplicationFeeInvoice = 47,
            ShortCodeRenewalApplicationFeeInvoice = 48
        }

        public enum ePreviewSize
        {
            Small = 1,
            Middle = 2
        }

        public enum eEmailTemplateType
        {
            None = 0,
            LeaveRequest = 1,
            LeaveRecommend = 2,
            LeaveForward = 3,
            LeaveResponse = 4,
            TravelRequest = 5,
            TravelRecommend = 6,
            TravelForward = 7,
            TravelResponse = 8,
            UserCreated = 9,
            PasswordReset = 10,
            ManualAttendanceApproveRequest = 11,
            ManualAttendanceApproveResponse = 12,
            PBISFormToSupervisor = 13,
            PBISFormToReviewer = 14,
            TypeApprovalCustomerConfirmation = 15,
            ShortCodeCustomerConfirmation = 16
        }

        public enum eRecommendStatus
        {
            Pending = 1,
            Accept = 2,
            Reject = 3
        }

        public enum eApproveStatus
        {
            Pending = 1,
            Accept = 2,
            Reject = 3
        }

        public enum eCheckType
        {
            In = 1,
            Out = 2
        }

        public enum eAttendanceReportCategory
        {
            All = 1,
            Present = 2,
            Absent = 3,
            Late = 4,
            Punctual = 5
        }

        public enum CurrentLang
        {
            En = 1,
            Np = 2
        }

        public enum eWorkFlowType
        {
            None = 0,
            TypeApprovalApplication = 1,
            LeaveApplication = 2,
            TravelApplication = 3,
            ItemRequest = 4,
            PurchaseOrder = 5,
            LicenseApplication = 6,
            LicenseTransfer = 7,
            SampleEquipmentImportAuthorizationApplication = 8,
            TypeApprovalRenewalApplication = 9,
            ShortCodeApplication = 10,
            ShortCodeRenewalApplication = 11,
            QOSApplication = 12
        }

        public enum eWorkflowStatus
        {
            Pending = 1,
            Accepted = 2,
            Rejected = 3,
            Complete = 4
        }

        public enum eWorkFlowActionType
        {
            None = 0,
            Commented = 1,
            Forwarded = 2,
            ApprovedAndForwarded = 3,
            Closed = 4,
            Rejected = 5
        }

        public enum eProcessStatus
        {
            Draft = 0,
            Applied = 1,
            Process = 2,
            Completed = 3,
            Rejected = 4,
        }

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


        public enum eApplicationStatus
        {
            None = 0,
            Applied = 1,
            Processing = 2,
            Accepted = 3,
            Rejected = 4
        }

        public enum eClientModule
        {
            None = 0,
            FrequencyManagement = 4,
            LicenseManagement = 5,
            ShortCodeAssignmentAndManagement = 9,
            QualityOfService = 11,
            TypeApproval = 12,
            WHOISISPDatabaseManagement = 13,
        }

        public enum eTypeApprovalApplicationType
        {
            TypeApproval = 1,
            SampleImportAuthorization = 2
        }

        public enum eTypeApprovalFrequencyBrandType
        {
            Demanded = 0,
            Allocated = 1
        }

        public enum eShortCodeApplicationType
        {
            ShortCode = 1,
            ShortCodeRenewal = 2
        }
    }
}
