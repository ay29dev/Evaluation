using Evaluation.Dtos;
using Evaluation.General;
using Evaluation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;


namespace Evaluation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly EvaluationContext _context;
        private readonly IConfiguration _config;

        public SkillController(EvaluationContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _tokenHandler = new JwtSecurityTokenHandler();
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllSkills([FromHeader] string Authorization)
        {
            string token = Authorization.Replace("Bearer ", "");
            if (!VerifyExpToken(token)) throw new SecurityTokenExpiredException("انتهت صلاحية التوكن");
            IList<Claim> tokenInfo = GetClaims(token);

            return Ok(await _context.Skills.ToListAsync());
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetailsSkillById(int id, [FromHeader] string Authorization)
        {
            string token = Authorization.Replace("Bearer ", "");
            if (!VerifyExpToken(token)) throw new SecurityTokenExpiredException("انتهت صلاحية التوكن");
            IList<Claim> tokenInfo = GetClaims(token);

            // DateTime.UtcNow.AddHours(3),
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) throw new KeyNotFoundException("المهارة المطلوبة غير موجودة");
            return Ok(new { skill, tokenInfo });
        }

        // POST api/<SkillController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SkillDto dto)
        {
            var skill = new Skill
            {
                SkillTitle = dto.SkillTitle,
                SkillType = dto.SkillType
            };

            await _context.AddAsync(skill);
            _context.SaveChanges();
            return Ok(skill);
        }

        // PUT api/<SkillController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SkillDto dto)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound($"The skill ID not found: {id}");

            skill.SkillTitle = dto.SkillTitle;
            skill.SkillType = dto.SkillType;

            _context.SaveChanges();
            return Ok(skill);
        }

        // DELETE api/<SkillController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _context.Skills.FindAsync(id);

            if (skill == null) return NotFound($"The skill ID not found: {id}");

            _context.Remove(skill);
            _context.SaveChanges();
            return Ok(skill);
        }

        private bool VerifyExpToken(string token)
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

        public IList<Claim> GetClaims(string tokenUser)
        {
            var claims = _tokenHandler.ReadJwtToken(tokenUser).Claims;
            return claims.ToList();
        }

        //private ClaimsPrincipal VerifyToken(string token)
        //{
        //    try
        //    {
        //        var claims = _tokenHandler.ValidateToken(token,
        //        new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("$aaaAAAbbbBBmnbmnbmnbm7868768768768768hjhgB@#$%12345")),
        //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"])),
        //            ValidateLifetime = true,
        //            ValidateAudience = false,
        //            ValidateIssuer = false,
        //            ClockSkew = TimeSpan.Zero
        //        }, out SecurityToken validatedToken);
        //        return claims;
        //    }
        //    catch (SecurityTokenExpiredException er)
        //    {
        //        var identity = new ClaimsIdentity();
        //        identity.AddClaim(new Claim("exp", "SecurityTokenExpiredException"));
        //        var newPrincipal = new ClaimsPrincipal(identity);
        //        return newPrincipal;
        //    }
        //    catch (Exception er)
        //    {
        //        var identity = new ClaimsIdentity();
        //        identity.AddClaim(new Claim(JwtRegisteredClaimNames.UniqueName, er.Message));
        //        var newPrincipal = new ClaimsPrincipal(identity);
        //        return newPrincipal;
        //    }
        //}

        //public ClaimsPrincipal TokenInfo(string tokenUser)
        //{
        //    var token = _tokenHandler.ReadJwtToken(tokenUser);

        //    var claims = _tokenHandler.ValidateToken(tokenUser,
        //        new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"])),
        //            ValidateLifetime = true,
        //            ValidateAudience = false,
        //            ValidateIssuer = false,
        //            ClockSkew = TimeSpan.Zero
        //        }, out SecurityToken validatedToken);
        //    return claims;
        //}


    }
}
