using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLBH.BLL.Constants;
using QLBH.BLL.DTOs;
using QLBH.DAL.Entities;
using QLBH.DAL.Repositories;
using QLBH.DAL.Data;

namespace QLBH.BLL.Services
{
    public class SanPhamService : ISanPhamService
    {
        private readonly IRepository<SanPham> _sanPhamRepo;
        private readonly IRepository<BienTheSanPham> _bienTheRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<SanPhamService> _logger;
        private readonly AppDbContext _context;   // Thêm DbContext để chạy SQL raw

        public SanPhamService(
            IRepository<SanPham> sanPhamRepo,
            IRepository<BienTheSanPham> bienTheRepo,
            IMapper mapper,
            ILogger<SanPhamService> logger,
            AppDbContext context)                // Inject AppDbContext
        {
            _sanPhamRepo = sanPhamRepo;
            _bienTheRepo = bienTheRepo;
            _mapper = mapper;
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<SanPhamDto>> GetAllAsync(string? keyword = null, int? danhMucId = null, decimal? minPrice = null, decimal? maxPrice = null, bool chiLaySanPhamHoatDong = false)
        {
            var query = _sanPhamRepo.GetAll()
                .Include(x => x.DanhMuc)
                .Include(x => x.BienTheSanPhams)
                .AsQueryable();
            // Lọc sản phẩm theo trạng thái hoạt động
            if (chiLaySanPhamHoatDong)
            {
                query = query.Where(x => x.TrangThai == SanPhamTrangThai.HoatDong);
                //Chỉ lấy sản phẩm thuộc danh mục ĐANG HOẠT ĐỘNG (TrangThai = true)
                query = query.Where(x => x.DanhMuc.TrangThai == true);
            }

                if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x => x.TenSanPham.Contains(keyword));
            if (danhMucId.HasValue)
                query = query.Where(x => x.DanhMucID == danhMucId.Value);
            if (minPrice.HasValue)
                query = query.Where(x => x.GiaCoBan >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(x => x.GiaCoBan <= maxPrice.Value);

            var entities = await query.ToListAsync();
            return _mapper.Map<IEnumerable<SanPhamDto>>(entities);
        }

        public async Task<SanPhamDto?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _sanPhamRepo.GetAll()
                    .Include(x => x.DanhMuc)
                    .Include(x => x.BienTheSanPhams)
                    .FirstOrDefaultAsync(x => x.SanPhamID == id);
                if (entity == null) return null;
                return _mapper.Map<SanPhamDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync error for id: {Id}", id);
                throw new ApplicationException("Không thể lấy chi tiết sản phẩm.");
            }
        }

        public async Task<SanPhamDto> CreateAsync(CreateSanPhamDto dto)
        {
            try
            {
                var entity = _mapper.Map<SanPham>(dto);
                entity.HinhAnh = string.IsNullOrEmpty(dto.HinhAnh) ? "/images/noimage.jpg" : dto.HinhAnh;
                entity.MoTa = dto.MoTa;
                entity.ChatLieu = dto.ChatLieu ?? string.Empty;
                entity.HuongDanBaoQuan = dto.HuongDanBaoQuan ?? string.Empty;
                entity.NgayTao = DateTime.Now;
                entity.NgayCapNhat = DateTime.Now;

                await _sanPhamRepo.AddAsync(entity);
                await _sanPhamRepo.SaveChangesAsync();
                return _mapper.Map<SanPhamDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync error");
                throw new ApplicationException("Thêm sản phẩm thất bại.");
            }
        }

