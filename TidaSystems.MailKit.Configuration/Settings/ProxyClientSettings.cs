using System.Security.Authentication;

namespace TidaSystems.MailKit.Configuration.Settings;

/// <summary>
/// Defines settings used to construct a MailKit proxy client.
/// </summary>
public sealed class ProxyClientSettings
{
    /// <summary>
    /// Gets or sets the concrete MailKit proxy client type.
    /// </summary>
    /// <remarks>
    /// Supported values are SystemProxy, Socks4Client, Socks4aClient,
    /// Socks5Client, HttpProxyClient, and HttpsProxyClient.
    /// </remarks>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the proxy server hostname.
    /// </summary>
    public string? ProxyHost { get; set; }

    /// <summary>
    /// Gets or sets the proxy server port.
    /// </summary>
    public int? ProxyPort { get; set; }

    /// <summary>
    /// Gets or sets the proxy authentication credentials.
    /// </summary>
    public ProxyCredentialsSettings? ProxyCredentials { get; set; }

    /// <summary>
    /// Gets or sets the local network endpoint used by the proxy.
    /// </summary>
    public LocalEndPointSettings? LocalEndPoint { get; set; }

    /// <summary>
    /// Gets or sets whether TLS certificate revocation checking is enabled
    /// for HTTPS proxy connections.
    /// </summary>
    public bool? CheckCertificateRevocation { get; set; }

    /// <summary>
    /// Gets or sets the SSL/TLS protocols allowed for HTTPS proxy connections.
    /// </summary>
    public SslProtocols? SslProtocols { get; set; }

    /// <summary>
    /// Gets or sets the client certificates used by HTTPS proxy connections.
    /// </summary>
    public CertificateFileSettings[]? ClientCertificates { get; set; }
}