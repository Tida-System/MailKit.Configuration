
using MailKit;
using System.Globalization;
using System.Net;
using System.Security.Authentication;

namespace TidaSystems.MailKit.Configuration;

public class MailServiceSettings
{
    public int? Timeout { get; init; }
    public IPEndPoint? LocalEndPoint { get; init; }
    public bool? CheckCertificateRevocation { get; init; }
    public SslProtocols? SslProtocols { get; init; }

    public void Apply(IMailService client)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (Timeout.HasValue)
        {
            client.Timeout = Timeout.Value;
        }

        if (LocalEndPoint is not null)
        {
            client.LocalEndPoint = new IPEndPoint(LocalEndPoint.Address, LocalEndPoint.Port);
        }

        if (CheckCertificateRevocation.HasValue)
        {
            client.CheckCertificateRevocation = CheckCertificateRevocation.Value;
        }

        if (SslProtocols.HasValue)
        {
            client.SslProtocols = SslProtocols.Value;
        }
    }
}