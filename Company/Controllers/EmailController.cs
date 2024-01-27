using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;

namespace Company.Controllers
{
    public class EmailController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("sendEmail")]
        public string SendEmail([FromBody] EmailRequest emailRequest)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("****************", "nde.digital"));
                message.To.Add(new MailboxAddress("", emailRequest.To));
                //message.Subject = emailRequest.Subject;
                message.Subject = "Login OTP";
                message.Body = new TextPart("plain")
                {
                    Text = $"Your OTP is: {emailRequest.OTP}"
                };


                using (var client = new SmtpClient())
                {
                    client.Connect(_configuration["SmtpSettings:Host"], int.Parse(_configuration["SmtpSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(_configuration["SmtpSettings:Email"], _configuration["SmtpSettings:Password"]);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (System.Exception ex)
            {
                return "error from server";
            }
            return "success";
        }

        public class EmailRequest
        {
            public string To { get; set; }
            //public string Subject { get; set; }
            public string OTP { get; set; }
        }
    }
}
