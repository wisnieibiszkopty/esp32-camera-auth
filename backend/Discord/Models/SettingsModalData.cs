using System.Text.RegularExpressions;
using backend.Models;
using Discord.Interactions;

namespace backend.Discord.Models;

public class SettingsModalData
{
    [InputLabel("Recognizable faces limit (1-10)")]
    [ModalTextInput("max_faces", placeholder: "5")]
    public string MaxRecognizableFaces { get; set; }

    [InputLabel("Security level (violation | nodetection | success)")]
    [ModalTextInput("security_level", placeholder: "Violation")]
    public string SecurityLevel { get; set; }

    [InputLabel("Violations limit (0-10)")]
    [ModalTextInput("violations_limit", placeholder: "3")]
    public string MaxViolationLimit { get; set; }

    [InputLabel("Reset time (hh:mm:ss)")]
    [ModalTextInput("reset_time", placeholder: "1hour")]
    public string TimeBeforeUnlockAfterViolation { get; set; }

    [InputLabel("Send logs to discord (t | f)")]
    [ModalTextInput("send_logs_to_discord", placeholder: "t")]
    public string SendLogsToDiscord { get; set; }
    
    public SecuritySettings ToSecuritySettings()
    {
        int maxFaces;
        if (!int.TryParse(MaxRecognizableFaces, out maxFaces))
        {
            throw new FormatException("Recognizable faces limit must be a number");
        }

        if (maxFaces < 1 || maxFaces > 20)
        {
            throw new FormatException("Amount of faces must be between 1 and 20");
        }

        SecurityLevel level;
        if (!Enum.TryParse<SecurityLevel>(SecurityLevel, out level))
        {
            throw new FormatException("Security level must be one of: Violation, NoDetection, Always");
        }

        int violationLimit;
        if (!int.TryParse(MaxViolationLimit, out violationLimit))
        {
            throw new FormatException("Violation limit must be a number");
        }

        if (violationLimit < 1 || violationLimit > 20)
        {
            throw new FormatException("Violation limit must be between 1 adn 20");
        }

        // handle reset time
        var regex = new Regex(@"^\d+:(?:[0-5]?\d):(?:[0-5]?\d)$");
        if (!regex.IsMatch(TimeBeforeUnlockAfterViolation))
        {
            throw new FormatException("Rest time must me in 'hh:mm:ss' format");
        }

        var match = regex.Match(TimeBeforeUnlockAfterViolation);
        var timeSpan = new TimeSpan(
            int.Parse(match.Groups["h"].Value),
            int.Parse(match.Groups["m"].Value),
            int.Parse(match.Groups["s"].Value)
        );
        
        if (!(SendLogsToDiscord.ToLower().Equals("t") || SendLogsToDiscord.ToLower().Equals("f")))
        {
            throw new FormatException("Send Logs To Discord must be 't' or 'f'");
        }
        bool sendLogs = SendLogsToDiscord.ToLower() == "t";
        
        return new SecuritySettings()
        {
            MaxRecognizableFaces = maxFaces,
            SecurityLevel = level,
            MaxViolationLimit = violationLimit,
            TimeBeforeUnlockAfterViolation = (int)timeSpan.TotalSeconds,
            SendLogsToDiscord = sendLogs
        };
    }
}