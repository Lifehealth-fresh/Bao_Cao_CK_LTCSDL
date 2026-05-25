USE QuanLyBanHangThoiTrang;
GO

-- Reset dữ liệu để nạp lại mẫu (chỉ dùng cho môi trường dev/test)
IF OBJECT_ID('trg_UpdateTongTienDonHang', 'TR') IS NOT NULL
    DISABLE TRIGGER trg_UpdateTongTienDonHang ON ChiTietDonHang;
GO

DELETE FROM ThanhToan;
DELETE FROM ChiTietDonHang;
DELETE FROM DonHang;
DELETE FROM ChiTietGioHang;
DELETE FROM GioHang;
DELETE FROM ChiTietPhieuNhap;
DELETE FROM PhieuNhap;
DELETE FROM BienTheSanPham;
DELETE FROM SanPham;
DELETE FROM DanhMuc;
DELETE FROM Voucher;
DELETE FROM KhachHang;
DELETE FROM NhanVien;
DELETE FROM TaiKhoan;
GO

DBCC CHECKIDENT ('ThanhToan', RESEED, 0);
DBCC CHECKIDENT ('DonHang', RESEED, 0);
DBCC CHECKIDENT ('ChiTietDonHang', RESEED, 0);
DBCC CHECKIDENT ('GioHang', RESEED, 0);
DBCC CHECKIDENT ('ChiTietGioHang', RESEED, 0);
DBCC CHECKIDENT ('PhieuNhap', RESEED, 0);
DBCC CHECKIDENT ('ChiTietPhieuNhap', RESEED, 0);
DBCC CHECKIDENT ('BienTheSanPham', RESEED, 0);
DBCC CHECKIDENT ('SanPham', RESEED, 0);
DBCC CHECKIDENT ('DanhMuc', RESEED, 0);
DBCC CHECKIDENT ('Voucher', RESEED, 0);
DBCC CHECKIDENT ('KhachHang', RESEED, 0);
DBCC CHECKIDENT ('NhanVien', RESEED, 0);
DBCC CHECKIDENT ('TaiKhoan', RESEED, 0);
GO

-- =====================================================
-- TAI KHOAN (30 dữ liệu thực tế)
-- =====================================================
INSERT INTO TaiKhoan (Email, MatKhauHash, VaiTro, TrangThai, NgayTao)
VALUES
('admin@4menshop.com','123456','NhanVien',1,GETDATE()),
('manager@routine.vn','123456','NhanVien',1,GETDATE()),
('sale01@yody.vn','123456','NhanVien',1,GETDATE()),
('sale02@coolmate.me','123456','NhanVien',1,GETDATE()),
('support@canifa.com','123456','NhanVien',1,GETDATE()),

('nguyenvanan@gmail.com','123456','KhachHang',1,GETDATE()),
('tranthibich@gmail.com','123456','KhachHang',1,GETDATE()),
('lehoangnam@gmail.com','123456','KhachHang',1,GETDATE()),
('phamngocmai@gmail.com','123456','KhachHang',1,GETDATE()),
('vuminhquan@gmail.com','123456','KhachHang',1,GETDATE()),
('dangthutrang@gmail.com','123456','KhachHang',1,GETDATE()),
('buituananh@gmail.com','123456','KhachHang',1,GETDATE()),
('ngothanhhuyen@gmail.com','123456','KhachHang',1,GETDATE()),
('dinhquochuy@gmail.com','123456','KhachHang',1,GETDATE()),
('hoangminhduc@gmail.com','123456','KhachHang',1,GETDATE()),
('caothaovy@gmail.com','123456','KhachHang',1,GETDATE()),
('lygiabao@gmail.com','123456','KhachHang',1,GETDATE()),
('tranquynhanh@gmail.com','123456','KhachHang',1,GETDATE()),
('phantuankiet@gmail.com','123456','KhachHang',1,GETDATE()),
('vothanhtruc@gmail.com','123456','KhachHang',1,GETDATE()),
('huynhngoclinh@gmail.com','123456','KhachHang',1,GETDATE()),
('nguyenquocbao@gmail.com','123456','KhachHang',1,GETDATE()),
('phamthanhdat@gmail.com','123456','KhachHang',1,GETDATE()),
('tranhaimy@gmail.com','123456','KhachHang',1,GETDATE()),
('danghoanglong@gmail.com','123456','KhachHang',1,GETDATE()),
('vuminhthu@gmail.com','123456','KhachHang',1,GETDATE()),
('lethaonhi@gmail.com','123456','KhachHang',1,GETDATE()),
('doquanghuy@gmail.com','123456','KhachHang',1,GETDATE()),
('nguyenkimngan@gmail.com','123456','KhachHang',1,GETDATE()),
('buitrungkien@gmail.com','123456','KhachHang',1,GETDATE());
GO

