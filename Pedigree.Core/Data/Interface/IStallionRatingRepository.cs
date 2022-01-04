using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Data.Interface
{
    public interface IStallionRatingRepository : IGenericRepository<StallionRating>
    {
        Task<StallionRatingsWithTotal> GetStallionRatings(string t, string q, string sort, string order, int page, int size);
        Task CalculateStallionRatings();
    }
}
