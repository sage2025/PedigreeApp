using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class StallionRating : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        public string HorseOId { get; set; }
        public int? CurrentRCount { get; set; } = null;
        public int? CurrentZCount { get; set; } = null;
        public double? CurrentStallionRating { get; set; } = null;
        public int? HistoricalRCount { get; set; } = null;
        public int? HistoricalZCount { get; set; } = null;
        public double? HistoricalStallionRating { get; set; } = null;
        public int? BMSCurrentRCount { get; set; } = null;
        public int? BMSCurrentZCount { get; set; } = null;
        public double? BMSCurrentStallionRating { get; set; } = null;
        public int? BMSHistoricalRCount { get; set; } = null;
        public int? BMSHistoricalZCount { get; set; } = null;
        public double? BMSHistoricalStallionRating { get; set; } = null;
        public int? SOSCurrentRCount { get; set; } = null;
        public int? SOSCurrentSCount { get; set; } = null;
        public int? SOSCurrentZCount { get; set; } = null;
        public double? SOSCurrentStallionRating { get; set; } = null;
        public int? SOSHistoricalRCount { get; set; } = null;
        public int? SOSHistoricalSCount { get; set; } = null;
        public int? SOSHistoricalZCount { get; set; } = null;
        public double? SOSHistoricalStallionRating { get; set; } = null;
        public int? BMSOSCurrentRCount { get; set; } = null;
        public int? BMSOSCurrentSCount { get; set; } = null;
        public int? BMSOSCurrentZCount { get; set; } = null;
        public double? BMSOSCurrentStallionRating { get; set; } = null;
        public int? BMSOSHistoricalRCount { get; set; } = null;
        public int? BMSOSHistoricalSCount { get; set; } = null;
        public int? BMSOSHistoricalZCount { get; set; } = null;
        public double? BMSOSHistoricalStallionRating { get; set; } = null;
        public int? CropAge { get; set; } = null;
        public double? CurrentIV { get; set; } = null;   
        public double? CurrentAE { get; set; } = null;   
        public double? CurrentPRB2 { get; set; } = null;
        public double? HistoricalIV { get; set; } = null;
        public double? HistoricalAE { get; set; } = null;
        public double? HistoricalPRB2 { get; set; } = null;

        [NotMapped]
        public string HorseName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
