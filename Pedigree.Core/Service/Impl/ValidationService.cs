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
    public class ValidationService : IValidationService
    {
        private readonly IHorseRepository _repo;
        private readonly IRelationshipRepository _relationshipRepo;
        private readonly IPositionRepository _positionRepo;
        private readonly IRaceRepository _raceRepo;
        public ValidationService(IHorseRepository repo, IRelationshipRepository relationshipRepo, IPositionRepository positionRepo, IRaceRepository raceRepo)
        {
            _repo = repo;
            _relationshipRepo = relationshipRepo;
            _positionRepo = positionRepo;
            _raceRepo = raceRepo;
        }

        /// <summary>
        /// True if duplicate found, false if not found unless false is accompied by error
        /// </summary>
        /// <param name="requestingHorse"></param>
        /// <returns></returns>
        public async Task<HorseValidationDTO> IsDuplicateHorseAsync(HorseDTO requestingHorse)
        {
            var result = new HorseValidationDTO();
            // Call horse Repo to check with Database
            try
            {
                result.Output = await _repo.DuplicateCheck(requestingHorse.Name, requestingHorse.Age, requestingHorse.Sex, requestingHorse.Country);
                result.Message = "There is already a record for this horse. Please check database.";
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// If parent sex is passed then check is against if same sex parent exists or not, if not passed
        /// then its simply to check if enough parents are attached or not
        /// </summary>
        /// <param name="horseOid"></param>
        /// <param name="parentSex"></param>
        /// <returns></returns>
        public async Task<HorseValidationDTO> IsParentAlreadyAttachedAsync(string horseOid, string parentType)
        {
            var response = new HorseValidationDTO();
            // Get parents 
            var parentRelations = await _relationshipRepo.GetImmediateParents(horseOid);

            if (parentRelations != null && parentRelations.Count() > 0) // if any relations exists then only we do further checks
            {

                // Now let's check if two parents are there already if so we simply return from here
                if (parentRelations.Count() == 2)
                {
                    response.Output = true;
                    response.Message = "Already two parents exits";
                    return response;
                }

                // Check if the parent been added already exists or not
                var sameParent = parentRelations.FirstOrDefault(p => p.ParentType == parentType);
                if (sameParent != null)
                {
                    response.Output = true; // already exists parent with the same sex
                    response.Message = "Two parents of the same sex cannot be attached to a child.";
                }

            }
            return response;
        }


        public async Task<HorseValidationDTO> IsParentYoungerAsync(RelationshipDTO relationship)
        {
            var result = new HorseValidationDTO();

            var currentHorse = await _repo.GetByOid(relationship.HorseOId);
            var parent = await _repo.GetByOid(relationship.ParentOId);
            if (parent.Age >= currentHorse.Age) // Since the age is basically year of birth, higher number would mean younger horse
            {
                result.Output = true;
                result.IsYoungerParent = true;
                result.Message = "Children cannot be older than parents. Please find correct parent or first amend and save YOB";
            }
            return result;
        }

        public async Task<HorseValidationDTO> IsChildOlderAsync(RelationshipDTO relationship)
        {
            var result = new HorseValidationDTO();

            var currentHorse = await _repo.GetByOid(relationship.HorseOId);
            var child = await _repo.GetByOid(relationship.ParentOId);
            if (currentHorse.Age >= child.Age) // Since the age is basically year of birth, higher number would mean younger horse
            {
                result.Output = true;
                result.IsYoungerParent = true;
                result.Message = "Childrend cannot be older than parents. Please find correct child or first amend and save YOB";
            }
            return result;
        }

        public async Task<bool> HasMotherDuplicateAgeChild(string motherOId, int age)
        {
            // Get children 
            var children = await _relationshipRepo.GetChildrenHorses(motherOId);
            
            return children.Where(x => x.Age == age).FirstOrDefault() != null;
        }
        public async Task CheckAvailableHorseInRace(PositionDTO position, int positionId)
        {
            var existingPosition = await _positionRepo.GetPosition(position.HorseOId, position.RaceId);

            var horse = await _repo.GetByOid(position.HorseOId);
            var race = await _raceRepo.GetById(position.RaceId);

            if ((race.Type.Equals("2YO Fillies") || race.Type.Equals("3YO Fillies") || race.Type.Equals("3YO + F&M") || race.Type.Equals("4YO+ Mares only")) && horse.Sex.Equals("Male"))
            {
                throw new Exception("Cannot attach a male in a female only race");
            }
            if (existingPosition != null && (positionId == 0 || existingPosition.Id != positionId))
            {
                throw new Exception("Duplicated horse in this race");
            } 
        }

        public async Task CheckRaceDuplicate(RaceDTO race, int raceId)
        {
            var existingRace = await _raceRepo.GetRace(race.Name, race.Date, race.Country);


            if (existingRace != null && (raceId == 0 || existingRace.Id != raceId))
            {
                throw new Exception("Duplicated race");
            }
        }

    }
}
