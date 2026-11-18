namespace TidaSystems.MailKit.Configuration.Settings;

/// <summary>
/// Defines a local network endpoint.
/// </summary>
public sealed class LocalEndPointSettings
{
    /// <summary>
    /// Gets or sets the local IP address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the local TCP port.
    /// </summary>
    /// <remarks>
    /// Zero allows the operating system to select an available port.
    /// </remarks>
    public int? Port { get; set; }
}