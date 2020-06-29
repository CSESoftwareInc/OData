using System;

namespace CSESoftware.OData.Exceptions
{
    public class OrderingException : Exception
    {
        public OrderingException(string message) : base(message) {}
    }
}
