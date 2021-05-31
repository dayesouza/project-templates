using System;

namespace TemplateIHD.Domain.Exceptions
{
    public class HttpInternalServerErrorException : Exception
    {
        private const string DefaultMessage = "An internal server error has ocurred";

        public HttpInternalServerErrorException() : base(DefaultMessage) { }
        public HttpInternalServerErrorException(string message) : base(message) { }
        public HttpInternalServerErrorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
