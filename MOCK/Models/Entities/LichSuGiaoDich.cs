using System;

namespace MOCK.Models.Entities
{
    public class LichSuGiaoDich
    {
        public int MaGiaoDich { get; set; }
        public int MaWallet { get; set; }
        public string LoaiGiaoDich { get; set; } = "NapTien"; // NapTien, RutTien, KyQuy, NhanTien, HoanTien, PhiDichVu
        public decimal SoTien { get; set; }
        public decimal SoDuTruoc { get; set; }
        public decimal SoDuSau { get; set; }
        public string? NoiDung { get; set; }
        public string? MaThamChieu { get; set; } // Mã hợp đồng, mã đơn nạp, mã hóa đơn
        public string? PhuongThuc { get; set; } // VietQR, VNPAY, MoMo, ChuyenKhoan, HeThong
        public string TrangThai { get; set; } = "ThanhCong"; // ThanhCong, DangXuLy, ThatBai, Huy
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
