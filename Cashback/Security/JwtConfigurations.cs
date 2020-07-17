using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Cashback.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cashback.Security
{
    public class JwtConfigurations
    {
        public string Audience { get; }
        public string Issuer { get; }
        public int ValidForMinutes { get; }
        public int RefreshTokenValidForMinutes { get; }
        public SigningCredentials SigningCredentials { get; }
        public DateTime IssuedAt => DateTime.Now;
        public DateTime NotBefore => IssuedAt;
        public DateTime AccessTokenExpiration => IssuedAt.AddMinutes(ValidForMinutes);
        public DateTime RefreshTokenExpiration => IssuedAt.AddMinutes(RefreshTokenValidForMinutes);
        
        public JwtConfigurations(IConfiguration configuration)
        {
            if(configuration == null) {
                throw new ArgumentNullException("Arquivo appsettings.json não encontrado!");
            }

             if (configuration["JwtSettings:SigningKey"] == null || configuration["JwtSettings:SigningKey"] == "")
            {
                 throw new ArgumentNullException("Configurações para o JWT não foram encontrados.\n\n Abra o arquivo appsettings.json " + 
                "e adicione na seguinte estrutura: \n 'JwtSettings':' {\n   'Issuer':'---------',\n   'Audience':'---------',\n   " +
                "'ValidForMinutes':'---------',\n   'RefreshTokenValidForMinutes':'---------',\n   'SigningKey': '----------'\n   }");
            }

            Issuer = configuration["JwtSettings:Issuer"];
            Audience = configuration["JwtSettings:Audience"];
            ValidForMinutes = Convert.ToInt32(configuration["JwtSettings:ValidForMinutes"]);
            RefreshTokenValidForMinutes = Convert.ToInt32(configuration["JwtSettings:RefreshTokenValidForMinutes"]);

            var signingKey = configuration["JwtSettings:SigningKey"];
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            SigningCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature);
        }

        public string GenerateJwtToken(User user)
        {
            var identity = GetClaimsIdentity(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,                
                NotBefore = NotBefore,
                Expires = AccessTokenExpiration,
                SigningCredentials = SigningCredentials
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static ClaimsIdentity GetClaimsIdentity(User user)
        {
            var identity = new ClaimsIdentity
            (new Claim[]
                {
                     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)                    
                }
            );

            return identity;
        }
    }
}