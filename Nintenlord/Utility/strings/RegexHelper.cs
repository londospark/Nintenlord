using System.Text.RegularExpressions;

namespace Nintenlord.Utility.Strings
{
    internal static class RegexHelper
    {
        public static string[] Substrings(this Group group) => Substrings(group.Captures);

        public static string[] Substrings(this CaptureCollection collection)
        {
            var result = new string[collection.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = collection[i].Value;
            }
            return result;
        }

    }
}
