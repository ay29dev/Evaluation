using Evaluation.Dtos;
using Evaluation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Evaluation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormController : ControllerBase
    {
        private readonly EvaluationContext _context;
        public FormController(EvaluationContext context) => _context = context;

        // GET: api/<FormController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] // in swager define what could be return as status code
        public async Task<IEnumerable<FormMst>> Get() => await _context.FormMsts.Include(e => e.Emp).ToListAsync();

        // GET api/<FormController>/5
        [HttpGet("{id}", Name= "GetForm")]
        public async Task<IActionResult> Get(int id)
        {
            //var emp = await _context.Employees.FindAsync(id);
            var form = await _context.FormMsts.Where(x => x.FormId== id).FirstOrDefaultAsync();
            if (form == null) return NotFound();
            return Ok(form);
        }
        
        // POST api/<FormController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FormMstDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _context.Employees.FindAsync(dto.EmpId);
            if (employee == null)
            {
                return BadRequest("Invalid Employee ID");
            }

            var form = new FormMst
            {
            FormDate = dto.FormDate,
            KnowledgeType = dto.KnowledgeType,
            EmpId = dto.EmpId,
            //Emp = employee
            };
            await _context.AddAsync(form);
            _context.SaveChanges();
            //return CreatedAtRoute("GetForm", new { FormId = _context.FormMsts.id }, form); //return the url of new form
            return Ok(form);
        }

        // PUT api/<FormController>/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] FormMstDto dto)
        {
            var form = await _context.FormMsts.FindAsync(id);
            if (form == null) return NotFound($"The emp ID not found: {id}");

            form.FormDate = dto.FormDate;
            form.KnowledgeType = dto.KnowledgeType;
            form.EmpId = dto.EmpId;
            
            _context.SaveChanges();
            return Ok(form);
        }

        // DELETE api/<FormController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var form = await _context.FormMsts.FindAsync(id);

            if (form == null) return NotFound($"The ID not found: {id}");

            _context.Remove(form);
            _context.SaveChanges();
            return Ok(form);

        }


        [HttpPost("fullForm")]
        public async Task<IActionResult> Post([FromBody] FullFormDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _context.Employees.FindAsync(dto.EmpId);
            var skill = await _context.Skills.FindAsync(dto.SkillId);

            if (employee == null || skill == null)
            {
                return BadRequest("Invalid Employee OR Skill ID");
            }

            var form = new FullFormDto
            {
                FormDate = dto.FormDate,
                KnowledgeType = dto.KnowledgeType,
                EmpId = dto.EmpId,
                SkillId = dto.SkillId,
                SkillDegree = dto.SkillDegree,
                //Emp = employee
            };
            await _context.AddAsync(form);
            _context.SaveChanges();
            //return CreatedAtRoute("GetForm", new { FormId = _context.FormMsts.id }, form); //return the url of new form
            return Ok(form);
        }

    }
}
