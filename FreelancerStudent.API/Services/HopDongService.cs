
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories;

namespace FreelancerStudent.API.Services
{
    public class HopDongService : IHopDongService
    {
        private readonly HopDongRepository _repository;

        public HopDongService(HopDongRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<HopDongResponseDTO>>
            GetDanhSachHopDongAsync(
                int maNhaTuyenDung,
                string? tuKhoa,
                string? trangThai)
        {
            // Kiểm tra mã nhà tuyển dụng
            if (maNhaTuyenDung <= 0)
            {
                throw new ArgumentException(
                    "Mã nhà tuyển dụng không hợp lệ.");
            }

            // Gọi Repository lấy danh sách hợp đồng
            var danhSach = await _repository
                .GetDanhSachHopDongAsync(
                    maNhaTuyenDung,
                    tuKhoa,
                    trangThai);

            return danhSach.Select(MapToResponseDTO).ToList();
        }

        public async Task<List<HopDongResponseDTO>>
            GetDanhSachHopDongTheoFreelancerAsync(
                int maFreelancerStudent,
                string? tuKhoa,
                string? trangThai)
        {
            if (maFreelancerStudent <= 0)
            {
                throw new ArgumentException(
                    "Mã Freelancer không hợp lệ.");
            }

            var danhSach = await _repository
                .GetDanhSachHopDongTheoFreelancerAsync(
                    maFreelancerStudent,
                    tuKhoa,
                    trangThai);

            return danhSach.Select(MapToResponseDTO).ToList();
        }

        public async Task<HopDongResponseDTO> TaoDuThaoHopDongAsync(
            TaoHopDong_RequestDTO request,
            int? maUserNTD)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Dữ liệu hợp đồng không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(request.maJob))
            {
                throw new ArgumentException("Mã bài tuyển dụng không được để trống.");
            }

            if (request.maFreelancerStudent <= 0)
            {
                throw new ArgumentException("Freelancer được chọn không hợp lệ.");
            }

            if (request.sotienkyquy <= 0)
            {
                throw new ArgumentException("Số tiền thù lao thỏa thuận phải lớn hơn 0.");
            }

            // Kiểm tra sự tồn tại của JobPost
            var job = await _repository.LayJobPostTheoMaAsync(request.maJob);
            if (job == null)
            {
                throw new ArgumentException("Không tìm thấy bài tuyển dụng tương ứng.");
            }

            // Kiểm tra quyền sở hữu bài đăng nếu có maUserNTD
            if (maUserNTD.HasValue && maUserNTD.Value > 0)
            {
                if (job.NhaTuyenDung != null && job.NhaTuyenDung.maUser != maUserNTD.Value)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền tạo hợp đồng cho bài tuyển dụng này.");
                }
            }

            // Kiểm tra xem Job đã có hợp đồng chưa
            var hopDongHienCo = await _repository.LayHopDongTheoMaJobAsync(request.maJob);
            if (hopDongHienCo != null && hopDongHienCo.trangThai != "DaHuy")
            {
                throw new InvalidOperationException($"Bài tuyển dụng này đã có hợp đồng ({hopDongHienCo.maHD}) đang ở trạng thái '{hopDongHienCo.trangThai}'.");
            }

            // Kiểm tra Freelancer
            var freelancer = await _repository.LayFreelancerTheoMaAsync(request.maFreelancerStudent);
            if (freelancer == null)
            {
                throw new ArgumentException("Không tìm thấy thông tin Freelancer.");
            }

            // Sinh mã hợp đồng ngẫu nhiên duy nhất (tối đa 20 ký tự)
            string maHD = $"HD{DateTime.Now:yyMMddHHmmss}{new Random().Next(10, 99)}";

            var hopDong = new HopDong
            {
                maHD = maHD,
                maJob = request.maJob,
                maFreelancerStudent = request.maFreelancerStudent,
                sotienkyquy = request.sotienkyquy,
                ngayBatDau = request.ngayBatDau ?? DateTime.Now,
                ngayKetThuc = request.ngayKetThuc,
                hinhthuclamviec = string.IsNullOrWhiteSpace(request.hinhthuclamviec) ? "Remote" : request.hinhthuclamviec,
                trangThai = "ChoXacNhan", // 2.6 Chuyển trạng thái hợp đồng thành "Chờ Freelancer xác nhận"
                dieuKhoan = request.dieuKhoan
            };

