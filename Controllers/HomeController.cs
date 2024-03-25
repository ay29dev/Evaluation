using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Evaluation.Models;
using Evaluation.Dtos.In;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Evaluation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly EvaluationContext _context;
        private readonly JwtSecurityTokenHandler tokenHandler;

        public HomeController(EvaluationContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            tokenHandler = new JwtSecurityTokenHandler();
        }


        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(LogInDto dto)
        {
            string hash = General.Helper.HashString(dto.Password);
            Employee emp = await _context.Employees.Where(u => u.Username.ToLower() == dto.Username.ToLower()).FirstOrDefaultAsync(); //&& u.Password == hash).FirstOrDefaultAsync();
            if (emp == null) return BadRequest("الرجاء التأكد من اسم المستخدم");

            var token = GenerateJwtToken(emp);

            return Ok(new { token });
        }



        private string GenerateJwtToken(Employee dto)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, dto.EmpId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, dto.Username),
                new Claim(JwtRegisteredClaimNames.GivenName, dto.EmpName)
            };

            var Sectoken = new JwtSecurityToken(
              issuer: _config["JwtSettings:Issuer"],
              audience: _config["JwtSettings:Audience"],
              claims: claims,
              notBefore: DateTime.UtcNow,
              expires: DateTime.UtcNow.AddMonths(1),   // Plus 5 min to exp
              signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);
            return token;
        }

        //private string GJwtToken(Employee dto)
        //{

        //    byte[] symmetricKey = Convert.FromBase64String(_config["JwtSettings:SecretKey"]);
        //    SymmetricSecurityKey securityKey = new(symmetricKey);
        //    string algorithms = SecurityAlgorithms.HmacSha256Signature;

        //    SecurityTokenDescriptor tokenDescriptor = new()
        //    {
        //        IssuedAt = DateTime.UtcNow.AddMinutes(-5),
        //        NotBefore = DateTime.UtcNow.AddMinutes(-5),
        //        Expires = DateTime.UtcNow.AddMinutes(1),
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim("role",dto.Username), new Claim("username",dto.EmpName)
        //        }),
        //        SigningCredentials = new SigningCredentials(securityKey, algorithms)
        //    };

        //    JwtSecurityTokenHandler tokenHandler = new();
        //    SecurityToken stoken = tokenHandler.CreateToken(tokenDescriptor);

        //    return tokenHandler.WriteToken(stoken);
        //}

        private IActionResult TokenInfo(string tokenUser)
        {
            //string jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenUser);

            //string subject = token.Subject;
            //string issuer = token.Issuer;
            //string audience = token.Audiences == null ? "" : string.Join(",", token.Audiences);
            DateTime validFrom = token.ValidFrom;
            DateTime validTo = token.ValidTo;
            IEnumerable<Claim> claims = token.Claims;

            return Ok(
                new
                {
                    //Subject = subject,
                    //Issuer = issuer,
                    //Audience = audience,
                    validFrom,
                    validTo,
                    claims
                });

        }

        private ClaimsPrincipal VerifyToken(string token)
        {
            try
            {
                var claims = tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("$aaaAAAbbbBBmnbmnbmnbm7868768768768768hjhgB@#$%12345")),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"])),
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                return claims;
            }
            catch (SecurityTokenExpiredException er)
            {
                var identity = new ClaimsIdentity();
                identity.AddClaim(new Claim("exp", "SecurityTokenExpiredException"));
                var newPrincipal = new ClaimsPrincipal(identity);
                return newPrincipal;
            }
            catch (Exception er)
            {
                var identity = new ClaimsIdentity();
                identity.AddClaim(new Claim(JwtRegisteredClaimNames.UniqueName, er.Message));
                var newPrincipal = new ClaimsPrincipal(identity);
                return newPrincipal;
            }
        }

    }
}
