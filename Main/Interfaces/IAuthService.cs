using Main.DTOs;
using Main.Requests.Auth;
using Main.Responses;
using Main.Responses.Auth;

namespace Main.Interfaces;

public interface IAuthService
{
    Task<BaseResponse> UserRegisterAsync(UserRequest request);
    Task<ApiResponse<TokenResponse>> UserLoginAsync(LoginRequest request);
}
