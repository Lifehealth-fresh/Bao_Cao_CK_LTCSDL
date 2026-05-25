using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;
using QLBH.Web.Extensions;

namespace QLBH.Web.Controllers
{
    [Authorize(Roles = "KhachHang,NhanVien")]
    public class GioHangController : Controller
    {
        private readonly IGioHangService _gioHangService;
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly ILogger<GioHangController> _logger;

        public GioHangController(
            IGioHangService gioHangService,
            ITaiKhoanService taiKhoanService,
            ILogger<GioHangController> logger)
        {
            _gioHangService = gioHangService;
            _taiKhoanService = taiKhoanService;
            _logger = logger;
        }

        private async Task<int> ResolveKhachHangIdAsync()
        {
            if (User.IsInRole("KhachHang"))
                return User.GetKhachHangId();

            var taiKhoanId = User.GetTaiKhoanId();
            if (taiKhoanId == 0) return 0;
            return await _taiKhoanService.GetOrCreateKhachHangIdAsync(taiKhoanId);
        }

        public async Task<IActionResult> Index()
        {
            int khachHangId = await ResolveKhachHangIdAsync();
            if (khachHangId == 0) return RedirectToAction("DangNhap", "TaiKhoan");

            var cart = await _gioHangService.GetCartByKhachHangIdAsync(khachHangId);
            if (cart != null && !cart.ChiTietGioHangs.Any() && TempData["AutoRemoved"] == null)
            {
                TempData["Warning"] = "Một số sản phẩm không còn khả dụng đã được xóa khỏi giỏ.";
            }
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
            try
            {
                int khachHangId = await ResolveKhachHangIdAsync();
                if (khachHangId == 0)
                {
                    TempData["Error"] = "Vui lòng đăng nhập.";
                    return RedirectToAction("DangNhap", "TaiKhoan");
                }
                dto.KhachHangID = khachHangId;

                await _gioHangService.AddToCartAsync(dto);
                TempData["Success"] = "Đã thêm vào giỏ hàng.";
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "SanPham");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi thêm vào giỏ");
                TempData["Error"] = "Thêm vào giỏ thất bại.";
                return RedirectToAction("Index", "SanPham");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(UpdateCartItemDto dto)
        {
            try
            {
                var result = await _gioHangService.UpdateQuantityAsync(dto);
                if (result)
                    TempData["Success"] = "Cập nhật số lượng thành công.";
                else
                    TempData["Error"] = "Cập nhật thất bại.";
            }
            catch (ApplicationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật số lượng");
                TempData["Error"] = "Cập nhật thất bại.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int chiTietId)
        {
            await _gioHangService.RemoveItemAsync(chiTietId);
            TempData["Success"] = "Đã xóa sản phẩm.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ClearCart()
        {
            int khachHangId = await ResolveKhachHangIdAsync();
            if (khachHangId == 0)
                return RedirectToAction("DangNhap", "TaiKhoan");
            await _gioHangService.ClearCartAsync(khachHangId);
            TempData["Success"] = "Đã xóa toàn bộ giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }
    }
}
