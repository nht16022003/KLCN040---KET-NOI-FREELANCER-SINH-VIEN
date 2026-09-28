using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class WalletRepository : IWalletRepository
    {

        private readonly ApplicationDBContext _context;

        public WalletRepository(ApplicationDBContext context)
        {
            _context = context;

        }

        //Lấy ví theo mã user
        public async Task<Wallet> layViTheoMaUser(int maUser)
        {
            var ketqua = await _context.Wallets.Include(w => w.User).FirstOrDefaultAsync(w => w.maUser == maUser);
            return ketqua!;
        }

        public async Task<Wallet> themViChoUserTheoMaUser(int maUser)
        {
            //Kiểm tra xem user có ví chưa
            var kiemTraVi = await _context.Wallets.FirstOrDefaultAsync(w => w.maUser == maUser);
            if (kiemTraVi != null)
            {
                return kiemTraVi; // Đã có ví thì trả về ví hiện tại, không tạo trùng
            }

            var viMoi = new Wallet
            {
                maUser = maUser,
                soDuKhaDung = 0,
                soDuDongBang = 0
            };

            //Lưu vào Database
            await _context.Wallets.AddAsync(viMoi);
            await _context.SaveChangesAsync();

            return viMoi;
        }
    }
}