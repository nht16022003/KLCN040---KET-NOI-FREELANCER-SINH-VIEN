using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Services
{
    public class TaskCongViecService : ITaskCongViecService
    {
        private readonly ITaskCongViecRepository _taskRepository;
        private readonly HopDongRepository _hopDongRepository;
        private readonly ApplicationDBContext _context;

        private static readonly HashSet<string> TrangThaiHopLe = new(StringComparer.OrdinalIgnoreCase)
        {
            "ChuaThucHien",
            "DangThucHien",
            "HoanThanh"
        };

        public TaskCongViecService(
            ITaskCongViecRepository taskRepository,
            HopDongRepository hopDongRepository,
            ApplicationDBContext context)
        {
            _taskRepository = taskRepository;
            _hopDongRepository = hopDongRepository;
            _context = context;
        }

        private async Task<bool> KiemTraQuyenFreelancerAsync(HopDong hopDong, int maUser)
        {
            if (hopDong == null || maUser <= 0) return false;

            // 1. Kiểm tra qua FreelancerStudent.maUser (trực tiếp từ entity FreelancerStudents)
            if (hopDong.FreelancerStudent != null && hopDong.FreelancerStudent.maUser == maUser)
            {
                return true;
            }

            // 2. Kiểm tra qua FreelancerStudent.User.maUser
            if (hopDong.FreelancerStudent?.User != null && hopDong.FreelancerStudent.User.maUser == maUser)
            {
                return true;
            }

            // 3. Dự phòng chắc chắn: Truy vấn DB theo maFreelancerStudent
            if (hopDong.maFreelancerStudent > 0)
            {
                var fl = await _context.FreelancerStudents.AsNoTracking()
                    .FirstOrDefaultAsync(f => f.maFreelancerStudents == hopDong.maFreelancerStudent);
                if (fl != null && fl.maUser == maUser)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> KiemTraQuyenNhaTuyenDungAsync(HopDong hopDong, int maUser)
        {
            if (hopDong == null || maUser <= 0) return false;

            // 1. Kiểm tra qua NhaTuyenDung.maUser
            if (hopDong.JobPost?.NhaTuyenDung != null && hopDong.JobPost.NhaTuyenDung.maUser == maUser)
            {
                return true;
            }

            // 2. Kiểm tra qua NhaTuyenDung.User.maUser
            if (hopDong.JobPost?.NhaTuyenDung?.User != null && hopDong.JobPost.NhaTuyenDung.User.maUser == maUser)
            {
                return true;
            }

            // 3. Dự phòng: Truy vấn DB theo maNhaTuyenDung
            if (hopDong.JobPost != null && hopDong.JobPost.maNhaTuyenDung > 0)
            {
                var ntd = await _context.NhaTuyenDungs.AsNoTracking()
                    .FirstOrDefaultAsync(n => n.maNhaTuyenDung == hopDong.JobPost.maNhaTuyenDung);
                if (ntd != null && ntd.maUser == maUser)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<TienDoDuAn_ResponseDTO> LayTienDoDuAnAsync(string maHD, int maUser)
        {
            if (string.IsNullOrWhiteSpace(maHD))
            {
                throw new ArgumentException("Mã hợp đồng không hợp lệ.");
            }

            var hopDong = await _hopDongRepository.LayChiTietHopDongTheoMaAsync(maHD);
            if (hopDong == null)
            {
                throw new ArgumentException("Không tìm thấy hợp đồng.");
            }

            // Kiểm tra quyền xem: Phải là Freelancer hoặc Nhà tuyển dụng của hợp đồng
            bool laFreelancer = await KiemTraQuyenFreelancerAsync(hopDong, maUser);
            bool laNhaTuyenDung = await KiemTraQuyenNhaTuyenDungAsync(hopDong, maUser);

            if (!laFreelancer && !laNhaTuyenDung)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xem tiến độ của hợp đồng này.");
            }

            var tasks = await _taskRepository.LayDanhSachTaskTheoMaHDAsync(maHD);

            int tongSoTask = tasks.Count;
            int soTaskChuaThucHien = tasks.Count(t => string.Equals(t.trangThai, "ChuaThucHien", StringComparison.OrdinalIgnoreCase));
            int soTaskDangThucHien = tasks.Count(t => string.Equals(t.trangThai, "DangThucHien", StringComparison.OrdinalIgnoreCase));
            int soTaskHoanThanh = tasks.Count(t => string.Equals(t.trangThai, "HoanThanh", StringComparison.OrdinalIgnoreCase));

            decimal tyLe = tongSoTask > 0
                ? Math.Round((decimal)soTaskHoanThanh / tongSoTask * 100, 1)
                : 0;

            bool coQuyenCapNhat = laFreelancer && string.Equals(hopDong.trangThai, "DangThucHien", StringComparison.OrdinalIgnoreCase);
            bool coQuyenDanhGia = laNhaTuyenDung;
            bool duAnDaDat = hopDong.GiaiDoan_HopDongs != null &&
                             hopDong.GiaiDoan_HopDongs.Any() &&
                             hopDong.GiaiDoan_HopDongs.All(g => string.Equals(g.trangThai, "Dat", StringComparison.OrdinalIgnoreCase));

            return new TienDoDuAn_ResponseDTO
            {
                maHD = hopDong.maHD,
                trangThaiHopDong = hopDong.trangThai ?? string.Empty,
                ngayBatDauHopDong = hopDong.ngayBatDau,
                ngayKetThucHopDong = hopDong.ngayKetThuc,
                tongSoTask = tongSoTask,
                soTaskChuaThucHien = soTaskChuaThucHien,
                soTaskDangThucHien = soTaskDangThucHien,
                soTaskHoanThanh = soTaskHoanThanh,
                tyLeHoanThanh = tyLe,
                coQuyenCapNhat = coQuyenCapNhat,
                coQuyenDanhGia = coQuyenDanhGia,
                duAnDaDat = duAnDaDat,
                danhSachGiaiDoan = hopDong.GiaiDoan_HopDongs?.Select(g => new GiaiDoanHopDong_ResponseDTO
                {
                    maGiaiDoan = g.maGiaiDoan,
                    maHD = g.maHD,
                    tenGiaiDoan = g.tenGiaiDoan,
                    moTa = g.moTa,
                    hanChot = g.hanChot,
                    trangThai = g.trangThai ?? "ChuaDat"
                }).ToList() ?? new List<GiaiDoanHopDong_ResponseDTO>(),
                danhSachTask = tasks.Select(t =>
                {
                    TachMoTaVaLink(t.moTa, out string moTaSach, out string? link);
                    return new TaskCongViec_ResponseDTO
                    {
                        maTask = t.maTask,
                        maHD = t.maHD,
                        maGiaiDoan = t.maGiaiDoan,
                        tenGiaiDoan = t.GiaiDoan_HopDong?.tenGiaiDoan,
                        tenTask = t.tenTask,
                        moTa = moTaSach,
                        linkSanPham = link,
                        hanHoanThanh = t.hanHoanThanh,
                        trangThai = t.trangThai,
                        ngayCapNhat = t.ngayCapNhat
                    };
                }).ToList()
            };
        }

        public async Task<TaskCongViec_ResponseDTO> ThemTaskAsync(TaoTaskCongViec_RequestDTO request, int maUser)
        {
            if (request == null)
            {
                throw new ArgumentException("Dữ liệu đầu việc không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(request.maHD))
            {
                throw new ArgumentException("Mã hợp đồng không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(request.tenTask))
            {
                throw new ArgumentException("Tiêu đề đầu việc không được để trống.");
            }

            var hopDong = await _hopDongRepository.LayChiTietHopDongTheoMaAsync(request.maHD);
            if (hopDong == null)
            {
                throw new ArgumentException("Không tìm thấy hợp đồng.");
            }

            // Kiểm tra quyền: Chỉ Freelancer thực hiện hợp đồng mới được tạo task
            if (!await KiemTraQuyenFreelancerAsync(hopDong, maUser))
            {
                throw new UnauthorizedAccessException("Chỉ Freelancer thực hiện hợp đồng mới có quyền thêm đầu việc.");
            }

            // Kiểm tra trạng thái hợp đồng: Chỉ được thêm khi DangThucHien
            if (!string.Equals(hopDong.trangThai, "DangThucHien", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Chỉ được cập nhật tiến độ khi hợp đồng đang trong trạng thái Đang thực hiện.");
            }

            // Kiểm tra deadline (EXC-01): Không được vượt quá deadline hợp đồng
            if (request.hanHoanThanh.HasValue && hopDong.ngayKetThuc.HasValue)
            {
                if (request.hanHoanThanh.Value.Date > hopDong.ngayKetThuc.Value.Date)
                {
                    throw new ArgumentException("Deadline của công việc phải trước hoặc trùng với hạn chót bàn giao của hợp đồng.");
                }
            }

            // Nếu có chọn giai đoạn, kiểm tra giai đoạn thuộc đúng hợp đồng
            if (request.maGiaiDoan.HasValue && request.maGiaiDoan.Value > 0)
            {
                var giaiDoan = hopDong.GiaiDoan_HopDongs?.FirstOrDefault(g => g.maGiaiDoan == request.maGiaiDoan.Value);
                if (giaiDoan == null)
                {
                    throw new ArgumentException("Giai đoạn / Mốc bàn giao đã chọn không thuộc hợp đồng này.");
                }
            }

            string moTaLuu = DongGoiMoTa(request.moTa ?? string.Empty, request.linkSanPham);
            var entity = new Task_CongViec
            {
                maHD = request.maHD.Trim(),
                maGiaiDoan = (request.maGiaiDoan.HasValue && request.maGiaiDoan.Value > 0) ? request.maGiaiDoan.Value : null,
                tenTask = request.tenTask.Trim(),
                moTa = moTaLuu,
                hanHoanThanh = request.hanHoanThanh,
                trangThai = "ChuaThucHien",
                ngayCapNhat = DateTime.Now
            };

            var created = await _taskRepository.ThemTaskAsync(entity);
            TachMoTaVaLink(created.moTa, out string moTaSach, out string? link);

            return new TaskCongViec_ResponseDTO
            {
                maTask = created.maTask,
                maHD = created.maHD,
                maGiaiDoan = created.maGiaiDoan,
                tenTask = created.tenTask,
                moTa = moTaSach,
                linkSanPham = link,
                hanHoanThanh = created.hanHoanThanh,
                trangThai = created.trangThai,
                ngayCapNhat = created.ngayCapNhat
            };
        }

        public async Task<bool> CapNhatTaskAsync(CapNhatTaskCongViec_RequestDTO request, int maUser)
        {
            if (request == null || request.maTask <= 0)
            {
                throw new ArgumentException("Dữ liệu đầu việc không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(request.tenTask))
            {
                throw new ArgumentException("Tiêu đề đầu việc không được để trống.");
            }

            if (!TrangThaiHopLe.Contains(request.trangThai))
            {
                throw new ArgumentException("Trạng thái công việc không hợp lệ. Chỉ chấp nhận: ChuaThucHien, DangThucHien, HoanThanh.");
            }

            var task = await _taskRepository.LayTaskTheoMaAsync(request.maTask);
            if (task == null)
            {
                throw new ArgumentException("Không tìm thấy đầu việc cần chỉnh sửa.");
            }

            var hopDong = task.HopDong ?? await _hopDongRepository.LayChiTietHopDongTheoMaAsync(task.maHD);
            if (hopDong == null)
            {
                throw new ArgumentException("Không tìm thấy hợp đồng liên kết.");
            }

            // Kiểm tra quyền Freelancer
            if (!await KiemTraQuyenFreelancerAsync(hopDong, maUser))
            {
                throw new UnauthorizedAccessException("Chỉ Freelancer thực hiện hợp đồng mới có quyền chỉnh sửa đầu việc.");
            }

            // Kiểm tra trạng thái hợp đồng
            if (!string.Equals(hopDong.trangThai, "DangThucHien", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Hợp đồng hiện tại không cho phép chỉnh sửa tiến độ.");
            }

            // Kiểm tra deadline
            if (request.hanHoanThanh.HasValue && hopDong.ngayKetThuc.HasValue)
            {
                if (request.hanHoanThanh.Value.Date > hopDong.ngayKetThuc.Value.Date)
                {
                    throw new ArgumentException("Deadline của công việc phải trước hoặc trùng với hạn chót bàn giao của hợp đồng.");
                }
            }

            // Kiểm tra giai đoạn
            if (request.maGiaiDoan.HasValue && request.maGiaiDoan.Value > 0)
            {
                var giaiDoan = hopDong.GiaiDoan_HopDongs?.FirstOrDefault(g => g.maGiaiDoan == request.maGiaiDoan.Value);
                if (giaiDoan == null)
                {
                    throw new ArgumentException("Giai đoạn / Mốc bàn giao đã chọn không thuộc hợp đồng này.");
                }
                task.maGiaiDoan = request.maGiaiDoan.Value;
            }
            else
            {
                task.maGiaiDoan = null;
            }

            task.tenTask = request.tenTask.Trim();
            task.moTa = DongGoiMoTa(request.moTa ?? string.Empty, request.linkSanPham);
            task.hanHoanThanh = request.hanHoanThanh;
            task.trangThai = request.trangThai;

            return await _taskRepository.CapNhatTaskAsync(task);
        }

        public async Task<bool> CapNhatTrangThaiTaskAsync(int maTask, string trangThaiMoi, int maUser)
        {
            if (maTask <= 0)
            {
                throw new ArgumentException("Mã đầu việc không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(trangThaiMoi) || !TrangThaiHopLe.Contains(trangThaiMoi))
            {
                throw new ArgumentException("Trạng thái công việc không hợp lệ. Chỉ chấp nhận: ChuaThucHien, DangThucHien, HoanThanh.");
            }

            var task = await _taskRepository.LayTaskTheoMaAsync(maTask);
            if (task == null)
            {
                throw new ArgumentException("Không tìm thấy đầu việc.");
            }

            var hopDong = task.HopDong ?? await _hopDongRepository.LayChiTietHopDongTheoMaAsync(task.maHD);
            if (hopDong == null)
            {
                throw new ArgumentException("Không tìm thấy hợp đồng liên kết.");
            }

            // Kiểm tra quyền Freelancer
            if (!await KiemTraQuyenFreelancerAsync(hopDong, maUser))
            {
                throw new UnauthorizedAccessException("Chỉ Freelancer thực hiện hợp đồng mới có quyền cập nhật trạng thái đầu việc.");
            }

            // Kiểm tra trạng thái hợp đồng
            if (!string.Equals(hopDong.trangThai, "DangThucHien", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Hợp đồng hiện tại không cho phép cập nhật trạng thái tiến độ.");
            }

            task.trangThai = trangThaiMoi;
            return await _taskRepository.CapNhatTaskAsync(task);
        }

        public async Task<bool> XoaTaskAsync(int maTask, int maUser)
        {
            if (maTask <= 0)
            {
                throw new ArgumentException("Mã đầu việc không hợp lệ.");
            }

            var task = await _taskRepository.LayTaskTheoMaAsync(maTask);
            if (task == null)
            {
                throw new ArgumentException("Không tìm thấy đầu việc cần xóa.");
            }

            var hopDong = task.HopDong ?? await _hopDongRepository.LayChiTietHopDongTheoMaAsync(task.maHD);
            if (hopDong == null)
            {
                throw new ArgumentException("Không tìm thấy hợp đồng liên kết.");
            }

            // Kiểm tra quyền Freelancer
            if (!await KiemTraQuyenFreelancerAsync(hopDong, maUser))
            {
                throw new UnauthorizedAccessException("Chỉ Freelancer thực hiện hợp đồng mới có quyền xóa đầu việc.");
            }

            // Kiểm tra trạng thái hợp đồng
            if (!string.Equals(hopDong.trangThai, "DangThucHien", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Hợp đồng hiện tại không cho phép xóa đầu việc tiến độ.");
            }

            return await _taskRepository.XoaTaskAsync(maTask);
        }

        public async Task<bool> DanhGiaGiaiDoanAsync(string maHD, int? maGiaiDoan, string trangThaiMoi, int maUser)
        {
            if (string.IsNullOrWhiteSpace(maHD) || maUser <= 0)
            {
                throw new ArgumentException("Thông tin đánh giá không hợp lệ.");
            }

            var trangThaiChuan = string.Equals(trangThaiMoi, "Dat", StringComparison.OrdinalIgnoreCase) ? "Dat" : "ChuaDat";

            var hopDong = await _hopDongRepository.LayChiTietHopDongTheoMaAsync(maHD);
            if (hopDong == null)
            {
                throw new ArgumentException("Không tìm thấy hợp đồng.");
            }

            // Chỉ Nhà tuyển dụng của hợp đồng mới có quyền đánh giá nghiệm thu mốc giai đoạn
            if (!await KiemTraQuyenNhaTuyenDungAsync(hopDong, maUser))
            {
                throw new UnauthorizedAccessException("Chỉ Nhà tuyển dụng của hợp đồng mới có quyền đánh giá nghiệm thu dự án.");
            }

            var danhSachGiaiDoan = await _context.GiaiDoan_HopDongs
                .Where(g => g.maHD == maHD)
                .ToListAsync();

            if (danhSachGiaiDoan == null || !danhSachGiaiDoan.Any())
            {
                throw new InvalidOperationException("Hợp đồng chưa có mốc giai đoạn nào để đánh giá.");
            }

            if (maGiaiDoan.HasValue && maGiaiDoan.Value > 0)
            {
                var target = danhSachGiaiDoan.FirstOrDefault(g => g.maGiaiDoan == maGiaiDoan.Value);
                if (target == null)
                {
                    throw new ArgumentException("Không tìm thấy mốc giai đoạn cần đánh giá.");
                }
                target.trangThai = trangThaiChuan;
            }
            else
            {
                // Đánh giá toàn bộ dự án: nếu đạt thì tất cả các mốc giai đoạn chuyển sang "Dat", ngược lại "ChuaDat"
                foreach (var gd in danhSachGiaiDoan)
                {
                    gd.trangThai = trangThaiChuan;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private static string DongGoiMoTa(string moTaGoc, string? linkSanPham)
        {
            if (string.IsNullOrWhiteSpace(linkSanPham))
            {
                return moTaGoc.Trim();
            }
            return $"{moTaGoc.Trim()}\n[LINK_SAN_PHAM]: {linkSanPham.Trim()}";
        }

        private static void TachMoTaVaLink(string moTaDb, out string moTaSach, out string? linkSanPham)
        {
            if (string.IsNullOrWhiteSpace(moTaDb))
            {
                moTaSach = string.Empty;
                linkSanPham = null;
                return;
            }

            const string tag = "[LINK_SAN_PHAM]:";
            int idx = moTaDb.IndexOf(tag, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                linkSanPham = moTaDb.Substring(idx + tag.Length).Trim();
                moTaSach = moTaDb.Substring(0, idx).Trim();
            }
            else
            {
                linkSanPham = null;
                moTaSach = moTaDb;
            }
        }
    }
}
