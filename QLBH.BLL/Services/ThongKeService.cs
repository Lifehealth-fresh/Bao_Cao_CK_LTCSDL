using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLBH.BLL.DTOs;
using QLBH.DAL.Entities;
using QLBH.DAL.Repositories;
using QLBH.DAL.Helpers;

namespace QLBH.BLL.Services
{
    public class ThongKeService : IThongKeService
    {
        private readonly IRepository<DonHang> _donHangRepo;
        private readonly IRepository<ChiTietDonHang> _chiTietRepo;
        private readonly IRepository<BienTheSanPham> _bienTheRepo;
        private readonly DataSetHelper _dataSetHelper;
        private readonly ILogger<ThongKeService> _logger;

        public ThongKeService(
            IRepository<DonHang> donHangRepo,
            IRepository<ChiTietDonHang> chiTietRepo,
            IRepository<BienTheSanPham> bienTheRepo,
            DataSetHelper dataSetHelper,
            ILogger<ThongKeService> logger)
        {
            _donHangRepo = donHangRepo;
            _chiTietRepo = chiTietRepo;
            _bienTheRepo = bienTheRepo;
            _dataSetHelper = dataSetHelper;
            _logger = logger;
        }
        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            try
            {
                // Tổng doanh thu
                var doanhThu = await _donHangRepo.GetAll()
                    .Where(x => x.TrangThai == "DaThanhToan")
                    .SumAsync(x => (decimal?)x.TongTien) ?? 0;

                // Số đơn thành công
                var soDon = await _donHangRepo.GetAll()
                    .CountAsync(x => x.TrangThai == "DaThanhToan");

                // SỐ LƯỢNG BIẾN THỂ SẮP HẾT (tồn kho <= 10 và >0)
                var soBienTheSapHet = await _bienTheRepo.GetAll()
                    .CountAsync(bt => bt.SoLuongTon <= 10 && bt.SoLuongTon > 0);

                // Top sản phẩm bán chạy (giữ nguyên)
                var topSp = await (from ct in _chiTietRepo.GetAll()
                                   join bt in _bienTheRepo.GetAll() on ct.BienTheID equals bt.BienTheID
                                   join sp in _bienTheRepo.GetDbContext().Set<SanPham>() on bt.SanPhamID equals sp.SanPhamID
                                   group new { ct, bt, sp } by new { sp.TenSanPham, bt.Size } into g
                                   select new SanPhamBanChayDto
                                   {
                                       TenSanPham = g.Key.TenSanPham,
                                       Size = g.Key.Size,
                                       SoLuongDaBan = g.Sum(x => x.ct.SoLuong),
                                       DoanhThuMangLai = g.Sum(x => x.ct.SoLuong * x.ct.DonGia)
                                   })
                                  .OrderByDescending(x => x.SoLuongDaBan)
                                  .Take(5)
                                  .ToListAsync();

                return new DashboardSummaryDto
                {
                    TongDoanhThu = doanhThu,
                    TongDonHangThanhCong = soDon,
                    SoSanPhamSapHetHang = soBienTheSapHet,  // ← sửa: đếm biến thể
                    TopSanPhamBanChay = topSp
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy dữ liệu Dashboard");
                throw new ApplicationException("Không thể tải dữ liệu thống kê.");
            }
        }
        public async Task<IEnumerable<DoanhThuTheoNgayDto>> GetDoanhThuTheoNgayAsync(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            try
            {
                var query = _donHangRepo.GetAll().Where(x => x.TrangThai == "DaThanhToan");
                if (tuNgay.HasValue) query = query.Where(x => x.NgayDat >= tuNgay.Value);
                if (denNgay.HasValue) query = query.Where(x => x.NgayDat <= denNgay.Value);
                var result = await query
                    .GroupBy(x => x.NgayDat.Date)
                    .Select(g => new DoanhThuTheoNgayDto
                    {
                        Ngay = g.Key,
                        SoDon = g.Count(),
                        DoanhThu = g.Sum(x => x.TongTien)
                    })
                    .OrderBy(x => x.Ngay)
                    .ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi thống kê doanh thu theo ngày");
                throw new ApplicationException("Không thể tải dữ liệu doanh thu theo ngày.");
            }
        }

        public async Task<IEnumerable<DoanhThuTheoThangDto>> GetDoanhThuTheoThangAsync(int? nam = null)
        {
            try
            {
                var query = _donHangRepo.GetAll().Where(x => x.TrangThai == "DaThanhToan");
                if (nam.HasValue) query = query.Where(x => x.NgayDat.Year == nam.Value);
                var result = await query
                    .GroupBy(x => new { x.NgayDat.Year, x.NgayDat.Month })
                    .Select(g => new DoanhThuTheoThangDto
                    {
                        Nam = g.Key.Year,
                        Thang = g.Key.Month,
                        SoDon = g.Count(),
                        DoanhThu = g.Sum(x => x.TongTien)
                    })
                    .OrderBy(x => x.Nam).ThenBy(x => x.Thang)
                    .ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi thống kê doanh thu theo tháng");
                throw new ApplicationException("Không thể tải dữ liệu doanh thu theo tháng.");
            }
        }

