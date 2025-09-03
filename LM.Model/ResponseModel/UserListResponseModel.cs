namespace LM.Model.ResponseModel;

public class UserListResponseModel
{
    public string UserSid { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public long? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}