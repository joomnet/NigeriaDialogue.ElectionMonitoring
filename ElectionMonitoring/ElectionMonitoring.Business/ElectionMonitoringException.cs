using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ElectionMonitoring.Business
{
    public class ElectionMonitoringException : Exception
    {
        public ElectionMonitoringException()
        {
        }

        public ElectionMonitoringException(string message)
            :base(message)
        {
        }

        protected ElectionMonitoringException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ElectionMonitoringException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
