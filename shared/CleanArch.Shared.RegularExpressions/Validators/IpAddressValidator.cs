namespace CleanArch.Shared.RegularExpressions.Validators;

public static class IpAddressValidator
{
    public static bool IsValidIPv4(string value) =>
        Patterns.IPv4Address().IsMatch(value);

    public static bool IsValidIPv6(string value) =>
        Patterns.IPv6Address().IsMatch(value);

    public static bool IsValidCidr(string value) =>
        Patterns.CidrBlock().IsMatch(value);
}
