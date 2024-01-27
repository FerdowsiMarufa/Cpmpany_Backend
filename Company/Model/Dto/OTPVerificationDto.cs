using Org.BouncyCastle.Bcpg;

namespace Company.Model.Dto
{
    public class OTPVerificationDto
    {
        public int UserId { get; set; }
        public string OTP { get; set; }
    }
}
