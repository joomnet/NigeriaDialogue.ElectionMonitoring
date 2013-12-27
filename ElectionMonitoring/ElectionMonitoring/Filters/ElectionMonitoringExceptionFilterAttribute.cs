using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace ElectionMonitoring.Filters
{
    using System.Net.Http;
    using System.Net;
    using System.Web.Http;

    public class ElectionMonitoringExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var exception = new ElectionMonitoring.Business.ElectionMonitoringException("ERROR! An unspecified error has occured");
            if (actionExecutedContext.Exception is NotSupportedException)
            {
                statusCode = HttpStatusCode.NoContent;
                exception = new ElectionMonitoring.Business.ElectionMonitoringException("Operation requested in not supported");
            }
            else if (actionExecutedContext.Exception is InvalidOperationException)
            {
                statusCode = HttpStatusCode.Ambiguous;
                exception = new ElectionMonitoring.Business.ElectionMonitoringException("Operation requested in not allowed");
            }
            else if (actionExecutedContext.Exception is NotImplementedException)
            {
                statusCode = HttpStatusCode.NotImplemented;
                exception = new ElectionMonitoring.Business.ElectionMonitoringException("Implementation is still underway");
            }
            else if (actionExecutedContext.Exception is ArgumentNullException)
            {
                statusCode = HttpStatusCode.NotImplemented;
                exception = new ElectionMonitoring.Business.ElectionMonitoringException(actionExecutedContext.Exception.Message);
            }
            else if (actionExecutedContext.Exception is NullReferenceException )
            {
                statusCode = HttpStatusCode.NotImplemented;
                exception = new ElectionMonitoring.Business.ElectionMonitoringException(actionExecutedContext.Exception.Message);
            }
            else
            {
                statusCode = HttpStatusCode.BadRequest;                
            }
            throw new HttpResponseException(actionExecutedContext.Request.CreateErrorResponse(statusCode,exception));
            
        }
    }

}