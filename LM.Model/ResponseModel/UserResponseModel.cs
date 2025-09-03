namespace LM.Model.ResponseModel;

public class UserResponseModel
{
        public string UserSid { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public long? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = null!;
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }

}