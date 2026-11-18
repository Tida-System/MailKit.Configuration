using System.Net.Security;
using System.Security.Authentication;

namespace TidaSystems.MailKit.Configuration.Settings;

/// <summary>
/// Defines the shared configuration for MailKit SMTP, POP3, and IMAP clients.
/// </summary>
/// <remarks>
/// Settings are loaded from the root "MailKit" configuration section.
/// Common settings are shared by all configured protocols.
/// </remarks>
public sealed class MailServiceSettings
{
    /// <summary>
    /// Gets or sets the authentication username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the authentication password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the OAuth2 access token.
    /// </summary>
    public string? OAuthToken { get; set; }

    /// <summary>
    /// Gets or sets the allowed authentication mechanisms.
    /// </summary>
    public string[]? AuthenticationMechanisms { get; set; }

    /// <summary>
    /// Gets or sets the network operation timeout, in milliseconds.
    /// </summary>
    public int? Timeout { get; set; }

    /// <summary>
    /// Gets or sets the local network endpoint.
    /// </summary>
    public LocalEndPointSettings? LocalEndPoint { get; set; }

    /// <summary>
    /// Gets or sets whether TLS certificate revocation checking is enabled.
    /// </summary>
    public bool? CheckCertificateRevocation { get; set; }

    /// <summary>
    /// Gets or sets the allowed SSL/TLS protocols.
    /// </summary>
    public SslProtocols? SslProtocols { get; set; }

    /// <summary>
    /// Gets or sets the allowed TLS cipher suites.
    /// </summary>
    /// <remarks>
    /// Corresponds to the MailKit SslCipherSuitesPolicy property.
    /// The policy is not supported on Windows or Android.
    /// </remarks>
    public TlsCipherSuite[]? SslCipherSuitesPolicy { get; set; }

    /// <summary>
    /// Gets or sets the client certificates used for TLS authentication.
    /// </summary>
    public CertificateFileSettings[]? ClientCertificates { get; set; }

    /// <summary>
    /// Gets or sets the proxy client configuration.
    /// </summary>
    public ProxyClientSettings? ProxyClient { get; set; }

    /// <summary>
    /// Gets or sets the protocol logging file path.
    /// </summary>
    /// <remarks>
    /// Used to create a MailKit ProtocolLogger instance.
    /// Protocol logs may contain sensitive information.
    /// </remarks>
    public string? ProtocolLogPath { get; set; }

    /// <summary>
    /// Gets or sets the SMTP configuration.
    /// </summary>
    public SmtpSettings? Smtp { get; set; }

    /// <summary>
    /// Gets or sets the POP3 configuration.
    /// </summary>
    public Pop3Settings? Pop3 { get; set; }

    /// <summary>
    /// Gets or sets the IMAP configuration.
    /// </summary>
    public ImapSettings? Imap { get; set; }

    /// <summary>
    /// Gets or sets the named default message profiles.
    /// </summary>
    public Dictionary<string, SenderSettings>? Senders { get; set; }
}