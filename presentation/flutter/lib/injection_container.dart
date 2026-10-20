import 'package:get_it/get_it.dart';

/// Service locator — mirrors the DI wiring in each service's Program.cs.
/// Register per feature: datasource → repository → use case → bloc.
final sl = GetIt.instance;

Future<void> initDependencies() async {
  // Example:
  // sl.registerLazySingleton<VmRemoteDatasource>(() => VmRemoteDatasourceImpl(sl()));
  // sl.registerLazySingleton<VirtualMachineRepository>(() => VmRepositoryImpl(sl()));
  // sl.registerLazySingleton(() => GetVmList(sl()));
  // sl.registerFactory(() => VmListBloc(sl()));
}
