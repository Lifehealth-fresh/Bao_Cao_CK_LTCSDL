using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLBH.BLL.DTOs;
using QLBH.BLL.Helpers;
using QLBH.DAL.Entities;
using QLBH.DAL.Repositories;
using QLBH.DAL.Helpers;
using System.Data;

namespace QLBH.BLL.Services
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly IRepository<TaiKhoan> _taiKhoanRepo;
        private readonly IRepository<KhachHang> _khachHangRepo;
        private readonly IRepository<GioHang> _gioHangRepo;
        private readonly IRepository<NhanVien> _nhanVienRepo;
        private readonly IMapper _mapper;
        private readonly JwtHelper _jwtHelper;
        private readonly DataSetHelper _dataSetHelper;
        private readonly ILogger<TaiKhoanService> _logger;

        public TaiKhoanService(
            IRepository<TaiKhoan> taiKhoanRepo,
            IRepository<KhachHang> khachHangRepo,
            IRepository<GioHang> gioHangRepo,
            IRepository<NhanVien> nhanVienRepo,
            IMapper mapper,
            JwtHelper jwtHelper,
            DataSetHelper dataSetHelper,
            ILogger<TaiKhoanService> logger)
        {
            _taiKhoanRepo = taiKhoanRepo;
            _khachHangRepo = khachHangRepo;
            _gioHangRepo = gioHangRepo;
            _nhanVienRepo = nhanVienRepo;
            _mapper = mapper;
            _jwtHelper = jwtHelper;
            _dataSetHelper = dataSetHelper;
            _logger = logger;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var hashed = PasswordHelper.HashPassword(request.MatKhau);
                var user = await _taiKhoanRepo.GetAll()
                    .FirstOrDefaultAsync(x => x.Email == request.Email && x.MatKhauHash == hashed && x.TrangThai);
                if (user == null) return null;
                var token = _jwtHelper.GenerateToken(_mapper.Map<TaiKhoanDto>(user));
                return new LoginResponseDto
                {
                    Token = token,
                    TaiKhoan = _mapper.Map<TaiKhoanDto>(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đăng nhập");
                throw;
            }
        }

        public async Task<bool> DangKyAsync(DangKyRequestDto request)
        {
            try
            {
                if (await _taiKhoanRepo.GetAll().AnyAsync(x => x.Email == request.Email))
                    return false;

                using var transaction = await _taiKhoanRepo.GetDbContext().Database.BeginTransactionAsync();
                try
                {
                    var taiKhoan = new TaiKhoan
                    {
                        Email = request.Email,
                        MatKhauHash = PasswordHelper.HashPassword(request.MatKhau),
                        VaiTro = "KhachHang",
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    };
                    await _taiKhoanRepo.AddAsync(taiKhoan);
                    await _taiKhoanRepo.SaveChangesAsync();

                    var khachHang = new KhachHang
                    {
                        TaiKhoanID = taiKhoan.TaiKhoanID,
                        HoTen = request.HoTen,
                        SoDienThoai = request.SoDienThoai,
                        DiaChiMacDinh = request.DiaChi,
                        GioiTinh = "Khac",
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now
                    };
                    await _khachHangRepo.AddAsync(khachHang);
                    await _khachHangRepo.SaveChangesAsync();

                    var gioHang = new GioHang
                    {
                        KhachHangID = khachHang.KhachHangID,
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now
                    };
                    await _gioHangRepo.AddAsync(gioHang);
                    await _gioHangRepo.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đăng ký");
                throw;
            }
        }

        public async Task<TaiKhoanDto?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _taiKhoanRepo.GetByIdAsync(id);
                return entity == null ? null : _mapper.Map<TaiKhoanDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy tài khoản theo ID {Id}", id);
                throw;
            }
        }

        public async Task<ThongTinCaNhanDto?> GetThongTinCaNhanAsync(int taiKhoanId)
        {
            var taiKhoan = await _taiKhoanRepo.GetByIdAsync(taiKhoanId);
            if (taiKhoan == null) return null;

            var profile = new ThongTinCaNhanDto
            {
                TaiKhoanID = taiKhoan.TaiKhoanID,
                Email = taiKhoan.Email,
                VaiTro = taiKhoan.VaiTro,
                TrangThai = taiKhoan.TrangThai
            };

            if (taiKhoan.VaiTro == "KhachHang")
            {
                var khachHang = await _khachHangRepo.GetAll()
                    .FirstOrDefaultAsync(x => x.TaiKhoanID == taiKhoanId);
                if (khachHang != null)
                {
                    profile.HoTen = khachHang.HoTen;
                    profile.SoDienThoai = khachHang.SoDienThoai ?? string.Empty;
                    profile.DiaChi = khachHang.DiaChiMacDinh ?? string.Empty;
                }
            }
            else if (taiKhoan.VaiTro == "NhanVien")
            {
                var nhanVien = await _nhanVienRepo.GetAll()
                    .FirstOrDefaultAsync(x => x.TaiKhoanID == taiKhoanId);
                if (nhanVien != null)
                {
                    profile.HoTen = nhanVien.HoTen;
                    profile.SoDienThoai = nhanVien.SoDienThoai ?? string.Empty;
                    profile.NgayVaoLam = nhanVien.NgayVaoLam;
                    // Dùng NhanVienID làm mã nhân viên (có thể chuyển thành string)
                    profile.MaNhanVien = nhanVien.NhanVienID.ToString();
                }
            }

            return profile;
        }

        public async Task<CapNhatThongTinDto?> GetCapNhatThongTinAsync(int taiKhoanId)
        {
            var profile = await GetThongTinCaNhanAsync(taiKhoanId);
            if (profile == null) return null;
            return new CapNhatThongTinDto
            {
                TaiKhoanID = profile.TaiKhoanID,
                HoTen = profile.HoTen,
                SoDienThoai = profile.SoDienThoai,
                DiaChi = profile.DiaChi
            };
        }

        public async Task<int?> GetNhanVienIdByTaiKhoanIdAsync(int taiKhoanId)
        {
            var nhanVien = await _nhanVienRepo.GetAll()
                .FirstOrDefaultAsync(x => x.TaiKhoanID == taiKhoanId);
            return nhanVien?.NhanVienID;
        }

        public async Task<IEnumerable<TaiKhoanDto>> GetAllAsync()
        {
            try
            {
                var list = await _taiKhoanRepo.GetAll().ToListAsync();
                return _mapper.Map<IEnumerable<TaiKhoanDto>>(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy danh sách tài khoản");
                throw;
            }
        }

        public async Task<bool> UpdateThongTinAsync(CapNhatThongTinDto dto)
        {
            try
            {
                var taiKhoan = await _taiKhoanRepo.GetByIdAsync(dto.TaiKhoanID);
                if (taiKhoan == null) return false;

                if (taiKhoan.VaiTro == "KhachHang")
                {
                    var khachHang = await _khachHangRepo.GetAll()
                        .FirstOrDefaultAsync(x => x.TaiKhoanID == dto.TaiKhoanID);
                    if (khachHang == null) return false;
                    khachHang.HoTen = dto.HoTen;
                    khachHang.SoDienThoai = dto.SoDienThoai;
                    khachHang.DiaChiMacDinh = dto.DiaChi;
                    khachHang.NgayCapNhat = DateTime.Now;
                    _khachHangRepo.Update(khachHang);
                }
                else if (taiKhoan.VaiTro == "NhanVien")
                {
                    var nhanVien = await _nhanVienRepo.GetAll()
                        .FirstOrDefaultAsync(x => x.TaiKhoanID == dto.TaiKhoanID);
                    if (nhanVien == null) return false;
                    nhanVien.HoTen = dto.HoTen;
                    nhanVien.SoDienThoai = dto.SoDienThoai;
                    _nhanVienRepo.Update(nhanVien);
                }
                else
                {
                    return false;
                }

                await _taiKhoanRepo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật thông tin");
                throw;
            }
        }

        public async Task<bool> DoiMatKhauAsync(DoiMatKhauDto dto)
        {
            try
            {
                var taiKhoan = await _taiKhoanRepo.GetByIdAsync(dto.TaiKhoanID);
                if (taiKhoan == null) return false;
                if (!PasswordHelper.VerifyPassword(dto.MatKhauCu, taiKhoan.MatKhauHash))
                    return false;
                taiKhoan.MatKhauHash = PasswordHelper.HashPassword(dto.MatKhauMoi);
                _taiKhoanRepo.Update(taiKhoan);
                await _taiKhoanRepo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đổi mật khẩu");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _taiKhoanRepo.GetByIdAsync(id);
                if (entity == null) return false;
                _taiKhoanRepo.Delete(entity);
                await _taiKhoanRepo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xóa tài khoản {Id}", id);
                throw;
            }
        }

        public async Task<List<NguoiDungDto>> GetNguoiDungListAsync()
        {
            try
            {
                var dt = await Task.Run(() => _dataSetHelper.GetDataTable("SELECT * FROM vw_NguoiDung"));
                var result = new List<NguoiDungDto>();
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new NguoiDungDto
                    {
                        TaiKhoanID = Convert.ToInt32(row["TaiKhoanID"]),
                        Email = row["Email"]?.ToString() ?? "",
                        VaiTro = row["VaiTro"]?.ToString() ?? "",
                        TrangThai = Convert.ToBoolean(row["TrangThai"]),
                        NgayTao = Convert.ToDateTime(row["NgayTao"]),
                        HoTen = row["HoTen"]?.ToString() ?? "",
                        SoDienThoai = row["SoDienThoai"]?.ToString() ?? "",
                        DiaChi = row["DiaChi"]?.ToString(),
                        NgaySinh = row["NgaySinh"] == DBNull.Value ? null : Convert.ToDateTime(row["NgaySinh"]),
                        GioiTinh = row["GioiTinh"]?.ToString()
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy danh sách người dùng từ view");
                throw;
            }
        }

        // === Implement 2 method mới ===
        public async Task<int> GetOrCreateKhachHangIdAsync(int taiKhoanId)
        {
            try
            {
                // Tìm khách hàng theo TaiKhoanID
                var khachHang = await _khachHangRepo.GetAll()
                    .FirstOrDefaultAsync(x => x.TaiKhoanID == taiKhoanId);
                if (khachHang != null)
                    return khachHang.KhachHangID;

                // Nếu chưa có, tạo mới
                var taiKhoan = await _taiKhoanRepo.GetByIdAsync(taiKhoanId);
                if (taiKhoan == null)
                    throw new ApplicationException("Tài khoản không tồn tại.");

                khachHang = new KhachHang
                {
                    TaiKhoanID = taiKhoanId,
                    HoTen = taiKhoan.Email,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };
                await _khachHangRepo.AddAsync(khachHang);
                await _khachHangRepo.SaveChangesAsync();

                // Tạo giỏ hàng cho khách mới
                var gioHang = new GioHang
                {
                    KhachHangID = khachHang.KhachHangID,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };
                await _gioHangRepo.AddAsync(gioHang);
                await _gioHangRepo.SaveChangesAsync();

                return khachHang.KhachHangID;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi GetOrCreateKhachHangIdAsync: TaiKhoanId={TaiKhoanId}", taiKhoanId);
                throw;
            }
        }

        public async Task<bool> CreateTaiKhoanAsync(TaoTaiKhoanDto dto)
        {
            try
            {
                // Kiểm tra email trùng
                var email = dto.Email.Trim().ToLowerInvariant();
                if (await _taiKhoanRepo.GetAll().AnyAsync(x => x.Email.ToLower() == email))
                    throw new ApplicationException("Email đã tồn tại. Vui lòng dùng email khác.");

                using var transaction = await _taiKhoanRepo.GetDbContext().Database.BeginTransactionAsync();
                try
                {
                    var taiKhoan = new TaiKhoan
                    {
                        Email = email,
                        MatKhauHash = PasswordHelper.HashPassword(dto.MatKhau),
                        VaiTro = dto.VaiTro,
                        TrangThai = dto.TrangThai,
                        NgayTao = DateTime.Now
                    };
                    await _taiKhoanRepo.AddAsync(taiKhoan);
                    await _taiKhoanRepo.SaveChangesAsync();

                    if (dto.VaiTro == "KhachHang")
                    {
                        var khachHang = new KhachHang
                        {
                            TaiKhoanID = taiKhoan.TaiKhoanID,
                            HoTen = dto.HoTen ?? dto.Email,
                            SoDienThoai = dto.SoDienThoai,
                            DiaChiMacDinh = dto.DiaChi,
                            GioiTinh = "Khac",
                            NgayTao = DateTime.Now,
                            NgayCapNhat = DateTime.Now
                        };
                        await _khachHangRepo.AddAsync(khachHang);
                        await _khachHangRepo.SaveChangesAsync();

                        // Tạo giỏ hàng cho khách
                        var gioHang = new GioHang
                        {
                            KhachHangID = khachHang.KhachHangID,
                            NgayTao = DateTime.Now,
                            NgayCapNhat = DateTime.Now
                        };
                        await _gioHangRepo.AddAsync(gioHang);
                        await _gioHangRepo.SaveChangesAsync();
                    }
                    else if (dto.VaiTro == "NhanVien")
                    {
                        var nhanVien = new NhanVien
                        {
                            TaiKhoanID = taiKhoan.TaiKhoanID,
                            HoTen = dto.HoTen ?? dto.Email,
                            SoDienThoai = dto.SoDienThoai,
                            NgayVaoLam = DateTime.Now,
                            TrangThai = true
                        };
                        await _nhanVienRepo.AddAsync(nhanVien);
                        await _nhanVienRepo.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tạo tài khoản mới: {@Dto}", dto);
                throw;
            }
        }
    }
}