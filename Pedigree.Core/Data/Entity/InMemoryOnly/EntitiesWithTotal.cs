using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
   public class EntitiesWithTotal<TEntity>
    {
       public IEnumerable<TEntity> Data { get; set; }
        public int Total { get; set; }

    }
}
