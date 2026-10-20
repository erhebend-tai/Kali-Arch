namespace CleanArch.Shared.Extensions;

public static class StreamExtensions
{
    public static async Task<byte[]> ToByteArrayAsync(
        this Stream stream, CancellationToken ct = default)
    {
        if (stream is MemoryStream ms && ms.Position == 0)
            return ms.ToArray();

        using var copy = new MemoryStream();
        await stream.CopyToAsync(copy, ct);
        return copy.ToArray();
    }

    public static MemoryStream ToMemoryStream(this byte[] bytes) =>
        new(bytes, writable: false);
}
