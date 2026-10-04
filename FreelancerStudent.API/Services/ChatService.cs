using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepo;

        public ChatService(IChatRepository chatRepo)
        {
            _chatRepo = chatRepo;
        }

        // 1. Tạo hoặc lấy phòng chat
        public async Task<PhongChat_ReponseDTO> TaoHoacLayPhongChatAsync(TaoPhongChat_RequestDTO request)
        {
            var phong = await _chatRepo.LayHoacTaoPhongChatAsync(request.maUserClient, request.maFreelancerStudent, request.maJob);
            return ChuyenSangPhongDTO(phong, request.maUserClient);
        }

        // 2. Lấy danh sách các phòng chat của User
        public async Task<List<PhongChat_ReponseDTO>> LayDanhSachPhongChatAsync(int maUser)
        {
            var danhSach = await _chatRepo.LayDanhSachPhongChatTheoUserAsync(maUser);
            return danhSach.Select(p => ChuyenSangPhongDTO(p, maUser)).ToList();
        }

        // 3. Lấy lịch sử tin nhắn & đánh dấu đã đọc
        public async Task<List<TinNhan_ReponseDTO>> LayLichSuTinNhanAsync(int maPhongChat, int maUserDangXem)
        {
            // Đánh dấu đã đọc các tin người khác gửi cho mình
            await _chatRepo.DanhDauDaDocAsync(maPhongChat, maUserDangXem);

            var tinNhans = await _chatRepo.LayLichSuTinNhanAsync(maPhongChat);
            return tinNhans.Select(t => new TinNhan_ReponseDTO
            {
                maTinNhan = t.maTinNhan,
                maPhongChat = t.maPhongChat,
                maNguoiGui = t.maNguoiGui,
                tenNguoiGui = t.NguoiGui?.hotenUser ?? "Người dùng",
                avatarNguoiGui = t.NguoiGui?.avatarUrl,
                noiDung = t.noiDung,
                fileDinhKem = t.fileDinhKem,
                loaiTinNhan = t.loaiTinNhan,
                daDoc = t.daDoc,
                ngayGui = t.ngayGui
            }).ToList();
        }

        // 4. Gửi tin nhắn mới
        public async Task<TinNhan_ReponseDTO> GuiTinNhanAsync(GuiTinNhan_RequestDTO request)
        {
            var tinMoi = new TinNhanChat
            {
                maPhongChat = request.maPhongChat,
                maNguoiGui = request.maNguoiGui,
                noiDung = request.noiDung,
                fileDinhKem = request.fileDinhKem,
                loaiTinNhan = request.loaiTinNhan ?? "Text",
                daDoc = false,
                ngayGui = DateTime.Now
            };

            var saved = await _chatRepo.LuuTinNhanAsync(tinMoi);

            return new TinNhan_ReponseDTO
            {
                maTinNhan = saved.maTinNhan,
                maPhongChat = saved.maPhongChat,
                maNguoiGui = saved.maNguoiGui,
                tenNguoiGui = saved.NguoiGui?.hotenUser ?? "Người dùng",
                avatarNguoiGui = saved.NguoiGui?.avatarUrl,
                noiDung = saved.noiDung,
                fileDinhKem = saved.fileDinhKem,
                loaiTinNhan = saved.loaiTinNhan,
                daDoc = saved.daDoc,
                ngayGui = saved.ngayGui
            };
        }

        // Hàm phụ trợ chuyển Entity -> PhongChat_ReponseDTO
        private PhongChat_ReponseDTO ChuyenSangPhongDTO(PhongChat p, int currentUserId)
        {
            var tinCuoi = p.TinNhanChats?.OrderByDescending(t => t.ngayGui).FirstOrDefault();
            int soTinChuaDoc = p.TinNhanChats?.Count(t => t.maNguoiGui != currentUserId && !t.daDoc) ?? 0;

            return new PhongChat_ReponseDTO
            {
                maPhongChat = p.maPhongChat,
                maUserClient = p.maUserClient,
                tenClient = p.UserClient?.hotenUser ?? "Nhà tuyển dụng",
                avatarClient = p.UserClient?.avatarUrl,
                maFreelancerStudent = p.maFreelancerStudent,
                maUserFreelancer = p.FreelancerStudent?.maUser ?? 0,
                tenFreelancer = p.FreelancerStudent?.User?.hotenUser ?? "Sinh viên Freelancer",
                avatarFreelancer = p.FreelancerStudent?.User?.avatarUrl,
                maJob = p.maJob,
                tieuDeJob = p.JobPost?.tieude,
                thulaoJob = p.JobPost?.thulao,
                tinNhanCuoi = tinCuoi?.noiDung ?? (tinCuoi?.fileDinhKem != null ? "[Tệp đính kèm]" : "Bắt đầu cuộc trò chuyện"),
                thoiGianTinNhanCuoi = tinCuoi?.ngayGui ?? p.ngayTao,
                soTinNhanChuaDoc = soTinChuaDoc,
                trangThai = p.trangThai,
                ngayTao = p.ngayTao
            };
        }
    }
}
