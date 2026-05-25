using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QLBH.BLL.DTOs
{
    public class NhanVienDto
    {
        public int NhanVienID { get; set; }
        public int TaiKhoanID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public DateTime? NgayVaoLam { get; set; }
        public bool TrangThai { get; set; }
    }

    public class CreateNhanVienDto
    {
        public int TaiKhoanID { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public DateTime? NgayVaoLam { get; set; }
    }

    public class UpdateNhanVienDto : CreateNhanVienDto
    {
        public int NhanVienID { get; set; }
        public bool TrangThai { get; set; }
    }
    
}