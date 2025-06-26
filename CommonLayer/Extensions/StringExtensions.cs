using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonLayer.Extensions
{
    public static class StringExtensions
    {
        public const int LogMaxStrLen = 5028;

        public static string Chop(this string str, int maxLen = LogMaxStrLen)
        {
            if (!string.IsNullOrWhiteSpace(str) || maxLen <= 0) return string.Empty;
            if (str.Length > maxLen) return string.Format("{0} .... string is too long to display full string: {1:#,###} bytes", str.Substring(0, maxLen), str.Length);

            return str;
        }

        public static string NullToString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;
            if (str == null) return default(string); //string.Format("");
            return str; 
        }

        public static int NullToInt(this int intval)
        {

            if (intval == null) return default(int);
            return intval;
        }

        public static bool IsObjEqualToString(this object left, string right)
        {
            if (left == null) return false;
            return left.ToString().Equals(right);
        }

        public static bool IsObjNotEqualToString(this object left, string right)
        {
            if (left == null) return true;
            return left.ToString().IsNotEqualTo(right);
        }        
        public static bool IsEqualTo(this string left, string right)
        {
            if (left == null && right == null) return true;
            if (left == null || right == null) return false;

            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }
        public static bool IsNotEqualTo(this string left, string right)
        {
            return !IsEqualTo(left, right);
        }

        public static int ToInt(this string str)
        {
            var result = 0;
            str.IfNotNull(x => int.TryParse(x, out result));

            return result;
        }

        public static long ToLong(this string str)
        {
            long result = 0;
            str.IfNotNull(x => long.TryParse(x, out result));

            return result;
        }

        public static double ToDouble(this string str)
        {
            double result = 0.0;
            str.IfNotNull(x => double.TryParse(x, out result));

            return result;
        }

        public static bool IsTrue(this string t)
        {
            return t.IsEqualTo("Y");
        }

        public static bool IsFalse(this string t)
        {
            return t.IsNotEqualTo("Y");
        }

        public static string AppendAmpersand(this string str)
        {
            var appendString = str;
            if(!string.IsNullOrWhiteSpace(str))
            {
                appendString = str.Replace("gt;", "&gt;").Replace("lt;", "&lt");
            }
            return appendString;
        }

        public static string ReplaceHTMLSymbols(this string str)
        {
            var result = str;
            if (!string.IsNullOrWhiteSpace(str))
            {
                result = str.Replace("gt;", ">").Replace("lt;", "<");
            }
            return result;
        }

        public static string Take(this string theString, int count, bool ellipsis = false)
        {
            int lengthToTake = Math.Min(count, theString.Length);
            var cutDownString = theString.Substring(0, lengthToTake);

            if (ellipsis && lengthToTake < theString.Length)
                cutDownString += "...";

            return cutDownString;
        }

        //like linq skip - skips the first x characters and returns the remaining string
        public static string Skip(this string theString, int count)
        {
            int startIndex = Math.Min(count, theString.Length);
            var cutDownString = theString.Substring(startIndex - 1);

            return cutDownString;
        }

        //reverses the string... pretty obvious really
        public static string ReverseWords(this string text)
        {
            var wordsList = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Reverse();
            return string.Join(" ", wordsList);
        }

        public static string ReverseString(this string text)
        {
            return string.Concat(Enumerable.Reverse(text));
        }

        //not so sure about this one -
        //"a string {0}".Format("blah") vs string.Format("a string {0}", "blah")
        public static string With(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        //ditches html tags - note it doesnt get rid of things like &nbsp;
        public static string StripHtml(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            return Regex.Replace(html, @"<[^>]*>", string.Empty);
        }

        public static bool Match(this string value, string pattern)
        {
            return Regex.IsMatch(value, pattern);
        }

        //splits string into array with chunks of given size. not really that useful..
        public static string[] SplitIntoChunks(this string toSplit, int chunkSize)
        {
            if (string.IsNullOrEmpty(toSplit))
                return new string[] { "" };

            int stringLength = toSplit.Length;

            int chunksRequired = (int)Math.Ceiling((decimal)stringLength / (decimal)chunkSize);
            var stringArray = new string[chunksRequired];

            int lengthRemaining = stringLength;

            for (int i = 0; i < chunksRequired; i++)
            {
                int lengthToUse = Math.Min(lengthRemaining, chunkSize);
                int startIndex = chunkSize * i;
                stringArray[i] = toSplit.Substring(startIndex, lengthToUse);

                lengthRemaining = lengthRemaining - lengthToUse;
            }

            return stringArray;
        }

        public static string TrimLastCharacter(this String str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                return str.TrimEnd(str[str.Length - 1]);
            }
        }

        /// <summary>
        /// This method will work only when string passed in yyyymmdd format
        /// </summary>
        /// <param name="str">passed in yyyymmdd format</param>
        /// <returns>Nullable Datetime</returns>
        public static DateTime? StringToDateTime(this string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return  new DateTime(Convert.ToInt16(str.Substring(0, 4)), Convert.ToInt16(str.Substring(4,2)), Convert.ToInt16(str.Substring(5,2)));
            }
            return  new DateTime();
        }
    }
}
