using Pedigree.Core.Data.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Interface
{
    public interface IStallionRatingService
    {
        Task<StallionRatingListDTO> GetStallionRatings(string t/*Data Type*/, string q/*Query*/, string sort, string order, int page, int size = 100);
        Task CalculateStallionRatings();
    }
}
