using AutoMapper;
using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Interface;
using Pedigree.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Impl
{
    public class StallionRatingService : IStallionRatingService
    {
        private readonly IStallionRatingRepository _repo;
        private readonly IRaceRepository _raceRepo;
        private readonly IMapper _mapper;

        public StallionRatingService(
            IStallionRatingRepository repo,
            IRaceRepository raceRepo,
            IMapper mapper)
        {
            _repo = repo;
            _raceRepo = raceRepo;
            _mapper = mapper;
        }

        public async Task<StallionRatingListDTO> GetStallionRatings(string t, string q, string sort, string order, int page, int size = 100)
        {
            var ratings = await _repo.GetStallionRatings(t, q, sort, order, page, size);

            return new StallionRatingListDTO
            {
                Data = ratings.StallionRatings.Select(r => {
                    var dto = new StallionRatingDTO
                    {
                        HorseOId = r.HorseOId,
                        HorseName = r.HorseName,
                        CropAge = r.CropAge
                    };

                    if (t.Equals("current-sr"))
                    {
                        dto.RCount = r.CurrentRCount;
                        dto.ZCount = r.CurrentZCount;
                        dto.Rating = r.CurrentStallionRating;

                        dto.IV = r.CurrentIV;
                        dto.AE = r.CurrentAE;
                        dto.PRB2 = r.CurrentPRB2;
                    } 
                    else if (t.Equals("historical-sr"))
                    {
                        dto.RCount = r.HistoricalRCount;
                        dto.ZCount = r.HistoricalZCount;
                        dto.Rating = r.HistoricalStallionRating;

                        dto.IV = r.HistoricalIV;
                        dto.AE = r.HistoricalAE;
                        dto.PRB2 = r.HistoricalPRB2;
                    }
                    else if (t.Equals("current-bms-sr"))
                    {
                        dto.RCount = r.BMSCurrentRCount;
                        dto.ZCount = r.BMSCurrentZCount;
                        dto.Rating = r.BMSCurrentStallionRating;
                    }
                    else if (t.Equals("historical-bms-sr"))
                    {
                        dto.RCount = r.BMSHistoricalRCount;
                        dto.ZCount = r.BMSHistoricalZCount;
                        dto.Rating = r.BMSHistoricalStallionRating;
                    }
                    else if (t.Equals("current-sos-sr"))
                    {
                        dto.SCount = r.SOSCurrentSCount;
                        dto.RCount = r.SOSCurrentRCount;
                        dto.ZCount = r.SOSCurrentZCount;
                        dto.Rating = r.SOSCurrentStallionRating;
                    }
                    else if (t.Equals("historical-sos-sr"))
                    {
                        dto.SCount = r.SOSHistoricalSCount;
                        dto.RCount = r.SOSHistoricalRCount;
                        dto.ZCount = r.SOSHistoricalZCount;
                        dto.Rating = r.SOSHistoricalStallionRating;
                    }
                    else if (t.Equals("current-bmsos-sr"))
                    {
                        dto.SCount = r.BMSOSCurrentSCount;
                        dto.RCount = r.BMSOSCurrentRCount;
                        dto.ZCount = r.BMSOSCurrentZCount;
                        dto.Rating = r.BMSOSCurrentStallionRating;
                    }
                    else if (t.Equals("historical-bmsos-sr"))
                    {
                        dto.SCount = r.BMSOSHistoricalSCount;
                        dto.RCount = r.BMSOSHistoricalRCount;
                        dto.ZCount = r.BMSOSHistoricalZCount;
                        dto.Rating = r.BMSOSHistoricalStallionRating;
                    }
                    return dto;
                }),
                Total = ratings.Total
            };
        }

        public async Task CalculateStallionRatings()
        {
            await _repo.CalculateStallionRatings();
        }
    }
}
