import { verifyJwt } from './middleware/jwt';

export interface Env {
  JWKS_URL: string;
  ISSUER: string;
  AUDIENCE: string;
}

export default {
  async fetch(request: Request, env: Env): Promise<Response> {
    const authHeader = request.headers.get('Authorization');
    if (!authHeader?.startsWith('Bearer ')) {
      return new Response(JSON.stringify({ error: 'Missing bearer token' }), {
        status: 401,
        headers: { 'Content-Type': 'application/json' },
      });
    }

    const token = authHeader.slice(7);
    const claims = await verifyJwt(token, env).catch(() => null);

    if (!claims) {
      return new Response(JSON.stringify({ error: 'Invalid token' }), {
        status: 401,
        headers: { 'Content-Type': 'application/json' },
      });
    }

    // Forward verified identity downstream via headers.
    return new Response(JSON.stringify({ sub: claims.sub, roles: claims.roles ?? [] }), {
      headers: { 'Content-Type': 'application/json' },
    });
  },
} satisfies ExportedHandler<Env>;
