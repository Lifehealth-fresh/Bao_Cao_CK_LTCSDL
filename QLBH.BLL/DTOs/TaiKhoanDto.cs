using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QLBH.BLL.DTOs
{

    public class TaiKhoanDto
    {
        public int TaiKhoanID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public bool TrangThai { get; set; }
    }

    public class ThongTinCaNhanDto
    {
        public int TaiKhoanID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public bool TrangThai { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string? DiaChi { get; set; }

        // === Thêm 2 dòng này ===
        public DateTime? NgayVaoLam { get; set; }   // Ngày vào làm (chỉ có với nhân viên)
        public string? MaNhanVien { get; set; }     // Mã nhân viên (lấy từ NhanVienID)
    }

    public class DangKyRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
    }

    public class CapNhatThongTinDto
    {
        public int TaiKhoanID { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string SoDienThoai { get; set; } = string.Empty;

        public string? DiaChi { get; set; } 
    }

    public class DoiMatKhauDto
    {
        public int TaiKhoanID { get; set; }
        public string MatKhauCu { get; set; } = string.Empty;
        public string MatKhauMoi { get; set; } = string.Empty;
    }

}