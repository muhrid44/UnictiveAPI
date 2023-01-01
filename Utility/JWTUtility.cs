using EntityModel;
using HelperClasses;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class JWTUtility
    {
        public static string CreateToken(UserAuthModel userAuth)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userAuth.Email)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(ApplicationSettings.TokenKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);


            return jwt;
        }

        public static string GetEmailToken(string token)
        {
            var getToken = $"[{token}]";
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            return jwtSecurityToken.Claims.First().Value;
        }

    }
}
