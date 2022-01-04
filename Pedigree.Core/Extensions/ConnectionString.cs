using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Extensions
{
    public sealed class ConnectionString
    {
        public ConnectionString(string value) => Value = value;

        public string Value { get; }
    }


}
