namespace CleanArch.Shared.Abstractions.MediatR;

public interface INotificationHandler<in TNotification>
    : global::MediatR.INotificationHandler<TNotification>
    where TNotification : global::MediatR.INotification;
