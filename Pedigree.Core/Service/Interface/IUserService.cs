using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Interface
{
    public interface IUserService
    {
       Task<UserDTO> Authenticate(AuthenticateDTO authInfo);

       Task<UserDTO> Register(RegisterDTO registration);

       Task<UserDTO> Update(UserDTO user);

       Task<IEnumerable<UserDTO>> GetAll();
    }
}
