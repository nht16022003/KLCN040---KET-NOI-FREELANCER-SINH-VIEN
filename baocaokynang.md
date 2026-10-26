# GIẢI THÍCH CHI TIẾT: CÁCH CODE HOẠT ĐỘNG TỪNG BƯỚC (CODE EXECUTION FLOW)

Tài liệu này giải thích **chính xác từng dòng code, từng hàm và luồng dữ liệu** chạy qua các file trong dự án từ giao diện Web/Mobile xuống API Backend và Database SQL Server.

---

# PHẦN 1: LUỒNG CODE GỢI Ý KỸ NĂNG KHI NHẬP TIÊU ĐỀ

Mục tiêu: Khi Nhà tuyển dụng gõ tiêu đề (ví dụ: *"Tuyển Freelancer thiết kế logo quán cà phê"*), code sẽ tự động suy ra các kỹ năng cần thiết (`Photoshop`, `Illustrator`, `Canva`, `Thiết kế logo`) và hiển thị thành các nút bấm.

---

### BƯỚC 1.1: GIAO DIỆN BẮT SỰ KIỆN GÕ PHÍM & DEBOUNCE

#### Phía Web (`FreelancerStudent.Web/Views/JobPost/Create.cshtml`)
Code Javascript lắng nghe ô nhập tiêu đề:
```javascript
// 1. Lấy thẻ input tiêu đề
const titleInput = document.querySelector('input[name="tieude"]');

// 2. Lắng nghe mỗi khi người dùng gõ phím
titleInput.addEventListener("input", function () {
    // Xóa timer cũ nếu người dùng vẫn đang gõ liên tục
    clearTimeout(debounceTimer);

    // Đặt timer mới: chờ đúng 350 mili-giây sau khi người dùng ngừng gõ mới chạy hàm gọi API
    debounceTimer = setTimeout(fetchSkillSuggestions, 350);
});

async function fetchSkillSuggestions() {
    const tieuDe = titleInput.value.trim();
    // Gửi request HTTP GET sang Web Controller
    const res = await fetch(`/JobPost/GoiYKyNang?tieuDe=${encodeURIComponent(tieuDe)}`);
    const json = await res.json();
    // Khi có dữ liệu trả về, xóa danh sách cũ và tạo các nút bấm <button class="skill-btn"> mới
    ...
}
```

#### Phía Mobile (`Mobile/freelancerstudent/lib/views/employer/man_hinh_tao_bai_dang_tuyen_dung.dart`)
Code Flutter lắng nghe sự kiện thay đổi của `_titleController`:
```dart
@override
void initState() {
  super.initState();
  // Gắn hàm lắng nghe vào controller của ô tiêu đề
  _titleController.addListener(_onTitleChanged);
}

void _onTitleChanged() {
  // Hủy Timer cũ nếu đang gõ tiếp
  _debounceTimer?.cancel();
  final text = _titleController.text.trim();
  if (text.isEmpty) return;

  // Chờ 400ms sau khi ngừng bấm phím mới gọi API
  _debounceTimer = Timer(const Duration(milliseconds: 400), () async {
    setState(() => _isLoadingSuggestions = true); // Hiện xoay tròn loading

    // Gọi hàm qua service
    final res = await _jobService.layGoiYKyNangTheoTieuDe(text);

    if (res.isSuccess && res.data != null) {
      setState(() {
        _suggestedSkills.clear();
        // Nạp danh sách kỹ năng mới từ API vào danh sách hiển thị
        _suggestedSkills.addAll(res.data!.map((item) => item['tenKyNang'].toString()));
        _isLoadingSuggestions = false;
      });
    }
  });
}
```

---

### BƯỚC 1.2: API TIẾP NHẬN YÊU CẦU (`JobPostController.cs`)

