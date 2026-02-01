import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 15,
  duration: '30s',
  thresholds: {
    http_req_duration: ['p(95)<800'],
  },
};

const BASE_URL = 'http://data-labeling.runasp.net';

export default function () {
  const res = http.get(`${BASE_URL}/api/Project/manager/me`);

  check(res, {
    'PERF-02 status is 200': (r) => r.status === 200,
  });

  sleep(1);
}
