namespace FreelancerStudent.Web.ViewModels
{
    public class DanhGiaRequestViewModel
    {
        public string maHD { get; set; } = string.Empty;
        public int soSaoTongThe { get; set; }
        public byte? soSaoChatLuong { get; set; }
        public byte? soSaoDungHan { get; set; }
        public byte? soSaoGiaoTiep { get; set; }
        public string? nhanXet { get; set; }
    }

    public class DanhGiaItemViewModel
    {
        public int maDanhGia { get; set; }
        public string maHD { get; set; } = string.Empty;
        public string tenNguoiDanhGia { get; set; } = string.Empty;
        public string? avatar { get; set; }
        public string tenCongViec { get; set; } = string.Empty;
        public int soSaoTongThe { get; set; }
        public byte? soSaoChatLuong { get; set; }
        public byte? soSaoDungHan { get; set; }
        public byte? soSaoGiaoTiep { get; set; }
        public string? nhanXet { get; set; }
        public DateTime ngayDanhGia { get; set; }
        public DateTime? ngayCapNhat { get; set; }
        public bool daChinhSua { get; set; }
        public bool coTheChinhSua { get; set; }
        public DateTime? hanChinhSua { get; set; }
    }

    public class DanhGiaFormViewModel
    {
        public string maHD { get; set; } = string.Empty;
        public string tenDoiTac { get; set; } = string.Empty;
        public string tenCongViec { get; set; } = string.Empty;
        public string vaiTroNguoiDanhGia { get; set; } = string.Empty;
        public string trangThaiDanhGia { get; set; } = "ChoDanhGia";
        public bool coTheDanhGia { get; set; }
        public DateTime? hanDanhGia { get; set; }
        public DanhGiaItemViewModel? danhGiaHienTai { get; set; }
        public string? thongBaoLoi { get; set; }
    }

    public class DanhGiaThongKeViewModel
    {
        public double diemTrungBinh { get; set; }
        public int tongLuotDanhGia { get; set; }
        public double trungBinhChatLuong { get; set; }
        public double trungBinhDungHan { get; set; }
        public double trungBinhGiaoTiep { get; set; }
        public List<int> phanBoSao { get; set; } = new();
        public double tyLeHaiLong { get; set; }
        public int trang { get; set; }
        public int kichThuocTrang { get; set; }
        public int tongSoTrang { get; set; }
        public List<DanhGiaItemViewModel> danhSach { get; set; } = new();
    }

    public class HopDongDanhGiaViewModel
    {
        public string maHD { get; set; } = string.Empty;
        public string tenCongViec { get; set; } = string.Empty;
        public string tenDoiTac { get; set; } = string.Empty;
        public string trangThaiHopDong { get; set; } = string.Empty;
        public string trangThaiDanhGia { get; set; } = "ChoDanhGia";
        public int? maDanhGia { get; set; }
        public bool coTheDanhGia { get; set; }
        public bool coTheChinhSuaDanhGia { get; set; }
        public DateTime? hanDanhGia { get; set; }
    }

    public class CongViecDaHoanThanhViewModel
    {
        public List<HopDongDanhGiaViewModel> DanhSach { get; set; } = new();
        public bool LaFreelancer { get; set; }
        public string? Loi { get; set; }
    }
}
