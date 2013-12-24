using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using ElectionMonitoring.Business;
using ElectionMonitoring.Controllers.Api;

namespace ElectionMonitoring
{
  public static class Bootstrapper
  {
    public static IUnityContainer Initialise()
    {
      var container = BuildUnityContainer();

      DependencyResolver.SetResolver(new UnityDependencyResolver(container));

      return container;
    }

    private static IUnityContainer BuildUnityContainer()
    {
      var container = new UnityContainer();

      // register all your components with the container here
      // it is NOT necessary to register your controllers

      // e.g. container.RegisterType<ITestService, TestService>();    
      RegisterTypes(container);

      return container;
    }

    public static void RegisterTypes(IUnityContainer container)
    {
        //container.RegisterType(ElectionMonitoringController, ElectionMonitoringController);
        //container.RegisterType<IRaceRepository, RaceRepository>();
        //container.RegisterType<IRaceResultService, RaceResultService>();
        //container.RegisterType<IRegionRepository, RegionRepository>();
    }
  }
}