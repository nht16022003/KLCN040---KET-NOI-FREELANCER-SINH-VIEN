using System.Reflection.Metadata.Ecma335;
using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Helper;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository; //Sử dụng để gọi các dữ liệu đucợ laays từ database để service xử lý

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }


        //
        public async Task<Wallet_ReponseDTO> layViTheoMaUsersAsync(Wallet_RequestDTO request)
        {
            var ketqua = await _walletRepository.layViTheoMaUser(request.maUser);

            // 1. Kiểm tra nếu không tìm thấy ví
            if (ketqua == null)
            {
                return null!;
            }

            var ketqua_Moi = new Wallet_ReponseDTO
            {
                maWallet = ketqua.maWallet,
                tenUsers = ketqua.User?.tenTaiKhoanUser,
                soDuKhaDung = ketqua.soDuKhaDung,
                soDuDongBang = ketqua.soDuDongBang
                /*
                
                  public int maWallet { get; set; }
                    public string? tenUsers { get; set; }
                    public decimal? soDuKhaDung { get; set; } = 0;


                    public decimal? soDuDongBang { get; set; } = 0;

                */
            };
            return ketqua_Moi;
        }
    }
}