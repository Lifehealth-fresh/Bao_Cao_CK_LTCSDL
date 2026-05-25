using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLBH.BLL.DTOs;

namespace QLBH.BLL.Services
{
    public interface IDonHangService
    {
        // Tạo đơn hàng từ giỏ (gọi sp_TaoDonHangTuGio)
        Task<DonHangDto> CreateOrderAsync(CreateDonHangDto dto);

        // Lấy danh sách đơn hàng (cho nhân viên)
        Task<IEnumerable<DonHangDto>> GetAllAsync();

        // Lấy chi tiết đơn hàng theo ID (khách hàng chỉ xem đơn của mình)
        Task<DonHangDto?> GetByIdAsync(int donHangId, int? khachHangId = null);

        // Lấy lịch sử đơn hàng của khách
        Task<IEnumerable<DonHangDto>> GetHistoryByKhachHangIdAsync(int khachHangId);

        // Tìm kiếm đơn hàng (cho nhân viên)
        Task<IEnumerable<DonHangDto>> SearchAsync(string? keyword, string? trangThai, DateTime? tuNgay, DateTime? denNgay);

        // Cập nhật trạng thái đơn hàng (gọi sp_CapNhatTrangThaiDonHang)
        Task<bool> UpdateTrangThaiAsync(int donHangId, string trangThaiMoi);
    }
}