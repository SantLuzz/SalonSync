namespace SalonSync.Core.Requests.Configs;

public class UpdateSystemConfigRequest : Request
{
    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; }
    public bool Saturday { get; set; }
    public bool Sunday { get; set; }
    public bool OpenScheduleAutomatically { get; set; }
}