USE QuanLyBanHangThoiTrang;
GO

-- ============================================================
-- 1. VIEWS
-- ============================================================

-- 1.1 Danh sÃ¡ch sáº£n pháº©m (kÃ¨m tá»•ng tá»“n kho, sá»‘ biáº¿n thá»ƒ)
CREATE OR ALTER VIEW vw_DanhSachSanPham AS
SELECT
    sp.SanPhamID,
    sp.TenSanPham,
    sp.GiaCoBan,
    sp.HinhAnh,
    sp.ThuongHieu,
    sp.TrangThai,
    sp.NgayTao,
    dm.TenDanhMuc,
    COUNT(bt.BienTheID) AS SoBienThe,
    ISNULL(SUM(bt.SoLuongTon), 0) AS TongTonKho
FROM SanPham sp
INNER JOIN DanhMuc dm ON sp.DanhMucID = dm.DanhMucID
LEFT JOIN BienTheSanPham bt ON bt.SanPhamID = sp.SanPhamID AND bt.TrangThai = 1
GROUP BY sp.SanPhamID, sp.TenSanPham, sp.GiaCoBan, sp.HinhAnh,
         sp.ThuongHieu, sp.TrangThai, sp.NgayTao, dm.TenDanhMuc;
GO

-- 1.2 Tá»•ng quan Ä‘Æ¡n hÃ ng (kÃ¨m tÃªn khÃ¡ch, mÃ£ voucher)
CREATE OR ALTER VIEW vw_DonHangTongQuan AS
SELECT
    dh.DonHangID,
    dh.NgayDat,
    dh.HanThanhToan,
    dh.TongTien,
    dh.TrangThai,
    dh.TenNguoiNhan,
    dh.SoDienThoai,
    kh.HoTen AS TenKhachHang,
    v.MaVoucher,
    dh.GiamGia,
    COUNT(ct.ChiTietID) AS SoLuongSanPham
FROM DonHang dh
INNER JOIN KhachHang kh ON dh.KhachHangID = kh.KhachHangID
LEFT JOIN Voucher v ON dh.VoucherID = v.VoucherID
LEFT JOIN ChiTietDonHang ct ON ct.DonHangID = dh.DonHangID
GROUP BY dh.DonHangID, dh.NgayDat, dh.HanThanhToan,
         dh.TongTien, dh.TrangThai, dh.TenNguoiNhan, dh.SoDienThoai,
         kh.HoTen, v.MaVoucher, dh.GiamGia;
GO

-- 1.3 Doanh thu theo thÃ¡ng
CREATE OR ALTER VIEW vw_DoanhThuThang AS
SELECT
    YEAR(NgayDat) AS Nam,
    MONTH(NgayDat) AS Thang,
    COUNT(DonHangID) AS SoDon,
    SUM(TongTien) AS DoanhThu
FROM DonHang
WHERE TrangThai = 'DaThanhToan'
GROUP BY YEAR(NgayDat), MONTH(NgayDat);
GO

-- 1.4 Top 10 sáº£n pháº©m bÃ¡n cháº¡y
CREATE OR ALTER VIEW vw_TopSanPhamBanChay AS
SELECT TOP 10
    sp.SanPhamID,
    sp.TenSanPham,
    SUM(ct.SoLuong) AS TongSoLuongBan,
    SUM(ct.SoLuong * ct.DonGia) AS DoanhThu
FROM ChiTietDonHang ct
INNER JOIN BienTheSanPham bt ON ct.BienTheID = bt.BienTheID
INNER JOIN SanPham sp ON bt.SanPhamID = sp.SanPhamID
INNER JOIN DonHang dh ON ct.DonHangID = dh.DonHangID
WHERE dh.TrangThai = 'DaThanhToan'
GROUP BY sp.SanPhamID, sp.TenSanPham
ORDER BY TongSoLuongBan DESC;
GO

-- 1.5 Top 10 khÃ¡ch hÃ ng thÃ¢n thiáº¿t
CREATE OR ALTER VIEW vw_TopKhachHang AS
SELECT TOP 10
    kh.KhachHangID,
    kh.HoTen,
    COUNT(dh.DonHangID) AS SoDon,
    SUM(dh.TongTien) AS TongChiTieu
