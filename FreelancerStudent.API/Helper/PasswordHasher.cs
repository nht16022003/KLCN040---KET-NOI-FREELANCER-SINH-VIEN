//Cài thư viện dotnet add package BCrypt.Net-Next
namespace FreelancerStudent.API.Helper
{
    public static class PasswordHasher
    {
        //1. Dùng băm mật khẩu khi đăng ký
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        //2. Kiểm tra mật khẩu khi đăng nhập

        public static bool VerifyPassword(string password, string matKhauDaHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, matKhauDaHash);
            }
            catch
            {
                return false;
            }
        }
    }
}