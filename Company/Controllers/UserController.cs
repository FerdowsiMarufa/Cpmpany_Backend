using AutoMapper;
using Company.Data;
using Company.Model;
using Company.Model.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Company.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        private readonly IMapper _mapper;
        private readonly EmailController _emailController;
        private readonly Random _random = new Random();
        private readonly IConfiguration _configuration;
        public UserController(ApplicationDBContext db,IMapper mapper, EmailController emailController, Random random, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _emailController = emailController;
            _random = random;
            _configuration = configuration;
         }

        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<IEnumerable<UserDto>>>GetUsers()
        //{
        //    IEnumerable<User>userList = await _db.Users.ToListAsync();
        //    return Ok(_mapper.Map<UserDto>(userList));
        //}
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> CreateUser([FromBody] UserDto userDto)
        {
            if( await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Name.ToLower() ==  userDto.Name.ToLower ()) != null)
            {
                ModelState.TryAddModelError("CustomError", " User Already Exits");
                return BadRequest(ModelState);
            }
            if(userDto == null)
            {
                return BadRequest(userDto);
            }
            User model =  _mapper.Map<User>(userDto);
            await _db.Users.AddAsync(model);
            await _db.SaveChangesAsync();
            return Ok(userDto);  
        }

        // to create OTP
        private string GenerateOTP()
        {
            // Generate a random 6-digit OTP
            int otp = _random.Next(100000, 999999);
            return otp.ToString();
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> LoginUser([FromBody] LoginDto loginDto)
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Name.ToLower() == loginDto.Name.ToLower() && u.Password == loginDto.Password);

            if (user == null)
            {
                ModelState.TryAddModelError("CustomError", "Invalid email or password");
                return Unauthorized(ModelState);
            }
            // Generate OTP
            string otp = GenerateOTP();

            HttpContext.Session.SetString($"UserOTP_{user.Id}", otp);  // store in session

            _mapper.Map<UserDto>(user);

            // Assuming 'SendEmail' method is part of the same controller
            var emailRequest = new EmailController.EmailRequest
            {
                To = user.Email,
                OTP = otp 
            };

            // Call SendEmail method from EmailController
            var emailResult = _emailController.SendEmail(emailRequest);


            if (emailResult == "success")
            {
                // Return the userDto if sending email is successful
                return Ok(user);
            }
            else
            {
                // Handle the case where sending email failed
                ModelState.TryAddModelError("CustomError", "Failed to send OTP email");
                return BadRequest(ModelState);
            }
        }


        [HttpPost("checkotp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CheckOTP([FromBody] OTPVerificationDto otpVerificationDto)
        {    
            // Retrieve OTP from session using the user identifier as part of the key
            var storedOTP = HttpContext.Session.GetString($"UserOTP_{otpVerificationDto.UserId}");

            if (storedOTP == null)
            {
                // No OTP found for the user
                return BadRequest(new { Message = "No OTP found for the user" });
            }

            if (otpVerificationDto.OTP != storedOTP)
            {
                // OTP is invalid
                return BadRequest(new { Message = "Invalid OTP" });
            }

            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == otpVerificationDto.UserId);
            string token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(
                _configuration.GetSection("JWT:Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