Request đến endpoint tại file `FreelancerStudent.API/Controllers/JobPostController.cs`:
```csharp
[HttpGet("GoiYKyNangTheoTieuDe")]
public async Task<IActionResult> GoiYKyNangTheoTieuDe([FromQuery] string? tieuDe, [FromQuery] string? moTa = null)
{
    // Nhận chuỗi tieuDe = "Tuyển Freelancer thiết kế logo quán cà phê"
    // Chuyển việc xử lý cho KyNangGoiYService
    var data = await _kyNangGoiYService.GoiYKyNangTheoVanBanAsync(tieuDe, moTa);

    // Trả về JSON thành công kèm danh sách kỹ năng tìm được
    return Ok(new
    {
        success = true,
        message = "Gợi ý kỹ năng thành công!",
        data = data
    });
}
```

---

### BƯỚC 1.3: CODE XỬ LÝ TỪ KHÓA & TRUY VẤN SQL (`KyNangGoiYService.cs`)

Hàm `GoiYKyNangTheoVanBanAsync` trong `FreelancerStudent.API/Services/KyNangGoiYService.cs` thực hiện lần lượt các bước:

#### 1. Chuẩn hóa chuỗi văn bản:
```csharp
// Chuyển chuỗi "Tuyển Freelancer thiết kế logo" thành "tuyen freelancer thiet ke logo"
var tieuDeKd = ChuanHoaKhongDau(tieuDe ?? string.Empty);
var moTaKd = ChuanHoaKhongDau(moTa ?? string.Empty);
```
Hàm `ChuanHoaKhongDau` dùng Regex quét qua toàn bộ ký tự Unicode tiếng Việt có dấu (`á, à, ả, ã, ạ,...`) chuyển thành ký tự Latin không dấu thường.

#### 2. Quét qua bộ từ điển quy tắc (Dictionary):
Trong code có định nghĩa sẵn danh sách các quy tắc `DanhSachQuyTac`:
```csharp
new QuyTacAnhXa
{
    NhomNganh = "DoHoa_UIUX",
    TuKhoas = new[] { "logo", "banner", "poster", "nhan dien thuong hieu", "figma", "photoshop", "illustrator" },
    KyNangs = new[] { "Photoshop", "Illustrator", "Adobe Illustrator", "Figma", "Canva", "Thiết kế logo", "Banner Design" }
}
```
Code chạy 2 vòng lặp `foreach`:
```csharp
foreach (var quyTac in DanhSachQuyTac)
{
    foreach (var tuKhoa in quyTac.TuKhoas)
    {
        var tuKhoaKd = ChuanHoaKhongDau(tuKhoa); // ví dụ: "logo"

        // Kiểm tra xem chuỗi tiêu đề có chứa từ "logo" hay không
        if (KiemTraChuaTuKhoa(tieuDeKd, tuKhoaKd))
        {
            // Nếu có, duyệt qua danh sách kỹ năng tương ứng của nhóm này
            foreach (var kn in quyTac.KyNangs)
            {
                // Gán điểm trọng số 1.0 vì xuất hiện trong tiêu đề
                diemTheoTenKyNang[kn] = (1.0, $"Phù hợp với từ khóa '{tuKhoa}' trong tiêu đề");
            }
        }
    }
}
```
*Kết quả bước này*: Dictionary `diemTheoTenKyNang` đang lưu các tên kỹ năng: `Photoshop`, `Illustrator`, `Figma`, `Canva`, `Thiết kế logo`,... với điểm số là `1.0`.

#### 3. Truy vấn bảng `KyNang` trong SQL Server:
```csharp
// Đọc danh mục kỹ năng có thật từ cơ sở dữ liệu
var kyNangsSQL = await _context.KyNangs.AsNoTracking().ToListAsync();
```
Hệ thống chỉ lọc lấy những kỹ năng nào **vừa được thuật toán tìm thấy**, **vừa có bản ghi trong CSDL**:
```csharp
kyNangsSQL = kyNangsSQL
    .Where(k => diemTheoTenKyNang.ContainsKey(k.tenKyNang))
    .ToList();
```

