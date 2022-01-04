using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Interface
{
    public interface IMLService
    {
        Task<MLModel> TrainMLModel(TrainModelDTO data);
        Task<MLModel> RetrainMLModel(int modelId);
        Task<bool> DeployMLModel(int modelId);
        Task<double> EvaluateMLModel(EvaluateModelDTO dto);
        Task<MLModel> GetLastMLModel();

        Task<double> GetHypotheticalMLScore(int maleHorseId, int femaleHorseId, string[] features, int modelId);
    }
}
