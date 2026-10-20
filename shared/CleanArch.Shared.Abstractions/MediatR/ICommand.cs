namespace CleanArch.Shared.Abstractions.MediatR;

// MediatR marker interfaces - used by behaviors (transaction,
// validation, caching) to distinguish commands from queries.
public interface ICommand<out TResponse> : global::MediatR.IRequest<TResponse>;

public interface ICommand : global::MediatR.IRequest;