-- =====================================================
-- NHAN VIEN
-- =====================================================
INSERT INTO NhanVien
(TaiKhoanID, HoTen, SoDienThoai, NgayVaoLam, TrangThai)
VALUES
(1,N'Nguyễn Văn Phúc','0909000001','2023-01-10',1),
(2,N'Trần Thị Thu Hà','0909000002','2023-02-15',1),
(3,N'Lê Minh Quân','0909000003','2023-03-01',1),
(4,N'Phạm Quốc Bảo','0909000004','2023-04-20',1),
(5,N'Đặng Mỹ Linh','0909000005','2023-05-05',1);
GO

-- =====================================================
-- KHACH HANG
-- =====================================================
INSERT INTO KhachHang
(TaiKhoanID,HoTen,NgaySinh,GioiTinh,SoDienThoai,DiaChiMacDinh,NgayTao,NgayCapNhat)
VALUES
(6,N'Nguyễn Văn An','2002-01-10','Nam','0911111111',N'Hồ Chí Minh',GETDATE(),GETDATE()),
(7,N'Trần Thị Bích','2001-02-12','Nu','0911111112',N'Hà Nội',GETDATE(),GETDATE()),
(8,N'Lê Hoàng Nam','2000-03-15','Nam','0911111113',N'Đà Nẵng',GETDATE(),GETDATE()),
(9,N'Phạm Ngọc Mai','2003-04-18','Nu','0911111114',N'Cần Thơ',GETDATE(),GETDATE()),
(10,N'Vũ Minh Quân','2002-05-21','Nam','0911111115',N'Bình Dương',GETDATE(),GETDATE()),
(11,N'Đặng Thu Trang','2001-06-25','Nu','0911111116',N'Hải Phòng',GETDATE(),GETDATE()),
(12,N'Bùi Tuấn Anh','2000-07-17','Nam','0911111117',N'Đồng Nai',GETDATE(),GETDATE()),
(13,N'Ngô Thanh Huyền','2004-08-09','Nu','0911111118',N'Hồ Chí Minh',GETDATE(),GETDATE()),
(14,N'Đinh Quốc Huy','2003-09-11','Nam','0911111119',N'Hà Nội',GETDATE(),GETDATE()),
(15,N'Hoàng Minh Đức','2002-10-13','Nam','0911111120',N'Đà Nẵng',GETDATE(),GETDATE()),
(16,N'Cao Thảo Vy','2001-11-15','Nu','0911111121',N'Long An',GETDATE(),GETDATE()),
(17,N'Lý Gia Bảo','2000-12-20','Nam','0911111122',N'Hồ Chí Minh',GETDATE(),GETDATE()),
(18,N'Trần Quỳnh Anh','2004-01-05','Nu','0911111123',N'Huế',GETDATE(),GETDATE()),
(19,N'Phan Tuấn Kiệt','2003-02-14','Nam','0911111124',N'Khánh Hòa',GETDATE(),GETDATE()),
(20,N'Võ Thanh Trúc','2002-03-22','Nu','0911111125',N'Cần Thơ',GETDATE(),GETDATE()),
(21,N'Huỳnh Ngọc Linh','2001-04-28','Nu','0911111126',N'Hồ Chí Minh',GETDATE(),GETDATE()),
(22,N'Nguyễn Quốc Bảo','2000-05-30','Nam','0911111127',N'Hà Nội',GETDATE(),GETDATE()),
(23,N'Phạm Thành Đạt','2004-06-06','Nam','0911111128',N'Đồng Nai',GETDATE(),GETDATE()),
(24,N'Trần Hải My','2003-07-07','Nu','0911111129',N'Đà Lạt',GETDATE(),GETDATE()),
(25,N'Đặng Hoàng Long','2002-08-08','Nam','0911111130',N'Bến Tre',GETDATE(),GETDATE()),
(26,N'Vũ Minh Thư','2001-09-09','Nu','0911111131',N'Hồ Chí Minh',GETDATE(),GETDATE()),
(27,N'Lê Thảo Nhi','2000-10-10','Nu','0911111132',N'Hải Phòng',GETDATE(),GETDATE()),
(28,N'Đỗ Quang Huy','2004-11-11','Nam','0911111133',N'Tây Ninh',GETDATE(),GETDATE()),
(29,N'Nguyễn Kim Ngân','2003-12-12','Nu','0911111134',N'An Giang',GETDATE(),GETDATE()),
(30,N'Bùi Trung Kiên','2002-01-20','Nam','0911111135',N'Kiên Giang',GETDATE(),GETDATE());
GO