FROM KhachHang kh
INNER JOIN DonHang dh ON kh.KhachHangID = dh.KhachHangID
WHERE dh.TrangThai = 'DaThanhToan'
GROUP BY kh.KhachHangID, kh.HoTen
ORDER BY TongChiTieu DESC;
GO

-- 1.6 Sáº£n pháº©m sáº¯p háº¿t hÃ ng (tá»“n kho < 10)
CREATE OR ALTER VIEW vw_SanPhamSapHetHang AS
SELECT
    sp.SanPhamID,
    sp.TenSanPham,
    SUM(bt.SoLuongTon) AS TongTonKho
FROM SanPham sp
INNER JOIN BienTheSanPham bt ON sp.SanPhamID = bt.SanPhamID
GROUP BY sp.SanPhamID, sp.TenSanPham
HAVING SUM(bt.SoLuongTon) < 10;
GO

-- 1.7 Doanh thu theo ngÃ y
CREATE OR ALTER VIEW vw_DoanhThuNgay AS
SELECT
    CAST(NgayDat AS DATE) AS Ngay,
    COUNT(DonHangID) AS SoDon,
    SUM(TongTien) AS DoanhThu
FROM DonHang
WHERE TrangThai = 'DaThanhToan'
GROUP BY CAST(NgayDat AS DATE);
GO

-- 1.8 Doanh thu theo danh má»¥c
CREATE OR ALTER VIEW vw_DoanhThuTheoDanhMuc AS
SELECT
    dm.TenDanhMuc,
    SUM(ct.SoLuong * ct.DonGia) AS DoanhThu
FROM ChiTietDonHang ct
INNER JOIN BienTheSanPham bt ON ct.BienTheID = bt.BienTheID
INNER JOIN SanPham sp ON bt.SanPhamID = sp.SanPhamID
INNER JOIN DanhMuc dm ON sp.DanhMucID = dm.DanhMucID
INNER JOIN DonHang dh ON ct.DonHangID = dh.DonHangID
WHERE dh.TrangThai = 'DaThanhToan'
GROUP BY dm.TenDanhMuc;
GO

-- 1.9 ThÃ´ng tin ngÆ°á»i dÃ¹ng (káº¿t há»£p TaiKhoan + KhachHang/NhanVien)
CREATE OR ALTER VIEW vw_NguoiDung AS
SELECT
    tk.TaiKhoanID,
    tk.Email,
    tk.VaiTro,
    tk.TrangThai,
    tk.NgayTao,
    kh.HoTen AS HoTen,
    kh.SoDienThoai AS SoDienThoai,
    kh.DiaChiMacDinh AS DiaChi,
    kh.NgaySinh,
    kh.GioiTinh
FROM TaiKhoan tk
LEFT JOIN KhachHang kh ON tk.TaiKhoanID = kh.TaiKhoanID
UNION ALL
SELECT
    tk.TaiKhoanID,
    tk.Email,
    tk.VaiTro,
    tk.TrangThai,
    tk.NgayTao,
    nv.HoTen,
    nv.SoDienThoai,
    NULL AS DiaChi,
    NULL AS NgaySinh,
    NULL AS GioiTinh
FROM TaiKhoan tk
INNER JOIN NhanVien nv ON tk.TaiKhoanID = nv.TaiKhoanID;
GO

-- ============================================================
-- 2. FUNCTIONS
-- ============================================================

-- 2.1 TÃ­nh giáº£m giÃ¡ voucher
CREATE OR ALTER FUNCTION fn_TinhGiamGia
(
    @MaVoucher VARCHAR(50),
    @TamTinh DECIMAL(18,2)
)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @Giam DECIMAL(18,2) = 0;
    SELECT @Giam = CASE
        WHEN LoaiGiam = 'PhanTram'
            THEN CASE
                WHEN (GiaTriGiam / 100) * @TamTinh > ISNULL(GiamToiDa, 999999999)
                THEN GiamToiDa
                ELSE (GiaTriGiam / 100) * @TamTinh
            END
        ELSE GiaTriGiam   -- 'TienMat'
    END
    FROM Voucher
    WHERE MaVoucher = @MaVoucher
        AND TrangThai = 1
        AND GETDATE() BETWEEN NgayBatDau AND NgayKetThuc
        AND @TamTinh >= DonToiThieu
        AND DaSuDung < SoLuong;
    RETURN ISNULL(@Giam, 0);
