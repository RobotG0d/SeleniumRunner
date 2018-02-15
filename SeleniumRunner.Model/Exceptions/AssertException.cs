using System;

namespace SeleniumRunner.Model.Exceptions
{
    public class AssertException : Exception
    {
        public AssertException(string message) : base(message)
        {
        }
    }
}
