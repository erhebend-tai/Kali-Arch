import { handleUpload } from './handlers/upload';
import { handleList } from './handlers/list';

export interface Env {
  CONTENT_BUCKET: R2Bucket;
  MAX_UPLOAD_BYTES: string;
}

export default {
  async fetch(request: Request, env: Env): Promise<Response> {
    const url = new URL(request.url);

    if (request.method === 'PUT' && url.pathname.startsWith('/upload/')) {
      const key = url.pathname.slice('/upload/'.length);
      return handleUpload(request, env, key);
    }

    if (request.method === 'GET' && url.pathname === '/list') {
      return handleList(env, url.searchParams.get('prefix') ?? undefined);
    }

    return new Response('Not Found', { status: 404 });
  },
} satisfies ExportedHandler<Env>;
