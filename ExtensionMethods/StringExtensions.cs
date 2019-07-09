using System;

namespace ExtensionMethods
{
    public static class StringExtentions
    {
        /// <summary>
        /// Convert TitleCase string to camelCase string
        /// </summary>
        /// <param name="str">String value to be modified</param>
        /// <returns>Json appropriate string casing</returns>
        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return Char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }
    }
}
