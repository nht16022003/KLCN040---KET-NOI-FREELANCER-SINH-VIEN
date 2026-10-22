using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.Repositories.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class ThongBaoService : IThongBaoService
    {
        private readonly IThongBaoRepository _repository;

        public ThongBaoService(IThongBaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ThongBaoDanhSach_ReponseDTO> LayDanhSachAsync(int maUser, int trang, int kichThuocTrang)
        {
            if (maUser <= 0)
                throw new ArgumentException("Mã người dùng không hợp lệ.");
            trang = Math.Max(1, trang);
            kichThuocTrang = Math.Clamp(kichThuocTrang, 1, 100);
            var (items, total, unread) = await _repository.LayDanhSachAsync(maUser, trang, kichThuocTrang);
            return new ThongBaoDanhSach_ReponseDTO
            {
                trang = trang,
                kichThuocTrang = kichThuocTrang,
                tongSoThongBao = total,
                soThongBaoChuaDoc = unread,
                danhSach = items.Select(t => new ThongBao_ReponseDTO
                {
                    maThongBao = t.maThongBao,
                    tieuDe = t.tieuDe,
                    noiDung = t.noiDung,
                    loaiThongBao = t.loaiThongBao,
                    linkDieuHuong = t.linkDieuHuong,
                    daDoc = t.daDoc,
                    ngayTao = t.ngayTao
                }).ToList()
            };
        }

        public Task<int> DemChuaDocAsync(int maUser)
        {
            if (maUser <= 0)
                throw new ArgumentException("Mã người dùng không hợp lệ.");
            return _repository.DemChuaDocAsync(maUser);
        }

        public Task<bool> DanhDauDaDocAsync(int maThongBao, int maUser)
        {
            if (maThongBao <= 0 || maUser <= 0)
                throw new ArgumentException("Thông tin thông báo không hợp lệ.");
            return _repository.DanhDauDaDocAsync(maThongBao, maUser);
        }

        public Task<int> DanhDauTatCaDaDocAsync(int maUser)
        {
            if (maUser <= 0)
                throw new ArgumentException("Mã người dùng không hợp lệ.");
            return _repository.DanhDauTatCaDaDocAsync(maUser);
        }
    }
}
