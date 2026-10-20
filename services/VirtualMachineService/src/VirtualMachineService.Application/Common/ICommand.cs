using MediatR;

namespace VirtualMachineService.Application.Common;

// MediatR marker interfaces — used by pipeline behaviors to
// distinguish commands from queries.
public interface ICommand<out TResponse> : IRequest<TResponse>;
public interface ICommand : IRequest;

public interface IQuery<out TResponse> : IRequest<TResponse>;
