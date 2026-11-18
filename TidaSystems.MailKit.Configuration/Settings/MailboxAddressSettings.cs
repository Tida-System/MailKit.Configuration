namespace TidaSystems.MailKit.Configuration.Settings;

/// <summary>
/// Defines a mailbox address with an optional display name.
/// </summary>
public sealed class MailboxAddressSettings
{
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string? Address { get; set; }
}