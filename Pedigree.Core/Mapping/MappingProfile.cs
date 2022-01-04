using AutoMapper;
using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<HorseDTO, Horse>().ReverseMap();
            CreateMap<HorseDTO, HorseInbreedingDTO>().ReverseMap();
            CreateMap<HorseHeirarchyDataDTO, HorseHeirarchy>().ReverseMap();
            CreateMap<HorseHeirarchySingleRecord, HorseHeirarchy>().ReverseMap();
            CreateMap<HorseTwinDTO, HorseTwin>().ReverseMap();
            CreateMap<RelationshipDTO, Relationship>().ReverseMap();
            CreateMap<UserDTO, User>().ReverseMap();
            CreateMap<RaceDTO, Race>().ReverseMap();
            CreateMap<WeightDTO, Weight>().ReverseMap();
            CreateMap<PositionDTO, Position>().ReverseMap();
            CreateMap<HaploGroupDTO, HaploGroup>().ReverseMap();
            CreateMap<HaploGroupStallionDTO, HaploGroup>().ReverseMap();
            CreateMap<HaploTypeDTO, HaploType>().ReverseMap();
            CreateMap<HorseRaceDTO, HorseRace>().ReverseMap();
            CreateMap<AncestryDTO, Ancestry>().ReverseMap();
            CreateMap<MtDNAFlagDTO, MtDNAFlag>().ReverseMap();

            CreateMap<AuctionDTO, Auction>().ReverseMap();
            CreateMap<AuctionDetailDTO, AuctionDetail>().ReverseMap();
        }
    }
}
