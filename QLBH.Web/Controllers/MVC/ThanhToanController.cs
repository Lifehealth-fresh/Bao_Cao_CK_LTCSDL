using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;
using QLBH.Web.Extensions;

namespace QLBH.Web.Controllers
{
    [Authorize(Roles = "NhanVien")]
    public class ThanhToanController : Controller
    {
        private readonly IThanhToanService _thanhToanService;
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly ILogger<ThanhToanController> _logger;

        public ThanhToanController(
            IThanhToanService thanhToanService,
            ITaiKhoanService taiKhoanService,
            ILogger<ThanhToanController> logger)
        {
            _thanhToanService = thanhToanService;
            _taiKhoanService = taiKhoanService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _thanhToanService.GetPendingPaymentsAsync();
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi hiển thị danh sách thanh toán chờ duyệt");
                TempData["Error"] = "Không thể tải danh sách thanh toán.";
                return View(Enumerable.Empty<ThanhToanDto>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhan(int thanhToanId)
        {
            try
            {
                int taiKhoanId = User.GetTaiKhoanId();
                var nhanVienId = await _taiKhoanService.GetNhanVienIdByTaiKhoanIdAsync(taiKhoanId);
                if (!nhanVienId.HasValue)
                {
                    TempData["Error"] = "Không xác định được nhân viên. Vui lòng đăng nhập lại.";
                    return RedirectToAction("DangNhap", "TaiKhoan");
                }

                var dto = new ConfirmPaymentDto
                {
                    ThanhToanID = thanhToanId,
                    NhanVienID = nhanVienId.Value,
                    ChapNhan = true,
                    GhiChu = "Nhân viên xác nhận thanh toán thành công"
                };
                await _thanhToanService.ProcessPaymentAsync(dto);
                TempData["Success"] = "Xác nhận thanh toán thành công! Đơn hàng đã được cập nhật và trừ kho.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xác nhận thanh toán {ThanhToanId}", thanhToanId);
                TempData["Error"] = "Xác nhận thanh toán thất bại. Có thể do không đủ tồn kho hoặc lỗi hệ thống.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoi(int thanhToanId)
        {
            try
            {
                int taiKhoanId = User.GetTaiKhoanId();
                var nhanVienId = await _taiKhoanService.GetNhanVienIdByTaiKhoanIdAsync(taiKhoanId);
                if (!nhanVienId.HasValue)
                {
                    TempData["Error"] = "Không xác định được nhân viên. Vui lòng đăng nhập lại.";
                    return RedirectToAction("DangNhap", "TaiKhoan");
                }

                var dto = new ConfirmPaymentDto
                {
                    ThanhToanID = thanhToanId,
                    NhanVienID = nhanVienId.Value,
                    ChapNhan = false,
                    GhiChu = "Nhân viên từ chối thanh toán"
                };
                await _thanhToanService.ProcessPaymentAsync(dto);
                TempData["Success"] = "Đã từ chối thanh toán và hủy đơn hàng.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi từ chối thanh toán {ThanhToanId}", thanhToanId);
                TempData["Error"] = "Từ chối thanh toán thất bại.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
