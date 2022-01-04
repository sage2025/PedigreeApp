using AutoMapper;
using Pedigree.Core.Data;
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
    public class RelationshipService : IRelationshipService
    {
        private readonly IRelationshipRepository _repo;
        private readonly IHorseRepository _horseRepo;

        private readonly IMapper _mapper;
        public RelationshipService(IRelationshipRepository repo, IHorseRepository horseRepo,
            IMapper mapper)
        {
            _repo = repo;
            _horseRepo = horseRepo;
            _mapper = mapper;
        }

        public async Task CreateRelationshipAsync(RelationshipDTO relationship)
        {
            try
            {
                await _repo.Create(_mapper.Map<RelationshipDTO, Relationship>(relationship));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> DeleteByOidComoboAsync(string horseOid, string parentOId)
        {
            var deleted = false;
            try
            {
                deleted = await _repo.DeleteRelationshipByOIdComoboAsync(horseOid, parentOId);

            }
            catch (Exception)
            {
                throw;
            }
            return deleted;
        }

        public async Task<bool> DeleteByParentTypeAsync(string horseOId, string parentType)
        {
            var deleted = false;
            try
            {
                deleted = await _repo.DeleteRelationshipByParentTypeAsync(horseOId, parentType);
            }
            catch (Exception)
            {
                throw;
            }
            return deleted;
        }


        public async Task<RelationshipDTO> GetByOidComoboAsync(string horseOid, string parentType)
        {
            RelationshipDTO relationshipDto = null;
            try
            {
                var relationship = await _repo.GetRelationshipByOIdComoboAsync(horseOid, parentType);
                relationshipDto = _mapper.Map<Relationship, RelationshipDTO>(relationship);

            }
            catch (Exception)
            {
                throw;
            }
            return relationshipDto;
        }

        public async Task DeleteRelationshipAsync(int id)
        {
            try
            {
                await _repo.Delete(id);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task UpdatRelationshipAsync(RelationshipDTO relationship)
        {
            try
            {
                var entity = _mapper.Map<RelationshipDTO, Relationship>(relationship);
                entity.UpdatedAt = DateTime.UtcNow;
                await _repo.Update(relationship.Id, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region Horse responses
        public async Task<HorseListDTO> GetHorseOffspringsAsync(int id)
        {
            var response = new HorseListDTO();
            var data = await _repo.GetHorseOffspringsAsync(id);
            if (data.Horses.Count() > 0) await _horseRepo.AttatchParents(data.Horses);
            response.Horses = data.Horses.Select(s => _mapper.Map<Horse, HorseDTO>(s));
            response.Total = data.Total;
            return response;
        }

        public async Task<HorseListDTO> GetHorseSibilingsAsync(int id)
        {
            var response = new HorseListDTO();
            var data = await _repo.GetHorseSibilingsAsync(id);
            response.Horses = data.Horses.Select(s => _mapper.Map<Horse, HorseDTO>(s));
            response.Total = data.Total;
            return response;
        }

        public async Task<HorseListDTO> GetHorseFemaleTailAsync(int id)
        {
            var response = new HorseListDTO();
            var data = await _repo.GetHorseFemaleTailAsync(id);
            response.Horses = data.Horses.Select(s => _mapper.Map<Horse, HorseDTO>(s));
            response.Total = data.Total;
            return response;
        }
        public async Task<HorseListDTO> GetUniqueAncestorsAsync(int horseId)
        {
            var response = new HorseListDTO();
            var data = await _repo.GetUniqueAncestorsAsync(horseId);
            response.Horses = data.Horses.Skip(1).Select(s => _mapper.Map<Horse, HorseDTO>(s));
            response.Total = data.Total;
            return response;
        }
        public async Task<HorseListDTO> GetUniqueAncestorsAsync(int maleHorseId, int femaleHorseId, int gen = 9)
        {

            var response = new HorseListDTO();
            var fAncestors = await _repo.GetUniqueAncestorsAsync(maleHorseId, gen);
            var mAncestors = await _repo.GetUniqueAncestorsAsync(femaleHorseId, gen);

            var horses = new List<HorseDTO>();
            foreach (var h in fAncestors.Horses.Select(s => _mapper.Map<Horse, HorseDTO>(s))) { horses.Add(h); }
            foreach(var h in mAncestors.Horses.Select(s => _mapper.Map<Horse, HorseDTO>(s)))
            {
                if (!horses.Any(h1 => h1.Id == h.Id))
                {
                    horses.Add(h);
                }
            }

            response.Horses = horses.AsEnumerable();
            response.Total = horses.Count();
            return response;
        }

        public async Task<HorseInbreedingListDTO> GetInbreedingsAsync(string horseOId, string sort, string order, int page, int size)
        {
            var horses = await _horseRepo.GetInbreedingHorses(horseOId, sort, order, page, size);

            var inbreedings = await _repo.GetInbreedingsAsync(horseOId, horses.Horses.Select(h => h.OId).ToArray());
            var dtos = horses.Horses.Select(s => _mapper.Map<Horse, HorseDTO>(s));
            var result = new List<HorseInbreedingDTO>();
            foreach (var h in dtos)
            {
                var horse = _mapper.Map<HorseDTO, HorseInbreedingDTO>(h);
                var hInbreedings = inbreedings.Where(ib => ib.OId == horse.OId);
                horse.Inbreeding = string.Join(" X ", hInbreedings.Select(ib => $"{ib.Depth}{ib.SD}"));
                result.Add(horse);
            }
            return new HorseInbreedingListDTO
            {
                Horses = result,
                Total = horses.Total
            };
        }

        public async Task<HorseListDTO> GetGrandparents(string sort, string order, int page, int size)
        {
            var response = new HorseListDTO();
            var data = await _repo.GetGrandparents(sort, order, page, size);
            response.Horses = data.Horses.Select(s => _mapper.Map<Horse, HorseDTO>(s));
            response.Total = data.Total;
            return response;
        }

        #endregion
    }
}
