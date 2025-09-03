namespace LM.Model.RequestModel;

public class UserSignupRequestModel
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!; 
    public long? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = "Student";
}