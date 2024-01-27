using Company.Model.Dto;
using Company.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Company.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Company.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly ApplicationDBContext _db;
        private readonly IMapper _mapper;
       
 
        public EmployeeController(ApplicationDBContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
           
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetEmployees(String? searchByName = null)
        {
            IQueryable<EmployeeModel> query = _db.Employees;

            if (!string.IsNullOrEmpty(searchByName))
            {
    
                query = query.Where(e =>
                    e.Name.Contains(searchByName));
            }

            IEnumerable<EmployeeModel> employeeList = await query.ToListAsync();
            return Ok(employeeList);
        }


    }
}