#### 4. Đóng gói kết quả trả về:
```csharp
var ketQua = kyNangsSQL
    .Select(k => new KyNangGoiY_ResponseDTO
    {
        maKyNang = k.maKyNang,       // Lấy đúng mã khóa chính trong SQL (vd: 5, 6, 8,...)
        tenKyNang = k.tenKyNang,     // "Photoshop", "Illustrator",...
        diemPhuHop = 1.0,
        lyDo = "Phù hợp với từ khóa 'logo' trong tiêu đề"
    })
    .OrderByDescending(x => x.diemPhuHop)
    .Take(12) // Lấy tối đa 12 kỹ năng
    .ToList();

return ketQua;
```

---

### BƯỚC 1.4: GIAO DIỆN NHẬN DỮ LIỆU & RENDER THẺ BẤM

Khi Frontend nhận được JSON:
- **Trên Web**: Code chạy vòng lặp tạo ra các thẻ `<button type="button" class="skill-btn" data-skill="Photoshop">+ Photoshop</button>`. Khi người dùng bấm vào, JS lấy giá trị này ghép vào ô `kynangyeucau`, phân tách bằng dấu phẩy.
- **Trên Mobile**: Flutter gọi `setState()` cập nhật danh sách `_suggestedSkills`. Màn hình vẽ lại các thẻ `ActionChip`. Khi người dùng chạm vào chip, hàm `_addSkill(skill)` được gọi để thêm vào danh sách `_selectedSkills`.

---
---

# PHẦN 2: LUỒNG CODE GỢI Ý VIỆC LÀM CHO FREELANCER STUDENT

Mục tiêu: Gợi ý công việc phù hợp **hoàn toàn dựa trên số kỹ năng sinh viên sở hữu so với danh sách kỹ năng yêu cầu của bài Job** (`job.kynangyeucau`). Không tính % dựa vào tiêu đề hay mô tả.

Toàn bộ luồng này nằm trong hàm `layDanhSachJobGoiYChoUserAsync` (`FreelancerStudent.API/Services/JobPostService.cs`).

---

### BƯỚC 2.1: ĐỌC KỸ NĂNG CỦA SINH VIÊN TỪ CSDL

Code truy vấn CSDL để lấy toàn bộ kỹ năng sinh viên đang có:
```csharp
// 1. Tìm thông tin sinh viên kèm theo các bảng kỹ năng liên kết
var freelancer = await _context.FreelancerStudents
    .Include(f => f.ChuyenNganh)
    .Include(f => f.FreelancerStudent_KyNangs)
        .ThenInclude(k => k.KyNang)
    .FirstOrDefaultAsync(f => f.maUser == maUser || f.maFreelancerStudents == maUser);

// 2. Gom tất cả tên kỹ năng vào 1 danh sách duy nhất (danhSachKyNangUser)
var danhSachKyNangUser = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

if (freelancer != null)
{
    // Lấy từ bảng quan hệ nhiều-nhiều FreelancerStudent_KyNang
    foreach (var k in freelancer.FreelancerStudent_KyNangs)
    {
        if (k.KyNang != null && !string.IsNullOrWhiteSpace(k.KyNang.tenKyNang))
        {
            danhSachKyNangUser.Add(k.KyNang.tenKyNang.Trim());
        }
    }

    // Lấy thêm từ trường chuỗi kyNangCoBan (nếu có)
    if (!string.IsNullOrWhiteSpace(freelancer.kyNangCoBan))
    {
        var splitCoBan = freelancer.kyNangCoBan.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var s in splitCoBan) danhSachKyNangUser.Add(s.Trim());
    }
}
```

---

### BƯỚC 2.2: LẤY DANH SÁCH CÁC CÔNG VIỆC ĐANG TUYỂN

```csharp
var dsJobSQL = await _context.JobPosts
    .Include(j => j.NhaTuyenDung).ThenInclude(ntd => ntd.User)
    .Include(j => j.UngTuyens)
    .Where(j => j.status == "DangTuyen") // Chỉ lấy bài đang mở tuyển
    .ToListAsync();
```

---

