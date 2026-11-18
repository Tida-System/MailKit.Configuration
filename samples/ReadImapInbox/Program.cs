using MailKit;
using MimeKit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TidaSystems.MailKit.Configuration;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});
builder.Services.AddMailKit();

using IHost host = builder.Build();
ImapClientFactory clientFactory = host.Services.GetRequiredService<ImapClientFactory>();

using MailKit.Net.Imap.ImapClient client = await clientFactory.CreateImapClientAsync();
IMailFolder inbox = client.Inbox;
await inbox.OpenAsync(FolderAccess.ReadOnly);

int firstMessage = Math.Max(0, inbox.Count - 10);
if (firstMessage < inbox.Count)
{
    IList<IMessageSummary> messages = await inbox.FetchAsync(
        firstMessage,
        inbox.Count - 1,
        MessageSummaryItems.Envelope | MessageSummaryItems.Flags);

    foreach (IMessageSummary message in messages)
    {
        Envelope? envelope = message.Envelope;
        string unread = message.Flags.HasValue && !message.Flags.Value.HasFlag(MessageFlags.Seen) ? " [non letto]" : string.Empty;
        Console.WriteLine($"{message.Index + 1}: {envelope?.Date?.LocalDateTime:g} | {envelope?.From} | {envelope?.Subject}{unread}");
    }
}

await client.DisconnectAsync(true);
