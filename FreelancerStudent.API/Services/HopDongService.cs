
using FreelancerStudent.API.DTOs.ReponseDTOs;
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

            // Chuyển dữ liệu từ Model sang ResponseDTO
            var result = danhSach.Select(h =>
                new HopDongResponseDTO
                {
                    maHD = h.maHD,

                    maJob = h.maJob,

                    tenCongViec =
                        h.JobPost?.tieude ?? string.Empty,

                    maFreelancerStudent =
                        h.maFreelancerStudent,

                    // Sẽ bổ sung tên sau khi kiểm tra
                    // Model FreelancerStudents và Users
                    tenFreelancer = string.Empty,

                    ngayBatDau = h.ngayBatDau,

                    ngayKetThuc = h.ngayKetThuc,

                    sotienkyquy = h.sotienkyquy,

                    trangThai =
                        h.trangThai ?? string.Empty
                }).ToList();

            return result;
        }
    }
}
