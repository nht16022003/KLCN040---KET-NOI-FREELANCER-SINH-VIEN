using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class TaskCongViecRepository : ITaskCongViecRepository
    {
        private readonly ApplicationDBContext _context;

        public TaskCongViecRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<Task_CongViec>> LayDanhSachTaskTheoMaHDAsync(string maHD)
        {
            return await _context.Task_CongViecs
                .Include(t => t.GiaiDoan_HopDong)
                .Where(t => t.maHD == maHD)
                .OrderBy(t => t.hanHoanThanh)
                .ThenBy(t => t.maTask)
                .ToListAsync();
        }

        public async Task<Task_CongViec?> LayTaskTheoMaAsync(int maTask)
        {
            return await _context.Task_CongViecs
                .Include(t => t.GiaiDoan_HopDong)
                .Include(t => t.HopDong)
                    .ThenInclude(h => h!.JobPost)
                        .ThenInclude(j => j!.NhaTuyenDung)
                            .ThenInclude(n => n!.User)
                .Include(t => t.HopDong)
                    .ThenInclude(h => h!.FreelancerStudent)
                        .ThenInclude(f => f!.User)
                .FirstOrDefaultAsync(t => t.maTask == maTask);
        }

        public async Task<Task_CongViec> ThemTaskAsync(Task_CongViec task)
        {
            task.ngayCapNhat = DateTime.Now;
            await _context.Task_CongViecs.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> CapNhatTaskAsync(Task_CongViec task)
        {
            task.ngayCapNhat = DateTime.Now;
            _context.Task_CongViecs.Update(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> XoaTaskAsync(int maTask)
        {
            var task = await _context.Task_CongViecs.FirstOrDefaultAsync(t => t.maTask == maTask);
            if (task == null) return false;

            _context.Task_CongViecs.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
