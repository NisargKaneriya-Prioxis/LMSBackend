namespace LM.Model.ResponseModel;

public class UserSignupResponseModel
{
    public string UserSid { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    
    public string Role { get; set; } = null!;
}