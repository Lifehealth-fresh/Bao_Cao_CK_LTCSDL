using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBH.BLL.Constants;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;
using QLBH.DAL.Entities;
using QLBH.DAL.Repositories;
using QLBH.Web.Extensions;

namespace QLBH.Web.Controllers
{
    public class DonHangController : Controller
    {
        private readonly IDonHangService _donHangService;
        private readonly IGioHangService _gioHangService;
        private readonly IRepository<BienTheSanPham> _bienTheRepo;
        private readonly IVoucherService _voucherService;
        private readonly ILogger<DonHangController> _logger;

        public DonHangController(
            IDonHangService donHangService,
            IGioHangService gioHangService,
            IRepository<BienTheSanPham> bienTheRepo,
            IVoucherService voucherService,
            ILogger<DonHangController> logger)
        {
            _donHangService = donHangService;
            _gioHangService = gioHangService;
            _bienTheRepo = bienTheRepo;
            _voucherService = voucherService;
            _logger = logger;
        }


        [Authorize(Roles = "KhachHang")]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            int khachHangId = User.GetKhachHangId();
            if (khachHangId == 0) return RedirectToAction("DangNhap", "TaiKhoan");

            var cart = await _gioHangService.GetCartByKhachHangIdAsync(khachHangId);
            if (cart == null || !cart.ChiTietGioHangs.Any())
            {
                TempData["Error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
                return RedirectToAction("Index", "GioHang");
            }

            // Tính tổng tạm tính từ giỏ
            decimal tongTamTinh = cart.TongTien;
            if (tongTamTinh <= 0)
            {
                TempData["Error"] = "Giỏ hàng không hợp lệ. Tổng tiền bằng 0.";
                return RedirectToAction("Index", "GioHang");
            }

            var vouchers = await _voucherService.GetAvailableVouchersAsync(tongTamTinh);
            ViewBag.Vouchers = vouchers;
            ViewBag.CartTotal = tongTamTinh;

            return View(new CreateDonHangDto());
        }
        [Authorize(Roles = "KhachHang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CreateDonHangDto dto)
        {
            try
            {
                int khachHangId = User.GetKhachHangId();
                if (khachHangId == 0) return RedirectToAction("DangNhap", "TaiKhoan");

                // ✅ Nếu GhiChu null thì gán chuỗi rỗng
                dto.GhiChu ??= string.Empty;

                // ✅ Xóa tất cả lỗi ModelState liên quan đến GhiChu (nếu có)
                if (ModelState.ContainsKey(nameof(dto.GhiChu)))
                    ModelState[nameof(dto.GhiChu)].Errors.Clear();

                var cart = await _gioHangService.GetCartByKhachHangIdAsync(khachHangId);
                if (cart == null || !cart.ChiTietGioHangs.Any())
                {
                    TempData["Error"] = "Giỏ hàng trống.";
                    return RedirectToAction("Index", "GioHang");
                }

                // Kiểm tra từng biến thể (giữ nguyên code cũ)
                foreach (var item in cart.ChiTietGioHangs)
                {
                    var bienThe = await _bienTheRepo.GetAll()
                        .Include(bt => bt.SanPham)
                        .FirstOrDefaultAsync(bt => bt.BienTheID == item.BienTheID);
                    if (bienThe == null || !bienThe.TrangThai
                        || (bienThe.SanPham != null && SanPhamTrangThai.IsNgungBan(bienThe.SanPham.TrangThai)))
                    {
                        TempData["Error"] = $"Sản phẩm '{item.TenSanPham}' hiện không khả dụng.";
                        return RedirectToAction("Index", "GioHang");
                    }
                    if (bienThe.SoLuongTon < item.SoLuong)
                    {
                        TempData["Error"] = $"Sản phẩm '{item.TenSanPham}' chỉ còn {bienThe.SoLuongTon} trong kho.";
                        return RedirectToAction("Index", "GioHang");
                    }
                }

                if (!ModelState.IsValid)
                {
                  
                    // Load lại view với dữ liệu giỏ
                    ViewBag.Vouchers = await _voucherService.GetAvailableVouchersAsync(cart.TongTien);
                    ViewBag.CartTotal = cart.TongTien;
                    return View(dto);
                }

                dto.KhachHangID = khachHangId;
                var result = await _donHangService.CreateOrderAsync(dto);
                TempData["Success"] = $"Đặt hàng thành công! Mã đơn #{result.DonHangID}";
                return RedirectToAction(nameof(Details), new { id = result.DonHangID });
            }
            catch (ApplicationException ex)
            {
                TempData["Error"] = ex.Message;
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đặt hàng");
                TempData["Error"] = "Đặt hàng thất bại. Vui lòng thử lại.";
                return View(dto);
            }
        }


        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> Index()
        {
            int khachHangId = User.GetKhachHangId();
            if (khachHangId == 0) return RedirectToAction("DangNhap", "TaiKhoan");
            var list = await _donHangService.GetHistoryByKhachHangIdAsync(khachHangId);
            return View(list);
        }

        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> Details(int id)
        {
            int khachHangId = User.GetKhachHangId();
            var donHang = await _donHangService.GetByIdAsync(id, khachHangId);
            if (donHang == null) return NotFound();
            return View(donHang);
        }

        [Authorize(Roles = "KhachHang")]
        [HttpPost]
        public async Task<IActionResult> MuaNgay(int bienTheId, int soLuong)
        {
            try
            {
                int khachHangId = User.GetKhachHangId();
                if (khachHangId == 0)
                    return RedirectToAction("DangNhap", "TaiKhoan");

                // Kiểm tra biến thể tồn tại, trạng thái và tồn kho trước khi thêm vào giỏ
                var bienThe = await _bienTheRepo.GetAll()
                    .Include(bt => bt.SanPham)
                    .FirstOrDefaultAsync(bt => bt.BienTheID == bienTheId);
                if (bienThe == null)
                {
                    TempData["Error"] = "Sản phẩm không tồn tại.";
                    return RedirectToAction("Index", "SanPham");
                }
                if (!bienThe.TrangThai
                    || (bienThe.SanPham != null && SanPhamTrangThai.IsNgungBan(bienThe.SanPham.TrangThai)))
                {
                    TempData["Error"] = "Sản phẩm này hiện không khả dụng để bán.";
                    return RedirectToAction("Details", "SanPham", new { id = bienThe.SanPhamID });
                }
                if (bienThe.SoLuongTon < soLuong)
                {
                    TempData["Error"] = $"Chỉ còn {bienThe.SoLuongTon} sản phẩm trong kho.";
                    return RedirectToAction("Details", "SanPham", new { id = bienThe.SanPhamID });
                }

                // Nếu hợp lệ, thêm vào giỏ
                await _gioHangService.AddToCartAsync(new AddToCartDto
                {
                    KhachHangID = khachHangId,
                    BienTheID = bienTheId,
                    SoLuong = soLuong
                });
                return RedirectToAction(nameof(Checkout));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi MuaNgay");
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "SanPham");
            }
        }

        [Authorize(Roles = "NhanVien")]
        public async Task<IActionResult> QuanLy(string? keyword, string? trangThai, DateTime? tuNgay, DateTime? denNgay)
        {
            try
            {
                var list = await _donHangService.SearchAsync(keyword, trangThai, tuNgay, denNgay);
                ViewBag.Keyword = keyword;
                ViewBag.TrangThai = trangThai;
                ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
                ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi quản lý đơn hàng");
                TempData["Error"] = "Không thể tải danh sách đơn hàng.";
                return View(Enumerable.Empty<DonHangDto>());
            }
        }

        [Authorize(Roles = "NhanVien")]
        public async Task<IActionResult> ChiTietDonHang(int id)
        {
            var donHang = await _donHangService.GetByIdAsync(id);
            if (donHang == null) return NotFound();
            return View(donHang);
        }
    }
}
