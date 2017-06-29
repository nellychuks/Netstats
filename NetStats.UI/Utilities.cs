using System;

namespace Netstats.UI
{
    public static class Utilities
    {
        public static double ToGigabyte(string value)
        {
            if (value == null)
                return 0;
            double ret = 0d;
            if (value.Contains("GB"))
                ret = double.Parse(value.Substring(0, value.IndexOf("GB") - 1));
            else if (value.Contains("MB"))
                ret = double.Parse(value.Substring(0, value.IndexOf("MB") - 1)) / 1024d;
            else
                throw new ArgumentException();

            return Math.Round(ret, 2);
        }
    }
}
