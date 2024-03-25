using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Evaluation.General
{
    public class Helper
    {
        private static JwtSecurityTokenHandler _tokenHandler;
        private static IConfiguration _config;

        public Helper(IConfiguration config)
        {
            _config = config;
            _tokenHandler = new JwtSecurityTokenHandler();
        }


        public static string HashString(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            byte[] salt = System.Text.Encoding.UTF8.GetBytes(text);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: text,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return hashed;
        }


        public static bool VerifyExpToken(string token)
        {
            try
            {
                var claims = _tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"])),
                    ValidIssuer = _config["JwtSettings:Issuer"],
                    ValidAudience = _config["JwtSettings:Audience"],
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                return true;
            }
            catch (SecurityTokenExpiredException er)
            {
                return false;
            }
        }

        public static ClaimsPrincipal TokenInfo(string tokenUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenUser);

            IEnumerable<Claim> claims = token.Claims;
            return (ClaimsPrincipal)claims;

            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("Subject", token.Subject));
            identity.AddClaim(new Claim("Issuer", token.Issuer));
            identity.AddClaim(new Claim("Audiences", token.Audiences == null ? "" : string.Join(",", token.Audiences)));
            identity.AddClaim(new Claim("ValidFrom", token.ValidFrom.ToString()));
            identity.AddClaim(new Claim("ValidTo", token.ValidTo.ToString()));
            var newClaims = new ClaimsPrincipal(identity);
        }

    }
}
