using System.Security.Claims;

namespace QLBH.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public const string TaiKhoanIdClaimType = "TaiKhoanId";

        /// <summary>
        /// KhachHangID (khách hàng) hoặc TaiKhoanID (nhân viên) — dùng cho giỏ hàng / đơn hàng khách.
        /// </summary>
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || string.IsNullOrEmpty(claim.Value))
                return 0;
            return int.Parse(claim.Value);
        }

        public static int GetTaiKhoanId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(TaiKhoanIdClaimType);
            if (claim != null && int.TryParse(claim.Value, out var taiKhoanId))
                return taiKhoanId;

            if (user.IsInRole("NhanVien"))
                return GetUserId(user);

            return 0;
        }

        /// <summary>
        /// KhachHangID — chỉ có khi đăng nhập với vai trò KhachHang.
        /// </summary>
        public static int GetKhachHangId(this ClaimsPrincipal user)
        {
            if (user.IsInRole("KhachHang"))
                return GetUserId(user);
            return 0;
        }

        public static string GetUserRole(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.Role);
            return claim?.Value ?? "";
        }
    }
}