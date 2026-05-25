using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBH.DAL.Entities
{
    [Table("PhieuNhap")]
    public class PhieuNhap
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PhieuNhapID { get; set; }

        [ForeignKey(nameof(NhanVien))]
        public int NhanVienID { get; set; }
        public virtual NhanVien NhanVien { get; set; } = null!;

        public DateTime NgayNhap { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [MaxLength(1000)]
        public string GhiChu { get; set; } = string.Empty;

        public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
    }
}
