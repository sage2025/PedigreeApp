using AutoMapper;
using Pedigree.Core.Data.Interface;
using Pedigree.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Impl
{
    public class InbreedService: IInbreedService
    {
        private readonly IInbreedRepository _repo;
        private readonly IMapper _mapper;

        public InbreedService(IInbreedRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task RemoveUnnecessaryInbreeds()
        {
            await _repo.RemoveUnnecessaryInbreeds();
        }
    }
}
