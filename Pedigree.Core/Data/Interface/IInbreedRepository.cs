using Pedigree.Core.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Data.Interface
{
    public interface IInbreedRepository : IGenericRepository<Inbreed>
    {
        Task RemoveUnnecessaryInbreeds();
    }
}
