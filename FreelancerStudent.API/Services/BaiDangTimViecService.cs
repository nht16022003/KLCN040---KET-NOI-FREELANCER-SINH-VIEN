using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class BaiDangTimViecService : IBaiDangTimViecService
    {
        private readonly IBaiDangTimViecRepository _repo;
        private readonly IFreelancerStudentRepository _freeRepo;

        public BaiDangTimViecService(IBaiDangTimViecRepository repo, IFreelancerStudentRepository freeRepo)
        {
            _repo = repo;
            _freeRepo = freeRepo;
        }

        //Tuấn
        public async Task<BaiDangTimViec_ReponseDTO> taoBaiDangAsync(BaiDangTimViec_RequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.tieude))
            {
                throw new Exception("Tiêu đề bài đăng không được để trống!");
            }

            var sinhVien = await _freeRepo.layFreelancerStudent_TheoMaUser(request.maUser);
            if (sinhVien == null)
            {
                throw new Exception("Không tìm thấy hồ sơ sinh viên của bạn!");
            }

            string maBaiDang = "PST" + DateTime.Now.ToString("yyMMddHHmmss");

            var baiDangMoi = new BaiDangTimViecFreelancerStudent
            {
                maBaiDang = maBaiDang,
                maFreelancerStudent = sinhVien.maFreelancerStudents,
                tieude = request.tieude.Trim(),
                mota = request.mota?.Trim(),
                kynang = request.kynang?.Trim(),
                mucGiaTu = request.mucGiaTu ?? 0,
                thoigiandang = DateTime.UtcNow,
                trangthai = "DangHienThi"
            };

            var luu = await _repo.themBaiDangAsync(baiDangMoi);

            return new BaiDangTimViec_ReponseDTO
            {
                maBaiDang = luu.maBaiDang,
                maFreelancerStudent = luu.maFreelancerStudent,
                tieude = luu.tieude,
                mota = luu.mota,
                kynang = luu.kynang,
                mucGiaTu = luu.mucGiaTu,
                thoigiandang = luu.thoigiandang,
                trangthai = luu.trangthai
            };
        }


        //Tuấn

        public async Task<List<BaiDangTimViec_ReponseDTO>> layDanhSachTheoMaUserAsync(int maUser)
        {
            var sinhVien = await _freeRepo.layFreelancerStudent_TheoMaUser(maUser);
            if (sinhVien == null) return new List<BaiDangTimViec_ReponseDTO>();

            var ds = await _repo.layDanhSachTheoFreelancerAsync(sinhVien.maFreelancerStudents);
            return ds.Select(b => MapToDTO(b)).ToList();
        }

        //Tuấn

        public async Task<List<BaiDangTimViec_ReponseDTO>> layTatCaBaiDangKhaDungAsync()
        {
            var ds = await _repo.layTatCaBaiDangHienThiAsync();
            return ds.Select(b => MapToDTO(b)).ToList();
        }

        //Tuấn

        public async Task<bool> doiTrangThaiAsync(string maBaiDang, string trangthaiMoi)
        {
            return await _repo.doiTrangThaiAsync(maBaiDang, trangthaiMoi);
        }

        //Tuấn

        public async Task<bool> xoaBaiDangAsync(string maBaiDang)
        {
            return await _repo.xoaBaiDangAsync(maBaiDang);
        }

        //Tuấn

        private static BaiDangTimViec_ReponseDTO MapToDTO(BaiDangTimViecFreelancerStudent b)
        {
            return new BaiDangTimViec_ReponseDTO
            {
                maBaiDang = b.maBaiDang,
                maFreelancerStudent = b.maFreelancerStudent,
                tieude = b.tieude,
                mota = b.mota,
                kynang = b.kynang,
                mucGiaTu = b.mucGiaTu,
                thoigiandang = b.thoigiandang,
                trangthai = b.trangthai,
                maUser = b.FreelamcerStudents?.User?.maUser ?? 0,
                tenUser = b.FreelamcerStudents?.User?.hotenUser,
                avatar = b.FreelamcerStudents?.User?.avatarUrl,
                tenTruong = b.FreelamcerStudents?.tenTruong,
                tenChuyenNganh = b.FreelamcerStudents?.ChuyenNganh?.tenChuyenNganh,
                namThu = b.FreelamcerStudents?.namThu ?? 1,
                GPA = b.FreelamcerStudents?.GPA ?? 0
            };
        }

        //Tuấn
        public async Task<bool> capNhatBaiDangAsync(string maBaiDang, BaiDangTimViec_RequestDTO request)
        {
            var baiDang = await _repo.layTheoMaAsync(maBaiDang);
            if (baiDang == null)
            {
                throw new Exception("Không tìm thấy bài đăng tìm việc để chỉnh sửa!");
            }

            baiDang.tieude = request.tieude.Trim();
            baiDang.mota = request.mota?.Trim();
            baiDang.kynang = request.kynang?.Trim();
            baiDang.mucGiaTu = request.mucGiaTu ?? 0;

            return await _repo.capNhatBaiDangAsync(baiDang);
        }

    }
}
