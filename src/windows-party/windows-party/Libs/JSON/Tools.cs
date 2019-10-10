using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace OrletSoir.JSON
{
    /**
     * A collection of various conversion and identification utilities and extension methods
     */
    internal static class Tools
    {
        /// <summary>
        /// Checks if the string is numeric
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>bool</returns>
        public static bool IsNumeric(this string str)
        {
            return Regex.IsMatch(str, @"^[+-]{0,1}[\d]+(\.[\d]+){0,1}([eE][+-]{0,1}[\d]{0,4}){0,1}$");
        }

        /// <summary>
        /// Checks if the string is integer
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>bool</returns>
        public static bool IsInt(this string str)
        {
            return Regex.IsMatch(str, @"^[+-]{0,1}[\d]+$");
        }

        /// <summary>
        /// Returns string's value as an integer; returns 0 if the string is not numerical.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>int representation of string</returns>
        public static int ToInt(this string str)
        {
            int x = 0;

            int.TryParse(str, out x);

            return x;
        }

        /// <summary>
        /// Returns string's value as a double; return 0.0 if the string is not numerical.
        /// </summary>
        /// <param name="str">string</param>
		/// <returns>double representation of string</returns>
        public static double ToDouble(this string str)
        {
            double x = 0;

            double.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out x);

            return x;
        }

        /// <summary>
        /// Returns string's value as a decimal; returns 0.0 if the string is not numerical.
        /// </summary>
        /// <param name="str">string</param>
		/// <returns>decimal representation of string</returns>
        public static decimal ToDecimal(this string str)
        {
            decimal x = 0;

            decimal.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out x);

            return x;
        }

        /// <summary>
        /// Returns string's value as a DateTime; returns defValue if the string is not a valid representation of date.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="defValue">Default value</param>
        /// <returns>DateTime representation of string</returns>
        public static DateTime ToDateTime(this string str, DateTime defValue)
        {
            try
            {
                return Convert.ToDateTime(str);
            }
            catch (FormatException)
            {
                return defValue;
            }
        }

        /// <summary>
        /// Returns unescaped string (C/C++ escaping rules)
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>string</returns>
        public static string Unescape(this string str)
        {
            // Microsoft.JScript.GlobalObject.unescape

            if (string.IsNullOrEmpty(str))
                return str;

            StringBuilder retval = new StringBuilder(str.Length);

            // process string
            for (int ix = 0; ix < str.Length;)
            {
                List<char> decimals = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                List<char> hexes = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

                int jx = str.IndexOf('\\', ix);

                if (jx < 0 || jx == str.Length - 1)
                    jx = str.Length;

                retval.Append(str, ix, jx - ix);

                if (jx >= str.Length)
                    break;

                switch (str[jx + 1])
                {
                    case 'n': // Line feed
                        retval.Append('\n');
                        break;

                    case 't': // Tab
                        retval.Append('\t');
                        break;

                    case 'v': // Vertical tab
                        retval.Append('\v');
                        break;

                    case 'b': // Backspace
                        retval.Append('\b');
                        break;

                    case 'r': // Carriage return
                        retval.Append('\r');
                        break;

                    case 'f': // Form feed
                        retval.Append('\f');
                        break;

                    case 'a': // Alert
                        retval.Append('\a');
                        break;

                    case '\\': // Backslash
                        retval.Append('\\');
                        break;

                    case '?': // Question mark
                        retval.Append('?');
                        break;

                    case '\'': // Single quote
                        retval.Append('\'');
                        break;

                    case '\"': // Double quote
                        retval.Append('\"');
                        break;

                    case '/': // special case - forward slash
                        retval.Append('/');
                        break;

                    case '0': // Null-character
                    case '1': // Octal value - because \0 as octal is the same as \0 as null-character anyway
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                        int x = 0;
                        int y = 0;
                        int i = 0;

                        do
                        {
                            if (!decimals.Contains(str[jx + i + 1]))
                                break;

                            x = decimals.IndexOf(str[jx + i + 1]);
                            y = y * 8 + x;
                            i++;
                        }
                        while (jx + i + 1 < str.Length);

                        jx = jx + i - 1;
                        retval.Append((char)y);
                        break;

                    case 'x': // Hexadecimal character representation
                    case 'u':
                        x = 0;
                        y = 0;
                        i = 1;

                        do
                        {
                            if (!hexes.Contains(str[jx + i + 1]))
                                break;

                            x = hexes.IndexOf(str[jx + i + 1]);
                            y = y * 16 + x;
                            i++;
                        }
                        while (jx + i + 1 < str.Length);

                        jx = jx + i - 1;
                        retval.Append((char)y);
                        break;

                    default: // Unrecognized, copy as-is
                        retval.Append('\\').Append(str[jx + 1]);
                        break;
                }
                ix = jx + 2;
            }

            return retval.ToString();
        }

        /// <summary>
        /// Returns escaped string (C/C++ escaping rules)
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>string</returns>
        public static string ToLiteral(this string input)
        {
            StringWriter writer = new StringWriter();
            CSharpCodeProvider provider = new CSharpCodeProvider();
            provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
            provider.Dispose();

            return writer.ToString();
        }

        /// <summary>
        /// Creates a DateTime object from a unix timestamp.
        /// </summary>
        /// <param name="timestamp">unix timestamp</param>
        /// <returns>DateTime representation of given unix timestamp</returns>
        public static DateTime UnixTimestampToDate(uint timestamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();

            return dtDateTime;
        }

        /// <summary>
        /// Creates a unix timestamp object from a DateTime object.
        /// </summary>
        /// <param name="timestamp">unix timestamp</param>
        /// <returns>DateTime representation of given unix timestamp</returns>
        public static uint ToUnixTimestamp(this DateTime dateTime)
        {
            TimeSpan t = (dateTime - new DateTime(1970, 1, 1).ToLocalTime());

            return (uint)t.TotalSeconds;
        }
    }
}
