using System.Threading.Tasks;
using EmbyStat.Common.Models.Account;

namespace EmbyStat.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest login, string remoteIp);
        Task Register(AuthenticateRequest login);
        Task LogOut();
        Task<AuthenticateResponse> RefreshToken(string accessToken, string refreshToken, string remoteIp);
        bool AnyAdmins();
        Task<bool> ResetPassword(string username);
        Task<bool> ChangePassword(ChangePasswordRequest request);
        Task<bool> ChangeUserName(ChangeUserNameRequest request);
    }
}
