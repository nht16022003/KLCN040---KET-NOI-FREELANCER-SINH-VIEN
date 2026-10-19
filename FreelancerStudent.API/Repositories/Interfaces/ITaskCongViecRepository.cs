using System.Collections.Generic;
using System.Threading.Tasks;
using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface ITaskCongViecRepository
    {
        Task<List<Task_CongViec>> LayDanhSachTaskTheoMaHDAsync(string maHD);
        Task<Task_CongViec?> LayTaskTheoMaAsync(int maTask);
        Task<Task_CongViec> ThemTaskAsync(Task_CongViec task);
        Task<bool> CapNhatTaskAsync(Task_CongViec task);
        Task<bool> XoaTaskAsync(int maTask);
    }
}