### BƯỚC 2.3: DUYỆT TỪNG CÔNG VIỆC VÀ SO KHỚP KỸ NĂNG

Hệ thống chạy vòng lặp `foreach (var job in dsJobSQL)`:

#### Bước 2.3.1: Tách danh sách kỹ năng yêu cầu trong bài Job:
```csharp
var kyNangJob = new List<string>();
if (!string.IsNullOrWhiteSpace(job.kynangyeucau))
{
    var splitKyNang = job.kynangyeucau.Split(
        new[] { ',', ';', '|', '\n', '/' },
        StringSplitOptions.RemoveEmptyEntries
    );
    foreach (var kn in splitKyNang)
    {
        var trimmed = kn.Trim();
        if (!string.IsNullOrWhiteSpace(trimmed) &&
            !kyNangJob.Any(x => string.Equals(x, trimmed, StringComparison.OrdinalIgnoreCase)))
        {
            kyNangJob.Add(trimmed);
        }
    }
}
```

#### Bước 2.3.2: So khớp kỹ năng sinh viên có với kỹ năng bài Job yêu cầu:
```csharp
var kyNangKhop = new List<string>();

if (kyNangJob.Count > 0)
{
    foreach (var knJob in kyNangJob)
    {
        var knJobLower = knJob.ToLowerInvariant();
        // Kiểm tra sinh viên có kỹ năng này trong hồ sơ không
        var khop = danhSachKyNangUser.Any(knUser =>
        {
            var knUserLower = knUser.ToLowerInvariant();
            return knUserLower == knJobLower ||
                   knJobLower.Contains(knUserLower) ||
                   knUserLower.Contains(knJobLower);
        });

        if (khop && !kyNangKhop.Contains(knJob))
        {
            kyNangKhop.Add(knJob);
        }
    }
}
```

#### Bước 2.3.3: Tính phần trăm phù hợp thuần túy dựa trên số kỹ năng đáp ứng:
```csharp
double phanTramPhuHop = 0;
string lyDo = string.Empty;

if (danhSachKyNangUser.Count == 0)
{
    phanTramPhuHop = 0;
    lyDo = "Bạn chưa cập nhật kỹ năng vào hồ sơ nên chưa thể tính toán độ phù hợp.";
}
else if (kyNangJob.Count > 0)
{
    int tongKyNangYeuCau = kyNangJob.Count;
    int soKyNangDapUng = kyNangKhop.Count;

    // Tỷ lệ % = (Số kỹ năng đáp ứng / Tổng kỹ năng yêu cầu) * 100
    phanTramPhuHop = Math.Min(100.0, Math.Round(((double)soKyNangDapUng / tongKyNangYeuCau) * 100.0, 0));

    if (soKyNangDapUng == tongKyNangYeuCau && tongKyNangYeuCau > 0)
    {
        lyDo = $"Phù hợp 100%: Bạn đáp ứng đầy đủ tất cả {tongKyNangYeuCau}/{tongKyNangYeuCau} kỹ năng yêu cầu ({string.Join(", ", kyNangKhop)}).";
    }
    else if (soKyNangDapUng > 0)
    {
        lyDo = $"Phù hợp {phanTramPhuHop:0}%: Bạn đáp ứng {soKyNangDapUng}/{tongKyNangYeuCau} kỹ năng yêu cầu ({string.Join(", ", kyNangKhop)}).";
    }
    else
    {
        lyDo = $"0% phù hợp: Bạn chưa có kỹ năng nào trong số {tongKyNangYeuCau} kỹ năng công việc yêu cầu ({string.Join(", ", kyNangJob)}).";
    }
}
else
{
    // Bài Job không yêu cầu kỹ năng cụ thể nào
    phanTramPhuHop = 0;
    lyDo = "Công việc không yêu cầu kỹ năng cụ thể.";
}
```