            // Tạo các mốc bàn giao (Milestones) nếu có
            var giaiDoans = new List<GiaiDoan_HopDong>();
            if (request.giaiDoans != null && request.giaiDoans.Any())
            {
                foreach (var gd in request.giaiDoans)
                {
                    if (string.IsNullOrWhiteSpace(gd.tenGiaiDoan)) continue;

                    string moTaGiaiDoan = gd.moTa ?? string.Empty;
                    if (gd.tyLeGiaiNgan.HasValue && gd.tyLeGiaiNgan.Value > 0)
                    {
                        decimal soTien = gd.soTien.HasValue && gd.soTien.Value > 0
                            ? gd.soTien.Value
                            : (request.sotienkyquy * gd.tyLeGiaiNgan.Value / 100);
                        moTaGiaiDoan = $"[Giải ngân: {gd.tyLeGiaiNgan.Value:0.#}% ~ {soTien:N0} VNĐ] {moTaGiaiDoan}".Trim();
                    }

                    giaiDoans.Add(new GiaiDoan_HopDong
                    {
                        maHD = maHD,
                        tenGiaiDoan = gd.tenGiaiDoan.Trim(),
                        moTa = moTaGiaiDoan,
                        hanChot = gd.hanChot > DateTime.MinValue ? gd.hanChot : request.ngayKetThuc,
                        trangThai = "ChuaDat",
                        ngayTao = DateTime.Now
                    });
                }
            }

            // Tạo thông báo cho Freelancer (2.6)
            ThongBao? thongBao = null;
            if (freelancer.User != null)
            {
                thongBao = new ThongBao
                {
                    maUser = freelancer.User.maUser,
                    tieuDe = "Dự thảo hợp đồng mới cần xác nhận",
                    noiDung = $"Bạn nhận được dự thảo hợp đồng cho công việc '{job.tieude}' với mức thù lao {request.sotienkyquy:N0} đ. Vui lòng vào danh sách hợp đồng để kiểm tra và xác nhận.",
                    loaiThongBao = "HopDong",
                    linkDieuHuong = "/HopDong",
                    daDoc = false,
                    ngayTao = DateTime.Now
                };
            }

            var hopDongMoi = await _repository.TaoHopDongAsync(hopDong, giaiDoans, request.maUngTuyen, thongBao);