END;
GO

-- 2.2 Tá»•ng tá»“n kho cá»§a sáº£n pháº©m
CREATE OR ALTER FUNCTION fn_TinhTonKhoSanPham
(
    @SanPhamID INT
)
RETURNS INT
AS
BEGIN
    DECLARE @TonKho INT;
    SELECT @TonKho = ISNULL(SUM(SoLuongTon), 0)
    FROM BienTheSanPham
    WHERE SanPhamID = @SanPhamID AND TrangThai = 1;
    RETURN @TonKho;
END;
GO

-- 2.3 Kiá»ƒm tra email tá»“n táº¡i
CREATE OR ALTER FUNCTION fn_KiemTraEmailTonTai
(
    @Email VARCHAR(100)
)
RETURNS BIT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM TaiKhoan WHERE Email = @Email)
        RETURN 1;
    RETURN 0;
END;
GO

-- ============================================================
-- 3. STORED PROCEDURES
-- ============================================================

-- 3.1 ThÃªm vÃ o giá» hÃ ng (tá»± táº¡o giá» má»›i náº¿u chÆ°a cÃ³)
CREATE OR ALTER PROCEDURE sp_ThemVaoGio
    @KhachHangID INT,
    @BienTheID INT,
    @SoLuong INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Kiá»ƒm tra khÃ¡ch hÃ ng tá»“n táº¡i
        IF NOT EXISTS (SELECT 1 FROM KhachHang WHERE KhachHangID = @KhachHangID)
        BEGIN
            RAISERROR(N'KhÃ¡ch hÃ ng khÃ´ng tá»“n táº¡i. Vui lÃ²ng Ä‘Äƒng nháº­p láº¡i.', 16, 1);
            RETURN;
        END

        -- Kiá»ƒm tra biáº¿n thá»ƒ vÃ  tá»“n kho
        DECLARE @Ton INT;
        SELECT @Ton = SoLuongTon FROM BienTheSanPham WHERE BienTheID = @BienTheID;
        IF @Ton IS NULL
        BEGIN
            RAISERROR(N'Sáº£n pháº©m khÃ´ng tá»“n táº¡i.', 16, 1);
            RETURN;
        END
        IF @Ton < @SoLuong
        BEGIN
            RAISERROR(N'Sá»‘ lÆ°á»£ng yÃªu cáº§u vÆ°á»£t quÃ¡ tá»“n kho.', 16, 1);
            RETURN;
        END

        BEGIN TRANSACTION;

        -- Láº¥y hoáº·c táº¡o giá» hÃ ng
        DECLARE @GioHangID INT;
        SELECT @GioHangID = GioHangID FROM GioHang WHERE KhachHangID = @KhachHangID;
        IF @GioHangID IS NULL
        BEGIN
            INSERT INTO GioHang (KhachHangID, NgayTao, NgayCapNhat)
            VALUES (@KhachHangID, GETDATE(), GETDATE());
            SET @GioHangID = SCOPE_IDENTITY();
        END

        -- ThÃªm hoáº·c cáº­p nháº­t chi tiáº¿t giá»
        IF EXISTS (SELECT 1 FROM ChiTietGioHang WHERE GioHangID = @GioHangID AND BienTheID = @BienTheID)
            UPDATE ChiTietGioHang
            SET SoLuong = SoLuong + @SoLuong
            WHERE GioHangID = @GioHangID AND BienTheID = @BienTheID;
        ELSE
            INSERT INTO ChiTietGioHang (GioHangID, BienTheID, SoLuong)
            VALUES (@GioHangID, @BienTheID, @SoLuong);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
-- 3.2 XÃ³a toÃ n bá»™ giá» hÃ ng
CREATE OR ALTER PROCEDURE sp_XoaGioHang
    @KhachHangID INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @GioHangID INT = (SELECT GioHangID FROM GioHang WHERE KhachHangID = @KhachHangID);
    IF @GioHangID IS NOT NULL
        DELETE FROM ChiTietGioHang WHERE GioHangID = @GioHangID;
