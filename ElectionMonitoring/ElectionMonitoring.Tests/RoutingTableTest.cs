using System;
using ElectionMonitoring.Controllers.Api;
using NUnit.Framework;

namespace ElectionMonitoring.Tests
{
    using Moq;
    using System.Web;
    using System.Web.Routing;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Http.Controllers;
    using System.Net.Http;
    using System.Web.Http.Hosting;
    using System.Web.Http.Dispatcher;

    [TestFixture]
    public class RoutingTableTest
    {
        private RouteCollection routes;
        HttpConfiguration webApiConfig;
        private Mock<HttpContextBase> mockHttpContext;
        private string alphanumeric = @"^[a-zA-Z]+[a-zA-Z0-9_]*$";
        private string numeric = @"^\d+$";

        [SetUp]
        public void SetUp()
        {
            routes = new RouteCollection();
            webApiConfig = new HttpConfiguration();
            webApiConfig.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            webApiConfig.Routes.MapHttpRoute(name: "ApiAbout", routeTemplate: "api/{controller}", defaults: new { action = "about" });
            webApiConfig.Routes.MapHttpRoute(name: "ApiRegionByRegionCode", routeTemplate: "api/electionmonitoring/regions/{regioncode}", defaults: new { controller = "electionmonitoring", action = "regions", regioncode = RouteParameter.Optional });
            webApiConfig.Routes.MapHttpRoute(name: "ActionApi", routeTemplate: "api/{controller}/{action}/{id}", defaults: new { id = RouteParameter.Optional });
            webApiConfig.Routes.MapHttpRoute(name: "ApiRaceResult", routeTemplate: "api/electionmonitoring/raceresults/{id}/{regioncode}", defaults: new { controller = "electionmonitoring", action = "raceresults", regioncode = RouteParameter.Optional },
                constraints: new { id = numeric, action = alphanumeric });
            webApiConfig.Routes.MapHttpRoute(name: "DefaultApi", routeTemplate: "api/{controller}/{id}", defaults: new { id = RouteParameter.Optional });                                   
        }

        [TearDown]
        public void TearDown()
        {
            routes = null;
            webApiConfig = null;
        }

        #region "RouteConfig Test"
        [Test]
        public void DefaultRouteConfigTest()
        {
            // Arrange
            mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");
            RouteConfig.RegisterRoutes(routes);
           
            // Act
            var routeData = routes.GetRouteData(mockHttpContext.Object);

            // Assert
            Assert.IsNotNull(routeData);
            Assert.AreEqual("ElectionResult", routeData.Values["controller"]);
            Assert.AreEqual("RaceResults", routeData.Values["action"]);
            Assert.AreEqual(UrlParameter.Optional, routeData.Values["id"]);
        }

        #endregion
        
        #region WebApiConfig Tests

        [Test]
        public void ApiRegionByRegionID()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/electionmonitoring/regions/12");
            var routeData = webApiConfig.Routes.GetRouteData(request);
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;

            // Act
            var controllerContext = new HttpControllerContext(webApiConfig, routeData, request);
            var controllerDescriptor = new DefaultHttpControllerSelector(webApiConfig).SelectController(request);
            controllerContext.ControllerDescriptor = controllerDescriptor;

            // Assert
            Assert.AreEqual("electionmonitoring", controllerContext.RouteData.Values["controller"].ToString().ToLower(), "Controller should be ElectionMonitoring, but is " + controllerContext.RouteData.Values["controller"]);
            Assert.AreEqual("regions", controllerContext.RouteData.Values["action"].ToString().ToLower(), "ActionName should be Races, but is " + controllerContext.RouteData.Values["action"]);
            Assert.AreEqual(12, Convert.ToInt32(controllerContext.RouteData.Values["regioncode"]), "Region code should be 12, but is " + controllerContext.RouteData.Values["regioncode"]);

        }

        [Test]
        public void ApiRegionByRegionCode()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/electionmonitoring/regions/ct");
            var routeData = webApiConfig.Routes.GetRouteData(request);
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;

            // Act
            var controllerContext = new HttpControllerContext(webApiConfig, routeData, request);
            var controllerDescriptor = new DefaultHttpControllerSelector(webApiConfig).SelectController(request);
            controllerContext.ControllerDescriptor = controllerDescriptor;

