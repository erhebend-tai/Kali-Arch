using Microsoft.AspNetCore.SignalR;

namespace CleanArch.API.Hubs;

/// <summary>
/// Real-time VM status push — clients subscribe to group
/// "vms" or a specific vm-{id} group.
/// </summary>
public sealed class VmStatusHub : Hub
{
    public Task SubscribeToVm(string vmId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, $"vm-{vmId}");

    public Task UnsubscribeFromVm(string vmId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, $"vm-{vmId}");

    public Task SubscribeToAll() =>
        Groups.AddToGroupAsync(Context.ConnectionId, "vms");
}
