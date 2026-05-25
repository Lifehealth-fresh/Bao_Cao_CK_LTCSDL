namespace QLBH.BLL.Constants
{
    public static class SanPhamTrangThai
    {
        public const string HoatDong = "HoatDong";
        public const string Ngung = "Ngung";
        public const string HetHang = "HetHang";

        public static bool IsNgungBan(string? trangThai) =>
            trangThai == Ngung || trangThai == HetHang;
    }
}
