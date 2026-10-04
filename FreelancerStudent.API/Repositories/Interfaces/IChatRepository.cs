using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IChatRepository
    {
        //Tuấn
        //Tìm hoặc tạo phòng chat giữa NTD và Freelancer
        Task<PhongChat> LayHoacTaoPhongChatAsync(int maUserClient, int maFreelancerStudent, string? maJob);
        //Tuấn
        //Lấy danh sách các phòng chat theo maUser (dù là NTD hay Freelancer)
        Task<List<PhongChat>> LayDanhSachPhongChatTheoUserAsync(int maUser);

        //Tuấn
        //Lấy thông tin chi tiết 1 phòng chat
        Task<PhongChat?> LayPhongChatTheoMaAsync(int maPhongChat);

        //Tuấn
        //Lấy toàn bộ lịch sử tin nhắn trong 1 phòng chat
        Task<List<TinNhanChat>> LayLichSuTinNhanAsync(int maPhongChat);

        //Tuấn
        //Lưu tin nhắn mới vào database
        Task<TinNhanChat> LuuTinNhanAsync(TinNhanChat tinNhan);

        //Tuấn
        //Đánh dấu đã đọc cho các tin nhắn được gửi đến cho maUser
        Task DanhDauDaDocAsync(int maPhongChat, int maUserDangXem);
    }
}
