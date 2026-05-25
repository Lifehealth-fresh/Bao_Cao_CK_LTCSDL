using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBH.DAL.Entities
{
    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int KhachHangID { get; set; }

        [ForeignKey(nameof(TaiKhoan))]
        public int TaiKhoanID { get; set; }
        public virtual TaiKhoan TaiKhoan { get; set; } = null!;

        [MaxLength(100)]
        public string HoTen { get; set; } = string.Empty;

        public DateTime? NgaySinh { get; set; }

        [MaxLength(10)]
        public string GioiTinh { get; set; } = string.Empty; // 'Nam', 'Nu', 'Khac'

        [MaxLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        [MaxLength(300)]
        public string DiaChiMacDinh { get; set; } = string.Empty;

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        // Navigation collections
        public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();
        public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
    }
}
