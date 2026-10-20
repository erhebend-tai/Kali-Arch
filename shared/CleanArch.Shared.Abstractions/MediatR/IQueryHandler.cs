namespace CleanArch.Shared.Abstractions.MediatR;

public interface IQueryHandler<in TQuery, TResponse>
    : global::MediatR.IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;
