namespace CleanArch.Shared.Abstractions.MediatR;

public interface IQuery<out TResponse> : global::MediatR.IRequest<TResponse>;
