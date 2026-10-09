using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class FreelancerStudentRepository : IFreelancerStudentRepository
    {
        private readonly ApplicationDBContext _context;

        public FreelancerStudentRepository(ApplicationDBContext context)
        {
            _context = context;

        }


        public async Task<List<FreelancerStudents>> layTatCaFreelancerStudentsAsync()
        {
            var ketqua = await _context.FreelancerStudents.Include(f => f.User).Include(f => f.ChuyenNganh).ToListAsync();
            return ketqua;
        }

        //Tuấn
        public async Task<FreelancerStudents?> layFreelancerStudent_TheoMaUser(int maUser)
        {
            var freelancerStudent = await _context.FreelancerStudents.Include(f => f.User).Include(f => f.ChuyenNganh).FirstOrDefaultAsync(f => f.maUser == maUser);
            return freelancerStudent;
        }


        //Tuấn
        public async Task<FreelancerStudents> themFreelancerStudent_DuaVaoMaRoleCuaUsers(Users user, int maRole)
        {
            if (maRole != 1)
            {
                return null!; // Không phải FreelancerStudent
            }

            //lấy mã chuyên ngành mặc định đầu tiên trong DB để tránh lỗi khóa ngoại FK
            var maChuyenNganhMacDinh = await _context.ChuyenNganhs
                .Select(c => c.maChuyenNganh)
                .FirstOrDefaultAsync() ?? "CNTT";

            var freeMoi = new FreelancerStudents
            {
                maUser = user.maUser,
                maChuyenNganh = maChuyenNganhMacDinh,
                maTruong = "DH",
                tenTruong = "Chưa cập nhật trường học",
                diaDiemTruong = "Chưa cập nhật",
                diaDiemFreelancerStudent = "Chưa cập nhật",
                namThu = 1,
                GPA = 0,
                nienKhoa = $"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Year + 4}",
                gioithieu = "Chưa cập nhật thông tin giới thiệu bản thân.",
                trangthaiNhanViec = true,
                chiPhiTu = 0
            };
            await _context.FreelancerStudents.AddAsync(freeMoi);
            await _context.SaveChangesAsync();

            return freeMoi;
        }


        //Tuấn
        public async Task<FreelancerStudents?> layChiTietTheoIdAsync(int id)
        {
            return await _context.FreelancerStudents
                .Include(f => f.User)
                .Include(f => f.ChuyenNganh)
                .Include(f => f.FreelancerStudent_KyNangs)
                    .ThenInclude(k => k.KyNang)
                .FirstOrDefaultAsync(f => f.maFreelancerStudents == id || f.maUser == id); // Tìm theo mã Freelancer hoặc mã User
        }

        //Tuấn - Tìm theo mã User rồi cập nhật
        public async Task<bool> capNhatHoSoAsync(ChiTietHoSoFreelancer_ReponseDTO dto)
        {
            //  Tìm bản ghi Freelancer theo mã User đang đăng nhập
            var student = await _context.FreelancerStudents
                .Include(f => f.User)
                .Include(f => f.FreelancerStudent_KyNangs)
                .FirstOrDefaultAsync(f => f.maUser == dto.maUser);

            // Nếu không tìm thấy thì báo lỗi / trả về false
            if (student == null)
            {
                return false;
            }

            //  Cập nhật thông tin sinh viên (FreelancerStudents)
            student.maChuyenNganh = dto.maChuyenNganh;
            student.tenTruong = dto.tenTruong ?? string.Empty;
            student.diaDiemFreelancerStudent = dto.diaDiemFreelancerStudent ?? string.Empty;
            student.namThu = dto.namThu;
            student.GPA = dto.GPA;
            student.nienKhoa = dto.nienKhoa ?? string.Empty;
            student.gioithieu = dto.gioithieu;
            student.kyNangCoBan = dto.kyNangCoBan;
            student.ngonNgu = dto.ngonNgu;
            student.trangthaiNhanViec = dto.trangthaiNhanViec;
            student.chiPhiTu = dto.chiPhiTu;

            //  Cập nhật thông tin tài khoản (Users)
            if (student.User != null)
            {
                student.User.hotenUser = dto.tenUser;
                student.User.sdtUser = dto.sdt;
            }

            // Cập nhật danh sách kỹ năng chuyên môn
            if (dto.danhSachKyNang != null)
            {
                // Xóa danh sách kỹ năng cũ của sinh viên
                var kyNangCu = _context.FreelancerStudent_KyNangs.Where(k => k.maFreelancerStudents == student.maFreelancerStudents);
                _context.FreelancerStudent_KyNangs.RemoveRange(kyNangCu);

                // Thêm danh sách kỹ năng mới
                foreach (var tenKn in dto.danhSachKyNang.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    var kn = await _context.KyNangs.FirstOrDefaultAsync(k => k.tenKyNang.ToLower() == tenKn.Trim().ToLower());
                    if (kn == null)
                    {
                        kn = new KyNang { tenKyNang = tenKn.Trim() };
                        await _context.KyNangs.AddAsync(kn);
                        await _context.SaveChangesAsync();
                    }

                    await _context.FreelancerStudent_KyNangs.AddAsync(new FreelancerStudent_KyNang
                    {
                        maFreelancerStudents = student.maFreelancerStudents,
                        maKyNang = kn.maKyNang
                    });
                }
            }

            //Lưu toàn bộ thay đổi xuống SQL Server
            await _context.SaveChangesAsync();
            return true;
        }




        //XS
        public async Task<FreelancerStudents?> layTheoMaFreelancerStudents(int maFreelancerStudents)
        {
            var freelancerStudent = await _context.FreelancerStudents.FirstOrDefaultAsync(f => f.maFreelancerStudents == maFreelancerStudents);
            return freelancerStudent;
        }
    }
}