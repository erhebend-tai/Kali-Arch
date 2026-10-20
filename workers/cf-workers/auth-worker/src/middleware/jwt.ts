import type { Env } from '../index';

export interface JwtClaims {
  sub: string;
  roles?: string[];
  exp?: number;
  iss?: string;
  aud?: string | string[];
}

/** JWK as served by a JWKS endpoint — adds `kid` to the WebCrypto shape. */
interface Jwk extends JsonWebKey {
  kid?: string;
}

let cachedJwks: { keys: Jwk[]; fetchedAt: number } | null = null;

async function getJwks(env: Env): Promise<Jwk[]> {
  if (cachedJwks && Date.now() - cachedJwks.fetchedAt < 5 * 60 * 1000) {
    return cachedJwks.keys;
  }
  const response = await fetch(env.JWKS_URL);
  const jwks = (await response.json()) as { keys: Jwk[] };
  cachedJwks = { keys: jwks.keys, fetchedAt: Date.now() };
  return jwks.keys;
}

/**
 * RS256 JWT verification with JWKS — the edge does authN so
 * origin services see only pre-validated requests.
 */
export async function verifyJwt(token: string, env: Env): Promise<JwtClaims> {
  const [headerB64, payloadB64, signatureB64] = token.split('.');
  if (!headerB64 || !payloadB64 || !signatureB64) {
    throw new Error('Malformed token');
  }

  const header = JSON.parse(atob(headerB64.replace(/-/g, '+').replace(/_/g, '/')));
  const claims = JSON.parse(
    atob(payloadB64.replace(/-/g, '+').replace(/_/g, '/')),
  ) as JwtClaims;

  const keys = await getJwks(env);
  const jwk = keys.find((k) => k.kid === header.kid) ?? keys[0];
  if (!jwk) throw new Error('No signing key');

  const cryptoKey = await crypto.subtle.importKey(
    'jwk',
    { ...jwk, ext: true } as JsonWebKey,
    { name: 'RSASSA-PKCS1-v1_5', hash: 'SHA-256' },
    false,
    ['verify'],
  );

  const data = new TextEncoder().encode(`${headerB64}.${payloadB64}`);
  const signature = Uint8Array.from(
    atob(signatureB64.replace(/-/g, '+').replace(/_/g, '/')),
    (c) => c.charCodeAt(0),
  );

  const valid = await crypto.subtle.verify(
    'RSASSA-PKCS1-v1_5',
    cryptoKey,
    signature,
    data,
  );
  if (!valid) throw new Error('Bad signature');

  if (claims.exp && claims.exp * 1000 < Date.now()) {
    throw new Error('Token expired');
  }
  if (claims.iss !== env.ISSUER) throw new Error('Bad issuer');

  return claims;
}
