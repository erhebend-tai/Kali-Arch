namespace CleanArch.Application.Interfaces.Sms;

public interface ISmsService
{
    Task SendAsync(string to, string body, CancellationToken ct = default);
}
