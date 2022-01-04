using Pedigree.Core.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Extensions
{
    public class RaceEqComparer : IEqualityComparer<Race>
    {
        bool IEqualityComparer<Race>.Equals(Race x, Race y)
        {
            return x.Id == y.Id;
        }

        int IEqualityComparer<Race>.GetHashCode(Race obj)
        {
            int hCode = obj.Id;
            return hCode.GetHashCode();
        }
    }
}
