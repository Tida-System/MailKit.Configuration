namespace TidaSystems.MailKit.Configuration.Settings;

/// <summary>
/// Defines a PKCS#12 certificate file used for TLS client authentication.
/// </summary>
public sealed class CertificateFileSettings
{
    /// <summary>
    /// Gets or sets the path to the certificate file.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the certificate password.
    /// </summary>
    public string? Password { get; set; }
}