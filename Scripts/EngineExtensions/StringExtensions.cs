using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public static class StringExtensions
    {
        public class NumericEndComparer : IComparer<string>
        {
            public int Compare(string lhs, string rhs)
            {
                return lhs.NumericCompareTo(rhs);
            }
        }

        private static readonly string FORMAT = "#,##0";

        public static string ToFormattedString(this int num)
        {
            return num.ToString(FORMAT);
        }

        public static int FindNumberIndexFromEnd(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }

            int i = str.Length - 1;
            for (; i >= 0; i--)
            {
                if (!char.IsNumber(str[i]))
                {
                    return i + 1;
                }
            }

            return str.Length;
        }

        public static int NumericCompareTo(this string lhs, string rhs)
        {
            if (lhs == rhs)
            {
                return 0;
            }

            if (lhs == null)
            {
                return -1;
            }

            if (rhs == null)
            {
                return 1;
            }

            int lhsNumberIndex = lhs.FindNumberIndexFromEnd();
            int rhsNumberIndex = rhs.FindNumberIndexFromEnd();

            string lhsStringPart = lhs[..lhsNumberIndex];
            string rhsStringPart = rhs[..rhsNumberIndex];

            int stringCompare = string.Compare(lhsStringPart, rhsStringPart, System.StringComparison.CurrentCulture);
            if (stringCompare != 0)
            {
                return stringCompare;
            }

            int.TryParse(lhs[lhsNumberIndex..], out int lhsNumberPart);
            int.TryParse(rhs[rhsNumberIndex..], out int rhsNumberPart);

            return lhsNumberPart.CompareTo(rhsNumberPart);
        }
    }
}
