using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Pedigree.Core.Data;
using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Extensions;
using Pedigree.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Pedigree.Core.Data.Interface;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Pedigree.Core.Service.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings, IUserRepository repo,
            IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _repo = repo;
            _mapper = mapper;

        }
        public async Task<UserDTO> Authenticate(AuthenticateDTO authInfo)
        {
            UserDTO userDto = null;
            try
            {
                var user = await _repo.GetByEmail(authInfo.Username);
                // Check if password match
                var validated = PasswordHashHelper.ValidatePassword(authInfo.Password, user.PasswordHash);
                if (validated)
                {
                    var tokenHandler = new JwtTokenHandler();
                    user.Token = tokenHandler.GenerateToken(user, _appSettings.Secret);
                    userDto = _mapper.Map<User, UserDTO>(user.WithoutPassword());
                }
            }
            catch (Exception ex)
            {
               // TODO: Implement this logging
            }
            return userDto;
        }

        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<UserDTO> Register(RegisterDTO registration)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDTO> Update(UserDTO user)
        {
            throw new NotImplementedException();
        }
    }
}
