using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLBH.BLL.Constants;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;

namespace QLBH.Web.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ISanPhamService _sanPhamService;
        private readonly IDanhMucService _danhMucService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<SanPhamController> _logger;

        public SanPhamController(
            ISanPhamService sanPhamService,
            IDanhMucService danhMucService,
            IWebHostEnvironment env,
            ILogger<SanPhamController> logger)
        {
            _sanPhamService = sanPhamService;
            _danhMucService = danhMucService;
            _env = env;
            _logger = logger;
        }

        // ========== KHÁCH HÀNG ==========
        public async Task<IActionResult> Index(string? keyword, int? danhMucId, decimal? minPrice, decimal? maxPrice)
        {
            // Lấy sản phẩm (đã được lọc theo danh mục hoạt động trong service)
            var list = await _sanPhamService.GetAllAsync(keyword, danhMucId, minPrice, maxPrice, chiLaySanPhamHoatDong: true);
            // CHỈ LẤY DANH MỤC ĐANG HOẠT ĐỘNG (TrangThai = true)
            var allDanhMucs = await _danhMucService.GetAllAsync();
            var danhMucHoatDong = allDanhMucs.Where(dm => dm.TrangThai == true).ToList();
            ViewBag.DanhMucList = danhMucHoatDong;
            return View(list);
        }
        public async Task<IActionResult> Details(int id)
        {
            var sp = await _sanPhamService.GetByIdAsync(id);
            if (sp == null || sp.TrangThai != SanPhamTrangThai.HoatDong) return NotFound();

            if (sp.BienTheSanPhams != null)
                sp.BienTheSanPhams = sp.BienTheSanPhams.Where(bt => bt.TrangThai).ToList();

            return View(sp);
        }

        // ========== NHÂN VIÊN ==========



        [Authorize(Roles = "NhanVien")]
        public async Task<IActionResult> Admin(string? keyword, int? danhMucId)
        {
            var list = await _sanPhamService.GetAllAsync(keyword, danhMucId, null, null);
            var danhMucs = await _danhMucService.GetAllAsync();
            ViewBag.DanhMucList = danhMucs;
            return View(list);
        }

        [Authorize(Roles = "NhanVien")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDanhMucToViewBag();
            return View(new CreateSanPhamDto());
        }

        [Authorize(Roles = "NhanVien")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateSanPhamDto dto, IFormFile? fileAnh)
        {
            try
            {
                // Mặc định trạng thái là "HoatDong" (kích hoạt) – không cần checkbox
                dto.TrangThai = "HoatDong";

                if (!ModelState.IsValid)
                {
                    await LoadDanhMucToViewBag();
                    return View(dto);
                }

                // Xử lý upload ảnh
                if (fileAnh != null && fileAnh.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileAnh.FileName);
                    var folderPath = Path.Combine(_env.WebRootPath, "images");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                    var filePath = Path.Combine(folderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await fileAnh.CopyToAsync(stream);
                    dto.HinhAnh = fileName;
                }
                else
                {
                    dto.HinhAnh = "no-image.png";
                }

                await _sanPhamService.CreateAsync(dto);
                TempData["Success"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(Admin));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm sản phẩm");
                TempData["Error"] = "Thêm sản phẩm thất bại. Vui lòng thử lại.";
                await LoadDanhMucToViewBag();
                return View(dto);
            }
        }

        [Authorize(Roles = "NhanVien")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var sp = await _sanPhamService.GetByIdAsync(id);
            if (sp == null) return NotFound();

            var dto = new UpdateSanPhamDto
            {
                SanPhamID = sp.SanPhamID,
                DanhMucID = sp.DanhMucID,
                TenSanPham = sp.TenSanPham,
                GiaCoBan = sp.GiaCoBan,
                HinhAnh = sp.HinhAnh,
                MoTa = sp.MoTa,
                ThuongHieu = sp.ThuongHieu,
                ChatLieu = sp.ChatLieu,
                HuongDanBaoQuan = sp.HuongDanBaoQuan,
                TrangThai = sp.TrangThai,
                NgayCapNhat = sp.NgayCapNhat   // Thêm dòng này
            };
            await LoadDanhMucToViewBag(dto.DanhMucID);
            return View(dto);
        }

        [Authorize(Roles = "NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateSanPhamDto dto, IFormFile? fileAnh, bool? isActive)
        {
            try
            {
                // Gán trạng thái từ checkbox
                dto.TrangThai = isActive==true ? "HoatDong" : "Ngung";

                if (!ModelState.IsValid)
                {
                    await LoadDanhMucToViewBag(dto.DanhMucID);
                    return View(dto);
                }

                // Xử lý upload ảnh mới
                if (fileAnh != null && fileAnh.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileAnh.FileName);
                    var folderPath = Path.Combine(_env.WebRootPath, "images");
                    Directory.CreateDirectory(folderPath);
                    var filePath = Path.Combine(folderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await fileAnh.CopyToAsync(stream);
                    dto.HinhAnh = fileName;
                }
                else if (string.IsNullOrWhiteSpace(dto.HinhAnh))
                {
                    // Giữ ảnh cũ nếu không upload mới
                    var existing = await _sanPhamService.GetByIdAsync(dto.SanPhamID);
                    dto.HinhAnh = existing?.HinhAnh ?? "no-image.png";
                }

                var result = await _sanPhamService.UpdateAsync(dto);
                if (result == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm để cập nhật.";
                    return RedirectToAction(nameof(Admin));
                }

                TempData["Success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Admin));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật sản phẩm {Id}", dto.SanPhamID);
                TempData["Error"] = "Cập nhật thất bại: " + ex.Message;
                await LoadDanhMucToViewBag(dto.DanhMucID);
                return View(dto);
            }
        }

        [Authorize(Roles = "NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _sanPhamService.DeleteAsync(id);
                if (success)
                    TempData["Success"] = "Xóa sản phẩm thành công!";
                else
                    TempData["Error"] = "Không tìm thấy sản phẩm để xóa.";
            }
            catch (InvalidOperationException ex)
            {
                // Trường hợp còn biến thể đã được xử lý trong service
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xóa sản phẩm {Id}", id);
                TempData["Error"] = "Xóa sản phẩm thất bại: " + ex.Message;
            }
            return RedirectToAction(nameof(Admin));
        }
        // ========== HELPER ==========
        private async Task LoadDanhMucToViewBag(int? selectedId = null)
        {
            var danhMucs = await _danhMucService.GetAllAsync();
            ViewBag.DanhMucList = new SelectList(danhMucs, "DanhMucID", "TenDanhMuc", selectedId);
        }
    }
}