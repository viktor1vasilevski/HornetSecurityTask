using Data.Context;
using EntityModels.Interfaces;
using EntityModels.Models;
using Main.Constants;
using Main.DTOs;
using Main.Enums;
using Main.Helpers;
using Main.Interfaces;
using Main.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Main.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork<AppDbContext> _uow;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork<AppDbContext> uow, IConfiguration configuration)
    {
        _uow = uow;
        _userRepository = _uow.GetGenericRepository<User>();
        _configuration = configuration;
    }
    public async Task<ApiResponse<TokenResponse>> UserLoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _userRepository.GetAsync(x => x.Email.ToLower() == request.Email.ToLower());
            var user = response?.FirstOrDefault();

            if (user is null)
            {
                return new ApiResponse<TokenResponse>
                {
                    Message = AuthConstants.USER_NOT_FOUND,
                    Success = false,
                    NotificationType = NotificationType.Info
                };
            }

            var isPasswordValid = PasswordHasher.VerifyPassword(request.Password, user.Password, user.SaltKey);

            if (!isPasswordValid)
            {
                return new ApiResponse<TokenResponse>
                {
                    Message = AuthConstants.INVALID_PASSWORD,
                    Success = false,
                    NotificationType = NotificationType.Info
                };
            }

            var token = GenerateJwtToken(user);

            return new ApiResponse<TokenResponse>
            {
                Message = AuthConstants.LOGIN_SUCCESS,
                Success = true,
                NotificationType = NotificationType.Success,
                Data = new TokenResponse { Token = token }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<TokenResponse>
            {
                Message = $"{AuthConstants.ERROR_LOGIN}:{ex.Message}",
                Success = false,
                NotificationType = NotificationType.Error
            };
        }
        

    }
    public async Task<BaseResponse> UserRegisterAsync(UserRequest request)
    {
        try
        {
            var doesUserExist = await _userRepository.ExistsAsync(x => x.Email.ToLower() == request.Email.ToLower());

            if (doesUserExist)
                return new BaseResponse { Message = AuthConstants.USER_EXISTS, Success = false, NotificationType = NotificationType.Info };

            var saltKey = GenerateSalt();

            var hash = PasswordHasher.HashPassword(request.Password, saltKey);

            var user = new User
            {
                Email = request.Email,
                Password = hash,
                FirstName = request.FirstName,
                LastName = request.LastName,
                SaltKey = Convert.ToBase64String(saltKey)
            };

            await _userRepository.InsertAsync(user);
            await _uow.SaveChangesAsync();

            return new BaseResponse { Message = AuthConstants.USER_REGISTER_SUCCESS, Success = true, NotificationType = NotificationType.Success };
        }
        catch (Exception ex)
        {
            return new BaseResponse 
            { 
                Message = $"{AuthConstants.ERROR_REGISTER}:{ex.Message}", 
                Success = false,
                NotificationType = NotificationType.Error
            };
        }
        
    }
    private static byte[] GenerateSalt(int size = 16)
    {
        byte[] salt = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        return salt;
    }
    private string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["JwtSettings:Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}


