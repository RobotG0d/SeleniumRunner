using System;

namespace SeleniumRunner.Model.Exceptions
{
    public class UnsupportedException : Exception
    {
        public UnsupportedException(string message) : base(message)
        {
        }
    }
}
