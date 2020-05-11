using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision
{
    public static class Extensions
    {
        public static bool Between<T>(this T actual, T lower, T upper) where T : IComparable<T>
        {
            return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
        }

        public static bool MatchesWith(this string value, params string[] args) 
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (value.ToLower().EndsWith(args[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool In(this string value, params string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (value.Equals(args[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
