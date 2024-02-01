using AutoMapper;
using Evaluation.Dtos;
using Evaluation.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Evaluation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EvaluationContext _context;
        private readonly IMapper _mapper;
        public EmployeeController(EvaluationContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        // GET: api/<EmployeeController>
        [HttpGet]
        public async Task<IEnumerable<Employee>> Get() => await _context.Employees.ToListAsync();


        //[HttpGet]
        //public IActionResult GetMapped()
        //{

        //    var emps = _context.Employees.ToListAsync();
        //    var empDto = _mapper.Map<List<EmployeeDto>>(emps);

        //    var response = new
        //    {
        //        Message = $"تمت عملية استدعاء بيانات الموظفين بنجاح",
        //        empDto
        //    };

        //    return Ok(response);
        //}


        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            //var emp = await _context.Employees.FindAsync(id);
            var emp = await _context.Employees.Where(x => x.EmpId == id).FirstOrDefaultAsync();
            //var emp = _mapper.Map<Employee>(_context.Employees.FirstOrDefaultAsync());

            return Ok(emp);
        }

        // POST api/<EmployeeController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmployeeDto dto)
        {
            var emp = new Employee
            {
                EmpName = dto.EmpName,
                EmpTitle = dto.EmpTitle,
                EmpDep = dto.EmpDep,
                EmpStep = dto.EmpStep
            };
            await _context.AddAsync(emp);
            _context.SaveChanges();
            return Ok(emp);
        }

        // PUT api/<EmployeeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] EmployeeDto dto)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return NotFound($"The emp ID not found: {id}");

            emp.EmpName = dto.EmpName;
            emp.EmpTitle = dto.EmpTitle;
            emp.EmpDep = dto.EmpDep;
            emp.EmpStep = dto.EmpStep;

            _context.SaveChanges();
            return Ok(emp);

        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _context.Employees.FindAsync(id);

            if (emp == null) return NotFound($"The ID not found: {id}");

            _context.Remove(emp);
            _context.SaveChanges();
            return Ok(emp);

        }
    }
}
