using System.Text.Json;
using System.Text.Json.Serialization;
using CleanArch.Shared.Contracts.Events.Compute;
using CleanArch.Shared.Contracts.Versioning;
using V2Event =
    CleanArch.Shared.Contracts.Events.Compute.V2
        .VmProvisionedIntegrationEvent;
using FluentAssertions;
using Xunit;

namespace CleanArch.ContractTests.Events;

/// <summary>
/// Message contract tests — pin the integration event's
/// serialized shape so producers and consumers never drift.
/// </summary>
public sealed class VmProvisionedEventContractTests
{
    [Fact]
    public void V1_Contract_HasExpectedVersionAttribute()
    {
        var attr = typeof(VmProvisionedIntegrationEvent)
            .GetCustomAttributes(typeof(EventVersionAttribute), false)
            .Cast<EventVersionAttribute>()
            .SingleOrDefault();

        attr.Should().NotBeNull();
        attr!.Topic.Should().Be("vm-provisioned");
        attr.Version.Should().Be(1);
    }

    [Fact]
    public void V1_SerializedShape_IsStable()
    {
        var evt = new VmProvisionedIntegrationEvent(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "web-01", "us-east", "rg-1",
            new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var json = JsonSerializer.Serialize(evt);
        var doc = JsonDocument.Parse(json).RootElement;

        // Contract fields — removing/renaming one breaks consumers.
        doc.TryGetProperty("VmId", out _).Should().BeTrue();
        doc.TryGetProperty("VmName", out _).Should().BeTrue();
        doc.TryGetProperty("Region", out _).Should().BeTrue();
        doc.TryGetProperty("ResourceGroup", out _).Should().BeTrue();
        doc.TryGetProperty("OccurredAt", out _).Should().BeTrue();
    }

    [Fact]
    public void V2_Contract_IsBackwardsCompatibleSuperset()
    {
        var v2Props = typeof(V2Event)
            .GetProperties().Select(p => p.Name).ToHashSet();
        var v1Props = typeof(VmProvisionedIntegrationEvent)
            .GetProperties().Select(p => p.Name);

        // V2 must contain every V1 property — additive only.
        v2Props.Should().Contain(v1Props);
    }
}
