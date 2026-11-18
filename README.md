# TidaSystems.MailKit.Configuration

Configuration-driven factories for [MailKit](https://github.com/jstedfast/MailKit) SMTP, POP3, and IMAP clients, supporting .NET 8 through .NET 11.

Simplify email integration using standard .NET configuration and dependency injection. Create connected and authenticated MailKit clients, manage named sender profiles, and build MIME messages without repeating connection and configuration code.

**Full documentation:** [TidaSystems MailKit.Configuration Documentation](https://tida.website/documentation/mailkit-configuration)

Developed and maintained by [TidaSystems](https://tida.website).

## Features

- SMTP, POP3, and IMAP client factories
- Username/password and OAuth 2.0 authentication
- Named sender profiles with configurable message defaults
- TLS, client certificates, proxies, and protocol logging
- Automatic configuration reload
- Built-in dependency injection support
- Native AOT support

## Requirements

- .NET 8 or later
- An SMTP, POP3, or IMAP server, depending on the protocols used

## Installation

Install the package from NuGet:

```bash
dotnet add package TidaSystems.MailKit.Configuration
```

## Registration

Register the services in `Program.cs`:

```csharp
using TidaSystems.MailKit.Configuration;

builder.Services.AddMailKit();
```

This registers the following services for dependency injection:

- `SmtpClientFactory`
- `Pop3ClientFactory`
- `ImapClientFactory`
- `MimeMessageFactory`

Each client factory creates a new connected client and authenticates it when credentials are configured. The caller is responsible for disposing the client.

## Configuration

Add a `MailKit` section to `appsettings.json`:

```json
{
  "MailKit": {
    "Username": "user@example.com",
    "Timeout": 30000,
    "CheckCertificateRevocation": true,
    "Smtp": {
      "Host": "smtp.example.com",
      "Port": 587,
      "SecureSocketOptions": "StartTls"
    },
    "Pop3": {
      "Host": "pop.example.com",
      "Port": 995,
      "SecureSocketOptions": "SslOnConnect"
    },
    "Imap": {
      "Host": "imap.example.com",
      "Port": 993,
      "SecureSocketOptions": "SslOnConnect"
    },
    "Senders": {
      "Info": {
        "From": {
          "Name": "Example Info",
          "Address": "info@example.com"
        },
        "ReplyTo": [
          {
            "Name": "Support",
            "Address": "support@example.com"
          }
        ],
        "Bcc": [
          {
            "Address": "archive@example.com"
          }
        ]
      },
      "Billing": {
        "From": {
          "Name": "Billing Department",
          "Address": "billing@example.com"
        }
      }
    }
  }
}
```

All protocol sections and sender profiles are optional. Configure only what your application needs.

### Credentials

Avoid storing passwords and access tokens in source control. Use environment variables, .NET User Secrets, or another secure configuration provider.

For example:

```powershell
$env:MailKit__Username = "user@example.com"
$env:MailKit__Password = "your-app-password"
```

For OAuth 2.0 authentication, configure `MailKit__OAuthToken` instead of `MailKit__Password`.

Shared authentication settings apply to all configured protocols.

## Sending email

Use `MimeMessageFactory` to create a message from a named sender profile:

```csharp
MimeMessage message = messageFactory.CreateMessage("Info");
message.To.Add(MailboxAddress.Parse("recipient@example.com"));
message.Subject = "Hello!";

BodyBuilder body = new()
{
    TextBody = "Hello from TidaSystems!",
    HtmlBody = "<h1>Hello!</h1><p>Welcome to TidaSystems.</p>"
};

message.Body = body.ToMessageBody();

await using SmtpClient client = await smtpFactory.CreateSmtpClientAsync(ct);
await client.SendAsync(message, ct);
await client.DisconnectAsync(true, ct);
```

The `"Info"` profile automatically applies its configured defaults, such as `From`, `Sender`, `ReplyTo`, `To`, `Cc`, `Bcc`, `Subject`, `Importance`, `Priority`, and `XPriority`.

You can modify any of these properties on the returned `MimeMessage` before sending.

### Named sender profiles

Define as many sender profiles as needed:

```csharp
MimeMessage infoMessage = messageFactory.CreateMessage("Info");
MimeMessage billingMessage = messageFactory.CreateMessage("Billing");
```

Each call creates a separate `MimeMessage` instance.

Sender profiles are optional, and you can also construct `MimeMessage` instances directly using MimeKit.

## Reading email with POP3

```csharp
await using Pop3Client client = await pop3Factory.CreatePop3ClientAsync(ct);

for (int index = 0; index < client.Count; index++)
{
    MimeMessage message = await client.GetMessageAsync(index, ct);
    Console.WriteLine($"{message.From}: {message.Subject}");
}

await client.DisconnectAsync(true, ct);
```

## Reading email with IMAP

```csharp
await using ImapClient client = await imapFactory.CreateImapClientAsync(ct);

IMailFolder inbox = client.Inbox;
await inbox.OpenAsync(FolderAccess.ReadOnly, ct);

if (inbox.Count > 0)
{
    IList<IMessageSummary> messages = await inbox.FetchAsync(0, inbox.Count - 1, MessageSummaryItems.Envelope | MessageSummaryItems.Flags, ct);
    foreach (IMessageSummary message in messages)
    {
        Console.WriteLine(message.Envelope?.Subject);
    }
}

await client.DisconnectAsync(true, ct);
```

## Optional configuration

The `MailKit` section also supports the following shared settings:

| Setting                    | Description                                     |
| -------------------------- | ----------------------------------------------- |
| `AuthenticationMechanisms` | Restrict authentication mechanisms              |
| `LocalEndPoint`            | Specify a local IP address and port             |
| `SslProtocols`             | Configure allowed TLS protocols                 |
| `SslCipherSuitesPolicy`    | Configure permitted TLS cipher suites           |
| `ClientCertificates`       | Configure client certificate files              |
| `ProxyClient`              | Configure HTTP, HTTPS, SOCKS, or system proxies |
| `ProtocolLogPath`          | Enable MailKit protocol logging                 |

The SMTP section additionally supports `LocalDomain` and `RequireTLS`.

TLS cipher suite policies are not applied on Windows or Android because of platform limitations.

**Security:** Protocol logs may contain sensitive information, including message content. Protect log files and avoid committing them to source control.

## Configuration reload

Configuration changes are automatically detected when the underlying .NET configuration provider supports reload notifications.

Newly created clients and messages use the updated settings. Existing client connections are not modified.

If configuration binding fails during a reload, the previous successfully loaded settings are preserved.

## Native AOT

The library is designed for Native AOT and uses the .NET configuration binding source generator.

It can be used in applications published with Native AOT, subject to the application's other dependencies and platform requirements.

## Samples

Complete usage examples are available in the [samples](samples/README.md) directory:

- [SendEmail](samples/SendEmail) — Send HTML and plain-text email using a sender profile.
- [ReadPop3](samples/ReadPop3) — Retrieve messages from a POP3 mailbox.
- [ReadImapInbox](samples/ReadImapInbox) — Read IMAP Inbox messages.

## Dependencies

Built on [MailKit](https://github.com/jstedfast/MailKit) and [MimeKit](https://github.com/jstedfast/MimeKit), using standard Microsoft.Extensions configuration and dependency injection.

## License

This project is licensed under the [MIT License](LICENSE).