            // Assert
            Assert.AreEqual("electionmonitoring", controllerContext.RouteData.Values["controller"].ToString().ToLower(), "Controller should be ElectionMonitoring, but is " + controllerContext.RouteData.Values["controller"]);
            Assert.AreEqual("regions", controllerContext.RouteData.Values["action"].ToString().ToLower(), "ActionName should be Races, but is " + controllerContext.RouteData.Values["action"]);
            Assert.AreEqual("ct", controllerContext.RouteData.Values["regioncode"].ToString().ToLower(), "Region code should be ct, but is " + controllerContext.RouteData.Values["regioncode"]);            
        }

        [Test]
        public void ActionApiRouteWithControllerWithActionNoId()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/electionmonitoring/Races");
            var routeData = webApiConfig.Routes.GetRouteData(request);
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;

            // Act
            var controllerContext = new HttpControllerContext(webApiConfig, routeData, request);            
            var controllerDescriptor = new DefaultHttpControllerSelector(webApiConfig).SelectController(request);
            controllerContext.ControllerDescriptor = controllerDescriptor;
            //var actionDescriptor = new ApiControllerActionSelector().SelectAction(controllerContext);

            // Assert
            Assert.AreEqual(typeof(ElectionMonitoringController), controllerDescriptor.ControllerType);
            Assert.AreEqual("electionmonitoring", controllerContext.RouteData.Values["controller"].ToString().ToLower(), "Controller should be ElectionMonitoring, but is " + controllerContext.RouteData.Values["controller"]);
            Assert.AreEqual("electionmonitoring", controllerDescriptor.ControllerName.ToLower(), "Controller should be ElectionMonitoring, but is " + controllerDescriptor.ControllerName);
            Assert.AreEqual("races", controllerContext.RouteData.Values["action"].ToString().ToLower(), "ActionName should be Races, but is " + controllerContext.RouteData.Values["action"]);
            //Assert.AreEqual("races", actionDescriptor.ActionName.ToLower(), "ActionName should be Races, but is " + actionDescriptor.ActionName); 
        }

        [Test]
        public void ActionApiRouteWithControllerWithActionWithId()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/electionmonitoring/Regions/1");
            var routeData = webApiConfig.Routes.GetRouteData(request);
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;

            // Act 
            var controllerContext = new HttpControllerContext(webApiConfig, routeData, request);

            // Assert
            Assert.AreEqual("electionmonitoring", controllerContext.RouteData.Values["controller"].ToString().ToLower(), "Controller should be ElectionMonitoring, but is " + controllerContext.RouteData.Values["controller"]);            
            Assert.AreEqual("regions", controllerContext.RouteData.Values["action"].ToString().ToLower(), "ActionName should be Regions, but is " + controllerContext.RouteData.Values["action"]);
            Assert.AreEqual(1, Convert.ToInt32(controllerContext.RouteData.Values["regioncode"]), "id should be 1, but is " + controllerContext.RouteData.Values["regioncode"]);
            
        }

        [Test]
        public void RaceResultsRouteWithRaceIDWithRegionCode()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/electionmonitoring/Raceresults/1/AB");
            var routeData = webApiConfig.Routes.GetRouteData(request);
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;

            // Act 
            var controllerContext = new HttpControllerContext(webApiConfig, routeData, request);

            // Assert
            Assert.AreEqual("electionmonitoring", controllerContext.RouteData.Values["controller"].ToString().ToLower(), "Controller should be ElectionMonitoring, but is " + controllerContext.RouteData.Values["controller"]);
            Assert.AreEqual("raceresults", controllerContext.RouteData.Values["action"].ToString().ToLower(), "ActionName should be Regions, but is " + controllerContext.RouteData.Values["action"]);
            Assert.AreEqual(1, Convert.ToInt32(controllerContext.RouteData.Values["id"]), "id should be 1, but is " + controllerContext.RouteData.Values["id"]);
            Assert.AreEqual("ab", Convert.ToString(controllerContext.RouteData.Values["regioncode"]).ToLower(), "regioncode should be AB, but is " + controllerContext.RouteData.Values["regioncode"]);            
        }

        [Test]
        public void RaceResultsRouteWithRaceIDNoRegionCode()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/electionmonitoring/Raceresults/1/");
            var routeData = webApiConfig.Routes.GetRouteData(request);
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;

            // Act 
            var controllerContext = new HttpControllerContext(webApiConfig, routeData, request);

            // Assert
            Assert.AreEqual("electionmonitoring", controllerContext.RouteData.Values["controller"].ToString().ToLower(), "Controller should be ElectionMonitoring, but is " + controllerContext.RouteData.Values["controller"]);
            Assert.AreEqual("raceresults", controllerContext.RouteData.Values["action"].ToString().ToLower(), "ActionName should be Regions, but is " + controllerContext.RouteData.Values["action"]);
            Assert.AreEqual(1, Convert.ToInt32(controllerContext.RouteData.Values["id"]), "id should be 1, but is " + controllerContext.RouteData.Values["id"]);
            Assert.AreEqual(controllerContext.RouteData.Route.RouteTemplate, "api/{controller}/{action}/{id}");
        }


        [Test]
        public void AboutApi()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/electionmonitoring/");
            var routeData = webApiConfig.Routes.GetRouteData(request);
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;

            // Act 
            var controllerContext = new HttpControllerContext(webApiConfig, routeData, request);

            // Assert
            Assert.AreEqual("electionmonitoring", controllerContext.RouteData.Values["controller"].ToString().ToLower(), "Controller should be ElectionMonitoring, but is " + controllerContext.RouteData.Values["controller"]);
            Assert.AreEqual("about", controllerContext.RouteData.Values["action"].ToString().ToLower(), "ActionName should be Regions, but is " + controllerContext.RouteData.Values["action"]);
            Assert.AreEqual(controllerContext.RouteData.Route.RouteTemplate, "api/{controller}");
        }

        #endregion

    }
}
