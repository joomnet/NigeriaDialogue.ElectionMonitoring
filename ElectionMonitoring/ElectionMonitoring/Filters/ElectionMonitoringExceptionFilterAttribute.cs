using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using ElectionMonitoring.Business;

namespace ElectionMonitoring.Filters
{
    public class ElectionMonitoringExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var exception = new ElectionMonitoringException("ERROR! An unspecified error has occured");
            if (actionExecutedContext.Exception is NotSupportedException)
            {
                statusCode = HttpStatusCode.NoContent;
                exception = new ElectionMonitoringException("Operation requested in not supported");
            }
            else if (actionExecutedContext.Exception is InvalidOperationException)
            {
                statusCode = HttpStatusCode.Ambiguous;
                exception = new ElectionMonitoringException("Operation requested in not allowed");
            }
            else if (actionExecutedContext.Exception is NotImplementedException)
            {
                statusCode = HttpStatusCode.NotImplemented;
                exception = new ElectionMonitoringException("Implementation is still underway");
            }
            else if (actionExecutedContext.Exception is ArgumentNullException)
            {
                statusCode = HttpStatusCode.NotImplemented;
                exception = new ElectionMonitoringException(actionExecutedContext.Exception.Message);
            }
            else if (actionExecutedContext.Exception is NullReferenceException)
            {
                statusCode = HttpStatusCode.NotImplemented;
                exception = new ElectionMonitoringException(actionExecutedContext.Exception.Message);
            }
            else
            {
                statusCode = HttpStatusCode.BadRequest;
            }
            throw new HttpResponseException(actionExecutedContext.Request.CreateErrorResponse(statusCode, exception));
        }
    }
}