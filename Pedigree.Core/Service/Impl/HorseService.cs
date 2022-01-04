using AutoMapper;
using Pedigree.Core.Data;
using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Service.Interface;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Pedigree.Core.Data.Interface;
using Pedigree.Core.Extensions;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System.Security.Cryptography.Xml;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.ML;

namespace Pedigree.Core.Service.Impl
{
    public class HorseService : IHorseService
    {
        private readonly IHorseRepository _repo;
        private readonly IRaceRepository _raceRepo;
        private readonly IPositionRepository _posRepo;
        private readonly IRelationshipRepository _relationshipRepo;
        private readonly IMapper _mapper;
        private static MLContext mlContext = new MLContext(seed: 1);

        public HorseService(IHorseRepository repo, IRaceRepository raceRepo, IPositionRepository posRepo, IRelationshipRepository relationshipRepo,
            IMapper mapper)
        {
            _repo = repo;
            _raceRepo = raceRepo;
            _posRepo = posRepo;
            _relationshipRepo = relationshipRepo;
            _mapper = mapper;
        }

        public async Task<HorseDTO> CreateHorseAsync(HorseDTO horse)
        {
            // Assign random and unique OId
            horse.OId = ExtensionMethods.GenerateRandomOID();
            var horseId = await _repo.Create(_mapper.Map<HorseDTO, Horse>(horse));
            await _repo.CreateCoefficient(new Coefficient { HorseOId = horse.OId }); 

            // Create relationship for parents
            if (horse.FatherOId != null)
            {
                var r = new Relationship
                {
                    HorseOId = horse.OId,
                    ParentOId = horse.FatherOId,
                    ParentType = "Father"
                };
                await _relationshipRepo.Create(r);
            }
            if (horse.MotherOId != null)
            {
                var r = new Relationship
                {
                    HorseOId = horse.OId,
                    ParentOId = horse.MotherOId,
                    ParentType = "Mother"
                };
                await _relationshipRepo.Create(r);
                await PickupParentData(horse.MotherOId);
            }
            horse.Id = horseId;
            return horse;
        }

        public async Task DeleteHorseAsync(int id)
        {
            await _repo.SoftDelete(id);
        }

        public async Task<HorseDTO> GetHorseAsync(int id)
        {
            var horse = await _repo.GetById(id);
            return _mapper.Map<Horse, HorseDTO>(horse);
        }

        public async Task<bool> HasChildren(string oid)
        {
            return await _repo.HasChildren(oid);
        }

        public async Task<HorseHeirarchyDataDTO> GetHeriarchy(int id, int generationLevel = 5)
        {
            var listOfHorses = await _repo.GetHorseHeirarchyBottomUp(id, 10, null);
            return CreateHeirarchyTree(id, listOfHorses.ToList(), generationLevel); ;
        }

        public async Task<HorseHeirarchyDataDTO> GetHypotheticalHeriarchy(int maleHorseId, int femaleHorseId, int generationLevel = 5)
        {
            var fHorses = await _repo.GetHorseHeirarchyBottomUp(maleHorseId, 9, "S");
            var mHorses = await _repo.GetHorseHeirarchyBottomUp(femaleHorseId, 9, "D");

            var father = fHorses.First(h => h.Id == maleHorseId);
            var mother = mHorses.First(h => h.Id == femaleHorseId);

            var horses = new List<HorseHeirarchy>();
            var startHorseId = 1;

            var horse = new HorseHeirarchy
            {
                Id = startHorseId,
                Name = "(Unnamed)",
                Sex = "N/A",
                Age = 0,
                Country = "N/A",
                FatherId = father.Id,
                FatherOId = father.OId,
                FatherName = father.Name,
                MotherId = mother.Id,
                MotherOId = mother.OId,
                MotherName = mother.Name,
                Depth = 0,
                MtDNA = mother.MtDNA,
                MtDNAColor = mother.MtDNAColor,
                MtDNATitle = mother.MtDNATitle,
            };

            horses.Add(horse);

            foreach (var h in fHorses) { h.Depth++; horses.Add(h); }
            foreach (var h in mHorses) { h.Depth++; horses.Add(h); }


            var coefficient = await DoCalculateCOIs(new HorsePedigree(startHorseId, horses), true);

            horse.Coi = coefficient.COI;
            horse.Pedigcomp = coefficient.Pedigcomp;
            horse.Gi = coefficient.GI;

            return CreateHeirarchyTree(startHorseId, horses, generationLevel);
        }

        private HorseHeirarchyDataDTO CreateHeirarchyTree(int currentHorseId, List<HorseHeirarchy> horses, int gen)
        {
            Calculator calculator = new Calculator();

            Dictionary<int, List<string>> inbreeding = calculator.Inbreeding(horses.ToList(), gen);
            
            var mainHorse = new HorseHeirarchyDataDTO();
            int cnt = 0;
            var colorDict = new Dictionary<int, string>();

            List<HorseHeirarchyDataDTO> singleSet = new List<HorseHeirarchyDataDTO>();
            foreach (var horse in horses)
            {
                if (horse.Depth <= gen)
                {
                    var n = _mapper.Map<HorseHeirarchy, HorseHeirarchyDataDTO>(horse);
                    if (inbreeding.ContainsKey(n.Id) && (hasMoreChildren(horse.Id, horses, gen) || !isOffspringInbreeded(horse.Id, horses, inbreeding, gen)))
                    {
                        if (!colorDict.ContainsKey(n.Id)) colorDict[n.Id] = Constants.ColourValues[cnt++ % Constants.ColourValues.Length];
                        n.BgColor = colorDict[n.Id];
                    }
                    singleSet.Add(n);
                }
            }

            var topHorse = singleSet.FirstOrDefault(s => s.Id == currentHorseId);

            if (topHorse == null) return null;

            topHorse.Children = singleSet.Where(q => (q.OId == topHorse.FatherOId || q.OId == topHorse.MotherOId) && (q.Depth == topHorse.Depth + 1)).GroupBy(o => o.Id).Select(o => o.FirstOrDefault()).OrderByDescending(o => o.Sex).ToList();

            foreach (var child in topHorse.Children)
            {
                child.Children = GetTheChildren(child, singleSet);
            }


            return topHorse;
        }

        private bool isOffspringInbreeded(int horseId, List<HorseHeirarchy> hierarchy, Dictionary<int, List<string>> inbreeding, int maxGen)
        {
            var horses = hierarchy.Where(h => h.Id == horseId && h.Depth <= maxGen);

            foreach(var horse in horses)
            {
                var offspring = hierarchy.FirstOrDefault(h => h.Depth == horse.Depth - 1 && h.Id == horse.OffspringId);
                if (!inbreeding.ContainsKey(offspring.Id)) return false;
            }
            return true;
        }

