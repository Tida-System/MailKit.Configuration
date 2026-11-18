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
Pop3ClientFactory clientFactory = host.Services.GetRequiredService<Pop3ClientFactory>();

using MailKit.Net.Pop3.Pop3Client client = await clientFactory.CreatePop3ClientAsync();
int firstMessage = Math.Max(0, client.Count - 10);

for (int index = firstMessage; index < client.Count; index++)
{
    MimeMessage message = await client.GetMessageAsync(index);
    Console.WriteLine($"{index + 1}: {message.Date.LocalDateTime:g} | {message.From} | {message.Subject}");
}

await client.DisconnectAsync(true);
