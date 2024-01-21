using Evaluation.Dtos;
using Evaluation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Evaluation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {

        private readonly EvaluationContext _context;
        public SkillController(EvaluationContext context) => _context = context;


        // GET: api/<SkillController>
        [HttpGet]
        public async Task<IEnumerable<Skill>> Get() => await _context.Skills.ToListAsync();

        // GET api/<SkillController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
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
    }
}
