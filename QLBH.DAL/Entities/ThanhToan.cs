using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBH.DAL.Entities
{
    [Table("ThanhToan")]
    public class ThanhToan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ThanhToanID { get; set; }

        [ForeignKey(nameof(DonHang))]
        public int DonHangID { get; set; }
        public virtual DonHang DonHang { get; set; } = null!;

        [ForeignKey(nameof(NhanVien))]
        public int? NhanVienID { get; set; }
        public virtual NhanVien? NhanVien { get; set; }

        [MaxLength(50)]
        public string PhuongThuc { get; set; } = "ChuyenKhoan";

        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTien { get; set; }

        [MaxLength(20)]
        public string TrangThai { get; set; } = "ChoDuyet"; // 'ChoDuyet', 'DaDuyet', 'TuChoi'

        public DateTime? NgayXuLy { get; set; }

        [MaxLength(500)]
        public string GhiChu { get; set; } = string.Empty;
    }
}
