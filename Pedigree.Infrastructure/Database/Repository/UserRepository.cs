using Microsoft.EntityFrameworkCore;
using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Infrastructure.Database.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context)
          : base(context)
        {

        }

        public Task<User> GetByEmail(string email)
        {
            throw new NotImplementedException();
        }
        
    }
}
