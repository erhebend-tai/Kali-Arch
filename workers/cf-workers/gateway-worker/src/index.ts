import { route } from './router';

export interface Env {
  AUTH_WORKER: Fetcher;
  VM_API_ORIGIN: string;
  IDENTITY_API_ORIGIN: string;
  STORAGE_API_ORIGIN: string;
}

export default {
  async fetch(request: Request, env: Env, ctx: ExecutionContext): Promise<Response> {
    return route(request, env, ctx);
  },
} satisfies ExportedHandler<Env>;