END;
GO

-- 3.3 Táº¡o Ä‘Æ¡n hÃ ng tá»« giá» (cÃ³ transaction)
CREATE OR ALTER PROCEDURE sp_TaoDonHangTuGio
    @KhachHangID INT,
    @TenNguoiNhan NVARCHAR(100),
    @SoDienThoai VARCHAR(15),
    @DiaChi NVARCHAR(300),
    @MaVoucher VARCHAR(50) = NULL,
    @GhiChu NVARCHAR(500) = NULL,
    @PhuongThucThanhToan NVARCHAR(50) = 'COD',
    @DonHangID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @VoucherID INT = NULL;
        DECLARE @GiamGia DECIMAL(18,2) = 0;
        DECLARE @TamTinh DECIMAL(18,2);
        
        -- TÃ­nh tá»•ng táº¡m tÃ­nh tá»« giá» hÃ ng
        SELECT @TamTinh = SUM(ISNULL(bt.Gia, sp.GiaCoBan) * ct.SoLuong)
        FROM ChiTietGioHang ct
        INNER JOIN GioHang gh ON ct.GioHangID = gh.GioHangID
        INNER JOIN BienTheSanPham bt ON ct.BienTheID = bt.BienTheID
        INNER JOIN SanPham sp ON bt.SanPhamID = sp.SanPhamID
        WHERE gh.KhachHangID = @KhachHangID;
        
        IF @TamTinh IS NULL 
            THROW 50000, N'Giá» hÃ ng trá»‘ng', 1;
        
        -- Xá»­ lÃ½ voucher náº¿u cÃ³
        IF @MaVoucher IS NOT NULL
        BEGIN
            SELECT @VoucherID = VoucherID, @GiamGia = dbo.fn_TinhGiamGia(@MaVoucher, @TamTinh)
            FROM Voucher WHERE MaVoucher = @MaVoucher;
        END
        
        DECLARE @TongTien DECIMAL(18,2) = @TamTinh - ISNULL(@GiamGia, 0);
        
        -- Táº¡o Ä‘Æ¡n hÃ ng (Ä‘Ã£ sá»­a Ä‘Ãºng tÃªn cá»™t)
        INSERT INTO DonHang (
            KhachHangID, VoucherID, NgayDat, HanThanhToan,
            TamTinh, GiamGia, TongTien, TrangThai,
            TenNguoiNhan, SoDienThoai, DiaChi, GhiChu,
            PhuongThucThanhToan  
        )
        VALUES (
            @KhachHangID, @VoucherID, GETDATE(), DATEADD(MINUTE, 15, GETDATE()),
            @TamTinh, @GiamGia, @TongTien, 'ChoXuLy',
            @TenNguoiNhan, @SoDienThoai, @DiaChi, @GhiChu,
            @PhuongThucThanhToan   -- â† giÃ¡ trá»‹ Ä‘Ãºng
        );
        
        SET @DonHangID = SCOPE_IDENTITY();
        
        -- Táº¡o chi tiáº¿t Ä‘Æ¡n hÃ ng tá»« giá»
        INSERT INTO ChiTietDonHang (DonHangID, BienTheID, TenSanPham, ThongTin, SoLuong, DonGia)
        SELECT @DonHangID, ct.BienTheID, sp.TenSanPham, bt.Size + ' - ' + bt.MauSac, ct.SoLuong, ISNULL(bt.Gia, sp.GiaCoBan)
        FROM ChiTietGioHang ct
        INNER JOIN GioHang gh ON ct.GioHangID = gh.GioHangID
        INNER JOIN BienTheSanPham bt ON ct.BienTheID = bt.BienTheID
        INNER JOIN SanPham sp ON bt.SanPhamID = sp.SanPhamID
        WHERE gh.KhachHangID = @KhachHangID;
        
        -- Cáº­p nháº­t sá»‘ láº§n sá»­ dá»¥ng voucher
        IF @VoucherID IS NOT NULL
            UPDATE Voucher SET DaSuDung = DaSuDung + 1 WHERE VoucherID = @VoucherID;
        
        -- XÃ³a giá» hÃ ng
        DELETE ct 
        FROM ChiTietGioHang ct 
        INNER JOIN GioHang gh ON ct.GioHangID = gh.GioHangID 
        WHERE gh.KhachHangID = @KhachHangID;
        
        -- Táº¡o báº£n ghi thanh toÃ¡n
        INSERT INTO ThanhToan (DonHangID, SoTien, PhuongThuc, TrangThai, NgayXuLy, GhiChu)
        VALUES (@DonHangID, @TongTien, @PhuongThucThanhToan, 'ChoDuyet', NULL, N'ÄÆ°á»£c táº¡o tá»± Ä‘á»™ng tá»« Ä‘Æ¡n hÃ ng');
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- 3.4 XÃ¡c nháº­n / tá»« chá»‘i thanh toÃ¡n (cÃ³ transaction)
IF OBJECT_ID('sp_XacNhanThanhToan', 'P') IS NOT NULL
    DROP PROCEDURE sp_XacNhanThanhToan;
