using CleanArch.Application.Interfaces.Sms;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CleanArch.Infrastructure.Common.Integrations.Twilio;

public sealed class TwilioSmsService(
    string accountSid, string authToken, string fromNumber)
    : ISmsService
{
    public async Task SendAsync(
        string to, string body, CancellationToken ct = default)
    {
        TwilioClient.Init(accountSid, authToken);

        await MessageResource.CreateAsync(
            to: new global::Twilio.Types.PhoneNumber(to),
            from: new global::Twilio.Types.PhoneNumber(fromNumber),
            body: body);
    }
}
