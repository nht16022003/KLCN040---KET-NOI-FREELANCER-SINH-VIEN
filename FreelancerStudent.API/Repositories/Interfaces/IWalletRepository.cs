using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IWalletRepository
    {
        //Lấy ví theo mã user
        Task<Wallet> layViTheoMaUser(int maUser);

        //Thêm ví cho User vừa được tạo theo mã user
        Task<Wallet> themViChoUserTheoMaUser(int maUser);
    }
}