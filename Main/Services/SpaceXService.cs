using Main.Constants;
using Main.DTOs.SpaceX;
using Main.Enums;
using Main.Interfaces;
using Main.Responses;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Main.Services;

public class SpaceXService : ISpaceXService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    public SpaceXService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ApiResponse<List<SpaceXLaunchDTO>>> GetListLaunchDataAsync(string type)
    {
        try
        {
            var launchesUrl = _configuration["SpaceX:LaunchesUrl"];
            var responseMessage = await _httpClient.GetAsync($"{launchesUrl}/{type}");

            if (!responseMessage.IsSuccessStatusCode)
            {
                var errorMessage = await responseMessage.Content.ReadAsStringAsync();
                return new ApiResponse<List<SpaceXLaunchDTO>>
                {
                    Success = false,
                    Message = $"{SpaceXConstants.ERROR_FETCHING_LAUNCH_DATA} : {errorMessage}",
                    NotificationType = NotificationType.ServerError
                };
            }

            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            var launches = JsonConvert.DeserializeObject<List<SpaceXLaunchDTO>>(responseContent);

            var launchesDTO = launches?.Select(x => new SpaceXLaunchDTO
            {
                Name = x.Name,
                DateUtc = x.DateUtc,
                Webcast = x.Links?.Webcast,
                Wikipedia = x.Links?.Wikipedia,
                Success = x.Success,
                Ships = x.Ships ?? new List<string>(),
                Crew = x.Crew?.Select(c => new CrewDTO
                {
                    Role = c.Role
                }).ToList(),
                Links = new LinksDTO
                {
                    Patch = new PatchDTO
                    {
                        Small = x.Links?.Patch?.Small,
                        Large = x.Links?.Patch?.Large
                    },
                    Reddit = new RedditDTO
                    {
                        Launch = x.Links?.Reddit?.Launch
                    },
                    Flickr = new FlickrDTO
                    {
                        Small = x.Links?.Flickr?.Small ?? new List<string>(),
                        Original = x.Links?.Flickr?.Original ?? new List<string>()
                    },
                    Webcast = x.Links?.Webcast,
                    Wikipedia = x.Links?.Wikipedia,
                    YoutubeId = x.Links?.YoutubeId
                }
            }).ToList();

            return new ApiResponse<List<SpaceXLaunchDTO>> 
            { 
                Data = launchesDTO, 
                Success = true, 
                NotificationType = NotificationType.Success 
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<SpaceXLaunchDTO>> 
            { 
                Success = false,
                Message = $"{ErrorMessagesConstants.GenericServerError} : {ex.Message}",
                NotificationType = NotificationType.ServerError
            };
        }
        
    }

    public async Task<ApiResponse<SpaceXLaunchDTO>> GetLatestLaunchDataAsync()
    {
        try
        {
            var launchesUrl = _configuration["SpaceX:LaunchesUrl"];
            var responseMessage = await _httpClient.GetAsync($"{launchesUrl}/latest");

            if (!responseMessage.IsSuccessStatusCode)
            {
                var errorMessage = await responseMessage.Content.ReadAsStringAsync();
                return new ApiResponse<SpaceXLaunchDTO>
                {
                    Success = false,
                    Message = $"{SpaceXConstants.ERROR_FETCHING_LAUNCH_DATA} : {errorMessage}",
                    NotificationType = NotificationType.ServerError
                };
            }

            var responseContent = await responseMessage.Content.ReadAsStringAsync();
            var launch = JsonConvert.DeserializeObject<SpaceXLaunchDTO>(responseContent);

            return new ApiResponse<SpaceXLaunchDTO>
            {
                Success = true,
                Data = new SpaceXLaunchDTO
                {
                    Name = launch?.Name,
                    DateUtc = launch?.DateUtc,
                    Webcast = launch?.Links?.Webcast,
                    Wikipedia = launch?.Links?.Wikipedia,
                    Success = launch?.Success,
                    Ships = launch?.Ships ?? new List<string>(),
                    Crew = launch?.Crew?.Select(c => new CrewDTO
                    {
                        Role = c.Role
                    }).ToList(),
                    Links = new LinksDTO
                    {
                        Patch = new PatchDTO
                        {
                            Small = launch?.Links?.Patch?.Small,
                            Large = launch?.Links?.Patch?.Large
                        },
                        Reddit = new RedditDTO
                        {
                            Launch = launch?.Links?.Reddit?.Launch
                        },
                        Flickr = new FlickrDTO
                        {
                            Small = launch?.Links?.Flickr?.Small ?? new List<string>(),
                            Original = launch?.Links?.Flickr?.Original ?? new List<string>()
                        },
                        Webcast = launch?.Links?.Webcast,
                        Wikipedia = launch?.Links?.Wikipedia,
                        YoutubeId = launch?.Links?.YoutubeId
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<SpaceXLaunchDTO> 
            { 
                Success = false,
                Message = $"{ErrorMessagesConstants.GenericServerError} : {ex.Message}",
                NotificationType = NotificationType.ServerError
            };
        }
        
    }

}
