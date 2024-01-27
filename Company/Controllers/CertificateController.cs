using AutoMapper;
using Company.Data;
using Company.Model;
using Company.Model.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Company.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
 
            private readonly ApplicationDBContext _db;
            private readonly IMapper _mapper;


            public CertificateController(ApplicationDBContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;

            }


            [HttpPost("createCertificateForExistingEmployee")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public ActionResult CreateCertificateForExistingEmployee([FromBody] CertificateDto request)
            {
                try
                {
                    // Validate the request
                    if (request == null || request.EmployeeId <= 0)
                    {
                        ModelState.TryAddModelError("CustomError", "Invalid request");
                        return BadRequest(ModelState);
                    }

                    // Retrieve the existing employee from the database
                    var existingEmployee = _db.Employees.SingleOrDefault(e => e.Id == request.EmployeeId);

                    if (existingEmployee == null)
                    {
                        // Handle the case where the employee with the specified ID is not found
                        ModelState.TryAddModelError("CustomError", "Employee not found");
                        return BadRequest(ModelState);
                    }

                    // Check if the employee already has a certificate
                    if (existingEmployee.CertificateId != null)
                    {
                        // Handle the case where the employee already has a certificate
                        ModelState.TryAddModelError("CustomError", "Employee already has a certificate");
                        return BadRequest(ModelState);
                    }

                    // Create a new certificate
                    var certificate = new CertificateModel
                    {
                        CreatedDate = DateTime.Now,
                        UserId = request.UserId
                    };

                    // Save the certificate to the database
                    _db.Certificates.Add(certificate);
                    _db.SaveChanges();

                    // Assign the certificate to the existing employee
                    existingEmployee.CertificateId = certificate.Id;

                    // Save the changes to the database
                    _db.SaveChanges();

                    return Ok("Certificate created successfully");
                }
                catch (Exception ex)
                {
                    // Log the exception
                    ModelState.TryAddModelError("CustomError", "Failed to create certificate");
                    return BadRequest(ModelState);
                }
            }

    }
}
