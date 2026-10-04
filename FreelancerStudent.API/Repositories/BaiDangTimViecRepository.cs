using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace FreelancerStudent.API.Repositories
{
    public class BaiDangTimViecRepository : IBaiDangTimViecRepository
    {
        private readonly ApplicationDBContext _context;



        public BaiDangTimViecRepository(ApplicationDBContext context)
        {
            _context = context;

        }

        //Tuấn
        public async Task<BaiDangTimViecFreelancerStudent> themBaiDangAsync(BaiDangTimViecFreelancerStudent baiDang)
        {
            await _context.BaiDangTimViecFreelancerStudents.AddAsync(baiDang);
            await _context.SaveChangesAsync();

            return baiDang;
        }

        //Tuấn
        public async Task<List<BaiDangTimViecFreelancerStudent>> layDanhSachTheoFreelancerAsync(int maFreelancerStudent)
        {
            return await _context.BaiDangTimViecFreelancerStudents.Include(b => b.FreelamcerStudents).ThenInclude(f => f!.User)
            .Include(b => b.FreelamcerStudents).ThenInclude(f => f!.ChuyenNganh).
            Where(b => b.maFreelancerStudent == maFreelancerStudent)
            .OrderByDescending(b => b.thoigiandang)
            .ToListAsync();
        }

        //Tuấn
        public async Task<List<BaiDangTimViecFreelancerStudent>> layTatCaBaiDangHienThiAsync()
        {
            return await _context.BaiDangTimViecFreelancerStudents
                .Include(b => b.FreelamcerStudents).ThenInclude(f => f!.User)
                .Include(b => b.FreelamcerStudents).ThenInclude(f => f!.ChuyenNganh)
                .Where(b => b.trangthai == "DangHienThi")
                .OrderByDescending(b => b.thoigiandang)
                .ToListAsync();
        }

        //Tuấn
        public async Task<BaiDangTimViecFreelancerStudent?> layTheoMaAsync(string maBaiDang)
        {
            return await _context.BaiDangTimViecFreelancerStudents
                .Include(b => b.FreelamcerStudents).ThenInclude(f => f!.User)
                .FirstOrDefaultAsync(b => b.maBaiDang == maBaiDang);
        }

        //Tuấn
        public async Task<bool> doiTrangThaiAsync(string maBaiDang, string trangthaiMoi)
        {
            var baiDang = await _context.BaiDangTimViecFreelancerStudents.FirstOrDefaultAsync(b => b.maBaiDang == maBaiDang);
            if (baiDang == null) return false;
            baiDang.trangthai = trangthaiMoi;
            await _context.SaveChangesAsync();
            return true;
        }

        //Tuấn
        public async Task<bool> xoaBaiDangAsync(string maBaiDang)
        {
            var baiDang = await _context.BaiDangTimViecFreelancerStudents.FirstOrDefaultAsync(b => b.maBaiDang == maBaiDang);
            if (baiDang == null) return false;
            _context.BaiDangTimViecFreelancerStudents.Remove(baiDang);
            await _context.SaveChangesAsync();
            return true;
        }

        //Tuấn
        public async Task<bool> capNhatBaiDangAsync(BaiDangTimViecFreelancerStudent baiDang)
        {
            _context.BaiDangTimViecFreelancerStudents.Update(baiDang);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}