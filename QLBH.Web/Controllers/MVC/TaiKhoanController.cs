using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using QLBH.BLL.DTOs;

using QLBH.BLL.Services;

using QLBH.Web.Extensions;

using System.Security.Claims;



namespace QLBH.Web.Controllers

{

    public class TaiKhoanController : Controller

    {

        private readonly ITaiKhoanService _taiKhoanService;

        private readonly ILogger<TaiKhoanController> _logger;



        public TaiKhoanController(ITaiKhoanService taiKhoanService, ILogger<TaiKhoanController> logger)

        {

            _taiKhoanService = taiKhoanService;

            _logger = logger;

        }



        [HttpGet]

        public IActionResult DangNhap(string? returnUrl)

        {

            ViewBag.ReturnUrl = returnUrl;

            return View();

        }



        [HttpPost]

        public async Task<IActionResult> DangNhap(LoginRequestDto request, string? returnUrl)

        {

            try

            {

                if (!ModelState.IsValid) return View(request);

                var result = await _taiKhoanService.LoginAsync(request);

                if (result == null)

                {

                    TempData["Error"] = "Sai email hoặc mật khẩu";

                    return View();

                }



                int identifier;

                if (result.TaiKhoan.VaiTro == "KhachHang")

                    identifier = await _taiKhoanService.GetOrCreateKhachHangIdAsync(result.TaiKhoan.TaiKhoanID);

                else

                    identifier = result.TaiKhoan.TaiKhoanID;



                var claims = new List<Claim>

                {

                    new Claim(ClaimTypes.NameIdentifier, identifier.ToString()),

                    new Claim(ClaimsPrincipalExtensions.TaiKhoanIdClaimType, result.TaiKhoan.TaiKhoanID.ToString()),

                    new Claim(ClaimTypes.Email, result.TaiKhoan.Email),

                    new Claim(ClaimTypes.Name, result.TaiKhoan.Email),

                    new Claim(ClaimTypes.Role, result.TaiKhoan.VaiTro)

                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));



                TempData["Success"] = $"Chào mừng {result.TaiKhoan.Email}!";

                if (result.TaiKhoan.VaiTro == "NhanVien")

                    return RedirectToAction("QuanLy", "DonHang");

                return RedirectToAction("Index", "SanPham");

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Lỗi đăng nhập");

                TempData["Error"] = "Đăng nhập thất bại";

                return View();

            }

        }



        [HttpGet]

        public IActionResult DangKy() => View();



        [HttpPost]

        public async Task<IActionResult> DangKy(DangKyRequestDto request)

        {

            try

            {

                if (!ModelState.IsValid) return View(request);

                var success = await _taiKhoanService.DangKyAsync(request);

                if (!success)

                {

                    TempData["Error"] = "Email đã tồn tại";

                    return View();

                }

                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";

                return RedirectToAction(nameof(DangNhap));

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Lỗi đăng ký");

                TempData["Error"] = "Đăng ký thất bại";

                return View();

            }

        }



        public async Task<IActionResult> DangXuat()

        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Success"] = "Đã đăng xuất.";

            return RedirectToAction(nameof(DangNhap));

        }



        [Authorize]

        public async Task<IActionResult> ThongTin()

        {

            int taiKhoanId = User.GetTaiKhoanId();

            if (taiKhoanId == 0) return RedirectToAction("DangNhap");

            var profile = await _taiKhoanService.GetThongTinCaNhanAsync(taiKhoanId);

            if (profile == null) return NotFound();

       
            return View(profile);

        }



        [Authorize]

        [HttpGet]

        public async Task<IActionResult> CapNhatThongTin()

        {

            int taiKhoanId = User.GetTaiKhoanId();

            if (taiKhoanId == 0) return RedirectToAction("DangNhap");

            var dto = await _taiKhoanService.GetCapNhatThongTinAsync(taiKhoanId);

            if (dto == null) return NotFound();

            return View(dto);

        }



        [Authorize]

        [HttpPost]

        public async Task<IActionResult> CapNhatThongTin(CapNhatThongTinDto dto)

        {

            try

            {

                int taiKhoanId = User.GetTaiKhoanId();

                if (taiKhoanId == 0) return RedirectToAction("DangNhap");

                dto.TaiKhoanID = taiKhoanId;



                if (!ModelState.IsValid) return View(dto);



                var result = await _taiKhoanService.UpdateThongTinAsync(dto);

                if (!result)

                {

                    TempData["Error"] = "Cập nhật thất bại";

                    return View(dto);

                }

                TempData["Success"] = "Cập nhật thông tin thành công!";

                return RedirectToAction(nameof(ThongTin));

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Lỗi cập nhật thông tin");

                TempData["Error"] = "Cập nhật thất bại";

                return View(dto);

            }

        }



        [Authorize]

        [HttpGet]

        public IActionResult DoiMatKhau() => View();



        [Authorize]

        [HttpPost]

        public async Task<IActionResult> DoiMatKhau(DoiMatKhauDto dto)

        {

            try

            {

                if (!ModelState.IsValid) return View(dto);

                int taiKhoanId = User.GetTaiKhoanId();

                if (taiKhoanId == 0) return RedirectToAction("DangNhap");

                dto.TaiKhoanID = taiKhoanId;



                var result = await _taiKhoanService.DoiMatKhauAsync(dto);

                if (!result)

                {

                    TempData["Error"] = "Mật khẩu cũ không đúng";

                    return View(dto);

                }

                TempData["Success"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại.";

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return RedirectToAction(nameof(DangNhap));

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Lỗi đổi mật khẩu");

                TempData["Error"] = "Đổi mật khẩu thất bại";

                return View(dto);

            }

        }



        [Authorize(Roles = "NhanVien")]

        public async Task<IActionResult> DanhSach(string? keyword)

        {

            var list = await _taiKhoanService.GetNguoiDungListAsync();

            if (!string.IsNullOrEmpty(keyword))

                list = list.Where(x => (x.Email?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)

                    || (x.HoTen?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

            return View(list);

        }



        [Authorize(Roles = "NhanVien")]

        [HttpGet]

        public IActionResult TaoTaiKhoan() => View();



        [Authorize(Roles = "NhanVien")]

        [HttpPost]

        public async Task<IActionResult> TaoTaiKhoan(TaoTaiKhoanDto dto)

        {

            try

            {

                if (!ModelState.IsValid) return View(dto);



                var success = await _taiKhoanService.CreateTaiKhoanAsync(dto);

                if (!success)

                {

                    TempData["Error"] = "Tạo tài khoản thất bại";

                    return View(dto);

                }

                TempData["Success"] = "Tạo tài khoản thành công!";

                return RedirectToAction(nameof(DanhSach));

            }

            catch (ApplicationException ex)

            {

                TempData["Error"] = ex.Message;

                return View(dto);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Lỗi tạo tài khoản");

                TempData["Error"] = "Tạo tài khoản thất bại. Email có thể đã tồn tại.";

                return View(dto);

            }

        }

    }

}

