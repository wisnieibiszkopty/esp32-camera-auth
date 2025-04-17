namespace backend.Models.Dto;

public class SecuritySettingsBaseDto
{
    public int MaxRecognizableFaces { get; set; }
    public SecurityLevel SecurityLevel { get; set; }
    public int MaxViolationLimit { get; set; }
    public int TimeBeforeUnlockAfterViolation { get; set; }
    public bool SendLogsToDiscord { get; set; }
}