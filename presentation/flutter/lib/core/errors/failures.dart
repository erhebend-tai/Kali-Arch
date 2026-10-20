import 'package:equatable/equatable.dart';

/// Base class for failures returned from the data layer.
/// Use cases return `Either<Failure, T>` — never throw across layers.
abstract class Failure extends Equatable {
  const Failure([this.message = '']);

  final String message;

  @override
  List<Object?> get props => [message];
}

class ServerFailure extends Failure {
  const ServerFailure([super.message]);
}

class CacheFailure extends Failure {
  const CacheFailure([super.message]);
}
