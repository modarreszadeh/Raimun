using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Web.Domain;
using Web.Infrastructure.Model;

namespace Web.Infrastructure
{
    public class JwtHandler : IJwtHandler
    {
        private IEnumerable<Claim> _getClaims(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("SecurityStamp", user.StampCode.ToString()),
            };

            return claims;
        }

        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _securityKey;
        private readonly int _expires;

        public JwtHandler(IOptions<JwtSetting> options)
        {
            _issuer = options.Value.Issuer;
            _audience = options.Value.Audience;
            _securityKey = options.Value.SecurityKey;
            _expires = options.Value.Expires;
        }

        public TokenModel GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(_securityKey));
            var signin = new SigningCredentials
                (securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = _getClaims(user);

            var expires = DateTime.Now.AddHours(_expires);

            var descriptor = new SecurityTokenDescriptor()
            {
                Audience = _audience,
                Issuer = _issuer,
                IssuedAt = DateTime.Now,
                Expires = expires,
                NotBefore = DateTime.Now,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signin,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(descriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return new TokenModel
            {
                Expires = expires,
                Token = token
            };
        }
    }

    public interface IJwtHandler
    {
        TokenModel GenerateToken(User user);
    }
}