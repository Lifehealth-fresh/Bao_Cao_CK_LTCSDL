using AutoMapper;
using QLBH.BLL.DTOs;
using QLBH.DAL.Entities;

namespace QLBH.BLL.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // TaiKhoan
            CreateMap<TaiKhoan, TaiKhoanDto>().ReverseMap();

            // KhachHang, NhanVien
            CreateMap<KhachHang, KhachHangDto>().ReverseMap();
            CreateMap<NhanVien, NhanVienDto>().ReverseMap();

            // DanhMuc
            CreateMap<DanhMuc, DanhMucDto>().ReverseMap();
            CreateMap<CreateDanhMucDto, DanhMuc>();
            CreateMap<UpdateDanhMucDto, DanhMuc>();

            // SanPham (ánh xạ từ entity sang DTO)
            CreateMap<SanPham, SanPhamDto>()
                .ForMember(dest => dest.TenDanhMuc,
                    opt => opt.MapFrom(src => src.DanhMuc != null ? src.DanhMuc.TenDanhMuc : string.Empty));

            // Ánh xạ từ DTO tạo mới sang Entity (bỏ qua các trường xử lý thủ công trong service)
            CreateMap<CreateSanPhamDto, SanPham>()
                .ForMember(dest => dest.HinhAnh, opt => opt.Ignore())
                .ForMember(dest => dest.NgayTao, opt => opt.Ignore())
                .ForMember(dest => dest.NgayCapNhat, opt => opt.Ignore())
                .ForMember(dest => dest.BienTheSanPhams, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Ánh xạ từ DTO cập nhật sang Entity (bỏ qua các trường xử lý thủ công)
            CreateMap<UpdateSanPhamDto, SanPham>()
                .ForMember(dest => dest.HinhAnh, opt => opt.Ignore())
                .ForMember(dest => dest.NgayTao, opt => opt.Ignore())
                .ForMember(dest => dest.NgayCapNhat, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // BienTheSanPham
            CreateMap<BienTheSanPham, BienTheSanPhamDto>()
                .ForMember(dest => dest.TenSanPham,
                    opt => opt.MapFrom(src => src.SanPham != null ? src.SanPham.TenSanPham : string.Empty));
            CreateMap<CreateBienTheSanPhamDto, BienTheSanPham>();
            CreateMap<UpdateBienTheSanPhamDto, BienTheSanPham>();

            // Voucher
            CreateMap<Voucher, VoucherDto>().ReverseMap();
            CreateMap<CreateVoucherDto, Voucher>();
            CreateMap<UpdateVoucherDto, Voucher>();

            // DonHang
            CreateMap<DonHang, DonHangDto>()
             .ForMember(dest => dest.DiaChiNhanHang, opt => opt.MapFrom(src => src.DiaChi))
             .ForMember(dest => dest.SoDienThoaiNhan, opt => opt.MapFrom(src => src.SoDienThoai))
             .ForMember(dest => dest.MaVoucher, opt => opt.MapFrom(src => src.Voucher != null ? src.Voucher.MaVoucher : ""));
            CreateMap<ChiTietDonHang, ChiTietDonHangDto>();

            // ThanhToan
            CreateMap<ThanhToan, ThanhToanDto>().ReverseMap();
        }
    }
}