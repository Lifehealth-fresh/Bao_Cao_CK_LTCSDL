using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBH.DAL.Entities
{
    [Table("SanPham")]
    public class SanPham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SanPhamID { get; set; }

        [ForeignKey(nameof(DanhMuc))]
        public int DanhMucID { get; set; }
        public virtual DanhMuc DanhMuc { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string TenSanPham { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaCoBan { get; set; }

        [MaxLength(500)]
        public string HinhAnh { get; set; } = "/images/noimage.jpg";

        public string? MoTa { get; set; }

        [MaxLength(100)]
        public string ThuongHieu { get; set; } = string.Empty;

        [MaxLength(200)]
        public string ChatLieu { get; set; } = string.Empty;

        [MaxLength(500)]
        public string HuongDanBaoQuan { get; set; } = string.Empty;

        [MaxLength(20)]
        public string TrangThai { get; set; } = "HoatDong"; // 'HoatDong', 'Ngung', 'HetHang'

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        public virtual ICollection<BienTheSanPham> BienTheSanPhams { get; set; } = new List<BienTheSanPham>();
    }
}