using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QLBH.BLL.DTOs
{
    public class PhieuNhapDto
    {
        public int PhieuNhapID { get; set; }
        public DateTime NgayNhap { get; set; }
        public decimal TongTien { get; set; }
        public string? GhiChu { get; set; }
        public string TenNhanVien { get; set; } = string.Empty;
        public int SoLuongMatHang { get; set; }
    }

    public class ChiTietPhieuNhapDto
    {
        public int ChiTietID { get; set; }
        public int BienTheID { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string MauSac { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal ThanhTien { get; set; }
        public decimal GiaBanHienTai { get; set; }
    }

    public class TaoPhieuNhapDto
    {
        [MaxLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
        public string? GhiChu { get; set; }
        public List<ChiTietNhapDto> ChiTiet { get; set; } = new();
    }

    public class ChiTietNhapDto
    {
        public int BienTheID { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaNhap { get; set; }
    }

    // Nếu chưa có PagedResult ở nơi khác, thêm vào đây

}
