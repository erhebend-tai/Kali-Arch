import type { Env } from '../index';

/**
 * Workers AI inference — models run in-region, no origin call.
 */
export async function runInference(
  env: Env,
  model: string,
  prompt: string,
): Promise<Response> {
  const result = await env.AI.run(model as keyof AiModels, {
    messages: [{ role: 'user', content: prompt }],
  } as AiTextGenerationInput);

  return new Response(JSON.stringify(result), {
    headers: { 'Content-Type': 'application/json' },
  });
}
