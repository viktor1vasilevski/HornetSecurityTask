using Main.DTOs.SpaceX;
using Main.Responses;

namespace Main.Interfaces;

public interface ISpaceXService
{
    Task<ApiResponse<SpaceXLaunchDTO>> GetLatestLaunchDataAsync();
    Task<ApiResponse<List<SpaceXLaunchDTO>>> GetListLaunchDataAsync(string type);
}
