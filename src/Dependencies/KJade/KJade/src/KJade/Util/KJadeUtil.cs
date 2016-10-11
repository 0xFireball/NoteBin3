using System;

namespace KJade.Util
{
    internal static class KJadeUtil
    {
        /// <summary>
        /// Strips all the strings that are in the array from the string
        /// </summary>
        /// <param name="str">The string to operate on</param>
        /// <param name="characters">The characters to strip</param>
        /// <returns></returns>
        public static string Strip(this string str, string[] characters)
        {
            int strippedChars = 0;
            foreach (var st in characters)
            {
                var ol = str.Length;
                str = str.Replace(st, "");
                var nl = str.Length;
                strippedChars += ol - nl;
            }
            return str;
        }

        /// <summary>
        /// Removes a string from the beginning of a string. The string must begin with the string to eat!
        /// </summary>
        /// <param name="str">The string to operate on</param>
        /// <param name="eat">The string to eat</param>
        /// <returns></returns>
        public static string Eat(this string str, string eat)
        {
            if (!str.StartsWith(eat, StringComparison.CurrentCulture)) { throw new ArgumentException("The input string must begin with the string to eat.", nameof(str)); }
            for (int i = 0; eat.Length > 0 && str[0] == eat[0]; i++)
            {
                str = str.Substring(1);
                eat = eat.Substring(1);
            }
            return str;
        }
    }
}