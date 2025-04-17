using System.Text.RegularExpressions;
using backend.Models;
using backend.Models.Dto;
using Discord.Interactions;

namespace backend.Discord.Models;

public class SettingsModalData : IModal
{
    public string Title => "Security Settings"; 

    [InputLabel("Security level")]
    [ModalTextInput("security_level")]
    public string SecurityLevel { get; set; }

    [InputLabel("Recognizable faces limit (1-10)")]
    [ModalTextInput("max_faces")]
    public string MaxRecognizableFaces { get; set; }

    [InputLabel("Violations limit (0-10)")]
    [ModalTextInput("violations_limit")]
    public string MaxViolationLimit { get; set; }

    [InputLabel("Reset time (hh:mm:ss)")]
    [ModalTextInput("reset_time")]
    public string TimeBeforeUnlockAfterViolation { get; set; }

    [InputLabel("Send logs to discord (True | False)")]
    [ModalTextInput("send_logs_to_discord")]
    public string SendLogsToDiscord { get; set; }
    
    public SecuritySettingsBaseDto ToSecuritySettingsDto()
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
        // I have to make working regex, which is not ai generated
        var regex = new Regex(@"^(?<h>\d+):(?<m>[0-5][0-9]):(?<s>[0-5][0-9])$");
        if (!regex.IsMatch(TimeBeforeUnlockAfterViolation))
        {
            throw new FormatException("Rest time must me in 'hh:mm:ss' format");
        }

        var match = regex.Match(TimeBeforeUnlockAfterViolation);

        if (!match.Success)
        {
            throw new FormatException("Invalid time format. hh:mm:ss is required");
        }
        
        var timeSpan = new TimeSpan(
            int.Parse(match.Groups["h"].Value),
            int.Parse(match.Groups["m"].Value),
            int.Parse(match.Groups["s"].Value)
        );
        
        if (!(SendLogsToDiscord.ToLower().Equals("true") || SendLogsToDiscord.ToLower().Equals("false")))
        {
            throw new FormatException("Send Logs To Discord must be True or False");
        }
        bool sendLogs = SendLogsToDiscord.ToLower() == "true";
        
        
        return new SecuritySettingsBaseDto()
        {
            MaxRecognizableFaces = maxFaces,
            SecurityLevel = level,
            MaxViolationLimit = violationLimit,
            TimeBeforeUnlockAfterViolation = (int)timeSpan.TotalSeconds,
            SendLogsToDiscord = sendLogs
        };
    }
}