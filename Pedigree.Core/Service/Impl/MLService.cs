using AutoMapper;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using Pedigree.Core.Data.Interface;
using Pedigree.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Impl
{
    public class MLService : IMLService
    {
        private readonly IMLRepository _repo;
        private readonly IHorseRepository _horseRepo;
        private readonly IHorseService _horseService;
        private readonly IMapper _mapper;
        private static MLContext mlContext = new MLContext(seed: 1);
        
        private static string[] columnNames = new string[] { "MtDNAGroups", "AHC", "Bal", "Kal", "COI", "MtDNAGroup", "COI1", "COI2", "COI3", "COI4", "COI5", "COI6", "COI7", "COI8", "COID1", "COID2", "COID3", "COID4", "COID5", "COID6", "GI", "GDGS", "GDGD", "GSSD", "GSDD", "UniqueAncestorsCount", "Pedigcomp", "SireHistoricalBPR", "SireZHistoricalBPR", "SireCOI", "SireAHC", "SireBal", "SireKal", "SireHistoricalSR", "DamHistoricalBPR", "DamZHistoricalBPR", "DamCOI", "DamAHC", "DamBal", "DamKal", "GrandSireHistoricalBPR", "GrandSireZHistoricalBPR", "GrandSireCOI", "GrandSireAHC", "GrandSireBal", "GrandSireKal", "GrandSireHistoricalSR", "GrandSireSOSHistoricalSR", "BroodmareSireHistoricalBPR", "BroodmareSireZHistoricalBPR", "BroodmareSireCOI", "BroodmareSireAHC", "BroodmareSireBal", "BroodmareSireKal", "BroodmareSireHistoricalSR", "BroodmareSireBMSHistoricalSR", "BroodmareSireOfSireHistoricalBPR", "BroodmareSireOfSireZHistoricalBPR", "BroodmareSireOfSireCOI", "BroodmareSireOfSireAHC", "BroodmareSireOfSireBal", "BroodmareSireOfSireKal", "BroodmareSireOfSireHistoricalSR", "BroodmareSireOfSireBMSHistoricalSR", "GrandDamHistoricalBPR", "GrandDamZHistoricalBPR", "GrandDamCOI", "GrandDamAHC", "GrandDamBal", "GrandDamKal" };

        public MLService(
            IMLRepository repo,
            IHorseRepository horseRepo,
            IHorseService horseService,
            IMapper mapper)
        {
            _repo = repo;
            _horseRepo = horseRepo;
            _horseService = horseService;
            _mapper = mapper;
        }

        public async Task<MLModel> TrainMLModel(TrainModelDTO data)
        {
            // 1. Import or create training data
            var mlModelData = await _repo.GetMLModelData();

            IDataView trainingDataView = mlContext.Data.LoadFromEnumerable(mlModelData);


            // 2. Create experiment settings
            var experimentSettings = new RegressionExperimentSettings();
            var cts = new CancellationTokenSource();
            experimentSettings.MaxExperimentTimeInSeconds = 60;
            experimentSettings.OptimizingMetric = RegressionMetric.RootMeanSquaredError;
            experimentSettings.CacheDirectoryName= null;
            experimentSettings.CancellationToken = cts.Token;

            // 3. Create an experiment
            RegressionExperiment experiment = mlContext.Auto().CreateRegressionExperiment(experimentSettings);

            // 4. Run the experiment

            var features = data.Columns.Where(x => x.Value == true).Select(x => x.Key).ToArray();
            IEstimator<ITransformer> preFeatureizer = mlContext.Transforms.Categorical
                .OneHotEncoding(inputColumnName: "MtDNAGroups", outputColumnName: "MtDNAGroups")
                //.Append(mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "MtDNAGroups", outputColumnName: "MtDNAGroups"))
                .Append(mlContext.Transforms.Concatenate("Features", features));//mlContext.Transforms.SelectColumns(selectedColumns.ToArray());

            //IEstimator<ITransformer> preFeatureizer = mlContext.Transforms.Concatenate("Features", features);

            var split = mlContext.Data.TrainTestSplit(trainingDataView, testFraction: 0.2);

            ExperimentResult<RegressionMetrics> experimentResult = experiment
                .Execute(split.TrainSet, split.TestSet, "Target", preFeatureizer);

            RegressionMetrics metrics = experimentResult.BestRun.ValidationMetrics;
            Console.WriteLine($"R-Squared: {metrics.RSquared:0.##}");
            Console.WriteLine($"Root Mean Squared Error: {metrics.RootMeanSquaredError:0.##}");

            System.IO.Directory.CreateDirectory("mlmodels");

            var modelPath = string.Format("mlmodels/model_{0}.zip", ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
            mlContext.Model.Save(experimentResult.BestRun.Model, trainingDataView.Schema, modelPath);

            MLModel currentMLModel = new MLModel
            {
                HorsesCount = mlModelData.Count(),
                ModelName = experimentResult.BestRun.TrainerName,
                RMSError = metrics.RootMeanSquaredError,
                RSquared = metrics.RSquared,
                ModelPath = modelPath,
                Features = string.Join(", ", features)
            };

            MLModel prevModel = await _repo.GetLastMLModel();
            if (prevModel != null)
            {
                currentMLModel.ModelVersion = prevModel.ModelVersion + 1;
            }
            else
            {
                currentMLModel.ModelVersion = 1;
            }

            await _repo.CreateMLModel(currentMLModel);

            return currentMLModel;
        }

        public async Task<MLModel> RetrainMLModel(int modelId)
        {
            var mlModel = await _repo.GetById(modelId);

            return mlModel;
        }
        public async Task<bool> DeployMLModel(int modelId)
        {
            var mlModel = await _repo.GetById(modelId);

            mlModel.Deployed = true;
            await _repo.Update(modelId, mlModel);

            return true;
        }

        public async Task<double> EvaluateMLModel(EvaluateModelDTO dto)
        {
            MLModel mlModel = await _repo.GetById(dto.ModelId);

            if (mlModel == null) throw new Exception("Not found model");

            // Load Trained Model
            DataViewSchema predictionPipelineSchema;
            ITransformer predictionPipeline = mlContext.Model.Load(mlModel.ModelPath, out predictionPipelineSchema);

            // Create PredictionEngines
            PredictionEngine<MLModelData, MLModelPredictionData> predictionEngine = mlContext.Model.CreatePredictionEngine<MLModelData, MLModelPredictionData>(predictionPipeline);

            // Input Data
            MLModelData inputData = MakeInputData(dto.Data);

            // Get Prediction
            MLModelPredictionData prediction = predictionEngine.Predict(inputData);


            return prediction.PredictedTarget;
        }

        public async Task<MLModel> GetLastMLModel()
        {
            return await _repo.GetLastMLModel();
        }

        private MLModelData MakeInputData(Dictionary<string, string> data)
        {
            MLModelData modelData = new MLModelData();

            if (data.ContainsKey("MtDNAGroups")) modelData.MtDNAGroups = data["MtDNAGroups"];
            if (data.ContainsKey("AHC")) modelData.AHC = float.Parse(data["AHC"]);
            if (data.ContainsKey("Bal")) modelData.Bal = float.Parse(data["Bal"]);
            if (data.ContainsKey("Kal")) modelData.Kal = float.Parse(data["Kal"]);

            if (data.ContainsKey("COI")) modelData.COI = float.Parse(data["COI"]);
            if (data.ContainsKey("COI1")) modelData.COI1 = float.Parse(data["COI1"]);
            if (data.ContainsKey("COI2")) modelData.COI2 = float.Parse(data["COI2"]);
            if (data.ContainsKey("COI3")) modelData.COI3 = float.Parse(data["COI3"]);
            if (data.ContainsKey("COI4")) modelData.COI4 = float.Parse(data["COI4"]);
            if (data.ContainsKey("COI5")) modelData.COI5 = float.Parse(data["COI5"]);
            if (data.ContainsKey("COI6")) modelData.COI6 = float.Parse(data["COI6"]);
            if (data.ContainsKey("COI7")) modelData.COI7 = float.Parse(data["COI7"]);
            if (data.ContainsKey("COI8")) modelData.COI8 = float.Parse(data["COI8"]);

            if (data.ContainsKey("COID1")) modelData.COID1 = float.Parse(data["COID1"]);
            if (data.ContainsKey("COID2")) modelData.COID2 = float.Parse(data["COID2"]);
            if (data.ContainsKey("COID3")) modelData.COID3 = float.Parse(data["COID3"]);
            if (data.ContainsKey("COID4")) modelData.COID4 = float.Parse(data["COID4"]);
            if (data.ContainsKey("COID5")) modelData.COID5 = float.Parse(data["COID5"]);
            if (data.ContainsKey("COID6")) modelData.COID6 = float.Parse(data["COID6"]);

            if (data.ContainsKey("GI")) modelData.GI = float.Parse(data["GI"]);
            if (data.ContainsKey("GDGS")) modelData.GDGS = float.Parse(data["GDGS"]);
            if (data.ContainsKey("GDGD")) modelData.GDGD = float.Parse(data["GDGD"]);
            if (data.ContainsKey("GSSD")) modelData.GSSD = float.Parse(data["GSSD"]);
            if (data.ContainsKey("GSDD")) modelData.GSDD = float.Parse(data["GSDD"]);
            if (data.ContainsKey("UniqueAncestorsCount")) modelData.UniqueAncestorsCount = float.Parse(data["UniqueAncestorsCount"]);
            if (data.ContainsKey("Pedigcomp")) modelData.Pedigcomp = float.Parse(data["Pedigcomp"]);

            if (data.ContainsKey("SireHistoricalBPR")) modelData.SireHistoricalBPR = float.Parse(data["SireHistoricalBPR"]);
            if (data.ContainsKey("SireZHistoricalBPR")) modelData.SireZHistoricalBPR = float.Parse(data["SireZHistoricalBPR"]);
            if (data.ContainsKey("SireCOI")) modelData.SireCOI = float.Parse(data["SireCOI"]);
            if (data.ContainsKey("SireAHC")) modelData.SireAHC = float.Parse(data["SireAHC"]);
            if (data.ContainsKey("SireBal")) modelData.SireBal = float.Parse(data["SireBal"]);
            if (data.ContainsKey("SireKal")) modelData.SireKal = float.Parse(data["SireKal"]);
            if (data.ContainsKey("SireHistoricalSR")) modelData.SireHistoricalSR = float.Parse(data["SireHistoricalSR"]);

            if (data.ContainsKey("DamHistoricalBPR")) modelData.DamHistoricalBPR = float.Parse(data["DamHistoricalBPR"]);
            if (data.ContainsKey("DamZHistoricalBPR")) modelData.DamZHistoricalBPR = float.Parse(data["DamZHistoricalBPR"]);
            if (data.ContainsKey("DamCOI")) modelData.DamCOI = float.Parse(data["DamCOI"]);
            if (data.ContainsKey("DamAHC")) modelData.DamAHC = float.Parse(data["DamAHC"]);
            if (data.ContainsKey("DamBal")) modelData.DamBal = float.Parse(data["DamBal"]);
            if (data.ContainsKey("DamKal")) modelData.DamKal = float.Parse(data["DamKal"]);

            if (data.ContainsKey("GrandSireHistoricalBPR")) modelData.GrandSireHistoricalBPR = float.Parse(data["GrandSireHistoricalBPR"]);
            if (data.ContainsKey("GrandSireZHistoricalBPR")) modelData.GrandSireZHistoricalBPR = float.Parse(data["GrandSireZHistoricalBPR"]);
            if (data.ContainsKey("GrandSireCOI")) modelData.GrandSireCOI = float.Parse(data["GrandSireCOI"]);
            if (data.ContainsKey("GrandSireAHC")) modelData.GrandSireAHC = float.Parse(data["GrandSireAHC"]);
            if (data.ContainsKey("GrandSireBal")) modelData.GrandSireBal = float.Parse(data["GrandSireBal"]);
            if (data.ContainsKey("GrandSireKal")) modelData.GrandSireKal = float.Parse(data["GrandSireKal"]);
            if (data.ContainsKey("GrandSireHistoricalSR")) modelData.GrandSireHistoricalSR = float.Parse(data["GrandSireHistoricalSR"]);
            if (data.ContainsKey("GrandSireSOSHistoricalSR")) modelData.GrandSireSOSHistoricalSR = float.Parse(data["GrandSireSOSHistoricalSR"]);

            if (data.ContainsKey("BroodmareSireHistoricalBPR")) modelData.BroodmareSireHistoricalBPR = float.Parse(data["BroodmareSireHistoricalBPR"]);
            if (data.ContainsKey("BroodmareSireZHistoricalBPR")) modelData.BroodmareSireZHistoricalBPR = float.Parse(data["BroodmareSireZHistoricalBPR"]);
            if (data.ContainsKey("BroodmareSireCOI")) modelData.BroodmareSireCOI = float.Parse(data["BroodmareSireCOI"]);
            if (data.ContainsKey("BroodmareSireAHC")) modelData.BroodmareSireAHC = float.Parse(data["BroodmareSireAHC"]);
            if (data.ContainsKey("BroodmareSireBal")) modelData.BroodmareSireBal = float.Parse(data["BroodmareSireBal"]);
            if (data.ContainsKey("BroodmareSireKal")) modelData.BroodmareSireKal = float.Parse(data["BroodmareSireKal"]);
            if (data.ContainsKey("BroodmareSireHistoricalSR")) modelData.BroodmareSireHistoricalSR = float.Parse(data["BroodmareSireHistoricalSR"]);
            if (data.ContainsKey("BroodmareSireBMSHistoricalSR")) modelData.BroodmareSireBMSHistoricalSR = float.Parse(data["BroodmareSireBMSHistoricalSR"]);

            if (data.ContainsKey("BroodmareSireOfSireHistoricalBPR")) modelData.BroodmareSireOfSireHistoricalBPR = float.Parse(data["BroodmareSireOfSireHistoricalBPR"]);
            if (data.ContainsKey("BroodmareSireOfSireZHistoricalBPR")) modelData.BroodmareSireOfSireZHistoricalBPR = float.Parse(data["BroodmareSireOfSireZHistoricalBPR"]);
            if (data.ContainsKey("BroodmareSireOfSireCOI")) modelData.BroodmareSireOfSireCOI = float.Parse(data["BroodmareSireOfSireCOI"]);
            if (data.ContainsKey("BroodmareSireOfSireAHC")) modelData.BroodmareSireOfSireAHC = float.Parse(data["BroodmareSireOfSireAHC"]);
            if (data.ContainsKey("BroodmareSireOfSireBal")) modelData.BroodmareSireOfSireBal = float.Parse(data["BroodmareSireOfSireBal"]);
            if (data.ContainsKey("BroodmareSireOfSireKal")) modelData.BroodmareSireOfSireKal = float.Parse(data["BroodmareSireOfSireKal"]);
            if (data.ContainsKey("BroodmareSireOfSireHistoricalSR")) modelData.BroodmareSireOfSireHistoricalSR = float.Parse(data["BroodmareSireOfSireHistoricalSR"]);
            if (data.ContainsKey("BroodmareSireOfSireBMSHistoricalSR")) modelData.BroodmareSireOfSireBMSHistoricalSR = float.Parse(data["BroodmareSireOfSireBMSHistoricalSR"]);

            if (data.ContainsKey("GrandDamHistoricalBPR")) modelData.GrandDamHistoricalBPR = float.Parse(data["GrandDamHistoricalBPR"]);
            if (data.ContainsKey("GrandDamZHistoricalBPR")) modelData.GrandDamZHistoricalBPR = float.Parse(data["GrandDamZHistoricalBPR"]);
            if (data.ContainsKey("GrandDamCOI")) modelData.GrandDamCOI = float.Parse(data["GrandDamCOI"]);
            if (data.ContainsKey("GrandDamAHC")) modelData.GrandDamAHC = float.Parse(data["GrandDamAHC"]);
            if (data.ContainsKey("GrandDamBal")) modelData.GrandDamBal = float.Parse(data["GrandDamBal"]);
            if (data.ContainsKey("GrandDamKal")) modelData.GrandDamKal = float.Parse(data["GrandDamKal"]);

            return modelData;
        }

        public async Task<double> GetHypotheticalMLScore(int maleHorseId, int femaleHorseId, string[] features, int modelId)
        {
            // Fields used - MtDNAGroups, AHC, Bal, Kal, COI, UniqueAncestorsCount, SireZHistoricalBPR, GrandSireZHistoricalBPR

            var result = new Dictionary<string, string>();
            var pedigree1 = await _horseRepo.GetHypotheticalPedigree(maleHorseId, femaleHorseId, 10);
            var pedigree2 = await _horseRepo.GetHypotheticalPedigree(maleHorseId, femaleHorseId, 100, 1845);

            var coefficient = await _horseService.DoCalculateCOIs(pedigree1, true);
            var grainitem = await _horseService.DoCalculateGRainByPedigree(pedigree2);

            if (Array.IndexOf(features, "COI") > -1) result["COI"] = $"{coefficient.COI}";
            if (Array.IndexOf(features, "AHC") > -1) result["AHC"] = $"{grainitem.Ancs}";
            if (Array.IndexOf(features, "Bal") > -1) result["Bal"] = $"{grainitem.Anc}";
            if (Array.IndexOf(features, "Kal") > -1) result["Kal"] = $"{grainitem.Anck}";
            if (Array.IndexOf(features, "UniqueAncestorsCount") > -1) result["UniqueAncestorsCount"] = $"{pedigree1.Count - 1}";

            var horse = pedigree1.GetStartHorse();
            if (Array.IndexOf(features, "SireZHistoricalBPR") > -1) result["SireZHistoricalBPR"] = $"{horse.Father?.ZHistoricalBPR ?? 0}";
            if (Array.IndexOf(features, "GrandSireZHistoricalBPR") > -1) result["GrandSireZHistoricalBPR"] = $"{horse.Father?.Father?.ZHistoricalBPR ?? 0}";

            // CONCAT(ISNULL(tfff.Name, 'UNK'), ISNULL(tffm.Name, 'UNK'), ISNULL(tfmf.Name, 'UNK'), ISNULL(tfmm.Name, 'UNK'), ISNULL(tmff.Name, 'UNK'), ISNULL(tmfm.Name, 'UNK'), ISNULL(tmmf.Name, 'UNK'), ISNULL(tmmm.Name, 'UNK')) MtDNAGroups

            if (Array.IndexOf(features, "MtDNAGroups") > -1) result["MtDNAGroups"] = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", horse.Father?.Father?.Father?.MtDNATitle ?? "UNK", horse.Father?.Father?.Mother?.MtDNATitle ?? "UNK", horse.Father?.Mother?.Father?.MtDNATitle ?? "UNK", horse.Father?.Mother?.Mother?.MtDNATitle ?? "UNK", horse.Mother?.Father?.Father?.MtDNATitle ?? "UNK", horse.Mother?.Father?.Mother?.MtDNATitle ?? "UNK", horse.Mother?.Mother?.Father?.MtDNATitle ?? "UNK", horse.Mother?.Mother?.Mother?.MtDNATitle ?? "UNK");

            EvaluateModelDTO model = new EvaluateModelDTO
            {
                ModelId = modelId,
                Data = result
            };

            double mlScore = await EvaluateMLModel(model);

            return mlScore;
        }
    }

}
