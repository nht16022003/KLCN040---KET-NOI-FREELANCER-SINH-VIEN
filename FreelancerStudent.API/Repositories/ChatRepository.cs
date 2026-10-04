using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace FreelancerStudent.API.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDBContext _context;

        public ChatRepository(ApplicationDBContext context)
        {
            _context = context;
        }


        //Tuấn
        public async Task<PhongChat> LayHoacTaoPhongChatAsync(int maUserClient, int maFreelancerStudent, string? maJob)
        {
            // 1. Tự động tìm kiếm nếu maFreelancerStudent được truyền vào là maUser của sinh viên
            var freeById = await _context.FreelamcerStudents.FirstOrDefaultAsync(f => f.maFreelancerStudents == maFreelancerStudent);
            if (freeById == null)
            {
                var freeByUser = await _context.FreelamcerStudents.FirstOrDefaultAsync(f => f.maUser == maFreelancerStudent);
                if (freeByUser != null)
                {
                    maFreelancerStudent = freeByUser.maFreelancerStudents;
                }
            }

            // 2. Nếu có maJob, luôn lấy chính xác maUser của Nhà tuyển dụng đã đăng bài Job này
            if (!string.IsNullOrEmpty(maJob))
            {
                var jobEntity = await _context.JobPosts.Include(j => j.NhaTuyenDung).FirstOrDefaultAsync(j => j.maJob == maJob);
                if (jobEntity?.NhaTuyenDung != null)
                {
                    maUserClient = jobEntity.NhaTuyenDung.maUser;
                }
            }
            else
            {
                // Kiểm tra nếu maUserClient truyền vào là maNhaTuyenDung thay vì maUser của NTD
                var ntdById = await _context.NhaTuyenDungs.FirstOrDefaultAsync(n => n.maNhaTuyenDung == maUserClient);
                if (ntdById != null)
                {
                    maUserClient = ntdById.maUser;
                }
            }

            // Kiểm tra xem đã có phòng chat giữa 2 người hay chưa
            var phongHienTai = await _context.PhongChats
                .Include(p => p.UserClient)
                .Include(p => p.FreelancerStudent).ThenInclude(f => f!.User)
                .Include(p => p.JobPost)
                .FirstOrDefaultAsync(p => p.maUserClient == maUserClient
                                       && p.maFreelancerStudent == maFreelancerStudent
                                       && (string.IsNullOrEmpty(maJob) || p.maJob == maJob));
            if (phongHienTai != null)
            {
                return phongHienTai;
            }
            // Nếu chưa có thì tạo mới
            var phongMoi = new PhongChat
            {
                maUserClient = maUserClient,
                maFreelancerStudent = maFreelancerStudent,
                maJob = maJob,
                ngayTao = DateTime.Now,
                trangThai = "Active"
            };
            await _context.PhongChats.AddAsync(phongMoi);
            await _context.SaveChangesAsync();

            // Load lại đầy đủ thông tin liên kết
            return await _context.PhongChats
                .Include(p => p.UserClient)
                .Include(p => p.FreelancerStudent).ThenInclude(f => f!.User)
                .Include(p => p.JobPost)
                .FirstAsync(p => p.maPhongChat == phongMoi.maPhongChat);
        }


        //Tuấn
        //Lấy danh sách các phòng chat theo maUser
        public async Task<List<PhongChat>> LayDanhSachPhongChatTheoUserAsync(int maUser)
        {
            // Lấy maFreelancerStudents nếu user này là Freelancer
            var freelancer = await _context.FreelamcerStudents.FirstOrDefaultAsync(f => f.maUser == maUser);
            int maFree = freelancer?.maFreelancerStudents ?? 0;
            return await _context.PhongChats
                .Include(p => p.UserClient)
                .Include(p => p.FreelancerStudent).ThenInclude(f => f!.User)
                .Include(p => p.JobPost)
                .Include(p => p.TinNhanChats)
                .Where(p => p.maUserClient == maUser || (maFree > 0 && p.maFreelancerStudent == maFree))
                .OrderByDescending(p => p.TinNhanChats.Max(m => (DateTime?)m.ngayGui) ?? p.ngayTao)
                .ToListAsync();
        }


        //Lấy thông tin 1 phòng chat theo ID
        public async Task<PhongChat?> LayPhongChatTheoMaAsync(int maPhongChat)
        {
            return await _context.PhongChats
                .Include(p => p.UserClient)
                .Include(p => p.FreelancerStudent).ThenInclude(f => f!.User)
                .Include(p => p.JobPost)
                .FirstOrDefaultAsync(p => p.maPhongChat == maPhongChat);
        }

        //Lấy lịch sử tin nhắn của 1 phòng
        //  Lấy lịch sử tin nhắn của 1 phòng
        public async Task<List<TinNhanChat>> LayLichSuTinNhanAsync(int maPhongChat)
        {
            return await _context.TinNhanChats
                .Include(t => t.NguoiGui)
                .Where(t => t.maPhongChat == maPhongChat)
                .OrderBy(t => t.ngayGui)
                .ToListAsync();
        }


        // Lưu tin nhắn mới
        public async Task<TinNhanChat> LuuTinNhanAsync(TinNhanChat tinNhan)
        {
            await _context.TinNhanChats.AddAsync(tinNhan);
            await _context.SaveChangesAsync();
            // Load lại thông tin người gửi
            await _context.Entry(tinNhan).Reference(t => t.NguoiGui).LoadAsync();
            return tinNhan;
        }
        // Đánh dấu đã đọc
        public async Task DanhDauDaDocAsync(int maPhongChat, int maUserDangXem)
        {
            var tinNhanChuaDocs = await _context.TinNhanChats
                .Where(t => t.maPhongChat == maPhongChat && t.maNguoiGui != maUserDangXem && !t.daDoc)
                .ToListAsync();
            if (tinNhanChuaDocs.Any())
            {
                foreach (var tin in tinNhanChuaDocs)
                {
                    tin.daDoc = true;
                }
                await _context.SaveChangesAsync();
            }
        }

    }

}