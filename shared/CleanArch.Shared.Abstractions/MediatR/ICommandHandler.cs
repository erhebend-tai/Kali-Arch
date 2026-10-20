namespace CleanArch.Shared.Abstractions.MediatR;

public interface ICommandHandler<in TCommand>
    : global::MediatR.IRequestHandler<TCommand>
    where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse>
    : global::MediatR.IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;
