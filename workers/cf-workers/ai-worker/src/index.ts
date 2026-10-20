import { runInference } from './handlers/inference';

export interface Env {
  AI: Ai;
}

export default {
  async fetch(request: Request, env: Env): Promise<Response> {
    if (request.method !== 'POST') {
      return new Response('Method Not Allowed', { status: 405 });
    }

    const { model, prompt } = (await request.json()) as {
      model?: string;
      prompt?: string;
    };
    if (!prompt) {
      return new Response(JSON.stringify({ error: 'prompt required' }), {
        status: 400,
        headers: { 'Content-Type': 'application/json' },
      });
    }

    return runInference(env, model ?? '@cf/meta/llama-3.1-8b-instruct', prompt);
  },
} satisfies ExportedHandler<Env>;