        public async Task<SanPhamDto?> UpdateAsync(UpdateSanPhamDto dto)
        {
            try
            {
                // Kiểm tra tồn tại
                var exists = await _sanPhamRepo.GetByIdAsync(dto.SanPhamID);
                if (exists == null) return null;

                // Xây dựng câu lệnh SQL cập nhật các trường (tránh trigger)
                string sql = @"
            UPDATE SanPham 
            SET DanhMucID = {0},
                TenSanPham = {1},
                GiaCoBan = {2},
                HinhAnh = {3},
                MoTa = {4},
                ThuongHieu = {5},
                ChatLieu = {6},
                HuongDanBaoQuan = {7},
                TrangThai = {8},
                NgayCapNhat = GETDATE()
            WHERE SanPhamID = {9}";

                int rows = await _context.Database.ExecuteSqlRawAsync(sql,
                    dto.DanhMucID,
                    dto.TenSanPham,
                    dto.GiaCoBan,
                    dto.HinhAnh ?? "no-image.png",
                    dto.MoTa ?? "",
                    dto.ThuongHieu ?? "",
                    dto.ChatLieu ?? "",
                    dto.HuongDanBaoQuan ?? "",
                    dto.TrangThai,
                    dto.SanPhamID);

                if (rows == 0) return null;

                if (SanPhamTrangThai.IsNgungBan(dto.TrangThai))
                    await NgungTatCaBienTheAsync(dto.SanPhamID);

                // Sau khi update, lấy lại entity để trả về DTO (có thể query lại bằng EF Core hoặc SQL)
                var updated = await _sanPhamRepo.GetByIdAsync(dto.SanPhamID);
                return _mapper.Map<SanPhamDto>(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật sản phẩm {Id}", dto.SanPhamID);
                throw new ApplicationException($"Cập nhật thất bại: {ex.Message}");
            }
        }


        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                // Lấy sản phẩm kèm biến thể (chỉ để kiểm tra, không dùng để lưu)
                var entity = await _sanPhamRepo.GetAll()
                    .Include(x => x.BienTheSanPhams)
                    .FirstOrDefaultAsync(x => x.SanPhamID == id);

                if (entity == null) return false;

                // Kiểm tra có biến thể hay không
                if (entity.BienTheSanPhams != null && entity.BienTheSanPhams.Any())
                {
                    // Còn biến thể: cập nhật trạng thái = 'Ngung' bằng SQL RAW
                    int rows = await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE SanPham SET TrangThai = N'Ngung', NgayCapNhat = GETDATE() WHERE SanPhamID = {0}",
                        id);

                    if (rows > 0)
                    {
                        await NgungTatCaBienTheAsync(id);
                        throw new InvalidOperationException(
                            "Sản phẩm còn biến thể, không thể xóa. Đã chuyển trạng thái thành Ngưng hoạt động. " +
                            "Vui lòng xóa các biến thể trước nếu muốn xóa sản phẩm."
                        );
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    // Không còn biến thể: xóa cứng bằng SQL RAW
                    int affected = await _context.Database.ExecuteSqlRawAsync(
                        "DELETE FROM SanPham WHERE SanPhamID = {0}", id);
                    return affected > 0;
                }
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                _logger.LogError(ex, "Lỗi DB khi xóa sản phẩm {Id}", id);
                throw new ApplicationException($"Xóa sản phẩm thất bại: {ex.Message}");
            }
        }

        public async Task<IEnumerable<BienTheSanPhamDto>> GetAllBienTheAsync(int sanPhamId)
        {
            try
            {
                var list = await _bienTheRepo.GetAll()
                    .Include(x => x.SanPham)
                    .Where(x => x.SanPhamID == sanPhamId)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<BienTheSanPhamDto>>(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllBienTheAsync error for sanPhamId: {Id}", sanPhamId);
                throw new ApplicationException("Không thể lấy danh sách biến thể.");
            }
        }

        public async Task<BienTheSanPhamDto?> GetBienTheByIdAsync(int bienTheId)
        {
            try
            {
                var entity = await _bienTheRepo.GetByIdAsync(bienTheId);
                return entity == null ? null : _mapper.Map<BienTheSanPhamDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBienTheByIdAsync error for id: {Id}", bienTheId);
                throw new ApplicationException("Không thể lấy chi tiết biến thể.");
            }
        }

        public async Task<BienTheSanPhamDto> CreateBienTheAsync(CreateBienTheSanPhamDto dto)
        {
            try
            {
                var sanPham = await _sanPhamRepo.GetByIdAsync(dto.SanPhamID);
                if (sanPham == null)
                    throw new ApplicationException("Sản phẩm không tồn tại.");

                var entity = _mapper.Map<BienTheSanPham>(dto);
                entity.TrangThai = !SanPhamTrangThai.IsNgungBan(sanPham.TrangThai);
                await _bienTheRepo.AddAsync(entity);
                await _bienTheRepo.SaveChangesAsync();
                return _mapper.Map<BienTheSanPhamDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateBienTheAsync error");
                throw new ApplicationException("Thêm biến thể thất bại.");
            }
        }

        public async Task<BienTheSanPhamDto?> UpdateBienTheAsync(UpdateBienTheSanPhamDto dto)
        {
            try
            {
                var sanPham = await _sanPhamRepo.GetByIdAsync(dto.SanPhamID);
                if (sanPham != null && SanPhamTrangThai.IsNgungBan(sanPham.TrangThai) && dto.TrangThai)
                    throw new ApplicationException("Không thể kích hoạt biến thể khi sản phẩm đang ngưng hoạt động.");

                var entity = await _bienTheRepo.GetByIdAsync(dto.BienTheID);
                if (entity == null) return null;
                _mapper.Map(dto, entity);
                _bienTheRepo.Update(entity);
                await _bienTheRepo.SaveChangesAsync();
                return _mapper.Map<BienTheSanPhamDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateBienTheAsync error for id: {Id}", dto.BienTheID);
                throw new ApplicationException("Cập nhật biến thể thất bại.");
            }
        }

        public async Task<bool> DeleteBienTheAsync(int bienTheId)
        {
            try
            {
                var entity = await _bienTheRepo.GetByIdAsync(bienTheId);
                if (entity == null) return false;
                _bienTheRepo.Delete(entity);
                await _bienTheRepo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteBienTheAsync error for id: {Id}", bienTheId);
                throw new ApplicationException("Xóa biến thể thất bại.");
            }
        }

        private async Task NgungTatCaBienTheAsync(int sanPhamId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "UPDATE BienTheSanPham SET TrangThai = 0 WHERE SanPhamID = {0} AND TrangThai = 1",
                sanPhamId);
        }
    }
}