        public async Task DoCalculateCOIsForAllHorses()
        {
            var ids = await _repo.GetHorseIdsForCoefficient();
            Console.WriteLine(">>>>>>>>>>>> Coefficient horses count:" + ids.Count());

            int n = 0;
            foreach(var id in ids)
            {
                try
                {
                    n++;
                    Console.WriteLine($"Sync for horse {n}: ${id}");
                    await DoCalculateCOIs(await _repo.GetPedigree(id, 10));                    
                } 
                catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        public async Task DoCalculateProbOrig()
        {
            // Remove ProbOrigs of non qualified horses
            await _repo.DeleteProbOrigs();

            var ids = await _repo.GetHorseIdsForProbOrig(100);
            Console.WriteLine(">>>>>>>>>>>> ProbOrig horses count:" + ids.Count());

            int n = 0;
            foreach (var id in ids)
            {
                try
                {
                    n++;
                    Console.WriteLine($"ProbOrig for horse {n}: ${id}");
                    await DoCalculateProbOrigForHorse(id);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        public async Task DoCalculateGRain()
        {
            // Delete non qualified horses GRain data
            await _repo.DeleteGRainData();


            var ids = await _repo.GetHorseIdsForGRain(15);
            Console.WriteLine(">>>>>>>>>>>> GRain horses count:" + ids.Count());

            foreach (var id in ids)
            {
                var coefficient = await _repo.GetCoefficientByHorseId(id);
                coefficient.GRainProcessStartedAt = DateTime.Now;
                await _repo.UpdateCoefficient(coefficient);
            }

            int n = 0;
            foreach (var id in ids)
            {
                try
                {
                    n++;
                    Console.WriteLine($"GRain for horse {n}: ${id}");
                    await DoCalculateGRainForHorse(id);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    var coefficient = await _repo.GetCoefficientByHorseId(id);
                    coefficient.GRainProcessStartedAt = null;
                    await _repo.UpdateCoefficient(coefficient);
                }
                Thread.Sleep(100);
            }
        }

        public async Task<GrainItem> DoCalculateGRainForHorse(int horseId)
        {
            HorsePedigree pedigree = await _repo.GetPedigree(horseId, 100, 1845);

            return await DoCalculateGRainByPedigree(pedigree);
        }

        public async Task<GrainItem> DoCalculateGRainByPedigree(HorsePedigree pedigree)
        {
            var horse = pedigree.GetStartHorse();
            var ancestors = (await _repo.GetAncestries()).Select(a => a.Id).ToArray();

            Dictionary<int, GrainItem> grainItems = PEDIG.Grain(pedigree, ancestors.ToArray(), 5000);
            //Console.WriteLine($"Replications={5000}, Time={DateTime.Now - beforeTime}, Bal={grainItems[horseId].Anc}, AHC={grainItems[horseId].Ancs}, Kal={grainItems[horseId].Anck}");

            var item = grainItems[horse.Id];


            var coefficient = await _repo.GetCoefficientByHorseId(horse.Id);

            if (coefficient != null)
            {
                coefficient.Bal = item.Anc;
                coefficient.AHC = item.Ancs;
                coefficient.Kal = item.Anck;

                coefficient.GRainUpdatedAt = DateTime.Now;

                await _repo.UpdateCoefficient(coefficient);
            }

            //Console.WriteLine($"Ended at {DateTime.Now}");
            return item;
        }

        public async Task DoCalculateAncestryLog()
        {
            // 1. Get all pedigs
            var pedigs = await _repo.GetPedigsForProbOrigs();

            Console.WriteLine($"Horses count for Ancestry List = {pedigs.Count()}");

            // 2. Count ancestor apperance and average marginal contribute from ProbOrigs
            var ancestorDict = new Dictionary<string, int>();
            var ancestorMCDict = new Dictionary<string, double>();
            foreach (var pedig in pedigs)
            {
                foreach (var probOrig in pedig.ProbOrigs)
                {
                    if (!ancestorDict.ContainsKey(probOrig.AncestorOId))
                    {
                        ancestorDict[probOrig.AncestorOId] = 1;
                        ancestorMCDict[probOrig.AncestorOId] = probOrig.MC;
                    }
                    else
                    {
                        ancestorDict[probOrig.AncestorOId]++;
                        ancestorMCDict[probOrig.AncestorOId] += probOrig.MC;
                    }
                }
            }

            foreach (var oid in ancestorDict.Keys) ancestorMCDict[oid] /= (float)ancestorDict[oid];

            // 3. Filter ancestors population(AncestorApperanceCount/HorsesCountInHaploGroup) is less than 2.5% and AvgMC is less than 0.005
            foreach (var oid in ancestorDict.Keys)
            {
                if (
                    ancestorMCDict[oid] < 0.01 //  We eliminate ancestors from this list where the average marginal contribution is LESS THAN 0.01.
                )
                {
                    ancestorDict.Remove(oid);
                }

            }

            // Calculate Log Odds Ratio
            if (ancestorDict.Count > 0)
            {
                var ancestries = await _repo.GetAncestries(false);

                foreach (var ancestry in ancestries)
                {
                    if (!ancestorDict.Keys.Contains(ancestry.AncestorOId)) await _repo.DeleteAncestry(ancestry);
                }

                foreach (var ancestorOId in ancestorDict.Keys)
                {
                    Ancestry ancestry = await _repo.GetAncestry(ancestorOId);
                    if (ancestry != null)
                    {
                        ancestry.AvgMC = ancestorMCDict[ancestorOId];
                        ancestry.UpdatedAt = DateTime.Now;
                        await _repo.UpdateAncestry(ancestry);
                    }
                    else
                    {
                        ancestry = new Ancestry
                        {
                            AncestorOId = ancestorOId,
                            AvgMC = ancestorMCDict[ancestorOId],
                        };
                        await _repo.CreateAncestry(ancestry);
                    }
                }
            }
        }

        private async Task DoCalculateProbOrigForHorse(int horseId)
        {
            HorsePedigree pedigree = await _repo.GetPedigree(horseId, 100, 1845);

            var items = PEDIG.ProbOrig(pedigree, 70, 1845, DateTime.Today.Year);

            var horseOId = pedigree.GetStartHorse().OId;
            
            var probOrigs = items.Select(item => new ProbOrig
            {
                AncestorOId = pedigree.GetHorse(item.Key).OId,
                MC = item.Value.Y
            }).ToList();

            await _repo.UpdateProbOrigs(horseId, probOrigs);
        }

        public async Task DoCalculateUniqueAncestorsCount()
        {
            var ids = await _repo.GetHorseIdsForUniqueAncestorsCount(300);
            Console.WriteLine(">>>>>>>>>>>> UniqueAncestors horses count:" + ids.Count());

            foreach (var id in ids)
            {
                Console.WriteLine($"{DateTime.Now}: Updating UniqueAncestorsCount for Horse: {id}");
                var coefficient = await _repo.GetCoefficientByHorseId(id);
                coefficient.UniqueAncestorsCount = await _repo.GetUniqueAncestorsCount(id);
                coefficient.UniqueAncestorsCountUpdatedAt = DateTime.Now;
                await _repo.UpdateCoefficient(coefficient);
            }
        }

        public async Task<Coefficient> DoCalculateCOIs(HorsePedigree pedigree, bool forHypo = false)
        {
            var horse = pedigree.GetStartHorse();
            var coefficient = await _repo.GetCoefficient(horse.OId);
            
            try
            {
                var calculator = new Calculator();
                List<int> fAncestors = new List<int>();
                if (horse.Father != null)
                {
                    fAncestors.Add(horse.Father.Id);
                    calculator.GetAncestors(horse.Father, 2, 2, fAncestors);
                }

                List<int> mAncestors = new List<int>();
                if (horse.Mother != null)
                {
                    mAncestors.Add(horse.Mother.Id);
                    calculator.GetAncestors(horse.Mother, 2, 2, mAncestors);
                }

                List<Par3Item> items = PEDIG.Par3(pedigree, fAncestors.ToArray(), mAncestors.ToArray());

                var item = items.FirstOrDefault(pi => pi.HorseId1 == horse.FatherId && pi.HorseId2 == horse.MotherId);
                var item1 = items.FirstOrDefault(pi => pi.HorseId1 == horse.FatherId && pi.HorseId2 == horse.Mother.FatherId);
                var item2 = items.FirstOrDefault(pi => pi.HorseId1 == horse.FatherId && pi.HorseId2 == horse.Mother.MotherId);
                var item3 = items.FirstOrDefault(pi => pi.HorseId1 == horse.Father.FatherId && pi.HorseId2 == horse.MotherId);
                var item4 = items.FirstOrDefault(pi => pi.HorseId1 == horse.Father.MotherId && pi.HorseId2 == horse.MotherId);
                var item5 = items.FirstOrDefault(pi => pi.HorseId1 == horse.Father.FatherId && pi.HorseId2 == horse.Mother.FatherId);
                var item6 = items.FirstOrDefault(pi => pi.HorseId1 == horse.Father.FatherId && pi.HorseId2 == horse.Mother.MotherId);
                var item7 = items.FirstOrDefault(pi => pi.HorseId1 == horse.Father.MotherId && pi.HorseId2 == horse.Mother.FatherId);
                var item8 = items.FirstOrDefault(pi => pi.HorseId1 == horse.Father.MotherId && pi.HorseId2 == horse.Mother.MotherId);

                bool isCreate = false;
                if (coefficient == null)
                {
                    coefficient = new Coefficient();
                    coefficient.HorseOId = horse.OId;
                    isCreate = true;
                }

                coefficient.COI = item != null ? item.Coi : 0;
                coefficient.COI1 = item1 != null ? item1.Coi : 0;
                coefficient.COI2 = item2 != null ? item2.Coi : 0;
                coefficient.COI3 = item3 != null ? item3.Coi : 0;
                coefficient.COI4 = item4 != null ? item4.Coi : 0;
                coefficient.COI5 = item5 != null ? item5.Coi : 0;
                coefficient.COI6 = item6 != null ? item6.Coi : 0;
                coefficient.COI7 = item7 != null ? item7.Coi : 0;
                coefficient.COI8 = item8 != null ? item8.Coi : 0;

                coefficient.Pedigcomp = calculator.CalculateNCG(horse, 10);

                if (!forHypo)
                {
                    coefficient.GI = calculator.CalculateGI(horse);
                    coefficient.GDGS = calculator.CalculateGDGS(horse);
                    coefficient.GDGD = calculator.CalculateGDGD(horse);
                    coefficient.GSSD = calculator.CalculateGSSD(horse);
                    coefficient.GSDD = calculator.CalculateGSDD(horse);

                    coefficient.COIUpdatedAt = DateTime.UtcNow;

                    if (isCreate)
                    {
                        await _repo.CreateCoefficient(coefficient);
                    } 
                    else
                    {
                        await _repo.UpdateCoefficient(coefficient);
                    }

                    await _repo.CalculateCOID(horse.OId);
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return coefficient;
        }
        public async Task DoUpdatePedigree()
        {
            var ids = await _repo.GetHorseIdsForPedigree(500);
            Console.WriteLine(">>>>>>>>>>>> UpdatePedigree horses count:" + ids.Count());
            Console.WriteLine($"Started at {DateTime.Now}");
            int n = 0;
            foreach (var id in ids)
            {
                try
                {
                    n++;
                    Console.WriteLine($"Update pedigree for horse {n}: ${id}");
                    await DoUpdatePedigreeForHorse(id);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
            Console.WriteLine($"Ended at {DateTime.Now}");
        }

        public async Task DoUpdatePedigreeForHorse(string horseOId)
        {
            var horse = await _repo.GetByOid(horseOId);
            if (horse != null)
            {
                await DoUpdatePedigreeForHorse(horse.Id);
            }
        }
        public async Task DoUpdatePedigreeForHorse(int horseId)
        {
            await _repo.UpdatePedigree(horseId);
        }

        private List<HorseHeirarchyDataDTO> GetTheChildren(HorseHeirarchyDataDTO child, List<HorseHeirarchyDataDTO> singleSet)
        {
            var result = singleSet.Where(q => (q.OId == child.FatherOId || q.OId == child.MotherOId) && (q.Depth == child.Depth + 1)).GroupBy(o => o.Id).Select(o => o.FirstOrDefault()).OrderByDescending(o => o.Sex).ToList();
            foreach (var item in result)
            {
                item.Children = GetTheChildren(item, singleSet).ToList();
            }
            return result;
        }

        public async Task<IEnumerable<HorseDTO>> GetHorsesByPageAsync(int page, int size)
        {
            IEnumerable<HorseDTO> horsesDtos = null;
            try
            {
                var horses = await _repo.GetPaginated(page, size);

                horsesDtos = horses.Select(h => _mapper.Map<Horse, HorseDTO>(h));
            }
            catch (Exception ex)
            {
                // TODO: Implement this logging
            }

            return horsesDtos;
        }

        public HorseListDTO GetHorsesByPageAndTotal(int page, string sort, string direction, int size)
        {
            HorseListDTO horsesDto = null;
            try
            {
                var horsesWithTotal = _repo.GetPaginatedWithTotal(page, sort, direction, size);

                var dtos = horsesWithTotal.Horses.Select(h => _mapper.Map<Horse, HorseDTO>(h));

                horsesDto = new HorseListDTO
                {
                    Horses = dtos,
                    Total = horsesWithTotal.Total
                };
            }
            catch (Exception ex)
            {
                // TODO: Implement this logging
                throw;
            }

            return horsesDto;
        }

        public async Task UpdateHorseAsync(HorseDTO horse)
        {
            var _horse = await _repo.GetById(horse.Id);
            _horse.Age = horse.Age;
            _horse.Country = horse.Country;
            _horse.UpdatedAt = DateTime.UtcNow;
            _horse.Id = horse.Id;
            _horse.Name = horse.Name;
            _horse.Sex = horse.Sex;
            _horse.Family = horse.Family;
            _horse.IsFounder = horse.IsFounder;
            _horse.MtDNA = horse.MtDNA;

            await _repo.Update(horse.Id, _horse);

            if (horse.IsFounder)
            {
                await _repo.UpdateFamilyForTailFemale(horse.Id, horse.Family);

                int mtDNA = -1;
                if (horse.MtDNA != null) mtDNA = (int)horse.MtDNA;
                await _repo.UpdateMtDNAForTailFemale(horse.Id, mtDNA);
            }

            // Update parent relationships
            if (horse.FatherOId != null)
            {
                var r = await _relationshipRepo.GetRelationshipByOIdComoboAsync(_horse.OId, "Father");
                if (r == null)
                {
                    await _relationshipRepo.Create(new Relationship
                    {
                        HorseOId = _horse.OId,
                        ParentOId = horse.FatherOId,
                        ParentType = "Father"
                    });
                } 
                else if (r.ParentOId != horse.FatherOId)
                {
                    r.ParentOId = horse.FatherOId;
                    r.UpdatedAt = DateTime.Now;
                    await _relationshipRepo.Update(r.Id, r);
                }
            }
            if (horse.MotherOId != null)
            {
                var r = await _relationshipRepo.GetRelationshipByOIdComoboAsync(_horse.OId, "Mother");
                if (r == null)
                {
                    await _relationshipRepo.Create(new Relationship
                    {
                        HorseOId = _horse.OId,
                        ParentOId = horse.MotherOId,
                        ParentType = "Mother"
                    });
                }
                else if (r.ParentOId != horse.MotherOId)
                {
                    r.ParentOId = horse.MotherOId;
                    r.UpdatedAt = DateTime.Now;
                    await _relationshipRepo.Update(r.Id, r);
                }

                await PickupParentData(horse.MotherOId);
            }
        }

        public async Task SetFounderAsync(int horseId, bool isFounder)
        {
            var horse = await _repo.GetById(horseId);
            horse.IsFounder = isFounder;
            horse.UpdatedAt = DateTime.UtcNow;

            await _repo.Update(horseId, horse);
            if (horse.Sex.Equals("Female"))
            {
                await _repo.UpdateFamilyForTailFemale(horseId, isFounder ? horse.Family : null);

                int mtDNA = -1;
                if (horse.MtDNA != null) mtDNA = (int)horse.MtDNA;

                await _repo.UpdateMtDNAForTailFemale(horseId, isFounder ? mtDNA : -1);
            }
        }

        public async Task SetMtDNAAsync(int horseId, int mtDNA)
        {
            var horse = await _repo.GetById(horseId);
            if (!horse.IsFounder) throw new Exception("Is not founder");

            if (mtDNA == -1) horse.MtDNA = null;
            else horse.MtDNA = mtDNA;

            horse.UpdatedAt = DateTime.UtcNow;

            await _repo.Update(horseId, horse);
            if (horse.Sex.Equals("Female"))
            {
                await _repo.UpdateMtDNAForTailFemale(horseId, mtDNA);
            }
        }

        public HorseListDTO SearchHorsesByPageAndTotal(string q, int page, int size)
        {
            HorseListDTO horsesDto = null;
            try
            {
                var horsesWithTotal = _repo.SearchNameStartsWithPaginatedWithTotal(q, page, size);

                var dtos = horsesWithTotal.Horses.Select(h => _mapper.Map<Horse, HorseDTO>(h));

                horsesDto = new HorseListDTO
                {
                    Horses = dtos,
                    Total = horsesWithTotal.Total
                };
            }
            catch (Exception ex)
            {
                // TODO: Implement this logging
                throw;
            }

            return horsesDto;
        }

        public HorseListDTO SearchHorsesByPageAndTotal(string q, int page, string sort, string direction, int size)
        {
            HorseListDTO horsesDto = null;
            try
            {
                var horsesWithTotal = _repo.SearchPaginatedWithTotal(q, page, sort, direction, size);

                var dtos = horsesWithTotal.Horses.Select(h => _mapper.Map<Horse, HorseDTO>(h)).ToList();
                
                horsesDto = new HorseListDTO
                {
                    Horses = dtos,
                    Total = horsesWithTotal.Total
                };
            }
            catch (Exception ex)
            {
                // TODO: Implement this logging
                throw;
            }

            return horsesDto;
        }

        public async Task<HorseListDTO> SearchHorsesByPageAndTotalAsync(string q, int page, string sort, string direction, int size)
        {
            HorseListDTO horsesDto = null;
            var finaList = new List<HorseDTO>();
            try
            {
                #region placeholders
                var matchingPlaceholder = new HorseDTO()
                {
                    Name = "",
                    ShowHeader = true,
                    ShowHeaderText = "Matching"
                };

                var exactMatchPlaceholder = new HorseDTO()
                {
                    Name = "",
                    ShowHeader = true,
                    ShowHeaderText = "Exact"
                };

                #endregion

                var horsesWithTotal = await _repo.SearchPaginatedWithTotalAsync(q, page, sort, direction, size);

                var dtos = horsesWithTotal.Horses.Select(h => _mapper.Map<Horse, HorseDTO>(h)).ToList();

                if (dtos.Count > 0)
                {
                    var ExactMatchs = dtos.Where(f => f.Name.Equals(q, StringComparison.OrdinalIgnoreCase));
                    if (ExactMatchs != null && ExactMatchs.Count() > 0)
                    {
                        // insert placeholder for exact

                        finaList.Add(exactMatchPlaceholder);
                        finaList.AddRange(ExactMatchs);
                        var remaining = dtos.Where(f => !f.Name.Equals(q, StringComparison.OrdinalIgnoreCase));
                        if (remaining != null && remaining.Count() > 0)
                        {
                            finaList.Add(matchingPlaceholder);
                            finaList.AddRange(remaining);
                        }
                    }
                    else
                    {
                        finaList.Add(matchingPlaceholder);
                        finaList.AddRange(dtos);
                    }
                }
                horsesDto = new HorseListDTO
                {
                    Horses = finaList,
                    Total = horsesWithTotal.Total
                };
            }
            catch (Exception ex)
            {
                // TODO: Implement this logging
                throw;
            }

            return horsesDto;
        }

        public async Task<HorseListDTO> SearchHorsesByGenderPageAndTotalAsync(string q, string gender, int page, string sort, string direction, int size)
        {
            #region placeholders
            var matchingPlaceholder = new HorseDTO()
            {
                Name = "",
                ShowHeader = true,
                ShowHeaderText = (gender.Equals("Male") ? "Sire" : "Dam") + " - Matching"
            };

            var exactMatchPlaceholder = new HorseDTO()
            {
                Name = "",
                ShowHeader = true,
                ShowHeaderText = (gender.Equals("Male") ? "Sire" : "Dam") + " - Exact"
            };
            #endregion

            HorseListDTO horsesDto = null;
            var finaList = new List<HorseDTO>();

            try
            {
                var horsesWithTotal = await _repo.SearchWithGenderPaginatedWithTotalAsync(q, gender, page, sort, direction, size);

                var dtos = horsesWithTotal.Horses.Select(h => _mapper.Map<Horse, HorseDTO>(h));

                var ExactMatchs = dtos.Where(f => f.Name.Equals(q, StringComparison.OrdinalIgnoreCase));
                if (ExactMatchs != null && ExactMatchs.Count() > 0)
                {
                    // insert placeholder for exact

                    finaList.Add(exactMatchPlaceholder);
                    finaList.AddRange(ExactMatchs);
                    var remaining = dtos.Where(f => !f.Name.Equals(q, StringComparison.OrdinalIgnoreCase));
                    if (remaining != null && remaining.Count() > 0)
                    {
                        finaList.Add(matchingPlaceholder);
                        finaList.AddRange(remaining);
                    }
                }
                else
                {
                    finaList.Add(matchingPlaceholder);
                    finaList.AddRange(dtos);
                }

                horsesDto = new HorseListDTO
                {
                    Horses = finaList,
                    Total = horsesWithTotal.Total
                };
            }
            catch (Exception ex)
            {
                // TODO: Implement this logging
                throw;
            }

            return horsesDto;
        }
        public async Task<HorseWithParentChildrenDTO> GetHorseWithParentChildrenById(int id)
        {
            try
            {

                var result = await _repo.GetHorseWithParentChildrenById(id);
                return DataRestAsDTO(result);
            }
            catch (Exception ex)
            {
                // TODO:
                return null;
            }
        }
        public async Task<HorseDTO> GetHorseById(int id)
        {
            return _mapper.Map<Horse, HorseDTO>(await _repo.GetHorseById(id));
        }

        public async Task<HorseWithParentChildrenDTO> GetHorseWithParentChildrenByOId(string Oid)
        {
            try
            {

                var result = await _repo.GetHorseWithParentChildrenByOId(Oid);
                return DataRestAsDTO(result);
            }
            catch (Exception ex)
            {
                // TODO:
                return null;
            }
        }

        public async Task<IEnumerable<HorseDTO>> GetParentsOrChildrenAsync(string Oid, bool needParents)
        {
            try
            {

                var result = await _repo.GetParentsOrChildren(Oid, needParents);
                var dtos = result.Select(r => _mapper.Map<Horse, HorseDTO>(r));
                return dtos;

            }
            catch (Exception ex)
            {
                // TODO:
                return null;
            }
        }

        public async Task<IEnumerable<LinebreedingItem>> GetLinebreedings(int id, int genLevel)
        {
            List<LinebreedingItem> linebreedings = new List<LinebreedingItem>();

            var hierarchy = await _repo.GetHorseHeirarchyBottomUp(id, 10, null);
            var pedigree = new HorsePedigree(id, hierarchy.ToList());

            Calculator calculator = new Calculator();

            var inbreeding = calculator.Inbreeding(hierarchy.ToList(), genLevel);

            var inbreedingIds = inbreeding.Keys.ToArray();
            List<Par3Item> items = PEDIG.Par3(pedigree, new int[] { id }, inbreedingIds);

            foreach (var p3item in items)
            {
                var hId = p3item.HorseId1 == id ? p3item.HorseId2 : p3item.HorseId1;

                var h = hierarchy.FirstOrDefault(h1 => h1.Id == hId);
                if (hasMoreChildren(h.Id, hierarchy.ToList(), genLevel) || !isOffspringInbreeded(h.Id, hierarchy.ToList(), inbreeding, genLevel))
                {
                    LinebreedingItem item = new LinebreedingItem();

                    item.Id = h.Id;
                    item.Name = h.Name;
                    item.Stats = inbreeding[hId];
                    item.Crosses = inbreeding[hId].Count;
                    item.Inbreed = Math.Round(p3item.Coi * 100, 2);
                    item.Relation = Math.Round(calculator.CalculateAGR(pedigree.GetHorse(p3item.HorseId1), pedigree.GetHorse(p3item.HorseId2)) * 100, 2);

                    linebreedings.Add(item);
                }
            }

            return linebreedings;
        }

        public async Task<IEnumerable<LinebreedingItem>> GetLinebreedingsForHypothetical(int maleHorseId, int femaleHorseId, int genLevel)
        {
            List<LinebreedingItem> linebreedings = new List<LinebreedingItem>();

            var fHierarchy = await _repo.GetHorseHeirarchyBottomUp(maleHorseId, 9, "S");
            var mHierarchy = await _repo.GetHorseHeirarchyBottomUp(femaleHorseId, 9, "D");

            var father = fHierarchy.First(h => h.Id == maleHorseId);
            var mother = mHierarchy.First(h => h.Id == femaleHorseId);

            var hierarchy = new List<HorseHeirarchy>();
            var startHorseId = 1;
            hierarchy.Add(new HorseHeirarchy {
                Id = startHorseId,
                Name = "Hypothetical",
                Sex = "N/A",
                Age = 0,
                Country = "N/A",
                FatherId = father.Id,
                FatherOId = father.OId,
                FatherName = father.Name,
                MotherId = mother.Id,
                MotherOId = mother.OId,
                MotherName = mother.Name,
                Depth = 0
            });

            foreach (var h in fHierarchy) { h.Depth++; hierarchy.Add(h); }
            foreach (var h in mHierarchy) { h.Depth++; hierarchy.Add(h); }

            var pedigree = new HorsePedigree(startHorseId, hierarchy);

            Calculator calculator = new Calculator();

            var inbreeding = calculator.Inbreeding(hierarchy, genLevel);

            var inbreedingIds = inbreeding.Keys.ToArray();
            List<Par3Item> items = PEDIG.Par3(pedigree, new int[] { startHorseId }, inbreedingIds);

            foreach (var p3item in items)
            {
                var hId = p3item.HorseId1 == startHorseId ? p3item.HorseId2 : p3item.HorseId1;

                var h = hierarchy.FirstOrDefault(h1 => h1.Id == hId);
                if (hasMoreChildren(h.Id, hierarchy, genLevel) || !isOffspringInbreeded(h.Id, hierarchy, inbreeding, genLevel))
                {
                    LinebreedingItem item = new LinebreedingItem();

                    item.Id = h.Id;
                    item.Name = h.Name;
                    item.Stats = inbreeding[hId];
                    item.Crosses = inbreeding[hId].Count;
                    item.Inbreed = Math.Round(p3item.Coi * 100, 2);
                    item.Relation = Math.Round(calculator.CalculateAGR(pedigree.GetHorse(p3item.HorseId1), pedigree.GetHorse(p3item.HorseId2)) * 100, 2);

                    linebreedings.Add(item);
                }
            }

            return linebreedings;
        }

        private bool hasMoreChildren(int horseId, List<HorseHeirarchy> hierarchy, int genLevel)
        {
            var horses = hierarchy.Where(h => h.Id == horseId && h.Depth <= genLevel);
            var offsprings = horses.GroupBy(h => h.OffspringId);

            return offsprings.Count() > 1;
        }

        public async Task<IEnumerable<Par3Item>> GetEquivalents(int horseId)
        {
            var hierarchy = await _repo.GetHorseHeirarchyBottomUp(horseId, 10, null);
            var pedigree = new HorsePedigree(horseId, hierarchy.ToList());

            // Get ancestors in 3 generation
            var calculator = new Calculator();
            var horse = pedigree.GetStartHorse();
            List<int> fAncestors = new List<int>();
            if (horse.Father != null)
            {
                fAncestors.Add(horse.Father.Id);
                calculator.GetAncestors(horse.Father, 1, 2, fAncestors);
            }

            List<int> mAncestors = new List<int>();
            if (horse.Mother != null)
            {
                mAncestors.Add(horse.Mother.Id);
                calculator.GetAncestors(horse.Mother, 1, 2, mAncestors);
            }

            List<Par3Item> items = PEDIG.Par3(pedigree, fAncestors.ToArray(), mAncestors.ToArray());

            foreach(var item in items)
            {
                var h1 = pedigree.GetHorse(item.HorseId1);
                var h2 = pedigree.GetHorse(item.HorseId2);
                item.HorseName1 = h1.Name;
                item.HorseName2 = h2.Name;

                List<string> ancestors = new List<string>();
                foreach (var aid in calculator.GetCommonAncestorsInGen(h1, h2, 4))
                {
                    ancestors.Add(pedigree.GetHorse(aid).Name);
                }
                item.CommonAncestors = ancestors;
            }

            return items;
        }
        public async Task<IEnumerable<Par3Item>> GetEquivalentsForHypothetical(int maleHorseId, int femaleHorseId)
        {
            var fHierarchy = await _repo.GetHorseHeirarchyBottomUp(maleHorseId, 9, "S");
            var mHierarchy = await _repo.GetHorseHeirarchyBottomUp(femaleHorseId, 9, "D");

            var father = fHierarchy.First(h => h.Id == maleHorseId);
            var mother = mHierarchy.First(h => h.Id == femaleHorseId);

            var hierarchy = new List<HorseHeirarchy>();
            var startHorseId = 1;
            hierarchy.Add(new HorseHeirarchy
            {
                Id = startHorseId,
                Name = "(Unnamed)",
                Sex = "N/A",
                Age = 0,
                Country = "N/A",
                FatherId = father.Id,
                FatherOId = father.OId,
                FatherName = father.Name,
                MotherId = mother.Id,
                MotherOId = mother.OId,
                MotherName = mother.Name,
                Depth = 0
            });

            foreach (var h in fHierarchy) { h.Depth++; hierarchy.Add(h); }
            foreach (var h in mHierarchy) { h.Depth++; hierarchy.Add(h); }

            var pedigree = new HorsePedigree(startHorseId, hierarchy);

            // Get ancestors in 3 generation
            var calculator = new Calculator();
            var horse = pedigree.GetStartHorse();
            List<int> fAncestors = new List<int>();
            if (horse.Father != null)
            {
                fAncestors.Add(horse.Father.Id);
                calculator.GetAncestors(horse.Father, 1, 2, fAncestors);
            }

            List<int> mAncestors = new List<int>();
            if (horse.Mother != null)
            {
                mAncestors.Add(horse.Mother.Id);
                calculator.GetAncestors(horse.Mother, 1, 2, mAncestors);
            }

            List<Par3Item> items = PEDIG.Par3(pedigree, fAncestors.ToArray(), mAncestors.ToArray());

            foreach (var item in items)
            {
                var h1 = pedigree.GetHorse(item.HorseId1);
                var h2 = pedigree.GetHorse(item.HorseId2);
                item.HorseName1 = h1.Name;
                item.HorseName2 = h2.Name;

                List<string> ancestors = new List<string>();
                foreach (var aid in calculator.GetCommonAncestorsInGen(h1, h2, 4))
                {
                    ancestors.Add(pedigree.GetHorse(aid).Name);
                }
                item.CommonAncestors = ancestors;
            }

            return items;
        }

        public async Task<HorseListDTO> GetCommonAncestors(int horseId1, int horseId2)
        {
            HorseListDTO horsesDto = null;
            try
            {
                var pedigree1 = await _repo.GetPedigree(horseId1, 10);
                var pedigree2 = await _repo.GetPedigree(horseId2, 10);

                var calculator = new Calculator();

                var commonAncestors = calculator.CommonAncestors(pedigree1.Horses, pedigree2.Horses);

                var dtos = commonAncestors.Select(h => _mapper.Map<Horse, HorseDTO>(h));

                horsesDto = new HorseListDTO
                {
                    Horses = dtos
                };
            }
            catch (Exception ex)
            {
                // TODO: Implement this logging
                throw;
            }

            return horsesDto;
        }

        public async Task<HorseListDTO> GetIncompletedPedigreeHorses(int year, string sort, string order, int page, int size)
        {
            HorseListDTO horsesDto = null;
            try
            {
                var horses = await _repo.GetIncompletedPedigreeHorses(year, sort, order, page, size);

                var dtos = horses.Horses.Select(h => _mapper.Map<Horse, HorseDTO>(h));

                horsesDto = new HorseListDTO
                {
                    Horses = dtos,
                    Total = horses.Total
                };
            }
            catch (Exception ex)
            {
                // TODO: Implement this logging
                throw;
            }

            return horsesDto;
        }

        public async Task<IEnumerable<HorseTwinDTO>> GetTwinDams()
        {
            try
            {
                var dams = await _repo.GetTwinDams();

                return dams.Select(d => _mapper.Map<HorseTwin, HorseTwinDTO>(d));
            }
            catch (Exception ex)
            {
                // TODO: Implement this logging
                throw;
            }
        }

        public async Task<HorseListDTO> GetFounders()
        {
            HorseListDTO horsesDto = null;
            try
            {
                var horses = await _repo.GetFounders();

                var dtos = horses.Horses.Select(h => _mapper.Map<Horse, HorseDTO>(h));

                horsesDto = new HorseListDTO
                {
                    Horses = dtos,
                    Total = horses.Total
                };
            }
            catch (Exception ex)
            {
                // TODO: Implement this logging
                throw;
            }

            return horsesDto;
        }

        public async Task<AncestriesDataDTO> GetAncestriesData()
        {
            try
            {
                var ancestries = (await _repo.GetAncestries()).Select(a => _mapper.Map<Ancestry, AncestryDTO>(a)).ToList();

                return new AncestriesDataDTO
                {
                    HorsesCount = await _repo.GetHorsesCountForProbOrig(),
                    AncestorsCount = ancestries.Count(),
                    GenomePercent = ancestries.Sum(a => a.AvgMC),
                    Ancestries = ancestries
                };
            }
            catch (Exception ex)
            {
                // TODO: Implement this logging
                throw;
            }
        }

        public async Task<IEnumerable<PlotItem>> GetPopulationData()
        {
            return await _repo.GetPopulationData();
        }

        public async Task<IEnumerable<PlotItem>> GetZCurrentPlotData()
        {
            return await _repo.GetZCurrentPlotData();
        }

        public async Task<IEnumerable<PlotItem>> GetZHistoricalPlotData()
        {
            return await _repo.GetZHistoricalPlotData();
        }

        public async Task<Horse> GetHorseByOId(string oid)
        {
            return await _repo.GetByOid(oid);
        }

        public async Task PickupParentData(string parentOId)
        {
            var parent = await GetHorseByOId(parentOId);
            if (parent != null)
            {
                await _repo.UpdateFamilyForTailFemale(parent.Id, parent.Family);

                int mtDNA = -1;
                if (parent.MtDNA != null) mtDNA = (int)parent.MtDNA;
                await _repo.UpdateMtDNAForTailFemale(parent.Id, mtDNA);
            }
        }

        public async Task RemoveParentData(string horseOId)
        {
            var horse = await _repo.GetByOid(horseOId);

            horse.MtDNA = null;
            horse.Family = null;

            horse.UpdatedAt = DateTime.UtcNow;

            await _repo.Update(horse.Id, horse);
        }
        public async Task SetMtDNAForFounders()
        {
            var founders = await _repo.GetCheckedFounders();
            Console.WriteLine(">>>>>>>>>>>> CheckedFounders count:" + founders.Count());

            int n = 0;
            foreach (var horse in founders)
            {
                try
                {
                    n++;
                    Console.WriteLine($"Set MtDNA for horse {n}: ${horse.Id}");
                    
                    if (horse.Sex.Equals("Female"))
                    {
                        await _repo.UpdateFamilyForTailFemale(horse.Id, horse.Family);

                        int mtDNA = -1;
                        if (horse.MtDNA != null) mtDNA = (int)horse.MtDNA;
                        await _repo.UpdateMtDNAForTailFemale(horse.Id, mtDNA);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
        public async Task CalculateCOIDs()
        {
            await _repo.CalculateCOIDs();
        }

        public async Task<IEnumerable<HorseRaceDTO>> GetRaces(int horseId, string sort, string order)
        {
            var horse = await _repo.GetById(horseId);
            var races = await _raceRepo.GetRacesForHorse(horse.OId, sort, order);

            return races.Select(x => _mapper.Map<HorseRace, HorseRaceDTO>(x));
        }

        public async Task<IEnumerable<HorseDTO>> GetHorsesForSireSearch(int maleHorseId, int femaleHorseId)
        {
            var offsprings = await _relationshipRepo.GetSWOffsprings(maleHorseId, femaleHorseId);

            return offsprings.Horses.Select(x => _mapper.Map<Horse, HorseDTO>(x));
        }

        public async Task<IEnumerable<HorseDTO>> GetHorsesForSireBroodmareSireSearch(string type, int maleHorseId)
        {
            IEnumerable<Horse> horses = null;
            if (type == "sire") horses = await _relationshipRepo.GetSWOffspringsBySire(maleHorseId);
            else if (type == "broodmare_sire") horses = await _relationshipRepo.GetSWOffspringByBroodmareSire(maleHorseId);

            return horses.Select(x => _mapper.Map<Horse, HorseDTO>(x)).ToList();
        }

        public async Task<IEnumerable<HorseDTO>> GetHorsesForSirelineSearch(string type, int maleHorseId)
        {
            IEnumerable<Horse> horses = null;
            if (type == "sire") horses = await _relationshipRepo.GetStakeWinnersBySireDescendants(maleHorseId);
            else if (type == "broodmare_sire") horses = await _relationshipRepo.GetStakeWinnersByBroodmareSireDescendants(maleHorseId);

            var result = new List<HorseDTO>();

            foreach (var horse in horses)
            {
                var horseDTO = _mapper.Map<Horse, HorseDTO>(horse);
                horseDTO.Races = new List<HorseRaceDTO>();
                result.Add(horseDTO);
            }
            return result;
        }

        public async Task<SireCrossData> GetSiresCrossData(int maleId1, int maleId2)
        {
            SireCrossData crossData = new SireCrossData();

            var sire1 = await _repo.GetById(maleId1);
            var sireOfSire1 = await _relationshipRepo.GetParent(maleId1, "Father");
            var sireOfSireOfSire1 = await _relationshipRepo.GetParent(sireOfSire1.Id, "Father");
            crossData.Sires1 = new Horse[] {
                sire1,
                sireOfSire1,
                sireOfSireOfSire1
            };

            var sire2 = await _repo.GetById(maleId2);
            var sireOfSire2 = await _relationshipRepo.GetParent(maleId2, "Father");
            var sireOfSireOfSire2 = await _relationshipRepo.GetParent(sireOfSire2.Id, "Father");
            crossData.Sires2 = new Horse[] {
                sire2,
                sireOfSire2,
                sireOfSireOfSire2
            };

            // Count sire1 and sire2
            var winnersOfSire1 = await _relationshipRepo.GetStakeWinnersBySireDescendants(sire1.Id, 3);
            var winnersOfSireOfSire1 = await _relationshipRepo.GetStakeWinnersBySireDescendants(sireOfSire1.Id, 3);
            var winnersOfSireOfSireOfSire1 = await _relationshipRepo.GetStakeWinnersBySireDescendants(sireOfSireOfSire1.Id, 3);

            var winnersOfSire2 = await _relationshipRepo.GetStakeWinnersByBroodmareSireDescendants(sire2.Id, 3);
            var winnersOfSireOfSire2 = await _relationshipRepo.GetStakeWinnersByBroodmareSireDescendants(sireOfSire2.Id, 3);
            var winnersOfSireOfSireOfSire2 = await _relationshipRepo.GetStakeWinnersByBroodmareSireDescendants(sireOfSireOfSire2.Id, 3);

            crossData.Crosses = new List<Horse>[]
            {
                winnersOfSire1.Intersect(winnersOfSire2, new CommonHorsesComparer()).ToList(),
                winnersOfSireOfSire1.Intersect(winnersOfSire2, new CommonHorsesComparer()).ToList(),
                winnersOfSireOfSireOfSire1.Intersect(winnersOfSire2, new CommonHorsesComparer()).ToList(),
                winnersOfSire1.Intersect(winnersOfSireOfSire2, new CommonHorsesComparer()).ToList(),
                winnersOfSireOfSire1.Intersect(winnersOfSireOfSire2, new CommonHorsesComparer()).ToList(),
                winnersOfSireOfSireOfSire1.Intersect(winnersOfSireOfSire2, new CommonHorsesComparer()).ToList(),
                winnersOfSire1.Intersect(winnersOfSireOfSireOfSire2, new CommonHorsesComparer()).ToList(),
                winnersOfSireOfSire1.Intersect(winnersOfSireOfSireOfSire2, new CommonHorsesComparer()).ToList(),
                winnersOfSireOfSireOfSire1.Intersect(winnersOfSireOfSireOfSire2, new CommonHorsesComparer()).ToList(),
            };

            return crossData;
        }
        public async Task<IEnumerable<HorseDTO>> GetHorsesForWildcard1Search(int horse1Id, int horse2Id)
        {
            IEnumerable<Horse> horses = await _relationshipRepo.GetStakeWinnersByWildcard1Search(horse1Id, horse2Id);
            
            return horses.Select(x => _mapper.Map<Horse, HorseDTO>(x)).ToList();
        }
        public async Task<IEnumerable<HorseDTO>> GetHorsesForWildcard2Search(int horse1Id, int? horse2Id, int horse3Id, int? horse4Id)
        {
            IEnumerable<Horse> horses = await _relationshipRepo.GetStakeWinnersByWildcard2Search(horse1Id, horse2Id, horse3Id, horse4Id);

            return horses.Select(x => _mapper.Map<Horse, HorseDTO>(x)).ToList();
        }

        public async Task<IEnumerable<HorseDTO>> GetHorsesForWildcardQueryByPosition(Dictionary<int, int> searches)
        {
            IEnumerable<Horse> horses = await _relationshipRepo.GetStakeWinnersByWildcardQueryByPosition(searches);

            return horses.Select(x => _mapper.Map<Horse, HorseDTO>(x)).ToList();
        }

        public async Task<IEnumerable<HorseHeirarchy>> GetHorsesForFamilyStakeSearch(int femaleId, int gen)
        {
            return SortByHierarchy(FilterByStakeWinners(await _relationshipRepo.GetStakeWinnersByMareDescendants(femaleId, gen)), femaleId);
        }

        private IEnumerable<HorseHeirarchy> FilterByStakeWinners(IEnumerable<HorseHeirarchy> horses)
        {
            var selected = horses.Where(x => x.Sex == "Male" || x.BestRaceClass == "G1Wnr" || x.BestRaceClass == "G2Wnr" || x.BestRaceClass == "G3Wnr" || x.BestRaceClass == "SWnr");

            var result = new List<HorseHeirarchy>();
            foreach(var horse in selected)
            {
                PushHorseRecursively(horses, horse, result);

            }
            return result;
        }

        private void PushHorseRecursively(IEnumerable<HorseHeirarchy> horses, HorseHeirarchy horse, List<HorseHeirarchy> result)
        {
            if (!result.Exists(x => x.Id == horse.Id)) result.Add(horse);

            var parents = horses.Where(x => x.Id == horse.MotherId); // We do not take males as father as requirement

            foreach(var p in parents)
            {
                PushHorseRecursively(horses, p, result);
            }
        }

        private IEnumerable<HorseHeirarchy> SortByHierarchy(IEnumerable<HorseHeirarchy> horses, int parentId)
        {
            var result = new List<HorseHeirarchy>();

            var children = horses.Where(x => x.MotherId == parentId); // Only female has children by requirement

            foreach(var child in children)
            {
                result.Add(child);
                result = result.Concat(SortByHierarchy(horses, child.Id)).ToList();
            }
            return result;
        }

        public async Task<HorseHeirarchyDataDTO> GetHorseHierarchyDataForFemaleLineSearch(int femaleId)
        {
            return CreateHierarcyForFemale(await _relationshipRepo.GetHorsesForFemaleLineSearch(femaleId), femaleId);
        }

        public async Task<IEnumerable<HorseMtDNALookupDTO>> GetHorsesForMtDNALookup(int haploGroupId)
        {
            var horses = await _relationshipRepo.GetHorsesForMtDNALookup(haploGroupId);

            foreach(var horse in horses)
            {
                horse.TotalWnrs = horse.G1Wnrs + horse.G2Wnrs + horse.G3Wnrs + horse.LRWnrs;
            }

            return horses;
        }

        private HorseHeirarchyDataDTO CreateHierarcyForFemale(IEnumerable<HorseHeirarchy> horses, int femaleId)
        {
            var startHorse = horses.FirstOrDefault(x => x.Id == femaleId);

            var hierarchyData = _mapper.Map<HorseHeirarchy, HorseHeirarchyDataDTO>(startHorse);

            hierarchyData.Children = GetChildrenForFemale(horses, femaleId);
            return hierarchyData;
        }

        private List<HorseHeirarchyDataDTO> GetChildrenForFemale(IEnumerable<HorseHeirarchy> horses, int femaleId)
        {
            var children = horses.Where(x => x.MotherId == femaleId);

            var result = new List<HorseHeirarchyDataDTO>();
            foreach (var child in children)
            {
                var hierarchyData = _mapper.Map<HorseHeirarchy, HorseHeirarchyDataDTO>(child);
                if (child.Sex == "Female")
                {
                    hierarchyData.Children = GetChildrenForFemale(horses, child.Id);
                }
                result.Add(hierarchyData);
            }

            return result;
        }

        public Horse[] SearchHorsesEx(string q, string sex)
        {
            if (q == null || q == "") return new Horse[] { };

            var result = _repo.SearchHorsesEx(q, sex);

            return result;
        }

        public async Task CalculateBPRs()
        {
            // 1. Calculate CurrentBPR

            var races = await _raceRepo.GetAllRaces();
            var positions = await _posRepo.GetAllPositions();

            var currentBPRs = new Dictionary<string, double>(); // Key: HorseOId, Value: CurrentBPR
            var zCurrentBPRs = new Dictionary<string, double>(); // Key: HorseOId, Value: ZCurrentBPR

            foreach(var race in races)
            {
                if (race.Rnrs > 0)
                {
                    double fsAdj = (double)(race.Weight * (race.Weight - (1 / race.Rnrs)));

                    var positionsInRace = positions.Where(x => x.RaceId == race.Id);
                    foreach (var pos in positionsInRace)
                    {
                        double fpAdj = ((double)(race.Rnrs - pos.Place) / race.Rnrs);
                        double score = (double)(500 * (fsAdj * fpAdj));
                        if (!currentBPRs.ContainsKey(pos.HorseOId)) currentBPRs.Add(pos.HorseOId, score);
                        else currentBPRs[pos.HorseOId] += score;
                    }
                }
            }

            foreach (var horseOId in currentBPRs.Keys.ToList())
            {
                if (currentBPRs[horseOId] > 0)
                {
                    var val = Math.Log(currentBPRs[horseOId]);
                    currentBPRs[horseOId] = val;
                }
                else
                {
                    currentBPRs[horseOId] = 0;
                }
            }

            // 2. Calculate ZCurrentBPR
            var positionsGroupByAge = positions.GroupBy(x => x.HorseAge);
            foreach (var horseAgeGroup in positionsGroupByAge)
            {
                var horseOIds = horseAgeGroup.GroupBy(x => x.HorseOId).Where(g => g.Count() > 2).Select(g => g.Key);

                double sum = 0.0;
                double mean = 0.0;
                double bigSum = 0.0;
                double stdDev = 0.0;

                foreach (var horseOId in horseOIds)
                {
                    sum += currentBPRs[horseOId];
                }
                mean = sum / horseOIds.Count();

                foreach (var horseOId in horseOIds)
                {
                    bigSum += (currentBPRs[horseOId] - mean) * (currentBPRs[horseOId] - mean);
                }
                stdDev = Math.Sqrt(bigSum / (horseOIds.Count() - 1));

                foreach (var horseOId in horseOIds)
                {
                    zCurrentBPRs[horseOId] = (currentBPRs[horseOId] - mean) / stdDev;
                }
            }

            // 3. Calculate HistoricalBPR
            var histRaces = await _raceRepo.GetHistRaces();
            var histPositions = positions.Where(x => histRaces.Any(r => r.Id == x.RaceId)).ToList();

            var histBPRs = new Dictionary<string, double>(); // Key: HorseOId, Value: HistoricalBPR
            var zHistBPRs = new Dictionary<string, double>(); // Key: HorseOId, Value: ZHistoricalBPR

            foreach (var race in histRaces)
            {
                if (race.Rnrs > 0)
                {
                    double fsAdj = race.Weight * (race.Weight - (1 / race.Rnrs));
                    var positionsInRace = histPositions.Where(x => x.RaceId == race.Id);

                    foreach (var pos in positionsInRace)
                    {
                        double fpAdj = (double)(race.Rnrs - pos.Place) / race.Rnrs;
                        double score = 500 * (fsAdj * fpAdj);
                        if (!histBPRs.ContainsKey(pos.HorseOId)) histBPRs.Add(pos.HorseOId, score);
                        else histBPRs[pos.HorseOId] += score;
                    }
                }
            }

            foreach (var horseOId in histBPRs.Keys.ToList())
            {
                if (histBPRs[horseOId] > 0)
                {
                    var val = Math.Log(histBPRs[horseOId]);
                    histBPRs[horseOId] = val;
                }
                else
                {
                    histBPRs[horseOId] = 0;
                }
            }

            // 4. Calculate ZHistoricalBPR
            var histPositionsGroupByAge = histPositions.GroupBy(x => x.HorseAge);
            foreach (var horseAgeGroup in histPositionsGroupByAge)
            {
                var horseOIds = horseAgeGroup.GroupBy(x => x.HorseOId).Where(g => g.Count() > 2).Select(g => g.Key);

                if (horseOIds.Count() < 10)
                {
                    foreach (var horseOId in horseOIds)
                    {
                        zHistBPRs[horseOId] = 0;
                    }
                } 
                else
                {
                    double sum = 0.0;
                    double mean = 0.0;
                    double bigSum = 0.0;
                    double stdDev = 0.0;

                    foreach (var horseOId in horseOIds)
                    {
                        sum += histBPRs[horseOId];
                    }
                    mean = sum / horseOIds.Count();

                    foreach (var horseOId in horseOIds)
                    {
                        bigSum += (histBPRs[horseOId] - mean) * (histBPRs[horseOId] - mean);
                    }
                    stdDev = Math.Sqrt(bigSum / (horseOIds.Count() - 1));

                    foreach (var horseOId in horseOIds)
                    {
                        zHistBPRs[horseOId] = (histBPRs[horseOId] - mean) / stdDev;
                    }
                }
            }

            // 5. Save BPRs
            int cnt = 0;
            foreach(var horseOId in currentBPRs.Keys)
            {
                double? zCurrentBPR = null;
                double? histBPR = null;
                double? zHistBPR = null;
                if (zCurrentBPRs.ContainsKey(horseOId)) zCurrentBPR = zCurrentBPRs[horseOId];
                if (histBPRs.ContainsKey(horseOId)) histBPR = histBPRs[horseOId];
                if (zHistBPRs.ContainsKey(horseOId)) zHistBPR = zHistBPRs[horseOId];
                var result = await _repo.SaveBPRs(horseOId, currentBPRs[horseOId], zCurrentBPR, histBPR, zHistBPR);
                // Console.WriteLine($">>>>>> Save BPR result for {horseOId}: {result}, time: {DateTime.Now}");

                cnt++;
                // if (cnt % 5000 == 0) break;
            }
         }

        
        #region Private Helpers

        /// <summary>
        /// Formats entity classes to DTO collection for Horse with parten children combo
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private HorseWithParentChildrenDTO DataRestAsDTO(HorseWithParentChildren result)
        {
            return new HorseWithParentChildrenDTO
            {
                MainHorse = _mapper.Map<Horse, HorseDTO>(result.MainHorse),
                Parents = result.Parents.OrderByDescending(o => o.Sex).Select(p => _mapper.Map<Horse, HorseDTO>(p)),
                Children = result.Children.Select(p => _mapper.Map<Horse, HorseDTO>(p))
            };
        }
        #endregion

    }
}
