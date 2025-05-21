using Click_Go.Enum;

namespace Click_Go.DTOs
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Application_Role? Role { get; set; } = Application_Role.CUSTOMER;
    }
}
