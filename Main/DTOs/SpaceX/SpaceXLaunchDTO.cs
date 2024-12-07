namespace Main.DTOs.SpaceX;

public class SpaceXLaunchDTO
{
    public string? Name { get; set; }
    public int? FlightNumber { get; set; }
    public DateTime? DateUtc { get; set; }
    public bool? Success { get; set; }
    public string? Webcast { get; set; }
    public string? Wikipedia { get; set; }
    public List<CrewDTO>? Crew { get; set; }
    public List<string>? Ships { get; set; }
    public LinksDTO? Links { get; set; }
    public bool? Upcoming { get; set; }
}

