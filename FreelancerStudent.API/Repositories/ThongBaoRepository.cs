using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class ThongBaoRepository : IThongBaoRepository
    {
        private readonly ApplicationDBContext _context;

        public ThongBaoRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<(List<ThongBao> items, int total, int unread)> LayDanhSachAsync(int maUser, int trang, int kichThuocTrang)
        {
            var query = _context.ThongBaos.AsNoTracking().Where(t => t.maUser == maUser);
            var total = await query.CountAsync();
            var unread = await query.CountAsync(t => !t.daDoc);
            var items = await query.OrderByDescending(t => t.ngayTao)
                .Skip((trang - 1) * kichThuocTrang).Take(kichThuocTrang).ToListAsync();
            return (items, total, unread);
        }

        public Task<int> DemChuaDocAsync(int maUser)
            => _context.ThongBaos.CountAsync(t => t.maUser == maUser && !t.daDoc);

        public async Task<bool> DanhDauDaDocAsync(int maThongBao, int maUser)
        {
            var item = await _context.ThongBaos.FirstOrDefaultAsync(t => t.maThongBao == maThongBao && t.maUser == maUser);
            if (item == null)
                return false;
            if (!item.daDoc)
            {
                item.daDoc = true;
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<int> DanhDauTatCaDaDocAsync(int maUser)
        {
            var items = await _context.ThongBaos.Where(t => t.maUser == maUser && !t.daDoc).ToListAsync();
            items.ForEach(t => t.daDoc = true);
            return items.Count == 0 ? 0 : await _context.SaveChangesAsync();
        }
    }
}
