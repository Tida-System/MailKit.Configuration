using MailKit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MimeKit;
using TidaSystems.MailKit.Configuration;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});
builder.Services.AddMailKit();

using IHost host = builder.Build();
MimeMessageFactory messageFactory = host.Services.GetRequiredService<MimeMessageFactory>();
SmtpClientFactory clientFactory = host.Services.GetRequiredService<SmtpClientFactory>();

MimeMessage message = messageFactory.CreateMessage("Info");
message.Body = new BodyBuilder
{
    TextBody = "Questa è la versione testuale del messaggio.",
    HtmlBody = "<html><body><h1>Ciao!</h1><p>Questa è la versione <strong>HTML</strong> del messaggio.</p></body></html>"
}.ToMessageBody();

using MailKit.Net.Smtp.SmtpClient client = await clientFactory.CreateSmtpClientAsync();
await client.SendAsync(message);
await client.DisconnectAsync(true);

Console.WriteLine($"Messaggio inviato a {message.To}.");
