namespace EmbyStat.Common.Models.Account;

public class AuthenticateRequest
{
    public string Password { get; set; }
    public string Username { get; set; }
    public bool RememberMe { get; set; }
}