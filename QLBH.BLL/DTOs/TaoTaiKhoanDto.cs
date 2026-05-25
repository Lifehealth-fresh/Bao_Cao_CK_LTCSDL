namespace QLBH.BLL.DTOs
{
    public class TaoTaiKhoanDto
    {
        public string Email { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty; // "KhachHang" hoặc "NhanVien"
        public bool TrangThai { get; set; } = true;
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
    }
}