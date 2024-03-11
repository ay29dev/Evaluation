using Evaluation.Dtos;
using Evaluation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Evaluation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly JwtSecurityTokenHandler tokenHandler;
        private readonly EvaluationContext _context;
        private readonly IConfiguration _config;

        public SkillController(EvaluationContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            tokenHandler = new JwtSecurityTokenHandler();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser()
        {
            var token = GenerateJwtToken();
            return Ok(new { token });
        }
        // GET: api/<SkillController>
        [HttpGet]
        public async Task<IEnumerable<Skill>> Get() => await _context.Skills.ToListAsync();

        // GET api/<SkillController>/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int varName, [FromHeader] string Authorization)
        {
            //var y = VerifyToken(Authorization.Replace("Bearer ", ""));
            var x = TokenInfo(Authorization.Replace("Bearer ", ""));
            if (true)
            {
                throw new KeyNotFoundException(nameof(varName));
            }
            var skill = await _context.Skills.FindAsync(varName);
            if (skill == null) return NotFound();
            return Ok(skill);
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

        private string GenerateJwtToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "new subject"),
                new Claim(JwtRegisteredClaimNames.UniqueName, "new unique name"),
                new Claim(JwtRegisteredClaimNames.GivenName, "new name")
            };


            var Sectoken = new JwtSecurityToken(
              issuer: _config["JwtSettings:Issuer"],
              audience: _config["JwtSettings:Audience"],
              claims: claims,
              notBefore: DateTime.UtcNow,//.AddHours(3),
              expires: DateTime.UtcNow.AddMonths(3),
              signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);
            return token;
        }

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
