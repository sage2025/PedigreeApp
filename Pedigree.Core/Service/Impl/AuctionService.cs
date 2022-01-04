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
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _repo;
        private readonly IHorseRepository _repoHorse;
        private readonly IRelationshipRepository _relationshipRepo;
        private readonly IMLService _mlService;
        private readonly IMapper _mapper;
        private static MLContext mlContext = new MLContext(seed: 1);
        public AuctionService(IAuctionRepository repo, IHorseRepository repoHorse, IRelationshipRepository repoRelationship, IMLService mlService)
        {
            _repo = repo;
            _repoHorse = repoHorse;
            _relationshipRepo = repoRelationship;
            _mlService = mlService;
        }
        public async Task<Auction> AddAuction(string date, string name)
        {
            string auctiontype = null;
            if(name != null)
            {
                auctiontype = "Mixed";
            } else
            {
                auctiontype = "Unnamed";
            }
            Auction data = new Auction
            {
                AuctionDate = date,
                AuctionName = name,
                AuctionType = auctiontype
            };

            await _repo.CreateAuction(data);
            return data;
        }

        public async Task<SaleSaveHorseResponseDTO> AddHorse(int auctionId, int number, string name, string type, int yob, string sex, string country, string fatherName, string motherName)
        {
            Horse father = await _repoHorse.GetHorseByName(fatherName);
            Horse mother = await _repoHorse.GetHorseByName(motherName);

            if (father == null || mother == null) return null;

            var calculator = new Calculator();

            HorsePedigree pedigree = await _repoHorse.GetHypotheticalPedigree(father.Id, mother.Id, 10);
            double pedigcomp = calculator.CalculateNCG(pedigree.GetStartHorse(), 10);

            var mlModel = await _mlService.GetLastMLModel();
            double mlScore = await _mlService.GetHypotheticalMLScore(father.Id, mother.Id, mlModel.Features.Split(','), mlModel.Id);

            var response = new SaleSaveHorseResponseDTO
            {
                Number = number,
                MtDNA = mother.MtDNA,
                MtDNATitle = mother.MtDNATitle,
                MtDNAColor = mother.MtDNAColor,
                Pedigcomp = pedigcomp,
                MLScore = mlScore,
                SireId = father.Id,
                DamId = mother.Id,
            };

            if (name != null)
            {
                Horse subjectHorse = await _repoHorse.GetHorseByName(name);
                if (subjectHorse == null)
                {
                    string OId = ExtensionMethods.GenerateRandomOID();
                    Horse horse = new Horse
                    {
                        OId = OId,
                        Name = name,
                        Age = yob,
                        Sex = sex,
                        Country = country
                    };
                    await _repoHorse.Create(horse);
                    await _repoHorse.CreateCoefficient(new Coefficient { HorseOId = horse.OId });

                    // Create relationship for parents
                    if (father.OId != null)
                    {
                        var r = new Relationship
                        {
                            HorseOId = OId,
                            ParentOId = father.OId,
                            ParentType = "Father"
                        };
                        await _relationshipRepo.Create(r);
                    }
                    if (mother.OId != null)
                    {
                        var r = new Relationship
                        {
                            HorseOId = OId,
                            ParentOId = mother.OId,
                            ParentType = "Mother"
                        };
                        await _relationshipRepo.Create(r);
                        await PickupParentData(mother.OId);
                    }
                    response.HorseId = horse.Id;
                } 
                else
                {
                    response.HorseId = subjectHorse.Id;
                }
            }
            

            return response;
        }

        public async Task<bool> AddAuctionDetails(AuctionDetailDTO[] details)
        {
            foreach(var dto in details)
            {
                var auctionDetail = new AuctionDetail
                {
                    AuctionId = dto.AuctionId,
                    LotNumber = dto.LotNumber,
                    Name = dto.Name,
                    Type = dto.Type,
                    YOB = dto.YOB,
                    Sex = dto.Sex,
                    Country = dto.Country,
                    SireId = dto.SireId,
                    DamId = dto.DamId,
                    mtDNAHapId = dto.mtDNAHapId,
                    mlScore = dto.mlScore
                };

                await _repo.CreateAuctionDetail(auctionDetail);
            }
            
            return true;
        }

        public async Task<HorsePedigree> GetPedigree(int horseId)
        {
            return await _repoHorse.GetPedigreeComp(horseId, 10);
        }

        public async Task PickupParentData(string parentOId)
        {
            var parent = await GetHorseByOId(parentOId);
            if (parent != null)
            {
                await _repoHorse.UpdateFamilyForTailFemale(parent.Id, parent.Family);

                int mtDNA = -1;
                if (parent.MtDNA != null) mtDNA = (int)parent.MtDNA;
                await _repoHorse.UpdateMtDNAForTailFemale(parent.Id, mtDNA);
            }
        }

        public async Task<Horse> GetHorseByOId(string oid)
        {
            return await _repoHorse.GetByOid(oid);
        }

        public async Task<Auction[]> GetAuctions()
        {
            return await _repo.GetAuctions();
        }

        public async Task DeleteAuction(int auctionId)
        {
            await _repo.DeleteAuction(auctionId);
        }

        public async Task DeleteAuctionDetail(int auctionDetailId)
        {
            await _repo.DeleteAuctionDetail(auctionDetailId);
        }

        public async Task<double> CheckPedigComp(int horseId)
        {
            Calculator calculator = new Calculator();
            HorsePedigree pedigree = await GetPedigree(horseId);
            var horse1 = pedigree.GetStartHorse();
            double pedigcomp = calculator.CalculateNCG(horse1, 10);
            return pedigcomp;
        }

        public async Task<AuctionDetail[]> GetAuctionDetail(int auctionId)
        {
            return await _repo.GetAuctionDetail(auctionId);
        }

        public async Task<Auction> GetAuction(int auctionId)
        {
            return await _repo.GetAuction(auctionId);
        }

        public async Task<MtDNAHap> GetMtDNAHap(string motherName)
        {
            return await _repo.GetMtDNAHap(motherName);
        }
    }
}
