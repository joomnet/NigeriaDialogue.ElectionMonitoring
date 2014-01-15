using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using ElectionMonitoring.Business;
using ElectionMonitoring.Helpers;
using ElectionMonitoring.Controllers.Api;

namespace ElectionMonitoring
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // for some weird reason Unity doesn't seem to work well with Mvc4 WEBAPI controller 
            // (Helpers.IocContainer to the rescue);
            ConfigureApi(GlobalConfiguration.Configuration);
            Bootstrapper.Initialise();
        }

        void ConfigureApi(HttpConfiguration config)
        {
            var unity = new UnityContainer();
            unity.RegisterType<ElectionMonitoringController>();
            unity.RegisterType<IRaceRepository, RaceRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<IRaceResultService, RaceResultService>(new HierarchicalLifetimeManager());
            unity.RegisterType<IRegionRepository, RegionRepository>(new HierarchicalLifetimeManager());

            unity.RegisterType<DonationManagementController>();
            unity.RegisterType<IDonationRepository, DonationRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<IDonorRepository, DonorRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<IProjectRepository, ProjectRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<IBudgetRepository, BudgetRepository>(new HierarchicalLifetimeManager());

            config.DependencyResolver = new IoCContainer(unity);
        }
    }
}