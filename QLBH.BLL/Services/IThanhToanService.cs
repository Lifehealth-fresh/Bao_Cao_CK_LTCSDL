using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLBH.BLL.DTOs;

namespace QLBH.BLL.Services
{
    public interface IThanhToanService
    {
        // Lấy danh sách thanh toán chờ duyệt (ChoDuyet)
        Task<IEnumerable<ThanhToanDto>> GetPendingPaymentsAsync();

        // Xác nhận hoặc từ chối thanh toán (gọi sp_XacNhanThanhToan)
        Task<bool> ProcessPaymentAsync(ConfirmPaymentDto dto);

        // Lấy lịch sử thanh toán của khách hàng
        Task<IEnumerable<ThanhToanDto>> GetHistoryByKhachHangIdAsync(int khachHangId);
    }
}