GO
CREATE PROCEDURE sp_XacNhanThanhToan
    @ThanhToanID INT,
    @NhanVienID INT,
    @ChapNhan BIT,
    @GhiChu NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DonHangID INT;
    SELECT @DonHangID = DonHangID FROM ThanhToan WHERE ThanhToanID = @ThanhToanID;
    IF @ChapNhan = 1
    BEGIN
        BEGIN TRANSACTION;
        BEGIN TRY
            IF EXISTS (
                SELECT 1 FROM ChiTietDonHang ct
                INNER JOIN BienTheSanPham bt ON ct.BienTheID = bt.BienTheID
                WHERE ct.DonHangID = @DonHangID AND bt.SoLuongTon < ct.SoLuong
            )
            BEGIN
                UPDATE DonHang SET TrangThai = 'DaHuy' WHERE DonHangID = @DonHangID;
                UPDATE ThanhToan SET TrangThai = 'TuChoi' WHERE ThanhToanID = @ThanhToanID;
                COMMIT TRANSACTION;
                RETURN;
            END
            UPDATE bt SET SoLuongTon = SoLuongTon - ct.SoLuong
            FROM BienTheSanPham bt
            INNER JOIN ChiTietDonHang ct ON bt.BienTheID = ct.BienTheID
            WHERE ct.DonHangID = @DonHangID;
            UPDATE DonHang SET TrangThai = 'DaThanhToan' WHERE DonHangID = @DonHangID;
            UPDATE ThanhToan SET TrangThai = 'DaDuyet', NhanVienID = @NhanVienID, NgayXuLy = GETDATE(), GhiChu = @GhiChu
            WHERE ThanhToanID = @ThanhToanID;
            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH
    END
    ELSE
    BEGIN
        UPDATE DonHang SET TrangThai = 'DaHuy' WHERE DonHangID = @DonHangID;
        UPDATE ThanhToan SET TrangThai = 'TuChoi' WHERE ThanhToanID = @ThanhToanID;
    END
END;
GO

-- 3.5 Láº¥y thÃ´ng tin ngÆ°á»i dÃ¹ng theo ID (káº¿t há»£p cáº£ khÃ¡ch vÃ  nhÃ¢n viÃªn)
CREATE OR ALTER PROCEDURE sp_GetNguoiDungById
    @TaiKhoanID INT
AS
BEGIN
    SELECT tk.TaiKhoanID, tk.Email, tk.VaiTro, tk.TrangThai, tk.NgayTao,
           kh.HoTen, kh.SoDienThoai, kh.DiaChiMacDinh, kh.NgaySinh, kh.GioiTinh,
           nv.HoTen AS NhanVienHoTen, nv.SoDienThoai AS NhanVienSDT, nv.NgayVaoLam
    FROM TaiKhoan tk
    LEFT JOIN KhachHang kh ON tk.TaiKhoanID = kh.TaiKhoanID
    LEFT JOIN NhanVien nv ON tk.TaiKhoanID = nv.TaiKhoanID
    WHERE tk.TaiKhoanID = @TaiKhoanID;
