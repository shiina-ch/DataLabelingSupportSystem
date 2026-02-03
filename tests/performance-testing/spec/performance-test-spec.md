# Performance Test Specification
Project: Data Labeling Support System  
Target: Public API - http://data-labeling.runasp.net

---

## PERF-01 — GET Project by ID

API: GET /api/Project/{id}  
Sample: /api/Project/1  

Purpose:  
Đo hiệu năng truy vấn chi tiết một project cụ thể (baseline)

Load:
- 20 Virtual Users
- 30 seconds

Metric:
- p95 response time

Threshold:
- p95 < 600 ms

Expected:
- HTTP 200

---

## PERF-02 — GET Projects of Manager

API: GET /api/Project/manager/me  

Purpose:  
API trả về danh sách project

Load:
- 15 Virtual Users
- 30 seconds

Metric:
- avg + p95

Threshold:
- p95 < 800 ms

Expected:
- HTTP 200

---

## PERF-03 — GET Project Statistics

API: GET /api/Project/{id}/stats  
Sample: /api/Project/1/stats  

Purpose:  
API trả về trang thống kê

Load:
- 10 Virtual Users
- 30 seconds

Metric:
- p95

Threshold:
- p95 < 1000 ms

Expected:
- HTTP 200
