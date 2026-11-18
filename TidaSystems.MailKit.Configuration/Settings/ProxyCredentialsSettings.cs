namespace TidaSystems.MailKit.Configuration.Settings;

/// <summary>
/// Defines authentication credentials for a MailKit proxy client.
/// </summary>
public sealed class ProxyCredentialsSettings
{
    /// <summary>
    /// Gets or sets the proxy authentication username.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the proxy authentication password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the proxy authentication domain.
    /// </summary>
    public string? Domain { get; set; }
}