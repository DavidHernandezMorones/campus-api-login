using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LoginAPI.Dtos.DTOs;
using LoginAPI.Entities.Models;
using LoginAPI.Persistence.Abstractions;
using LoginAPI.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LoginAPI.Services.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepositoryWrapper _repository;

        public JwtService(IConfiguration configuration, IRepositoryWrapper repository)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<Tokens> Authenticate(UserWriteDto userWriteDto)
        {
            var users = await _repository
                .UserRepository
                .GetByCondition(user => user.Username.Equals(userWriteDto.Username));
            
            var user = users.FirstOrDefault();
            
            if (user is null || !user.Password.Equals(userWriteDto.Password))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, "admin")
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new Tokens { Token = tokenHandler.WriteToken(token) };
        }
    }
}