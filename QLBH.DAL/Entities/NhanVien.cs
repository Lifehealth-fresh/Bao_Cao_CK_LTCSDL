using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBH.DAL.Entities
{
    [Table("NhanVien")]
    public class NhanVien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NhanVienID { get; set; }

        [ForeignKey(nameof(TaiKhoan))]
        public int TaiKhoanID { get; set; }
        public virtual TaiKhoan TaiKhoan { get; set; } = null!;

        [MaxLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [MaxLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        public DateTime? NgayVaoLam { get; set; }

        public bool TrangThai { get; set; } = true;

        // Navigation collections
        public virtual ICollection<PhieuNhap> PhieuNhaps { get; set; } = new List<PhieuNhap>();
        public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
    }
}