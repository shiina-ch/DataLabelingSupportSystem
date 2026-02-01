import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 20,
  duration: '30s',
  thresholds: {
    http_req_duration: ['p(95)<600'],
  },
};

const BASE_URL = 'http://data-labeling.runasp.net';

export default function () {
  const res = http.get(`${BASE_URL}/api/Project/1`);

  check(res, {
    'PERF-01 status is 200': (r) => r.status === 200,
  });

  sleep(1);
}
