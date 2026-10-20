import type { Env } from '../index';

/**
 * Direct-to-R2 upload at the edge — the origin storage service
 * is bypassed entirely for hot-path uploads.
 */
export async function handleUpload(
  request: Request,
  env: Env,
  key: string,
): Promise<Response> {
  const maxBytes = Number(env.MAX_UPLOAD_BYTES ?? 25 * 1024 * 1024);
  const contentLength = Number(request.headers.get('Content-Length') ?? 0);

  if (contentLength > maxBytes) {
    return new Response(JSON.stringify({ error: 'Upload too large' }), {
      status: 413,
      headers: { 'Content-Type': 'application/json' },
    });
  }

  await env.CONTENT_BUCKET.put(key, request.body, {
    httpMetadata: {
      contentType: request.headers.get('Content-Type') ?? 'application/octet-stream',
    },
  });

  return new Response(JSON.stringify({ key }), {
    status: 201,
    headers: { 'Content-Type': 'application/json' },
  });
}
