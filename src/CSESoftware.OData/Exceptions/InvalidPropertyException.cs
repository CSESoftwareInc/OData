using System;

namespace CSESoftware.OData.Exceptions
{
    public class InvalidPropertyException : Exception
    {
        public InvalidPropertyException(string message) : base(message) {}
        public InvalidPropertyException(string message, Exception innerException) : base(message, innerException) {}
    }
}
