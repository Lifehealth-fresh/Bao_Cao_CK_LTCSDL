using QLBH.BLL.DTOs;

namespace QLBH.BLL.Services
{
    public interface IThongKeService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
        Task<IEnumerable<DoanhThuTheoNgayDto>> GetDoanhThuTheoNgayAsync(DateTime? tuNgay = null, DateTime? denNgay = null);
        Task<IEnumerable<DoanhThuTheoThangDto>> GetDoanhThuTheoThangAsync(int? nam = null);
        Task<IEnumerable<DoanhThuTheoDanhMucDto>> GetDoanhThuTheoDanhMucAsync();
        // Thêm 2 phương thức mới
        Task<List<SanPhamSapHetDto>> GetSanPhamSapHetAsync();
        Task<List<DonHangThanhCongDto>> GetDonHangThanhCongAsync();

        // === Các phương thức xuất báo cáo thống kê ra XML ===
        Task<string> ExportDoanhThuTheoNgayXmlAsync(DateTime? tuNgay, DateTime? denNgay);
        Task<string> ExportDoanhThuTheoThangXmlAsync(int? nam);
        Task<string> ExportDoanhThuTheoDanhMucXmlAsync();
        Task<string> ExportDashboardXmlAsync();

    }
}