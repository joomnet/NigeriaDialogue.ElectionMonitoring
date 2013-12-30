using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Routing;
using System.Net.Http;

namespace ElectionMonitoring
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            string alphanumeric = @"^[a-zA-Z]+[a-zA-Z0-9_]*$";
            string numeric = @"^\d+$";

            config.Routes.MapHttpRoute(
                name: "ApiAboutElectionMonitoring",
                routeTemplate: "api/{controller}",
                defaults: new { action = "about" }
            );

            config.Routes.MapHttpRoute(
                name: "ApiRegionByRegionCode",
                routeTemplate: "api/electionmonitoring/regions/{regioncode}",
                defaults: new { controller = "electionmonitoring", action = "regions", regioncode = RouteParameter.Optional },
                constraints: new { action = alphanumeric}
            );

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { action = alphanumeric} // action must start with character
            );

            config.Routes.MapHttpRoute(
                name: "ApiRaceResult",
                routeTemplate: "api/electionmonitoring/raceresults/{id}/{regioncode}",
                defaults: new { controller = "electionmonitoring", action = "raceresults", regioncode = RouteParameter.Optional },
                constraints: new { action = alphanumeric,  id = numeric, regioncode = alphanumeric } 
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApiControllerPost",
                routeTemplate: "api/{controller}",
                defaults: new { action = "Post" },
                constraints: new { httpMethod = new HttpMethodConstraint(new string[] { "Post" }) }
            );

            config.Filters.Add(new ElectionMonitoring.Filters.ElectionMonitoringExceptionFilterAttribute());

            /* sets the JSON formatter to preserve object references, and removes the XML formatter from the pipeline entirely. 
             * (You can configure the XML formatter to preserve object references, 
             * but it's a little more work, and we only need JSON for this application. For more information, 
             * see Handling Circular Object References [http://www.asp.net/web-api/overview/formats-and-model-binding/json-and-xml-serialization#handling_circular_object_references].) */
            //var json = config.Formatters.JsonFormatter;
            //json.SerializerSettings.PreserveReferencesHandling =
            //    Newtonsoft.Json.PreserveReferencesHandling.Objects;

            //var json = config.Formatters.JsonFormatter;
            //json.SerializerSettings.PreserveReferencesHandling =
            //    Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.JsonFormatter.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings
                {
                    PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All
                    , ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize
                    , 
                };


            //config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = new Newtonsoft.Json.ReferenceLoopHandling.Serialize;
            //config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            //  config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // when using the next line, it is best to install a json formatter plug-in in the browser.
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
