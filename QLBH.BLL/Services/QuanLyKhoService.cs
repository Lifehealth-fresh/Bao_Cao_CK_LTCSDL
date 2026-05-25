using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLBH.BLL.Constants;
using QLBH.BLL.DTOs;
using QLBH.DAL.Data;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QLBH.BLL.Services
{
    public class QuanLyKhoService : IQuanLyKhoService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<QuanLyKhoService> _logger;

        public QuanLyKhoService(AppDbContext context, ILogger<QuanLyKhoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<PhieuNhapDto>> GetPhieuNhapAsync(DateTime? tuNgay, DateTime? denNgay, int pageIndex, int pageSize)
        {
            // Gọi stored procedure sp_GetPhieuNhap, trả về kết quả dạng PhieuNhapDto
            var sql = "EXEC sp_GetPhieuNhap @TuNgay = {0}, @DenNgay = {1}, @PageIndex = {2}, @PageSize = {3}";
            var items = await _context.Database
                .SqlQueryRaw<PhieuNhapDto>(sql,
                    tuNgay ?? (object)DBNull.Value,
                    denNgay ?? (object)DBNull.Value,
                    pageIndex,
                    pageSize)
                .ToListAsync();

            // Đếm tổng số phiếu nhập (dùng Entity) để phân trang
            var query = _context.PhieuNhaps.AsQueryable();
            if (tuNgay.HasValue) query = query.Where(p => p.NgayNhap.Date >= tuNgay.Value);
            if (denNgay.HasValue) query = query.Where(p => p.NgayNhap.Date <= denNgay.Value);
            var totalCount = await query.CountAsync();

            return new PagedResult<PhieuNhapDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<PhieuNhapDto?> GetPhieuNhapByIdAsync(int id)
        {
            // Lấy phiếu nhập bằng Entity Framework (không cần DTO)
            var entity = await _context.PhieuNhaps
                .Include(p => p.NhanVien)
                .FirstOrDefaultAsync(p => p.PhieuNhapID == id);
            if (entity == null) return null;

            return new PhieuNhapDto
            {
                PhieuNhapID = entity.PhieuNhapID,
                NgayNhap = entity.NgayNhap,
                TongTien = entity.TongTien,
                GhiChu = entity.GhiChu,
                TenNhanVien = entity.NhanVien?.HoTen ?? "Không rõ",
                SoLuongMatHang = await _context.ChiTietPhieuNhaps.CountAsync(ct => ct.PhieuNhapID == id)
            };
        }

        public async Task<List<ChiTietPhieuNhapDto>> GetChiTietPhieuNhapAsync(int phieuNhapId)
        {
            var sql = "EXEC sp_GetChiTietPhieuNhap @PhieuNhapID = {0}";
            var result = await _context.Database
                .SqlQueryRaw<ChiTietPhieuNhapDto>(sql, phieuNhapId)
                .ToListAsync();
            return result;
        }

        public async Task<int> TaoPhieuNhapAsync(TaoPhieuNhapDto model, int nhanVienId)
        {
            if (model.ChiTiet == null || !model.ChiTiet.Any())
                throw new ApplicationException("Vui lòng thêm ít nhất một sản phẩm.");

            var bienTheIds = model.ChiTiet.Select(x => x.BienTheID).Distinct().ToList();
            var bienTheKhongHopLe = await _context.BienTheSanPhams
                .Include(x => x.SanPham)
                .Where(x => bienTheIds.Contains(x.BienTheID))
                .Where(x => !x.TrangThai
                    || x.SanPham.TrangThai == SanPhamTrangThai.Ngung
                    || x.SanPham.TrangThai == SanPhamTrangThai.HetHang)
                .Select(x => x.BienTheID)
                .ToListAsync();

            if (bienTheKhongHopLe.Any())
            {
                throw new ApplicationException(
                    $"Biến thể không hợp lệ hoặc đã ngưng hoạt động: {string.Join(", ", bienTheKhongHopLe)}");
            }

            // Tạo DataTable để truyền vào stored procedure sp_NhapKho
            var dt = new DataTable();
            dt.Columns.Add("BienTheID", typeof(int));
            dt.Columns.Add("SoLuong", typeof(int));
            dt.Columns.Add("GiaNhap", typeof(decimal));

            foreach (var ct in model.ChiTiet)
            {
                dt.Rows.Add(ct.BienTheID, ct.SoLuong, ct.GiaNhap);
            }

            var paramList = new SqlParameter("@ChiTietList", SqlDbType.Structured)
            {
                TypeName = "dbo.ChiTietPhieuNhapType",
                Value = dt
            };
            var paramNhanVien = new SqlParameter("@NhanVienID", nhanVienId);
            var ghiChu = string.IsNullOrWhiteSpace(model.GhiChu) ? string.Empty : model.GhiChu.Trim();
            var paramGhiChu = new SqlParameter("@GhiChu", ghiChu);
            var paramOutput = new SqlParameter("@PhieuNhapID", SqlDbType.Int) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_NhapKho @NhanVienID, @GhiChu, @ChiTietList, @PhieuNhapID OUTPUT",
                paramNhanVien, paramGhiChu, paramList, paramOutput);

            return Convert.ToInt32(paramOutput.Value);
        }
    }
}