-- =====================================================
-- DANH MUC
-- =====================================================
INSERT INTO DanhMuc (TenDanhMuc, MoTa, TrangThai)
VALUES
(N'Áo Thun',N'Áo thun nam nữ',1),
(N'Áo Polo',N'Áo polo thời trang',1),
(N'Áo Sơ Mi',N'Sơ mi công sở',1),
(N'Áo Khoác',N'Áo khoác hoodie bomber',1),
(N'Quần Jean',N'Quần jean nam nữ',1),
(N'Quần Short',N'Quần short thể thao',1),
(N'Quần Tây',N'Quần tây công sở',1),
(N'Váy Đầm',N'Đầm thời trang nữ',1),
(N'Giày Sneaker',N'Giày sneaker thời trang',1),
(N'Dép Sandal',N'Dép sandal nam nữ',1),
(N'Túi Xách',N'Túi xách thời trang',1),
(N'Balo',N'Balo đi học',1),
(N'Nón',N'Nón thời trang',1),
(N'Phụ Kiện',N'Phụ kiện thời trang',1),
(N'Thắt Lưng',N'Thắt lưng da',1),
(N'Ví Da',N'Ví nam nữ',1),
(N'Áo Hoodie',N'Hoodie local brand',1),
(N'Đồ Thể Thao',N'Trang phục thể thao',1),
(N'Đồ Ngủ',N'Pijama ngủ',1),
(N'Đồ Trẻ Em',N'Thời trang trẻ em',1),
(N'Áo Len',N'Áo len mùa đông',1),
(N'Áo Blazer',N'Blazer công sở',1),
(N'Áo Gile',N'Áo gile thời trang',1),
(N'Khăn Choàng',N'Khăn thời trang',1),
(N'Tất Vớ',N'Vớ nam nữ',1),
(N'Đồng Hồ',N'Đồng hồ thời trang',1),
(N'Mắt Kính',N'Kính mát',1),
(N'Áo Dài',N'Áo dài truyền thống',1),
(N'Đồ Bơi',N'Đồ bơi nam nữ',1),
(N'Đồ Lót',N'Đồ lót thời trang',1);
GO

