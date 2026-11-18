using MailKit.Security;

namespace TidaSystems.MailKit.Configuration.Settings;

/// <summary>
/// Defines POP3-specific connection settings.
/// </summary>
public sealed class Pop3Settings
{
    /// <summary>
    /// Gets or sets the POP3 server hostname.
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// Gets or sets the POP3 server port.
    /// </summary>
    /// <remarks>
    /// Zero allows MailKit to select the default port.
    /// </remarks>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the security options used when connecting.
    /// </summary>
    public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.Auto;
}