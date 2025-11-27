namespace SPARTA.API.Controllers.Feature.Models;

public class GenerateHistoryRequest
{
    public string Text { get; set; } = string.Empty;
    public bool HasTest { get; set; } = false;
    public bool HasUpgrade { get; set; } = false;
}

