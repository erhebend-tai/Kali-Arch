import type { Env } from './index';
import { corsHeaders } from './middleware/cors';

/**
 * Edge routing — the worker replaces a traditional API gateway
 * for requests that need global-anycast latency.
 * Path prefixes map to upstream origins.
 */
const ROUTES: Record<string, (env: Env) => string> = {
  '/api/v1/vms': (env) => env.VM_API_ORIGIN,
  '/api/v1/identity': (env) => env.IDENTITY_API_ORIGIN,
  '/api/v1/storage': (env) => env.STORAGE_API_ORIGIN,
};

export async function route(
  request: Request,
  env: Env,
  ctx: ExecutionContext,
): Promise<Response> {
  const url = new URL(request.url);

  if (request.method === 'OPTIONS') {
    return new Response(null, { status: 204, headers: corsHeaders() });
  }

  const prefix = Object.keys(ROUTES).find((p) => url.pathname.startsWith(p));
  if (!prefix) {
    return new Response('Not Found', { status: 404 });
  }

  const upstream = `${ROUTES[prefix](env)}${url.pathname}${url.search}`;

  const response = await fetch(upstream, {
    method: request.method,
    headers: request.headers,
    body: request.body,
    redirect: 'manual',
  });

  const headers = new Headers(response.headers);
  Object.entries(corsHeaders()).forEach(([k, v]) => headers.set(k, v));

  return new Response(response.body, {
    status: response.status,
    headers,
  });
}
