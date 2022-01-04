using Pedigree.Core.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Extensions
{
    public class HorseEqComparer : IEqualityComparer<Horse>
    {
        bool IEqualityComparer<Horse>.Equals(Horse x, Horse y)
        {
            return x.OId == y.OId;
        }

        int IEqualityComparer<Horse>.GetHashCode(Horse obj)
        {
            int hCode = obj.OId.Length;
            return hCode.GetHashCode();
        }
    }
}
