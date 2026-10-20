import type { Env } from '../index';

export async function handleList(env: Env, prefix?: string): Promise<Response> {
  const listed = await env.CONTENT_BUCKET.list({ prefix, limit: 1000 });

  return new Response(
    JSON.stringify({
      keys: listed.objects.map((o) => o.key),
      truncated: listed.truncated,
    }),
    { headers: { 'Content-Type': 'application/json' } },
  );
}
