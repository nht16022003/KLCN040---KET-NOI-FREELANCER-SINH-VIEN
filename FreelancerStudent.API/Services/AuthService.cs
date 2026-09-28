using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Helper;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository; //Sử dụng để gọi các dữ liệu đucợ laays từ database để service xử lý
        private readonly INhaTuyenDungRepository _nhaTuyenDungRepository;
        private readonly IWalletRepository _walletRepository;

        public AuthService(IUserRepository userRepository, INhaTuyenDungRepository nhaTuyenDungRepository,
        IWalletRepository walletRepository)
        {
            _userRepository = userRepository;
            _nhaTuyenDungRepository = nhaTuyenDungRepository;
            _walletRepository = walletRepository;
        }

        public async Task<DangKy_ReponseDTO> DangKyTaiKhoanAsync(DangKy_RequestDTO request)
        {
            //Kiểm tra mật khẩu có trùng không
            if (request.password != request.xacnhanmatkhau)
            {
                throw new Exception("Mật khẩu xác nhận không khớp với mật khẩu đã nhập");
            }

            //Kiểm tra email đã được đăng ký chưa
            var emailDaTonTai = await _userRepository.kiemTraTonTaiEmailAsync(request.email);
            if (emailDaTonTai)
            {
                throw new Exception("Email này đã được sử dụng. Vui lòng nhập email khác");
            }

            //Kiểm tra tên tài khoản đã tồn tại hay chưa
            var taikhoanTonTai = await _userRepository.kiemTraTenTaiKhoanTonTaiChuaAsync(request.tentaikhoan);
            if (taikhoanTonTai)
            {
                throw new Exception("Tài khoản này đã được sử dụng. Vui lòng nhập lại!");
            }

            //Hash mật khẩu bằng Helper
            string matKhauMoi = PasswordHasher.HashPassword(request.password);


            //Sau khi kiểm tra đúng hết thì lưu xuống database
            var userMoi = new Users
            {
                hotenUser = request.hovaten.Trim(),
                tenTaiKhoanUser = request.tentaikhoan.Trim().ToLower(),
                emailUser = request.email.Trim().ToLower(),
                sdtUser = request.sodienthoai!.Trim(),
                pashWordHash = matKhauMoi,
                maRole = request.marole,
                status = "ACTIVE",
                ngayTao = DateTime.UtcNow
            };



            //sỬ DỤNG REPO THÊM VÀO DATABASE
            var luuUser = await _userRepository.themUserAsync(userMoi);

            if (luuUser.maRole == 2)
            {
                await _nhaTuyenDungRepository.themNhaTuyenDungDuaVaoMaRoleCuaUsers(luuUser, luuUser.maRole);
                await _walletRepository.themViChoUserTheoMaUser(luuUser.maUser);
            }

            var tenRole = await _userRepository.layTenRoleTheoUser(luuUser, luuUser.maRole);

            //Trả về kết quả đã lưu
            var traKetQuaDangKy = new DangKy_ReponseDTO
            {
                hovaten = luuUser.hotenUser,
                tentaikhoan = luuUser.tenTaiKhoanUser,
                email = luuUser.emailUser,
                sodienthoai = luuUser.sdtUser,
                marole = luuUser.maRole,
                tenrole = tenRole,
                ngaytao = luuUser.ngayTao
            };

            return traKetQuaDangKy;
        }

        public async Task<DangNhap_ReponseDTO> DangNhapTaiKhoanAsync(DangNhap_RequestDTO request)
        {
            //Kiểm tra xem request.tenTaikhoan có giống với tên trong csdl không
            var user = await _userRepository.timUserTheoTenTaiKhoan(request.tentaikhoan_email);

            if (user == null)
            {
                throw new Exception("Tên đăng nhập hoặc mật khẩu không chính xác!");
            }

            //Kiểm tra trạng thái tài khoản
            if (user.status != "ACTIVE")
            {
                throw new Exception("Tài khoản hiện đang bị khóa! Vui lòng liên hệ admin");
            }

            //Kiểm tra mật khẩu
            bool matKhauGiaiMa = PasswordHasher.VerifyPassword(request.password, user.pashWordHash);
            if (!matKhauGiaiMa)
            {
                throw new Exception("Tên đăng nhập hoặc mật khẩu không chính xác!");
            }

            var tenRole = await _userRepository.layTenRoleTheoUser(user, user.maRole);

            //Trả kết quả
            var ketquadangnhap = new DangNhap_ReponseDTO
            {
                maUser = user.maUser,
                hovaten = user.hotenUser,
                tentaikhoan = user.tenTaiKhoanUser,
                email = user.emailUser,
                sodienthoai = user.sdtUser,
                marole = user.maRole,
                tenrole = tenRole,
                status = user.status
            };

            return ketquadangnhap;
        }

    }
}