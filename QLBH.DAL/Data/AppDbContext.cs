using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLBH.DAL.Entities;

namespace QLBH.DAL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<BienTheSanPham> BienTheSanPhams { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }
        public DbSet<PhieuNhap> PhieuNhaps { get; set; }
        public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình decimal(18,2) cho tất cả decimal properties
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            // ========== TaiKhoan ==========
            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.ToTable(t => t.HasCheckConstraint("CK_TaiKhoan_VaiTro", "[VaiTro] IN ('KhachHang', 'NhanVien')"));
            });

            // ========== KhachHang ==========
            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasIndex(e => e.TaiKhoanID).IsUnique();
                entity.ToTable(t => t.HasCheckConstraint("CK_KhachHang_GioiTinh", "[GioiTinh] IN ('Nam', 'Nu', 'Khac')"));

                entity.HasOne(k => k.TaiKhoan)
                      .WithOne(t => t.KhachHang)
                      .HasForeignKey<KhachHang>(k => k.TaiKhoanID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== NhanVien ==========
            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.HasIndex(e => e.TaiKhoanID).IsUnique();

                entity.HasOne(n => n.TaiKhoan)
                      .WithOne(t => t.NhanVien)
                      .HasForeignKey<NhanVien>(n => n.TaiKhoanID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== DanhMuc - SanPham ==========
            modelBuilder.Entity<SanPham>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_SanPham_TrangThai", "[TrangThai] IN ('HoatDong', 'Ngung', 'HetHang')"));
                entity.ToTable(t => t.HasCheckConstraint("CK_SanPham_GiaCoBan", "[GiaCoBan] >= 0"));

                entity.HasOne(s => s.DanhMuc)
                      .WithMany(d => d.SanPhams)
                      .HasForeignKey(s => s.DanhMucID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== BienTheSanPham ==========
            modelBuilder.Entity<BienTheSanPham>(entity =>
            {
                entity.HasIndex(e => e.SanPhamID);
                entity.ToTable(t => t.HasCheckConstraint("CK_BienTheSanPham_SoLuongTon", "[SoLuongTon] >= 0"));
                entity.ToTable(t => t.HasCheckConstraint("CK_BienTheSanPham_Gia", "[Gia] >= 0"));

                entity.HasOne(b => b.SanPham)
                      .WithMany(s => s.BienTheSanPhams)
                      .HasForeignKey(b => b.SanPhamID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== GioHang ==========
            modelBuilder.Entity<GioHang>(entity =>
            {
                entity.HasIndex(e => e.KhachHangID).IsUnique();

                entity.HasOne(g => g.KhachHang)
                      .WithMany(k => k.GioHangs)
                      .HasForeignKey(g => g.KhachHangID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== ChiTietGioHang ==========
            modelBuilder.Entity<ChiTietGioHang>(entity =>
            {
                entity.HasIndex(e => new { e.GioHangID, e.BienTheID }).IsUnique();
                entity.ToTable(t => t.HasCheckConstraint("CK_ChiTietGioHang_SoLuong", "[SoLuong] > 0"));

                entity.HasOne(ct => ct.GioHang)
                      .WithMany(g => g.ChiTietGioHangs)
                      .HasForeignKey(ct => ct.GioHangID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== Voucher ==========
            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.HasIndex(e => e.MaVoucher).IsUnique();
                entity.ToTable(t => t.HasCheckConstraint("CK_Voucher_LoaiGiam", "[LoaiGiam] IN ('PhanTram', 'TienMat')"));
                entity.ToTable(t => t.HasCheckConstraint("CK_Voucher_SoLuong", "[SoLuong] >= 0"));
                entity.ToTable(t => t.HasCheckConstraint("CK_Voucher_DaSuDung", "[DaSuDung] >= 0"));
            });

            // ========== DonHang ==========
            modelBuilder.Entity<DonHang>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_DonHang_TrangThai", "[TrangThai] IN ('ChoXuLy', 'DaThanhToan', 'DaHuy')"));
                entity.ToTable(t => t.HasCheckConstraint("CK_DonHang_TongTien", "[TongTien] >= 0"));

                entity.HasOne(d => d.KhachHang)
                      .WithMany(k => k.DonHangs)
                      .HasForeignKey(d => d.KhachHangID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Voucher)
                      .WithMany(v => v.DonHangs)
                      .HasForeignKey(d => d.VoucherID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ========== ChiTietDonHang ==========
            modelBuilder.Entity<ChiTietDonHang>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_ChiTietDonHang_SoLuong", "[SoLuong] > 0"));
                entity.ToTable(t => t.HasCheckConstraint("CK_ChiTietDonHang_DonGia", "[DonGia] >= 0"));

                entity.HasOne(ct => ct.DonHang)
                      .WithMany(d => d.ChiTietDonHangs)
                      .HasForeignKey(ct => ct.DonHangID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== ThanhToan ==========
            modelBuilder.Entity<ThanhToan>(entity =>
            {
                entity.HasIndex(e => e.DonHangID).IsUnique();
                entity.ToTable(t => t.HasCheckConstraint("CK_ThanhToan_TrangThai", "[TrangThai] IN ('ChoDuyet', 'DaDuyet', 'TuChoi')"));

                entity.HasOne(t => t.DonHang)
                      .WithMany(d => d.ThanhToans)
                      .HasForeignKey(t => t.DonHangID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.NhanVien)
                      .WithMany(n => n.ThanhToans)
                      .HasForeignKey(t => t.NhanVienID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== PhieuNhap ==========
            modelBuilder.Entity<PhieuNhap>(entity =>
            {
                entity.HasOne(p => p.NhanVien)
                      .WithMany(n => n.PhieuNhaps)
                      .HasForeignKey(p => p.NhanVienID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== ChiTietPhieuNhap ==========
            modelBuilder.Entity<ChiTietPhieuNhap>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_ChiTietPhieuNhap_SoLuong", "[SoLuong] > 0"));

                entity.HasOne(ct => ct.PhieuNhap)
                      .WithMany(p => p.ChiTietPhieuNhaps)
                      .HasForeignKey(ct => ct.PhieuNhapID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ct => ct.BienTheSanPham)
                      .WithMany(b => b.ChiTietPhieuNhaps)
                      .HasForeignKey(ct => ct.BienTheID)
                      .OnDelete(DeleteBehavior.Restrict);
            });


        }
    }
}