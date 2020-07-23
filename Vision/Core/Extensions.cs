using System;

namespace Vision
{
    public static class Extensions
    {
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
