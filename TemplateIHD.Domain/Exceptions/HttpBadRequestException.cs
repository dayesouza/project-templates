using System;

namespace TemplateIHD.Domain.Exceptions
{
    public class HttpBadRequestException : Exception
    {
        private const string DefaultMessage = "HTTP request was in a bad or invalid state";

        public HttpBadRequestException() : base(DefaultMessage) { }
        public HttpBadRequestException(string message) : base(message) { }
        public HttpBadRequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}
