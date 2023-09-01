using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public static class StringExtensions
    {
        public static string ToFormattedString(this int num)
        {
            return num.ToString("#,##0");
        }
    }
}