-- =====================================================
-- SAN PHAM (30 dữ liệu thực tế)
-- =====================================================
INSERT INTO SanPham
(DanhMucID,TenSanPham,GiaCoBan,HinhAnh,MoTa,ThuongHieu,ChatLieu,HuongDanBaoQuan,TrangThai,NgayTao,NgayCapNhat)
VALUES
(1,N'Áo Thun Basic Trơn',199000,'/images/aothun1.jpg',N'Áo thun cotton basic',N'CoolMate',N'Cotton',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(1,N'Áo Thun Oversize Local Brand',259000,'/images/aothun2.jpg',N'Áo oversize streetwear',N'DirtyCoins',N'Cotton',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(2,N'Áo Polo Slim Fit',299000,'/images/polo1.jpg',N'Polo nam công sở',N'Yody',N'Cá sấu',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(3,N'Áo Sơ Mi Trắng Công Sở',349000,'/images/somi1.jpg',N'Sơ mi công sở cao cấp',N'Routine',N'Kate',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(4,N'Áo Khoác Hoodie Zip',499000,'/images/hoodie1.jpg',N'Hoodie có khóa kéo',N'Hades',N'Nỉ',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(5,N'Quần Jean Slim Fit',459000,'/images/jean1.jpg',N'Jean co giãn',N'Levis',N'Denim',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(6,N'Quần Short Thể Thao',199000,'/images/short1.jpg',N'Short thể thao nam',N'Nike',N'Poly',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(7,N'Quần Tây Công Sở',399000,'/images/quantay1.jpg',N'Quần tây nam',N'An Phước',N'Kaki',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(8,N'Váy Đầm Dự Tiệc',599000,'/images/dam1.jpg',N'Đầm nữ cao cấp',N'IVY Moda',N'Lụa',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(9,N'Giày Sneaker Trắng',799000,'/images/giay1.jpg',N'Sneaker basic',N'Adidas',N'Da tổng hợp',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),

(10,N'Dép Sandal Nam',299000,'/images/sandal1.jpg',N'Sandal nam thời trang',N'Biti''s',N'Cao su',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(11,N'Túi Xách Nữ Mini',459000,'/images/tuixach1.jpg',N'Túi xách nữ',N'Charles & Keith',N'Da PU',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(12,N'Balo Laptop 15 inch',699000,'/images/balo1.jpg',N'Balo chống nước',N'Arctic Hunter',N'Poly',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(13,N'Nón Bucket Local Brand',159000,'/images/non1.jpg',N'Nón bucket',N'DirtyCoins',N'Cotton',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(14,N'Vòng Tay Titan',129000,'/images/phukien1.jpg',N'Vòng tay titan nam',N'Silver Shop',N'Titan',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(15,N'Thắt Lưng Da Nam',249000,'/images/thatlung1.jpg',N'Thắt lưng da bò',N'ELLY',N'Da bò',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(16,N'Ví Da Nam Mini',199000,'/images/vida1.jpg',N'Ví da nam nhỏ gọn',N'ELLY',N'Da',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(17,N'Áo Hoodie Oversize',559000,'/images/hoodie2.jpg',N'Hoodie form rộng',N'Hades',N'Nỉ',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(18,N'Bộ Đồ Thể Thao Nam',499000,'/images/thethao1.jpg',N'Đồ thể thao',N'Puma',N'Poly',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(19,N'Pijama Nữ Dài Tay',359000,'/images/pijama1.jpg',N'Đồ ngủ nữ',N'Wannabe',N'Lụa',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),

(20,N'Áo Trẻ Em Hoạt Hình',199000,'/images/treem1.jpg',N'Áo trẻ em dễ thương',N'Canifa',N'Cotton',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(21,N'Áo Len Hàn Quốc',429000,'/images/aolen1.jpg',N'Áo len mùa đông',N'5TheWay',N'Len',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(22,N'Áo Blazer Nữ',699000,'/images/blazer1.jpg',N'Blazer nữ công sở',N'IVY Moda',N'Tuytsi',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(23,N'Áo Gile Len',259000,'/images/gile1.jpg',N'Gile thời trang',N'Routine',N'Len',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(24,N'Khăn Choàng Len',179000,'/images/khan1.jpg',N'Khăn choàng mùa đông',N'Yody',N'Len',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(25,N'Combo 5 Đôi Tất',99000,'/images/tat1.jpg',N'Tất cotton',N'CoolMate',N'Cotton',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(26,N'Đồng Hồ Nam Dây Da',1299000,'/images/dongho1.jpg',N'Đồng hồ thời trang',N'Casio',N'Thép',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(27,N'Kính Mát Unisex',359000,'/images/kinh1.jpg',N'Kính chống UV',N'Gentle Monster',N'Nhựa',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(28,N'Áo Dài Truyền Thống',899000,'/images/aodai1.jpg',N'Áo dài nữ',N'Việt Thy',N'Lụa',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(29,N'Đồ Bơi Nữ 1 Mảnh',459000,'/images/doboi1.jpg',N'Đồ bơi nữ',N'Speedo',N'Poly',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE()),
(30,N'Bộ Đồ Lót Nam',229000,'/images/dolot1.jpg',N'Đồ lót cotton',N'Jockey',N'Cotton',N'Bảo quản nơi khô, giặt nhẹ','HoatDong',GETDATE(),GETDATE());
GO

-- =====================================================
-- BIEN THE SAN PHAM
-- =====================================================
INSERT INTO BienTheSanPham
(SanPhamID,Size,MauSac,MaMau,Gia,SoLuongTon,TrangThai)
VALUES
(1,'M',N'Trắng','#FFFFFF',199000,50,1),
(2,'L',N'Đen','#000000',259000,40,1),
(3,'M',N'Xanh Navy','#1E3A5F',299000,35,1),
(4,'L',N'Trắng','#FFFFFF',349000,25,1),
(5,'XL',N'Đen','#000000',499000,20,1),
(6,'32',N'Xanh Jean','#3B5998',459000,30,1),
(7,'L',N'Đen','#000000',199000,45,1),
(8,'31',N'Đen','#000000',399000,18,1),
(9,'M',N'Đỏ','#FF0000',599000,10,1),
(10,'42',N'Trắng','#FFFFFF',799000,15,1),

(11,'41',N'Nâu','#8B4513',299000,22,1),
(12,'Free Size',N'Kem','#FFFDD0',459000,12,1),
(13,'XL',N'Đen','#000000',699000,17,1),
(14,'Free Size',N'Trắng','#FFFFFF',159000,60,1),
(15,'Free Size',N'Bạc','#C0C0C0',129000,40,1),
(16,'110cm',N'Đen','#000000',249000,25,1),
(17,'Free Size',N'Nâu','#8B4513',199000,35,1),
(18,'L',N'Xám','#808080',559000,20,1),
(19,'XL',N'Đen','#000000',499000,18,1),
(20,'M',N'Hồng','#FFC0CB',359000,14,1),

(21,'8',N'Vàng','#FFD700',199000,28,1),
(22,'L',N'Be','#F5F5DC',429000,16,1),
(23,'M',N'Đen','#000000',699000,12,1),
(24,'Free Size',N'Trắng','#FFFFFF',259000,30,1),
(25,'Free Size',N'Đỏ','#FF0000',179000,50,1),
(26,'Free Size',N'Trắng','#FFFFFF',99000,70,1),
(27,'42mm',N'Đen','#000000',1299000,8,1),
(28,'Free Size',N'Đen','#000000',359000,19,1),
(29,'L',N'Đỏ','#FF0000',899000,6,1),
(30,'XL',N'Xanh','#0000FF',459000,11,1);
GO

-- =====================================================
-- VOUCHER
-- =====================================================
INSERT INTO Voucher
(MaVoucher,MoTa,LoaiGiam,GiaTriGiam,DonToiThieu,GiamToiDa,SoLuong,DaSuDung,NgayBatDau,NgayKetThuc,TrangThai)
VALUES
('WELCOME10',N'Giảm 10% cho khách mới','PhanTram',10,300000,100000,100,0,'2026-01-01','2026-12-31',1),
('FREESHIP',N'Giảm 30k phí ship','TienMat',30000,200000,NULL,200,0,'2026-01-01','2026-12-31',1),
('SUMMER20',N'Giảm 20% mùa hè','PhanTram',20,500000,200000,50,0,'2026-05-01','2026-08-31',1),
('SALE50K',N'Giảm trực tiếp 50k','TienMat',50000,400000,NULL,80,0,'2026-01-01','2026-12-31',1),
('VIP30',N'Khách VIP giảm 30%','PhanTram',30,1000000,500000,20,0,'2026-01-01','2026-12-31',1);
GO

-- =====================================================
-- GIO HANG
-- =====================================================
INSERT INTO GioHang (KhachHangID, NgayTao, NgayCapNhat)
VALUES
(1,GETDATE(),GETDATE()),(2,GETDATE(),GETDATE()),(3,GETDATE(),GETDATE()),(4,GETDATE(),GETDATE()),(5,GETDATE(),GETDATE()),
(6,GETDATE(),GETDATE()),(7,GETDATE(),GETDATE()),(8,GETDATE(),GETDATE()),(9,GETDATE(),GETDATE()),(10,GETDATE(),GETDATE()),
(11,GETDATE(),GETDATE()),(12,GETDATE(),GETDATE()),(13,GETDATE(),GETDATE()),(14,GETDATE(),GETDATE()),(15,GETDATE(),GETDATE());
GO

-- =====================================================
-- CHI TIET GIO HANG
-- =====================================================
INSERT INTO ChiTietGioHang
(GioHangID,BienTheID,SoLuong)
VALUES
(1,1,2),
(1,10,1),
(2,5,1),
(2,6,2),
(3,12,1),
(4,15,3),
(5,8,1),
(6,20,2),
(7,25,1),
(8,30,1),
(9,18,2),
(10,22,1),
(11,3,1),
(12,9,2),
(13,27,1),
(14,29,1),
(15,14,2);
GO

-- =====================================================
-- DON HANG
-- =====================================================
INSERT INTO DonHang
(KhachHangID,VoucherID,NgayDat,HanThanhToan,TamTinh,GiamGia,TongTien,TrangThai,TenNguoiNhan,SoDienThoai,DiaChi,GhiChu,PhuongThucThanhToan)
VALUES
(1,1,GETDATE(),DATEADD(DAY,1,GETDATE()),658000,65800,592200,'DaThanhToan',N'Nguyễn Văn An','0911111111',N'Hồ Chí Minh',N'Giao giờ hành chính','COD'),
(2,2,GETDATE(),DATEADD(DAY,1,GETDATE()),499000,30000,469000,'ChoXuLy',N'Trần Thị Bích','0911111112',N'Hà Nội',N'','COD'),
(3,NULL,GETDATE(),DATEADD(DAY,1,GETDATE()),259000,0,259000,'DaThanhToan',N'Lê Hoàng Nam','0911111113',N'Đà Nẵng',N'','COD'),
(4,4,GETDATE(),DATEADD(DAY,1,GETDATE()),799000,50000,749000,'ChoXuLy',N'Phạm Ngọc Mai','0911111114',N'Cần Thơ',N'','COD'),
(5,3,GETDATE(),DATEADD(DAY,1,GETDATE()),1200000,200000,1000000,'DaThanhToan',N'Vũ Minh Quân','0911111115',N'Bình Dương',N'','COD');
GO

-- =====================================================
-- CHI TIET DON HANG
-- =====================================================
INSERT INTO ChiTietDonHang
(DonHangID,BienTheID,TenSanPham,ThongTin,SoLuong,DonGia)
VALUES
(1,1,N'Áo Thun Basic Trơn',N'Size M - Trắng',2,199000),
(1,10,N'Giày Sneaker Trắng',N'Size 42 - Trắng',1,799000),
(2,5,N'Áo Khoác Hoodie Zip',N'Size XL - Đen',1,499000),
(3,2,N'Áo Thun Oversize Local Brand',N'Size L - Đen',1,259000),
(4,10,N'Giày Sneaker Trắng',N'Size 42 - Trắng',1,799000),
(5,27,N'Đồng Hồ Nam Dây Da',N'42mm - Đen',1,1299000);
GO

-- =====================================================
-- THANH TOAN
-- =====================================================
INSERT INTO ThanhToan
(DonHangID,NhanVienID,PhuongThuc,SoTien,TrangThai,NgayXuLy,GhiChu)
VALUES
(1,1,'ChuyenKhoan',592200,'DaDuyet',GETDATE(),N'Đã xác nhận'),
(2,2,'TienMat',469000,'ChoDuyet',NULL,N'Chờ thanh toán'),
(3,3,'ChuyenKhoan',259000,'DaDuyet',GETDATE(),N'Thành công'),
(4,4,'TienMat',749000,'ChoDuyet',NULL,N''),
(5,5,'ChuyenKhoan',1000000,'DaDuyet',GETDATE(),N'VIP customer');
GO


UPDATE TaiKhoan 
SET MatKhauHash = '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92'
WHERE MatKhauHash = '123456';
GO

-- Kiểm tra kết quả
SELECT TaiKhoanId, Email, MatKhauHash, VaiTro FROM TaiKhoan;
GO

IF OBJECT_ID('trg_UpdateTongTienDonHang', 'TR') IS NOT NULL
    ENABLE TRIGGER trg_UpdateTongTienDonHang ON ChiTietDonHang;
GO