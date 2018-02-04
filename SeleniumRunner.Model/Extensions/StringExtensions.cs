using System;

namespace SeleniumRunner.Model.Extensions
{
    internal static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string src, string trgt)
        {
            return src.Equals(trgt, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
