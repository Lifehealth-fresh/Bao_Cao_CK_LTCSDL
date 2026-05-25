using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace QLBH.BLL.DTOs
{
    public class KhachHangDto
    {
        public int KhachHangID { get; set; }
        public int TaiKhoanID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; } = string.Empty; // Nam, Nu, Khac
        public string SoDienThoai { get; set; } = string.Empty;
        public string DiaChiMacDinh { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
    }

    public class UpdateKhachHangDto
    {
        public int KhachHangID { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string DiaChiMacDinh { get; set; } = string.Empty;
    }
}