END;
GO

-- 3.6 Cáº­p nháº­t tráº¡ng thÃ¡i Ä‘Æ¡n hÃ ng
CREATE OR ALTER PROCEDURE sp_CapNhatTrangThaiDonHang
    @DonHangID INT,
    @TrangThaiMoi NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE DonHang SET TrangThai = @TrangThaiMoi WHERE DonHangID = @DonHangID;
END;
GO

-- ============================================================
-- 4. TRIGGERS
-- ============================================================

-- 4.1 Tá»± Ä‘á»™ng há»§y Ä‘Æ¡n quÃ¡ háº¡n (khi UPDATE)
CREATE OR ALTER TRIGGER trg_HuyDonQuaHan
ON DonHang
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE DonHang
    SET TrangThai = 'DaHuy'
    WHERE TrangThai = 'ChoXuLy' AND HanThanhToan < GETDATE();
END;
GO

-- 4.2 Cáº­p nháº­t NgayCapNhat khi sá»­a sáº£n pháº©m
CREATE OR ALTER TRIGGER trg_UpdateSanPham
ON SanPham
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE SanPham
    SET NgayCapNhat = GETDATE()
    WHERE SanPhamID IN (SELECT SanPhamID FROM inserted);
END;
GO

-- 4.3 Tá»± Ä‘á»™ng cáº­p nháº­t tá»•ng tiá»n Ä‘Æ¡n hÃ ng khi chi tiáº¿t thay Ä‘á»•i
CREATE OR ALTER TRIGGER trg_UpdateTongTienDonHang
ON ChiTietDonHang
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DonHangIds TABLE (DonHangID INT);
    INSERT INTO @DonHangIds SELECT DonHangID FROM inserted;
    INSERT INTO @DonHangIds SELECT DonHangID FROM deleted;
    UPDATE dh
    SET TamTinh = (SELECT ISNULL(SUM(SoLuong * DonGia), 0) FROM ChiTietDonHang WHERE DonHangID = dh.DonHangID),
        TongTien = (SELECT ISNULL(SUM(SoLuong * DonGia), 0) FROM ChiTietDonHang WHERE DonHangID = dh.DonHangID) - dh.GiamGia
    FROM DonHang dh
    WHERE dh.DonHangID IN (SELECT DISTINCT DonHangID FROM @DonHangIds);
END;
GO



--===================================Bá»” SUNG CHO CHá»¨C NÄ‚NG QUáº¢N LÃ KHO ================================================
-- ============================================================
-- 2. Function tÃ­nh tá»•ng tiá»n phiáº¿u nháº­p (khÃ´ng báº¯t buá»™c nhÆ°ng minh há»a)
-- ============================================================
USE QuanLyBanHangThoiTrang;
GO

-- ============================================================
-- 1. FUNCTION: TÃ­nh tá»•ng tiá»n cá»§a phiáº¿u nháº­p (dÃ¹ng cho trigger)
-- ============================================================
IF OBJECT_ID('fn_TinhTongTienPhieuNhap', 'FN') IS NOT NULL
    DROP FUNCTION fn_TinhTongTienPhieuNhap;
