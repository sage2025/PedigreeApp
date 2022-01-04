using Pedigree.Core.Data.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Interface
{
    public interface IValidationService
    {
        Task<HorseValidationDTO> IsDuplicateHorseAsync(HorseDTO requestingHorse);

        /// <summary>
        /// Threshold is 25 years older
        /// </summary>
        /// <param name="relationship"></param>
        /// <returns></returns>
        Task<HorseValidationDTO> IsParentYoungerAsync(RelationshipDTO relationship);

        Task<HorseValidationDTO> IsChildOlderAsync(RelationshipDTO relationship);
        
        Task<HorseValidationDTO> IsParentAlreadyAttachedAsync(string horseOid, string parentType);
        Task<bool> HasMotherDuplicateAgeChild(string motherOId, int age);

        Task CheckAvailableHorseInRace(PositionDTO position, int positionId = 0);
        Task CheckRaceDuplicate(RaceDTO race, int raceId = 0);
    }
}
