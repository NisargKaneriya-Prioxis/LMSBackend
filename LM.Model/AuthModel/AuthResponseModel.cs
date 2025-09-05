namespace LM.Model.AuthModel;

public class AuthResponseModel
{
    public int Id { get; set; }
    public string Sid { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; } 
    public string Token { get; set; }
}
