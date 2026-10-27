import os, re

# 1. Parse diem_cuoi_api.dart
api_endpoints_path = r"d:\KLTN\KLTN\Improve\Mobile\freelancerstudent\lib\core\constants\diem_cuoi_api.dart"
with open(api_endpoints_path, "r", encoding="utf-8") as f:
    endpoint_lines = f.readlines()

mobile_endpoints = {}
for line in endpoint_lines:
    m = re.search(r'static const String ([a-zA-Z0-9_]+)\s*=\s*[\'"]?(\$baseUrl/[^\'"]+)[\'"]?', line)
    if m:
        name = m.group(1)
        url_expr = m.group(2).replace("$baseUrl/", "api/")
        mobile_endpoints[name] = url_expr
    m_hub = re.search(r'static const String ([a-zA-Z0-9_]+)\s*=\s*[\'"]?(\$apiBaseUrl/[^\'"]+)[\'"]?', line)
    if m_hub:
        name = m_hub.group(1)
        url_expr = m_hub.group(2).replace("$apiBaseUrl/", "")
        mobile_endpoints[name] = url_expr

print(f"Mobile Endpoints defined: {len(mobile_endpoints)}")

# 2. Parse FreelancerStudent.API Controllers
controllers_dir = r"d:\KLTN\KLTN\Improve\FreelancerStudent.API\Controllers"
api_routes = []

for cfile in os.listdir(controllers_dir):
    if not cfile.endswith(".cs"): continue
    cpath = os.path.join(controllers_dir, cfile)
    with open(cpath, "r", encoding="utf-8") as f:
        ccontent = f.read()
    
    # Class route
    class_route_match = re.search(r'\[Route\("([^"]+)"\)\]', ccontent)
    base_c_route = class_route_match.group(1) if class_route_match else "api/[controller]"
    controller_name = cfile.replace("Controller.cs", "")
    base_c_route = base_c_route.replace("[controller]", controller_name)

    # Actions
    # match [HttpGet], [HttpPost], etc. with optional route
    action_matches = re.findall(r'\[(HttpGet|HttpPost|HttpPut|HttpDelete|HttpPatch)(?:\("([^"]*)"\))?\]\s*(?:\[[^\]]+\]\s*)*public\s+(?:async\s+)?(?:Task<)?([^\s>]+)>?\s+([a-zA-Z0-9_]+)\s*\(([^)]*)\)', ccontent)
    
    for http_m, route_param, ret_type, act_name, params in action_matches:
        full_route = base_c_route
        if route_param:
            if route_param.startswith("/"):
                full_route = route_param.lstrip("/")
            else:
                full_route = f"{base_c_route.rstrip('/')}/{route_param.lstrip('/')}"
        
        api_routes.append({
            "controller": controller_name,
            "action": act_name,
            "method": http_m.replace("Http", "").upper(),
            "route": full_route,
            "params": params.strip()
        })

print(f"API Actual Actions found: {len(api_routes)}")

# Save to file
out_path = r"d:\KLTN\KLTN\Improve\scratch\api_vs_mobile_audit.txt"
with open(out_path, "w", encoding="utf-8") as out:
    out.write("=== MOBILE ENDPOINTS DEFINED IN diem_cuoi_api.dart ===\n")
    for k, v in mobile_endpoints.items():
        # find matching API route
        # exact or prefix
        v_clean = v.split("?")[0].rstrip("/")
        matches = [r for r in api_routes if r['route'].lower() == v_clean.lower() or r['route'].lower().startswith(v_clean.lower() + "/")]
        status = "MATCH" if matches else "NOT_FOUND_OR_MISMATCH"
        out.write(f"[{status}] {k}: {v}\n")
        if matches:
            for m in matches:
                out.write(f"      -> {m['method']} {m['route']} ({m['controller']}.{m['action']})\n")
        else:
            # check partial match
            partial = [r for r in api_routes if v_clean.split("/")[-1].lower() in r['route'].lower()]
            if partial:
                out.write(f"      (Did you mean?)\n")
                for p in partial[:3]:
                    out.write(f"      ? {p['method']} {p['route']} ({p['controller']}.{p['action']})\n")
    
    out.write("\n\n=== ALL BACKEND API ENDPOINTS ===\n")
    for r in api_routes:
        out.write(f"{r['method']:6} {r['route']} -> {r['controller']}.{r['action']}({r['params']})\n")

print(f"Audit written to {out_path}")
