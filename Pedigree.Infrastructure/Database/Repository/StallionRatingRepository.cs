using Dapper;
using Microsoft.EntityFrameworkCore;
using Pedigree.Core.Data;
using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using Pedigree.Core.Data.Interface;
using Pedigree.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Infrastructure.Database.Repository
{
    public class StallionRatingRepository : GenericRepository<StallionRating>, IStallionRatingRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ConnectionString _connectionString;
        public StallionRatingRepository(ApplicationDbContext context, ConnectionString connectionString)
          : base(context)
        {
            _dbContext = context;
            _connectionString = connectionString;

        }

        public async Task<StallionRatingsWithTotal> GetStallionRatings(string t, string q, string sort, string order, int page, int size)
        {
            string andWhere = "";
            if (sort.Equals("name")) sort = "HorseName";
            if (sort.Equals("age")) sort = "CropAge";

            if (t.Equals("current-sr"))
            {
                if (sort.Equals("rCount")) sort = "CurrentRCount";
                else if (sort.Equals("zCount")) sort = "CurrentZCount";
                else if (sort.Equals("rating")) sort = "CurrentStallionRating";
                else if (sort.Equals("iv")) sort = "CurrentIV";
                else if (sort.Equals("ae")) sort = "CurrentAE";
                else if (sort.Equals("prb2")) sort = "CurrentPRB2";

                andWhere = "CurrentRCount >= 3 AND CurrentZCount >= 3 AND CurrentStallionRating > 0";
            }
            else if (t.Equals("historical-sr"))
            {
                if (sort.Equals("rCount")) sort = "HistoricalRCount";
                else if (sort.Equals("zCount")) sort = "HistoricalZCount";
                else if (sort.Equals("rating")) sort = "HistoricalStallionRating";
                else if (sort.Equals("iv")) sort = "HistoricalIV";
                else if (sort.Equals("ae")) sort = "HistoricalAE";
                else if (sort.Equals("prb2")) sort = "HistoricalPRB2";

                andWhere = "HistoricalRCount >= 3 AND HistoricalZCount >= 3 AND HistoricalStallionRating > 0";
            }
            else if (t.Equals("current-bms-sr"))
            {
                if (sort.Equals("rCount")) sort = "BMSCurrentRCount";
                else if (sort.Equals("zCount")) sort = "BMSCurrentZCount";
                else if (sort.Equals("rating")) sort = "BMSCurrentStallionRating";

                andWhere = "BMSCurrentRCount >= 3 AND BMSCurrentZCount >= 3 AND BMSCurrentStallionRating > 0";
            }
            else if (t.Equals("historical-bms-sr"))
            {
                if (sort.Equals("rCount")) sort = "BMSHistoricalRCount";
                else if (sort.Equals("zCount")) sort = "BMSHistoricalZCount";
                else if (sort.Equals("rating")) sort = "BMSHistoricalStallionRating";

                andWhere = "BMSHistoricalRCount >= 3 AND BMSHistoricalZCount >= 3 AND BMSHistoricalStallionRating > 0";
            }
            else if (t.Equals("current-sos-sr"))
            {
                if (sort.Equals("rCount")) sort = "SOSCurrentRCount";
                else if (sort.Equals("sCount")) sort = "SOSCurrentSCount";
                else if (sort.Equals("zCount")) sort = "SOSCurrentZCount";
                else if (sort.Equals("rating")) sort = "SOSCurrentStallionRating";

                andWhere = "SOSCurrentRCount >= 3 AND SOSCurrentZCount >= 3 AND SOSCurrentStallionRating > 0";
            }
            else if (t.Equals("historical-sos-sr"))
            {
                if (sort.Equals("rCount")) sort = "SOSHistoricalRCount";
                else if (sort.Equals("sCount")) sort = "SOSHistoricalSCount";
                else if (sort.Equals("zCount")) sort = "SOSHistoricalZCount";
                else if (sort.Equals("rating")) sort = "SOSHistoricalStallionRating";

                andWhere = "SOSHistoricalRCount >= 3 AND SOSHistoricalZCount >= 3 AND SOSHistoricalStallionRating > 0";
            }
            else if (t.Equals("current-bmsos-sr"))
            {
                if (sort.Equals("rCount")) sort = "BMSOSCurrentRCount";
                else if (sort.Equals("sCount")) sort = "BMSOSCurrentSCount";
                else if (sort.Equals("zCount")) sort = "BMSOSCurrentZCount";
                else if (sort.Equals("rating")) sort = "BMSOSCurrentStallionRating";

                andWhere = "BMSOSCurrentRCount >= 3 AND BMSOSCurrentZCount >= 3 AND BMSOSCurrentStallionRating > 0";
            }
            else if (t.Equals("historical-bmsos-sr"))
            {
                if (sort.Equals("rCount")) sort = "BMSOSHistoricalRCount";
                else if (sort.Equals("sCount")) sort = "BMSOSHistoricalSCount";
                else if (sort.Equals("zCount")) sort = "BMSOSHistoricalZCount";
                else if (sort.Equals("rating")) sort = "BMSOSHistoricalStallionRating";

                andWhere = "BMSOSHistoricalRCount >= 3 AND BMSOSHistoricalZCount >= 3 AND BMSOSHistoricalStallionRating > 0";
            }

            q = q == null ? "" : q;
            IEnumerable<StallionRating> ratings;
            var rowCount = 0;
            var sorting = string.Format("{0} {1}", sort, order);
            var query = string.Format(Queries.Get_Stallion_Ratings_Paged_Query, q, andWhere, sorting);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                using (var multi = await conn.QueryMultipleAsync(query, new { PageNumber = page, PageSize = size }))
                {
                    ratings = multi.Read<StallionRating>();
                    rowCount = multi.Read<int>().FirstOrDefault();
                }
            }

            return new StallionRatingsWithTotal
            {
                StallionRatings = ratings,
                Total = rowCount
            };
        }

        public async Task CalculateStallionRatings()
        {
            var query = Queries.Calculate_Stallion_Rating;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                await conn.ExecuteAsync(query, null, null, 600);
            }
        }
    }
}
