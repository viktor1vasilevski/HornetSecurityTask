using Main.DTOs;
using Main.Responses;

namespace Main.Interfaces;

public interface IAuthService
{
    Task<BaseResponse> UserRegisterAsync(UserRequest request);
    Task<ApiResponse<TokenResponse>> UserLoginAsync(LoginRequest request);
}
