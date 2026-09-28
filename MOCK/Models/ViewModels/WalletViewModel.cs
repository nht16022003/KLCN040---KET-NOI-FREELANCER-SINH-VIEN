using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class WalletViewModel
    {
        public User User { get; set; } = new();
        public Wallet Wallet { get; set; } = new();
        public decimal TongTaiSan => Wallet.SoDuKhaDung + Wallet.SoDuDongBang;
        
        // Thống kê tổng quan
        public decimal TongNapTien { get; set; }
        public decimal TongRutTien { get; set; }
        public decimal TongDaThanhToanEscrow { get; set; }
        public decimal TongNhanTien { get; set; }

        // Giao dịch gần đây
        public List<LichSuGiaoDich> RecentTransactions { get; set; } = new();
        
        // Hợp đồng đang ký quỹ/đóng băng tiền
        public List<HopDong> DangKyQuyContracts { get; set; } = new();
    }
}
