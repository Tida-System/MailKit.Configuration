using MimeKit;
using TidaSystems.MailKit.Configuration.Helpers;
using TidaSystems.MailKit.Configuration.Settings;

namespace TidaSystems.MailKit.Configuration;

/// <summary>
/// Creates MIME messages using named sender profiles from MailKit configuration.
/// </summary>
public sealed class MimeMessageFactory
{
    private readonly MailKitConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MimeMessageFactory"/> class.
    /// </summary>
    /// <param name="configuration">The MailKit configuration provider.</param>
    public MimeMessageFactory(MailKitConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Creates a new MIME message with the defaults configured for the specified sender profile.
    /// </summary>
    /// <param name="name">The name of the sender profile.</param>
    /// <returns>A new MIME message with the configured headers and addresses.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the sender profile is not configured.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a configured mailbox address is missing.</exception>
    public MimeMessage CreateMessage(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        MailServiceSettings? settings = configuration.Current;
        if (settings?.Senders is null || !settings.Senders.TryGetValue(name, out SenderSettings? sender) || sender is null)
        {
            throw new KeyNotFoundException($"MailKit sender profile '{name}' was not found in 'MailKit:Senders'.");
        }

        if (sender.From is null)
        {
            throw new InvalidOperationException($"MailKit sender profile '{name}' must define 'From'.");
        }

        MimeMessage message = new();
        message.From.Add(CreateMailbox(sender.From));
        if (sender.Sender is not null)
        {
            message.Sender = CreateMailbox(sender.Sender);
        }

        AddAddresses(message.ReplyTo, sender.ReplyTo);
        AddAddresses(message.To, sender.To);
        AddAddresses(message.Cc, sender.Cc);
        AddAddresses(message.Bcc, sender.Bcc);
        if (sender.Subject is not null)
        {
            message.Subject = sender.Subject;
        }

        if (sender.Importance.HasValue)
        {
            message.Importance = sender.Importance.Value;
        }

        if (sender.Priority.HasValue)
        {
            message.Priority = sender.Priority.Value;
        }

        if (sender.XPriority.HasValue)
        {
            message.XPriority = sender.XPriority.Value;
        }

        return message;
    }

    private static MailboxAddress CreateMailbox(MailboxAddressSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Address))
        {
            throw new InvalidOperationException("A mailbox configured in 'MailKit:Senders' has no email address.");
        }

        return new MailboxAddress(settings.Name, settings.Address);
    }

    private static void AddAddresses(InternetAddressList destination, MailboxAddressSettings[]? addresses)
    {
        if (addresses is null)
        {
            return;
        }

        foreach (MailboxAddressSettings address in addresses)
        {
            destination.Add(CreateMailbox(address));
        }
    }
}