using MediatR;

namespace VirtualMachineService.Application.Common;

public interface ICommandHandler<in TCommand>
    : IRequestHandler<TCommand>
    where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse>
    : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;

public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;

public interface INotificationHandler<in TNotification>
    : MediatR.INotificationHandler<TNotification>
    where TNotification : INotification;