            return MapToResponseDTO(hopDongMoi);
        }

        public async Task<HopDongResponseDTO?> LayChiTietHopDongTheoMaAsync(string maHD)
        {
            if (string.IsNullOrWhiteSpace(maHD)) return null;

            var hopDong = await _repository.LayChiTietHopDongTheoMaAsync(maHD);
            return hopDong != null ? MapToResponseDTO(hopDong) : null;
        }

        public async Task<HopDongResponseDTO?> LayChiTietHopDongTheoMaJobAsync(string maJob)
        {
            if (string.IsNullOrWhiteSpace(maJob)) return null;

            var hopDong = await _repository.LayHopDongTheoMaJobAsync(maJob);
            return hopDong != null ? MapToResponseDTO(hopDong) : null;
        }

        public async Task<bool> XacNhanHopDongAsync(
            XacNhanHopDong_RequestDTO request,
            int? maUserFreelancer)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.maHD))
            {
                throw new ArgumentException("Mã hợp đồng không hợp lệ.");
            }

            var hopDong = await _repository.LayChiTietHopDongTheoMaAsync(request.maHD);
            if (hopDong == null)
            {
                throw new ArgumentException("Không tìm thấy hợp đồng tương ứng.");
            }

            // Kiểm tra quyền của Freelancer nếu có truyền maUserFreelancer
            if (maUserFreelancer.HasValue && maUserFreelancer.Value > 0)
            {
                if (hopDong.FreelancerStudent?.User != null && hopDong.FreelancerStudent.User.maUser != maUserFreelancer.Value)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền xác nhận hợp đồng này.");
                }
            }

            // Kiểm tra trạng thái hiện tại
            if (hopDong.trangThai != "ChoXacNhan")
            {
                throw new InvalidOperationException($"Hợp đồng hiện tại đang ở trạng thái '{hopDong.trangThai}', không thể thực hiện xác nhận hoặc từ chối.");
            }

            // Trạng thái mới: nếu đồng ý thì chuyển sang ChoKyQuy, nếu từ chối thì chuyển sang DaHuy
            string trangThaiMoi = request.dongY ? "ChoKyQuy" : "DaHuy";

            // Tạo thông báo gửi cho Nhà tuyển dụng
            ThongBao? thongBao = null;
            if (hopDong.JobPost?.NhaTuyenDung?.User != null)
            {
                string tenFreelancer = hopDong.FreelancerStudent?.User?.hotenUser ?? "Freelancer";
                string tieuDeCongViec = hopDong.JobPost?.tieude ?? "công việc";
                string tieuDeTB = request.dongY ? "Freelancer đã xác nhận hợp đồng" : "Freelancer đã từ chối hợp đồng";
                string noiDungTB = request.dongY
                    ? $"Freelancer {tenFreelancer} đã đồng ý và xác nhận dự thảo hợp đồng {hopDong.maHD} cho công việc '{tieuDeCongViec}'. Vui lòng tiến hành ký quỹ để bắt đầu dự án."
                    : $"Freelancer {tenFreelancer} đã từ chối dự thảo hợp đồng {hopDong.maHD} cho công việc '{tieuDeCongViec}'" + (!string.IsNullOrWhiteSpace(request.lyDoTuChoi) ? $". Lý do: {request.lyDoTuChoi}" : ".");

                thongBao = new ThongBao
                {
                    maUser = hopDong.JobPost.NhaTuyenDung.User.maUser,
                    tieuDe = tieuDeTB,
                    noiDung = noiDungTB,
                    loaiThongBao = "HopDong",
                    linkDieuHuong = "/HopDong",
                    daDoc = false,
                    ngayTao = DateTime.Now
                };
            }

            return await _repository.CapNhatTrangThaiHopDongAsync(request.maHD, trangThaiMoi, thongBao);
        }

        private static HopDongResponseDTO MapToResponseDTO(HopDong h)
        {
            return new HopDongResponseDTO
            {
                maHD = h.maHD,
                maJob = h.maJob,
                tenCongViec = h.JobPost?.tieude ?? string.Empty,
                moTaCongViec = h.JobPost?.mota,
                maFreelancerStudent = h.maFreelancerStudent,
                tenFreelancer = h.FreelancerStudent?.User?.hotenUser ?? string.Empty,
                emailFreelancer = h.FreelancerStudent?.User?.emailUser,
                sdtFreelancer = h.FreelancerStudent?.User?.sdtUser,
                truongFreelancer = h.FreelancerStudent?.tenTruong,
                chuyenNganhFreelancer = h.FreelancerStudent?.ChuyenNganh?.tenChuyenNganh,
                maUserFreelancer = h.FreelancerStudent?.User?.maUser,
                maNhaTuyenDung = h.JobPost?.NhaTuyenDung?.maNhaTuyenDung,
                tenNhaTuyenDung = !string.IsNullOrWhiteSpace(h.JobPost?.NhaTuyenDung?.tencongty)
                    ? h.JobPost.NhaTuyenDung.tencongty
                    : (h.JobPost?.NhaTuyenDung?.User?.hotenUser ?? string.Empty),
                emailNhaTuyenDung = h.JobPost?.NhaTuyenDung?.User?.emailUser,
                sdtNhaTuyenDung = h.JobPost?.NhaTuyenDung?.User?.sdtUser,
                maUserNhaTuyenDung = h.JobPost?.NhaTuyenDung?.User?.maUser,
                ngayBatDau = h.ngayBatDau,
                ngayKetThuc = h.ngayKetThuc,
                sotienkyquy = h.sotienkyquy,
                hinhthuclamviec = h.hinhthuclamviec,
                dieuKhoan = h.dieuKhoan,
                trangThai = h.trangThai ?? string.Empty,
                giaiDoans = h.GiaiDoan_HopDongs?.Select(g => new GiaiDoanHopDong_ResponseDTO
                {
                    maGiaiDoan = g.maGiaiDoan,
                    maHD = g.maHD,
                    tenGiaiDoan = g.tenGiaiDoan,
                    moTa = g.moTa,
                    hanChot = g.hanChot,
                    trangThai = g.trangThai ?? "ChuaDat"
                }).ToList() ?? new List<GiaiDoanHopDong_ResponseDTO>()
            };
        }
    }
}