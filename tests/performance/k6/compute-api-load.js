// k6 load test — run: k6 run tests/performance/k6/compute-api-load.js
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';

const errorRate = new Rate('errors');
const provisionDuration = new Trend('provision_duration_ms');

export const options = {
  stages: [
    { duration: '30s', target: 20 },   // ramp up
    { duration: '1m',  target: 50 },   // sustained
    { duration: '30s', target: 0 },    // ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<500', 'p(99)<1000'],
    errors: ['rate<0.01'],
  },
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5000';

export default function () {
  // Read path — most traffic.
  const listRes = http.get(`${BASE_URL}/api/compute/vms`);
  check(listRes, {
    'list 200': (r) => r.status === 200,
  }) || errorRate.add(1);

  // Write path — 1 in 10 iterations.
  if (__ITER % 10 === 0) {
    const payload = JSON.stringify({
      name: `load-vm-${__VU}-${__ITER}`,
      resourceGroup: 'load-test',
      region: 'us-east',
      osType: 'Linux',
      size: 'small',
    });

    const res = http.post(`${BASE_URL}/api/compute/vms`, payload, {
      headers: { 'Content-Type': 'application/json' },
    });

    provisionDuration.add(res.timings.duration);
    check(res, {
      'provision 201 or 400': (r) => r.status === 201 || r.status === 400,
    }) || errorRate.add(1);
  }

  sleep(0.5);
}
