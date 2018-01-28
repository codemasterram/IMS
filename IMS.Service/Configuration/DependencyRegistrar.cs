using Autofac;
using IMS.Data;
using IMS.Data.Infrastructure;
using IMS.Logic.Contract;
using IMS.Logic.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IMS.Logic.Configuration
{
    public class DependencyRegistrar
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterModule(new DbModule());
            builder.RegisterModule(new ServiceModule());
        }
    }

    public class DbModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<NTADbContext>().As<IDbContext>().WithProperty("CurrentUserEmployeeId", 0).InstancePerRequest();
            builder.Register<IDbContext>(x =>
            {
                var context = new IMSDbContext();

                if (HttpContext.Current != null && HttpContext.Current.User != null)
                {
                    context.CurrentUserName = HttpContext.Current.User.Identity.Name;
                }

                return context;
            }).InstancePerRequest();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerRequest();
        }
    }

    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DocumentService>().As<IDocumentService>().InstancePerRequest();
            builder.RegisterType<HRService>().As<IHRService>().InstancePerRequest();
            builder.RegisterType<InventoryService>().As<IInventoryService>().InstancePerRequest();
            builder.RegisterType<MessagingService>().As<IMessagingService>().InstancePerRequest();
            builder.RegisterType<LocalizationService>().As<ILocalizationService>().InstancePerRequest();
            builder.RegisterType<CommonService>().As<ICommonService>().InstancePerRequest();
            builder.RegisterType<AuthService>().As<IAuthService>().InstancePerRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<ExceptionService>().As<IExceptionService>().InstancePerRequest();
            builder.RegisterType<EmployeeService>().As<IEmployeeService>().InstancePerRequest();
            builder.RegisterType<EmployeeProfileService>().As<IEmployeeProfileService>().InstancePerRequest();
            builder.RegisterType<DashboardService>().As<IDashboardServicecs>().InstancePerRequest();
            builder.RegisterType<ImsMasterService>().As<IImsMasterService>().InstancePerRequest();
            builder.RegisterType<ImsEntryService>().As<IImsEntryService>().InstancePerRequest();
            builder.RegisterType<ImsReportService>().As<IImsReportService>().InstancePerRequest();
            builder.RegisterType<SetupService>().As<ISetupService>().InstancePerRequest();
            builder.RegisterType<DocumentNumberingService>().As<IDocumentNumberingService>().InstancePerRequest();
           
        }
    }
}
