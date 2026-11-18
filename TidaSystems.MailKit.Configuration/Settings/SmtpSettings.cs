using MailKit.Security;

namespace TidaSystems.MailKit.Configuration.Settings;

/// <summary>
/// Defines SMTP-specific connection settings.
/// </summary>
public sealed class SmtpSettings
{
    /// <summary>
    /// Gets or sets the SMTP server hostname.
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// Gets or sets the SMTP server port.
    /// </summary>
    /// <remarks>
    /// Zero allows MailKit to select the default port.
    /// </remarks>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the security options used when connecting.
    /// </summary>
    public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.Auto;

    /// <summary>
    /// Gets or sets the domain used by SMTP HELO and EHLO commands.
    /// </summary>
    public string? LocalDomain { get; set; }

    /// <summary>
    /// Gets or sets whether the SMTP REQUIRETLS extension should be used when available.
    /// </summary>
    public bool? RequireTLS { get; set; }
}