GO
CREATE FUNCTION fn_TinhTongTienPhieuNhap (@PhieuNhapID INT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    RETURN (SELECT ISNULL(SUM(ThanhTien), 0) FROM ChiTietPhieuNhap WHERE PhieuNhapID = @PhieuNhapID);
END;
GO

-- ============================================================
-- 2. VIEW: Danh sÃ¡ch phiáº¿u nháº­p kÃ¨m tÃªn nhÃ¢n viÃªn (dÃ¹ng LEFT JOIN an toÃ n)
-- ============================================================
IF OBJECT_ID('vw_PhieuNhap', 'V') IS NOT NULL
    DROP VIEW vw_PhieuNhap;
GO
CREATE VIEW vw_PhieuNhap AS
SELECT 
    pn.PhieuNhapID,
    pn.NgayNhap,
    pn.TongTien,
    pn.GhiChu,
    ISNULL(nv.HoTen, N'NhÃ¢n viÃªn #' + CAST(pn.NhanVienID AS NVARCHAR(10))) AS TenNhanVien
FROM PhieuNhap pn
LEFT JOIN NhanVien nv ON pn.NhanVienID = nv.NhanVienID;
GO

-- ============================================================
-- 3. VIEW: Chi tiáº¿t phiáº¿u nháº­p (kÃ¨m thÃ´ng tin sáº£n pháº©m tá»« báº£ng BienTheSanPham, SanPham)
--    Sá»­ dá»¥ng LEFT JOIN Ä‘á»ƒ trÃ¡nh lá»—i náº¿u chÆ°a cÃ³ dá»¯ liá»‡u
-- ============================================================
IF OBJECT_ID('vw_ChiTietPhieuNhap', 'V') IS NOT NULL
    DROP VIEW vw_ChiTietPhieuNhap;
GO
CREATE VIEW vw_ChiTietPhieuNhap AS
SELECT 
    ct.ChiTietID,
    ct.PhieuNhapID,
    ct.BienTheID,
    ct.SoLuong,
    ct.GiaNhap,
    ct.ThanhTien,
    ISNULL(sp.TenSanPham, N'KhÃ´ng xÃ¡c Ä‘á»‹nh') AS TenSanPham,
    ISNULL(bt.Size, N'') AS Size,
    ISNULL(bt.MauSac, N'') AS MauSac,
    ISNULL(bt.Gia, 0) AS GiaBanHienTai
FROM ChiTietPhieuNhap ct
LEFT JOIN BienTheSanPham bt ON ct.BienTheID = bt.BienTheID
LEFT JOIN SanPham sp ON bt.SanPhamID = sp.SanPhamID;
GO

-- ============================================================
-- 4. STORED PROCEDURE: Nháº­p kho (táº¡o phiáº¿u nháº­p + chi tiáº¿t + cáº­p nháº­t tá»“n kho)
--    CÃ³ transaction, kiá»ƒm tra tá»“n táº¡i cá»§a biáº¿n thá»ƒ
-- ============================================================
IF TYPE_ID(N'dbo.ChiTietPhieuNhapType') IS NULL
BEGIN
    EXEC('CREATE TYPE dbo.ChiTietPhieuNhapType AS TABLE
    (
        BienTheID INT NOT NULL,
        SoLuong INT NOT NULL,
        GiaNhap DECIMAL(18,2) NOT NULL
    )');
END
GO
IF OBJECT_ID('sp_NhapKho', 'P') IS NOT NULL
    DROP PROCEDURE sp_NhapKho;
GO
CREATE PROCEDURE sp_NhapKho
   @NhanVienID INT,
    @GhiChu NVARCHAR(500) = NULL,
    @ChiTietList dbo.ChiTietPhieuNhapType READONLY,
    @PhieuNhapID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @TongTien DECIMAL(18,2);
        SELECT @TongTien = SUM(SoLuong * GiaNhap) FROM @ChiTietList;
        
        INSERT INTO PhieuNhap (NhanVienID, NgayNhap, TongTien, GhiChu)
        VALUES (@NhanVienID, GETDATE(), @TongTien, @GhiChu);
        SET @PhieuNhapID = SCOPE_IDENTITY();
        
        -- âœ… Sá»­a: thÃªm cá»™t ThanhTien vÃ  tÃ­nh giÃ¡ trá»‹
        INSERT INTO ChiTietPhieuNhap (PhieuNhapID, BienTheID, SoLuong, GiaNhap, ThanhTien)
        SELECT @PhieuNhapID, BienTheID, SoLuong, GiaNhap, SoLuong * GiaNhap
        FROM @ChiTietList;
        
        -- Cáº­p nháº­t tá»“n kho
        UPDATE bt
        SET bt.SoLuongTon = ISNULL(bt.SoLuongTon, 0) + ct.SoLuong
        FROM BienTheSanPham bt
        INNER JOIN @ChiTietList ct ON bt.BienTheID = ct.BienTheID;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ============================================================
-- 5. STORED PROCEDURE: Láº¥y danh sÃ¡ch phiáº¿u nháº­p (cÃ³ phÃ¢n trang, lá»c theo ngÃ y)
-- ============================================================
CREATE OR ALTER PROCEDURE sp_GetPhieuNhap
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL,
    @PageIndex INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        pn.PhieuNhapID,
        pn.NgayNhap,
        pn.TongTien,
        pn.GhiChu,
        ISNULL(nv.HoTen, CAST(pn.NhanVienID AS NVARCHAR(10))) AS TenNhanVien,
        (SELECT COUNT(*) FROM ChiTietPhieuNhap WHERE PhieuNhapID = pn.PhieuNhapID) AS SoLuongMatHang
    FROM PhieuNhap pn
    LEFT JOIN NhanVien nv ON pn.NhanVienID = nv.NhanVienID
    WHERE (@TuNgay IS NULL OR CAST(pn.NgayNhap AS DATE) >= @TuNgay)
      AND (@DenNgay IS NULL OR CAST(pn.NgayNhap AS DATE) <= @DenNgay)
    ORDER BY pn.NgayNhap DESC
    OFFSET (@PageIndex - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- ============================================================
-- 6. STORED PROCEDURE: Láº¥y chi tiáº¿t phiáº¿u nháº­p theo ID
-- ============================================================
IF OBJECT_ID('sp_GetChiTietPhieuNhap', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetChiTietPhieuNhap;
GO
CREATE PROCEDURE sp_GetChiTietPhieuNhap @PhieuNhapID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM vw_ChiTietPhieuNhap WHERE PhieuNhapID = @PhieuNhapID;
END;
GO

-- ============================================================
-- 7. TRIGGER: Tá»± Ä‘á»™ng cáº­p nháº­t tá»•ng tiá»n phiáº¿u nháº­p khi chi tiáº¿t thay Ä‘á»•i
-- ============================================================
IF OBJECT_ID('trg_UpdateTongTienPhieuNhap', 'TR') IS NOT NULL
    DROP TRIGGER trg_UpdateTongTienPhieuNhap;
GO
CREATE TRIGGER trg_UpdateTongTienPhieuNhap ON ChiTietPhieuNhap
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF (ROWCOUNT_BIG() = 0) RETURN;
    
    UPDATE pn
    SET TongTien = dbo.fn_TinhTongTienPhieuNhap(pn.PhieuNhapID)
    FROM PhieuNhap pn
    WHERE pn.PhieuNhapID IN (SELECT PhieuNhapID FROM inserted UNION SELECT PhieuNhapID FROM deleted);
END;
GO

-- ============================================================
-- 8. (TÃ¹y chá»n) FUNCTION: Kiá»ƒm tra tá»“n kho sau nháº­p (minh há»a)
-- ============================================================
IF OBJECT_ID('fn_KiemTraTonKho', 'FN') IS NOT NULL
    DROP FUNCTION fn_KiemTraTonKho;
GO
CREATE FUNCTION fn_KiemTraTonKho (@BienTheID INT)
RETURNS INT
AS
BEGIN
    RETURN (SELECT ISNULL(SoLuongTon, 0) FROM BienTheSanPham WHERE BienTheID = @BienTheID);
END;
GO



IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CK_Voucher_Ngay'
      AND parent_object_id = OBJECT_ID('Voucher')
)
BEGIN
    ALTER TABLE Voucher
    ADD CONSTRAINT CK_Voucher_Ngay CHECK (NgayBatDau <= NgayKetThuc);
END
GO

-- ============================================================
-- Äá»“ng bá»™: sáº£n pháº©m ngÆ°ng/háº¿t hÃ ng => táº¥t cáº£ biáº¿n thá»ƒ ngÆ°ng bÃ¡n
-- Cháº¡y má»™t láº§n sau khi cáº­p nháº­t logic á»©ng dá»¥ng (náº¿u DB Ä‘Ã£ cÃ³ dá»¯ liá»‡u cÅ©)
-- ============================================================
UPDATE bt
SET bt.TrangThai = 0
FROM BienTheSanPham bt
INNER JOIN SanPham sp ON bt.SanPhamID = sp.SanPhamID
WHERE sp.TrangThai IN (N'Ngung', N'HetHang') AND bt.TrangThai = 1;
GO


