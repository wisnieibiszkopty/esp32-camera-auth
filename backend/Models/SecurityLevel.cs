namespace backend.Models;

/// <summary>
/// Defines the security level used for saving logs
/// </summary>
public enum SecurityLevel
{
    /// <summary>
    /// Logs are saved only when a person without access is recognized (security violation)
    /// </summary>
    Violation,
    /// <summary>
    /// Logs are saved when no person is recognized and violation case
    /// </summary>
    NoDetection,
    /// <summary>
    /// Logs are saved in all cases
    /// </summary>
    Always
}