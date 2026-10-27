import os, re

services_dir = r"d:\KLTN\KLTN\Improve\Mobile\freelancerstudent\lib\services"
files = [f for f in os.listdir(services_dir) if f.endswith(".dart")]

print(f"Total service files: {len(files)}")
service_analysis = {}

for f in files:
    path = os.path.join(services_dir, f)
    with open(path, "r", encoding="utf-8") as file:
        content = file.read()
    
    # find api calls like _apiClient.get, _apiClient.post, etc. or endpoints
    methods = re.findall(r'(Future<[^>]+>\s+([a-zA-Z0-9_]+)\s*\([^)]*\))', content)
    endpoints = re.findall(r'ApiEndpoints\.([a-zA-Z0-9_]+)', content)
    raw_urls = re.findall(r'[\'"](/api/[^\'"]+)[\'"]', content)
    
    service_analysis[f] = {
        "methods": [m[1] for m in methods],
        "endpoints_used": list(set(endpoints)),
        "raw_urls": list(set(raw_urls))
    }

for k, v in service_analysis.items():
    print(f"\n=== {k} ===")
    print(f"Methods ({len(v['methods'])}): {', '.join(v['methods'])}")
    print(f"Endpoints constant: {', '.join(v['endpoints_used'])}")
    if v['raw_urls']:
        print(f"Raw URLs: {', '.join(v['raw_urls'])}")
