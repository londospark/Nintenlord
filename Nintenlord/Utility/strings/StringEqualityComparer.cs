using System;
using System.Collections.Generic;
using System.Linq;

namespace Nintenlord.Utility.Strings
{
    class StringEqualityComparer : IEqualityComparer<string>
    {
        #region IEqualityComparer<string> Members

        public bool Equals(string? x, string? y)
        {
            if (x is null) return y is null;
            if (y is null) return false;
            if (x.Length != y.Length)
            {
                return false;
            }
            return !x.Where((t, i) => t != y[i]).Any();
        }

        public int GetHashCode(string obj)
        {
            if (obj is null) return 0;
            const int max = 16;
            var result = 0;
            var min = Math.Min(max, obj.Length);
            for (var i = 0; i < min; i++)
            {
                result |= obj[i].GetHashCode() >> ((16 / max) * i);
            }
            return result;
        }

        #endregion
    }
}