#### Bước 2.3.4: Thêm vào danh sách kết quả:
```csharp
// Chỉ gợi ý công việc có ít nhất 1 kỹ năng trùng khớp (> 0%)
if (phanTramPhuHop > 0)
{
    ketQua.Add(new JobPost_ReponseDTO
    {
        maJob = job.maJob,
        tieude = job.tieude,
        thulao = job.thulao,
        doPhuHop = phanTramPhuHop,
        lyDoGoiY = lyDo,
        kyNangTrungKhop = kyNangKhop,
        ...
    });
}
```

---

### BƯỚC 2.4: SẮP XẾP VÀ TRẢ VỀ CHO CLIENT

Code dòng cuối của hàm:
```csharp
// Sắp xếp: bài nào điểm phù hợp cao nhất (100%, 75%, 50%, 25%) đứng đầu tiên, sau đó đến bài mới đăng nhất
return ketQua
    .OrderByDescending(j => j.doPhuHop)
    .ThenByDescending(j => j.thoigiandangtuyen)
    .ToList();
```

---

### BƯỚC 2.5: GIAO DIỆN HIỂN THỊ CHO SINH VIÊN

Khi App Mobile hoặc Web nhận mảng `data` từ API:
- Các bài đăng mà sinh viên đáp ứng được nhiều kỹ năng nhất (ví dụ: $100\%$, $75\%$, $50\%$, $25\%$) sẽ được sắp xếp lên đầu danh sách.
- Trên thẻ bài đăng có nhãn màu xanh hiển thị rõ tỷ lệ phù hợp:
  - Nếu sinh viên có đủ $2/2$ kỹ năng yêu cầu $\rightarrow$ **`100% Phù hợp`**
  - Nếu bài Job yêu cầu 4 kỹ năng (`HTML, CSS, Photoshop, Lightroom`) mà sinh viên có 1 kỹ năng (`Photoshop`) $\rightarrow$ **`25% Phù hợp`** kèm lý do: *"Phù hợp 25%: Bạn đáp ứng 1/4 kỹ năng yêu cầu (Photoshop)"*.
  - Nếu bài Job yêu cầu 4 kỹ năng mà sinh viên có 2 kỹ năng (`Photoshop, Lightroom`) $\rightarrow$ **`50% Phù hợp`** kèm lý do: *"Phù hợp 50%: Bạn đáp ứng 2/4 kỹ năng yêu cầu (Photoshop, Lightroom)"*.
  - Nếu sinh viên có $0$ kỹ năng nào trong danh sách yêu cầu $\rightarrow$ $0\%$ phù hợp (không đưa vào danh sách gợi ý).

---

# TỔNG KẾT ĐỐI CHIẾU

| Tính năng | Cơ chế hoạt động của Code | Nguồn dữ liệu sử dụng |
|---|---|---|
| **1. Gợi ý kỹ năng khi tạo/sửa tin tuyển dụng** (Nhà tuyển dụng) | Khi gõ tiêu đề hoặc mô tả, sau 350ms - 400ms ngừng gõ, hệ thống gọi API `GoiYKyNangTheoTieuDe`. Hệ thống phân tích các từ khóa trong tiêu đề/mô tả $\rightarrow$ ánh xạ với từ điển quy tắc $\rightarrow$ truy vấn bảng `KyNang` trong SQL Server $\rightarrow$ trả về các chip kỹ năng chuẩn để NTD bấm chọn nhanh vào ô `kynangyeucau`. | Chuỗi tiêu đề, mô tả và bảng `KyNang` trong SQL Server. |
| **2. Gợi ý việc làm phù hợp cho Freelancer** (Sinh viên) | Thuần túy so khớp số kỹ năng sinh viên có với danh sách kỹ năng yêu cầu trong bài Job (`job.kynangyeucau`). Không tính điểm suy diễn từ tiêu đề hay mô tả. <br>**Công thức:** $\text{Phần trăm} = \left(\frac{\text{Số kỹ năng đáp ứng}}{\text{Tổng số kỹ năng yêu cầu của Job}}\right) \times 100\%$ | Kỹ năng của Freelancer (`FreelancerStudent_KyNangs`, `kyNangCoBan`) và trường `kynangyeucau` của bài Job. |
