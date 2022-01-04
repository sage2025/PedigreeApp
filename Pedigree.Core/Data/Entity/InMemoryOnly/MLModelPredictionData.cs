using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
    public class MLModelPredictionData
    {
        [ColumnName("Score")]
        public float PredictedTarget { get; set; }
    }
}