        public async Task<IEnumerable<DoanhThuTheoDanhMucDto>> GetDoanhThuTheoDanhMucAsync()
        {
            try
            {
                var dt = await Task.Run(() => _dataSetHelper.GetDataTable("SELECT TenDanhMuc, DoanhThu FROM vw_DoanhThuTheoDanhMuc"));
                var result = new List<DoanhThuTheoDanhMucDto>();
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    result.Add(new DoanhThuTheoDanhMucDto
                    {
                        TenDanhMuc = row["TenDanhMuc"]?.ToString() ?? "",
                        DoanhThu = Convert.ToDecimal(row["DoanhThu"])
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi thống kê doanh thu theo danh mục");
                throw new ApplicationException("Không thể tải dữ liệu doanh thu theo danh mục.");
            }
        }

        public async Task<string> ExportDoanhThuTheoNgayXmlAsync(DateTime? tuNgay, DateTime? denNgay)
        {
            var data = await GetDoanhThuTheoNgayAsync(tuNgay, denNgay);
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("BaoCaoDoanhThuTheoNgay",
                    new XAttribute("TuNgay", tuNgay?.ToString("yyyy-MM-dd") ?? ""),
                    new XAttribute("DenNgay", denNgay?.ToString("yyyy-MM-dd") ?? ""),
                    new XAttribute("NgayXuat", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    from item in data
                    select new XElement("Ngay",
                        new XElement("NgayThang", item.Ngay.ToString("yyyy-MM-dd")),
                        new XElement("SoDon", item.SoDon),
                        new XElement("DoanhThu", item.DoanhThu)
                    )
                )
            );
            return doc.ToString();
        }

        public async Task<string> ExportDoanhThuTheoThangXmlAsync(int? nam)
        {
            var data = await GetDoanhThuTheoThangAsync(nam);
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("BaoCaoDoanhThuTheoThang",
                    new XAttribute("Nam", nam?.ToString() ?? "Tất cả"),
                    new XAttribute("NgayXuat", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    from item in data
                    select new XElement("Thang",
                        new XElement("Nam", item.Nam),
                        new XElement("Thang", item.Thang),
                        new XElement("SoDon", item.SoDon),
                        new XElement("DoanhThu", item.DoanhThu)
                    )
                )
            );
            return doc.ToString();
        }

        public async Task<string> ExportDoanhThuTheoDanhMucXmlAsync()
        {
            var data = await GetDoanhThuTheoDanhMucAsync();
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("BaoCaoDoanhThuTheoDanhMuc",
                    new XAttribute("NgayXuat", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    from item in data
                    select new XElement("DanhMuc",
                        new XElement("TenDanhMuc", item.TenDanhMuc),
                        new XElement("DoanhThu", item.DoanhThu)
                    )
                )
            );
            return doc.ToString();
        }

        public async Task<string> ExportDashboardXmlAsync()
        {
            var data = await GetDashboardSummaryAsync();
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("BaoCaoTongHop",
                    new XAttribute("NgayXuat", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("ChiSoChinh",
                        new XElement("TongDoanhThu", data.TongDoanhThu),
                        new XElement("SoDonThanhCong", data.TongDonHangThanhCong),
                        new XElement("SanPhamSapHet", data.SoSanPhamSapHetHang)
                    ),
                    new XElement("TopSanPhamBanChay",
                        from sp in data.TopSanPhamBanChay
                        select new XElement("SanPham",
                            new XElement("TenSanPham", sp.TenSanPham),
                            new XElement("Size", sp.Size),
                            new XElement("SoLuongBan", sp.SoLuongDaBan),
                            new XElement("DoanhThuMangLai", sp.DoanhThuMangLai)
                        )
                    )
                )
            );
            return doc.ToString();
        }
        public async Task<List<SanPhamSapHetDto>> GetSanPhamSapHetAsync()
        {
            // Lấy tất cả biến thể có tồn kho <= 10 và >0, kèm tên sản phẩm
            var items = await _bienTheRepo.GetAll()
                .Where(bt => bt.SoLuongTon <= 10 && bt.SoLuongTon > 0)
                .Include(bt => bt.SanPham)
                .Select(bt => new SanPhamSapHetDto
                {
                    TenSanPham = bt.SanPham.TenSanPham,
                    Size = bt.Size,
                    SoLuongTon = bt.SoLuongTon
                })
                .ToListAsync();
            return items;
        }

        public async Task<List<DonHangThanhCongDto>> GetDonHangThanhCongAsync()
        {
            var orders = await _donHangRepo.GetAll()
                .Where(dh => dh.TrangThai == "DaThanhToan")
                .Include(dh => dh.KhachHang)
                .Select(dh => new DonHangThanhCongDto
                {
                    DonHangID = dh.DonHangID,
                    NgayDat = dh.NgayDat,
                    TongTien = dh.TongTien,
                    KhachHang = dh.KhachHang.HoTen
                })
                .ToListAsync();
            return orders;
        }
    }
}