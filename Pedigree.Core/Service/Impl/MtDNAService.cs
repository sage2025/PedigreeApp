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
    public class MtDNAService : IMtDNAService
    {
        private readonly IMtDNARepository _repo;
        private readonly IHorseRepository _horseRepo;
        private readonly IRelationshipService _relationhsipService;
        private readonly IRelationshipRepository _relationhsipRepo;
        private readonly IRaceRepository _raceRepo;
        private readonly IMapper _mapper;

        public MtDNAService(
            IMtDNARepository repo,
            IHorseRepository horseRepo,
            IRelationshipService relationhsipService,
            IRelationshipRepository relationhsipRepo,
            IRaceRepository raceRepo,
            IMapper mapper)
        {
            _repo = repo;
            _horseRepo = horseRepo;
            _relationhsipService = relationhsipService;
            _relationhsipRepo = relationhsipRepo;
            _raceRepo = raceRepo;
            _mapper = mapper;
        }

        async Task<int> IMtDNAService.CreateHaploTypeAsync(HaploTypeDTO haploType)
        {
            return await _repo.CreateHaploType(_mapper.Map<HaploTypeDTO, HaploType>(haploType));
        }

        async Task IMtDNAService.DeleteHaploTypeAsync(int typeId)
        {
            await _repo.DeleteHaploType(typeId);
        }

        async Task<HaploGroupDTO[]> IMtDNAService.GetHaploGroups()
        {
            var haploGroups = await _repo.GetHaploGroups();
            var horses = (await _horseRepo.GetModernHorses()).ToArray();

            return GetHaploGroupsData(haploGroups, horses);
        }

        async Task<HaploGroupDTO[]> IMtDNAService.GetHorseHaploGroups(int horseId)
        {
            var haploGroups = await _repo.GetHaploGroups();
            var horses = (await _horseRepo.GetUniqueAncestors(horseId, 100)).ToArray();

            return GetHaploGroupsData(haploGroups, horses);
        }
        async Task<HaploGroupDTO[]> IMtDNAService.GetHypotheticalHaploGroups(int maleId, int femaleId)
        {
            var haploGroups = await _repo.GetHaploGroups();
            var horses = (await _relationhsipService.GetUniqueAncestorsAsync(maleId, femaleId, 100)).Horses.ToArray();

            return GetHaploGroupsData(haploGroups, horses.Select(x => _mapper.Map<HorseDTO, Horse>(x)).ToArray());
        }

        private HaploGroupDTO[] GetHaploGroupsData(HaploGroup[] haploGroups, Horse[] horses)
        {
            var totalRefPopCount = horses.Length;
            var totalRatedHorses = horses.Where(x => x.HistoricalBPR != null).Count();
            var totalThreePlusStarts = horses.Where(x => x.ZHistoricalBPR != null).Count();

            foreach (var g in haploGroups)
            {
                var refPopCount = 0;
                var ratedHorses = 0;
                var threePlusStarts = 0;
                var elite = 0;
                var nonElite = 0;

                foreach (var t in g.Types)
                {
                    var horses1 = horses.Where(x => x.MtDNA == t.Id);
                    t.Population = horses1.Count();

                    refPopCount += t.Population;

                    ratedHorses += horses1.Where(x => x.HistoricalBPR != null).Count();
                    threePlusStarts += horses1.Where(x => x.ZHistoricalBPR != null).Count();
                    elite += horses1.Where(x => x.ZHistoricalBPR >= 1).Count();
                    nonElite += horses1.Where(x => x.ZHistoricalBPR <= -1 && x.ZHistoricalBPR >= -6).Count();
                }

                g.RefPopCount = refPopCount;
                g.RatedHorses = ratedHorses;
                g.ThreePlusStarts = threePlusStarts;
                g.Elite = elite;
                g.NonElite = nonElite;
            }

            foreach (var g in haploGroups)
            {
                foreach (var t in g.Types)
                {
                    t.PopulationPercent = 100 * t.Population / totalRefPopCount;
                }
                g.RefPopCountPercent = 100 * g.RefPopCount / totalRefPopCount;
                g.RatedHorsesPercent = g.RefPopCount > 0 ? 100 * g.RatedHorses / g.RefPopCount : 0;
                g.ThreePlusStartsPercent = g.RatedHorses > 0 ? 100 * g.ThreePlusStarts / g.RatedHorses : 0;
                g.ElitePercent = g.ThreePlusStarts > 0 ? 100 * g.Elite / g.ThreePlusStarts : 0;
                g.NonElitePercent = g.ThreePlusStarts > 0 ? 100 * g.NonElite / g.ThreePlusStarts : 0;
            }

            var haploGroupsDTO = haploGroups.Select(g => _mapper.Map<HaploGroup, HaploGroupDTO>(g)).ToList();

            // UNK group
            var unkHorses = horses.Where(x => x.MtDNA == null);

            var unkGroup = new HaploGroupDTO();
            unkGroup.Id = 0;
            unkGroup.Title = "UNK";
            unkGroup.Color = "#ccc";
            unkGroup.RefPopCount = unkHorses.Count();
            unkGroup.RefPopCountPercent = 100 * unkGroup.RefPopCount / totalRefPopCount;
            unkGroup.RatedHorses = unkHorses.Where(x => x.HistoricalBPR != null).Count();
            unkGroup.RatedHorsesPercent = unkGroup.RefPopCount > 0 ? 100 * unkGroup.RatedHorses / unkGroup.RefPopCount : 0;
            unkGroup.ThreePlusStarts = unkHorses.Where(x => x.ZHistoricalBPR != null).Count();
            unkGroup.ThreePlusStartsPercent = unkGroup.RatedHorses > 0 ? 100 * unkGroup.ThreePlusStarts / unkGroup.RatedHorses : 0;
            unkGroup.Elite = unkHorses.Where(x => x.ZHistoricalBPR >= 1).Count();
            unkGroup.ElitePercent = unkGroup.ThreePlusStarts > 0 ? 100 * unkGroup.Elite / unkGroup.ThreePlusStarts : 0;
            unkGroup.NonElite = unkHorses.Where(x => x.ZHistoricalBPR <= -1 && x.ZHistoricalBPR >= -6).Count();
            unkGroup.NonElitePercent = unkGroup.ThreePlusStarts > 0 ? 100 * unkGroup.NonElite / unkGroup.ThreePlusStarts : 0;

            haploGroupsDTO.Add(unkGroup);

            return haploGroupsDTO.ToArray();
        }

        async Task<HaploGroupDTO[]> IMtDNAService.GetSimpleHaploGroups()
        {
            var haploGroups = await _repo.GetHaploGroups();

            return haploGroups.Select(x => _mapper.Map<HaploGroup, HaploGroupDTO>(x)).ToArray();
        }

        async Task<HaploGroupStallionDTO[]> IMtDNAService.GetHaploGroupsStallion(int maleId)
        {
            var haploGroups = (await _repo.GetHaploGroups()).Select(x => _mapper.Map<HaploGroup, HaploGroupStallionDTO>(x)).ToList();
            var offsprings = (await _relationhsipRepo.GetHorseOffspringsAsync(maleId)).Horses.ToArray();

            var totalRefPopCount = offsprings.Length;
            var totalRatedHorses = offsprings.Where(x => x.CurrentBPR != null).Count();
            var totalThreePlusStarts = offsprings.Where(x => x.ZCurrentBPR != null).Count();

            foreach (var g in haploGroups)
            {
                var refPopCount = 0;
                var ratedHorses = 0;
                var threePlusStarts = 0;

                var g1Wnr = 0;
                var g2Wnr = 0;
                var g3Wnr = 0;
                var sWnr = 0;

                foreach (var t in g.Types)
                {
                    var horses = offsprings.Where(x => x.MtDNA == t.Id);
                    t.Population = horses.Count();

                    refPopCount += t.Population;

                    ratedHorses += horses.Where(x => x.CurrentBPR != null).Count();
                    threePlusStarts += horses.Where(x => x.ZCurrentBPR != null).Count();

                    g1Wnr += horses.Where(x => x.BestRaceClass == "G1Wnr").Count();
                    g2Wnr += horses.Where(x => x.BestRaceClass == "G2Wnr").Count();
                    g3Wnr += horses.Where(x => x.BestRaceClass == "G3Wnr").Count();
                    sWnr += horses.Where(x => x.BestRaceClass == "SWnr").Count();
                }

                g.RefPopCount = refPopCount;
                g.RatedHorses = ratedHorses;
                g.ThreePlusStarts = threePlusStarts;

                g.G1Wnr = g1Wnr;
                g.G2Wnr = g2Wnr;
                g.G3Wnr = g3Wnr;
                g.SWnr = sWnr;
            }

            foreach (var g in haploGroups)
            {
                foreach (var t in g.Types)
                {
                    t.PopulationPercent = 100 * t.Population / totalRefPopCount;
                }
                g.RefPopCountPercent = 100 * g.RefPopCount / totalRefPopCount;
                g.RatedHorsesPercent = g.RefPopCount > 0 ? 100 * g.RatedHorses / g.RefPopCount : 0;
                g.ThreePlusStartsPercent = g.RatedHorses > 0 ? 100 * g.ThreePlusStarts / g.RatedHorses : 0;

                g.G1WnrPercent = g.RefPopCount > 0 ? 100 * g.G1Wnr / g.RefPopCount : 0;
                g.G2WnrPercent = g.RefPopCount > 0 ? 100 * g.G2Wnr / g.RefPopCount : 0;
                g.G3WnrPercent = g.RefPopCount > 0 ? 100 * g.G3Wnr / g.RefPopCount : 0;
                g.SWnrPercent = g.RefPopCount > 0 ? 100 * g.SWnr / g.RefPopCount : 0;
            }

            // UNK group
            var unkHorses = offsprings.Where(x => x.MtDNA == null);

            var unkGroup = new HaploGroupStallionDTO();
            unkGroup.Id = 0;
            unkGroup.Title = "UNK";
            unkGroup.Color = "#ccc";
            unkGroup.RefPopCount = unkHorses.Count();
            unkGroup.RefPopCountPercent = 100 * unkGroup.RefPopCount / totalRefPopCount;
            unkGroup.RatedHorses = unkHorses.Where(x => x.CurrentBPR != null).Count();
            unkGroup.RatedHorsesPercent = unkGroup.RefPopCount > 0 ? 100 * unkGroup.RatedHorses / unkGroup.RefPopCount : 0;
            unkGroup.ThreePlusStarts = unkHorses.Where(x => x.ZCurrentBPR != null).Count();
            unkGroup.ThreePlusStartsPercent = unkGroup.RatedHorses > 0 ? 100 * unkGroup.ThreePlusStarts / unkGroup.RatedHorses : 0;

            unkGroup.G1Wnr = unkHorses.Where(x => x.BestRaceClass == "G1Wnr").Count();
            unkGroup.G1WnrPercent = unkGroup.RefPopCount > 0 ? 100 * unkGroup.G1Wnr / unkGroup.RefPopCount : 0;
            unkGroup.G2Wnr = unkHorses.Where(x => x.BestRaceClass == "G2Wnr").Count();
            unkGroup.G2WnrPercent = unkGroup.RefPopCount > 0 ? 100 * unkGroup.G2Wnr / unkGroup.RefPopCount : 0;
            unkGroup.G3Wnr = unkHorses.Where(x => x.BestRaceClass == "G3Wnr").Count();
            unkGroup.G3WnrPercent = unkGroup.RefPopCount > 0 ? 100 * unkGroup.G3Wnr / unkGroup.RefPopCount : 0;
            unkGroup.SWnr = unkHorses.Where(x => x.BestRaceClass == "SWnr").Count();
            unkGroup.SWnrPercent = unkGroup.RefPopCount > 0 ? 100 * unkGroup.SWnr / unkGroup.RefPopCount : 0;

            haploGroups.Add(unkGroup);

            return haploGroups.ToArray();
        }


        async Task<HaploGroupDistanceDTO[]> IMtDNAService.GetHaploGroupsDistance()
        {
            var haploGroups = (await _repo.GetHaploGroups()).ToList();

            var races = (await _raceRepo.GetRacesForStakeWinners()).ToArray();

            var dtos = new List<HaploGroupDistanceDTO>();
            foreach (var g in haploGroups)
            {
                var firstPlaceCount = 0;
                var sprintCount = 0;
                var sprinterMilerCount = 0;
                var intermediateCount = 0;
                var longCount = 0;
                var extendedCount = 0;

                foreach (var t in g.Types)
                {
                    var tRaces = races.Where(x => x.MtDNA == t.Id);
                    firstPlaceCount += tRaces.Count();

                    sprintCount += tRaces.Where(x => x.Distance == "Sprint").Count();
                    sprinterMilerCount += tRaces.Where(x => x.Distance == "Sprinter/Miler").Count();
                    intermediateCount += tRaces.Where(x => x.Distance == "Intermediate").Count();
                    longCount += tRaces.Where(x => x.Distance == "Long").Count();
                    extendedCount += tRaces.Where(x => x.Distance == "Extended").Count();
                }


                var dto = new HaploGroupDistanceDTO
                {
                    Id = g.Id,
                    Title = g.Title,
                    Color = g.Color,
                    FirstPlaceCount = firstPlaceCount,
                    SprintPercent = firstPlaceCount > 0 ? 100 * sprintCount / firstPlaceCount : 0,
                    SprinterMilerPercent = firstPlaceCount > 0 ? 100 * sprinterMilerCount / firstPlaceCount : 0,
                    IntermediatePercent = firstPlaceCount > 0 ? 100 * intermediateCount / firstPlaceCount : 0,
                    LongPercent = firstPlaceCount > 0 ? 100 * longCount / firstPlaceCount : 0,
                    ExtendedPercent = firstPlaceCount > 0 ? 100 * extendedCount / firstPlaceCount : 0
                };

                dtos.Add(dto);
            }

            // UNK group
            var unkRaces = races.Where(x => x.MtDNA == null);

            var unkGroup = new HaploGroupDistanceDTO();
            unkGroup.Id = 0;
            unkGroup.Title = "UNK";
            unkGroup.Color = "#ccc";
            unkGroup.FirstPlaceCount = unkRaces.Count();

            unkGroup.SprintPercent = unkGroup.FirstPlaceCount > 0 ? 100 * unkRaces.Where(x => x.Distance == "Sprint").Count() / unkGroup.FirstPlaceCount : 0;
            unkGroup.SprinterMilerPercent = unkGroup.FirstPlaceCount > 0 ? 100 * unkRaces.Where(x => x.Distance == "Sprinter/Miler").Count() / unkGroup.FirstPlaceCount : 0;
            unkGroup.IntermediatePercent = unkGroup.FirstPlaceCount > 0 ? 100 * unkRaces.Where(x => x.Distance == "Intermediate").Count() / unkGroup.FirstPlaceCount : 0;
            unkGroup.LongPercent = unkGroup.FirstPlaceCount > 0 ? 100 * unkRaces.Where(x => x.Distance == "Long").Count() / unkGroup.FirstPlaceCount : 0;
            unkGroup.ExtendedPercent = unkGroup.FirstPlaceCount > 0 ? 100 * unkRaces.Where(x => x.Distance == "Extended").Count() / unkGroup.FirstPlaceCount : 0;

            dtos.Add(unkGroup);

            return dtos.ToArray();
        }

        async Task IMtDNAService.UpdatHaploTypeAsync(HaploTypeDTO haploType)
        {
            await _repo.UpdateHaploType(_mapper.Map<HaploTypeDTO, HaploType>(haploType));
        }

        async Task IMtDNAService.UpdateHaploGroupAsync(int groupId, string color)
        {
            var group = await _repo.GetById(groupId);
            group.Color = color;

            await _repo.Update(groupId, group);
        }

        async Task<int> IMtDNAService.CreateMtDNAFlag(MtDNAFlagDTO flagDTO)
        {
            await _horseRepo.SetMtDNAFlags(flagDTO.StartHorseOId, flagDTO.EndHorseOId, true);
            return await _repo.CreateMtDNAFlag(_mapper.Map<MtDNAFlagDTO, MtDNAFlag>(flagDTO));
        }

        async Task<MtDNAFlagDTO[]> IMtDNAService.GetMtDNAFlags()
        {
            var mtDNAFlags = await _repo.GetMtDNAFlags();
            return mtDNAFlags.Select(x => _mapper.Map<MtDNAFlag, MtDNAFlagDTO>(x)).ToArray();
        }

        async Task IMtDNAService.DeleteMtDNAFlag(int flagId)
        {
            var mtDNAFlag = await _repo.GetMtDNAFlag(flagId);
            if (mtDNAFlag == null) throw new Exception("Not found entity");

            await _horseRepo.SetMtDNAFlags(mtDNAFlag.StartHorseOId, mtDNAFlag.EndHorseOId, false);
            await _repo.DeleteMtDNAFlag(flagId);
        }
    }

}
