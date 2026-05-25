using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBH.BLL.DTOs
{
        public class SanPhamDto
        {
            public int SanPhamID { get; set; }
            public int DanhMucID { get; set; }
            public string TenDanhMuc { get; set; } = string.Empty;
            public string TenSanPham { get; set; } = string.Empty;
            public decimal GiaCoBan { get; set; }
            public string HinhAnh { get; set; } = string.Empty;
            public string? MoTa { get; set; }
            public string ThuongHieu { get; set; } = string.Empty;
        public string? ChatLieu { get; set; }
        public string? HuongDanBaoQuan { get; set; }
            public string TrangThai { get; set; } = string.Empty;
            public DateTime NgayTao { get; set; }
            public DateTime NgayCapNhat { get; set; }
            public List<BienTheSanPhamDto> BienTheSanPhams { get; set; } = new();
        }

        public class CreateSanPhamDto
        {
            public int DanhMucID { get; set; }

            [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
            public string TenSanPham { get; set; } = string.Empty;
             
             [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
             public decimal GiaCoBan { get; set; }
            public string? HinhAnh { get; set; } 
            public string? MoTa { get; set; }
            [MaxLength(100)]
            public string ThuongHieu { get; set; } = string.Empty;
            [MaxLength(200)]

            public string? ChatLieu { get; set; }
            [MaxLength(500)]
            public string? HuongDanBaoQuan { get; set; } 
            public string TrangThai { get; set; } = "HoatDong";
        }

        public class UpdateSanPhamDto : CreateSanPhamDto
        {
            public int SanPhamID { get; set; }
            public DateTime? NgayCapNhat { get; set; }   // thay đổi thành nullable
    }

        
    
}