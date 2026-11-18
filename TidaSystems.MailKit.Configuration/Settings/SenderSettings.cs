using MimeKit;

namespace TidaSystems.MailKit.Configuration.Settings;

/// <summary>
/// Defines the default message properties for a named sender profile.
/// </summary>
public sealed class SenderSettings
{
    /// <summary>
    /// Gets or sets the default From mailbox.
    /// </summary>
    public MailboxAddressSettings? From { get; set; }

    /// <summary>
    /// Gets or sets the default Sender mailbox.
    /// </summary>
    public MailboxAddressSettings? Sender { get; set; }

    /// <summary>
    /// Gets or sets the default Reply-To mailboxes.
    /// </summary>
    public MailboxAddressSettings[]? ReplyTo { get; set; }

    /// <summary>
    /// Gets or sets the default recipients.
    /// </summary>
    public MailboxAddressSettings[]? To { get; set; }

    /// <summary>
    /// Gets or sets the default carbon-copy recipients.
    /// </summary>
    public MailboxAddressSettings[]? Cc { get; set; }

    /// <summary>
    /// Gets or sets the default blind carbon-copy recipients.
    /// </summary>
    public MailboxAddressSettings[]? Bcc { get; set; }

    /// <summary>
    /// Gets or sets the default message subject.
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Gets or sets the default message importance.
    /// </summary>
    public MessageImportance? Importance { get; set; }

    /// <summary>
    /// Gets or sets the default message priority.
    /// </summary>
    public MessagePriority? Priority { get; set; }

    /// <summary>
    /// Gets or sets the default X-Priority header value.
    /// </summary>
    public XMessagePriority? XPriority { get; set; }
}