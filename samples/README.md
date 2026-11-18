# .NET 10 Samples

These sample applications demonstrate how to use **TidaSystems.MailKit.Configuration** with SMTP, POP3, and IMAP.

All samples use the library's public factories and target .NET 10. They reference the library directly through `ProjectReference` and are configured for Native AOT publishing.

## Available Samples

| Sample                         | Description                                                                  |
| ------------------------------ | ---------------------------------------------------------------------------- |
| [SendEmail](SendEmail)         | Send an email with HTML and plain-text content using a named sender profile. |
| [ReadPop3](ReadPop3)           | Retrieve the latest 10 messages from a POP3 mailbox.                         |
| [ReadImapInbox](ReadImapInbox) | Read the latest 10 messages from an IMAP Inbox in read-only mode.            |

## Configuration

Each sample includes an `appsettings.json` containing example server settings.

Update the server hostnames and other non-sensitive settings for your email provider.

**Do not store passwords or access tokens in the repository.** Use environment variables instead:

```powershell
$env:MailKit__Username = "user@example.com"
$env:MailKit__Password = "your-app-password"
```

For OAuth 2.0 authentication, set `MailKit__OAuthToken` instead of the password.

Environment variables override values from `appsettings.json` when using the standard .NET configuration providers in the samples.

For the SMTP example, also update the `MailKit:Senders:Info` profile in `SendEmail/appsettings.json` with your sender address.

## Running the Samples

From the repository root, run the example you need:

```powershell
dotnet run --project samples/SendEmail
dotnet run --project samples/ReadPop3
dotnet run --project samples/ReadImapInbox
```

The **SendEmail** sample uses `MimeMessageFactory` to initialize a message from the `Info` sender profile and MimeKit's `BodyBuilder` to create HTML and plain-text content.

The **ReadPop3** and **ReadImapInbox** samples retrieve up to 10 messages. IMAP opens the Inbox in read-only mode.

## Build and Native AOT

Build the solution:

```powershell
dotnet build TidaSystems.MailKit.Configuration.slnx -c Release
```

To publish individual samples with Native AOT:

```powershell
dotnet publish samples/SendEmail -c Release -r win-x64
dotnet publish samples/ReadPop3 -c Release -r win-x64
dotnet publish samples/ReadImapInbox -c Release -r win-x64
```

Replace `win-x64` with the runtime identifier appropriate for your target platform.

## Documentation

For installation instructions, configuration options, and general usage, see the [main README](../README.md).

For complete configuration references, advanced examples, and troubleshooting, visit the [TidaSystems MailKit.Configuration documentation](https://tida.website/documentation/mailkit-configuration).

Developed by [TidaSystems](https://tida.website).