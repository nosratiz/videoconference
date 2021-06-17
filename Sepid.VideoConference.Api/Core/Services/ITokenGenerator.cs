using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sepid.VideoConference.Api.Core.Dto;
using Sepid.VideoConference.Api.Core.Options;

namespace Sepid.VideoConference.Api.Core.Services
{
    public interface ITokenGenerator
    {
        Task<LoginDto> Generate(string name, CancellationToken cancellationToken);
    }

    public class TokenGenerator : ITokenGenerator
    {
        private readonly JwtSetting _jwtSettings;

        public TokenGenerator(IOptions<JwtSetting> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public Task<LoginDto> Generate(string name, CancellationToken cancellationToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var userId = new Random().Next(1, 10000).ToString();

            var claims = new List<Claim>
            {
                new("fullName", $"{name}"),
                new("Id", userId),
                new(ClaimTypes.Name, $"{name}")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(10000),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _jwtSettings.ValidAudience,
                Issuer = _jwtSettings.ValidIssuer,
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            string token = tokenHandler.WriteToken(securityToken);

            return Task.FromResult(new LoginDto
            {
                Token = token,
                UserId = int.Parse(userId)
            });
        }
    }
}