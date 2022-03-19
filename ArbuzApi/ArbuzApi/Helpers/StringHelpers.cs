using System;
using System.Linq;

namespace ArbuzApi.Helpers
{
    public static class StringHelpers
    {
        public static string ToCleanString(this string text)
        {
            var arr = text.Split(new char[] { '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim());
            var result = string.Join("", arr);
            return result;
        }
    }
}
