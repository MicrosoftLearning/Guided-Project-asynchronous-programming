using System;

namespace Json
{
    public class JSONSerializationException : Exception
    {
        public JSONSerializationException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}