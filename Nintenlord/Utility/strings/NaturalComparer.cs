using System;
using System.Collections.Generic;

namespace Nintenlord.Utility.Strings
{
    public class NaturalComparer : IComparer<string>
    {


        #region IComparer<string> Members

        public int Compare(string? x, string? y)
        {
            if (x is null) return y is null ? 0 : -1;
            if (y is null) return 1;
            var length = Math.Min(x.Length, y.Length);

            for (var i = 0; i < length; i++)
            {
                if (x[i] != y[i])
                {
                    if (x[i] == '_')
                        return 1;
                    else if (y[i] == '_')
                        return -1;
                    else
                        return x[i].CompareTo(y[i]);
                }
            }
            return x.Length - y.Length;
        }

        #endregion
    }
}
