using System;
using System.Collections.Generic;
using MOCK.Models.Entities;

namespace MOCK.Models.ViewModels
{
    public class NghiemThuSanPhamViewModel
    {
        public HopDong Contract { get; set; } = new();
        public JobPost Job { get; set; } = new();
        public FreelancerStudent Freelancer { get; set; } = new();
        public User FreelancerUser { get; set; } = new();
        public NhaTuyenDung Employer { get; set; } = new();
        public User EmployerUser { get; set; } = new();

        public BanGiao_SanPham? LatestBanGiao { get; set; }
        public List<BanGiao_SanPham> LichSuBanGiao { get; set; } = new();
    }
}
