CREATE DATABASE QLCC
GO
USE QLCC
GO

-- Bảng Chung Cư
CREATE TABLE ChungCu (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Ten NVARCHAR(255) NOT NULL,
    DiaChi NVARCHAR(255) NOT NULL,
    ChuDauTu NVARCHAR(255) NULL,
    NamXayDung INT NULL,
    SoTang INT NULL,
    MoTa NVARCHAR(MAX) NULL
);

-- Bảng Căn Hộ
CREATE TABLE CanHo (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    MaCan NVARCHAR(50) UNIQUE NOT NULL,
    ID_ChungCu INT NOT NULL,
    DienTich FLOAT NOT NULL,
    SoPhong INT NOT NULL,
    Gia DECIMAL(18,2) NOT NULL,
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'Đang bán', N'Đã bán', N'Cho thuê', N'Đã thuê')) NOT NULL,
    MoTa NVARCHAR(MAX) NULL,
    URLs NVARCHAR(MAX) NULL,
    FOREIGN KEY (ID_ChungCu) REFERENCES ChungCu(ID) ON DELETE CASCADE
);

-- Bảng Người Dùng
CREATE TABLE NguoiDung (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    MatKhau NVARCHAR(255) NOT NULL,
    SoDienThoai NVARCHAR(15) NOT NULL UNIQUE,
    LoaiNguoiDung NVARCHAR(50) CHECK (LoaiNguoiDung IN (N'Cư dân', N'Ban quản lý', N'Khách')) NULL
);

-- Bảng Cư Dân
CREATE TABLE CuDan (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ID_NguoiDung INT NOT NULL,
    ID_CanHo INT NOT NULL,
    ID_ChungCu INT NULL,
    FOREIGN KEY (ID_NguoiDung) REFERENCES NguoiDung(ID) ON DELETE CASCADE,
    FOREIGN KEY (ID_CanHo) REFERENCES CanHo(ID) ON DELETE CASCADE,
    FOREIGN KEY (ID_ChungCu) REFERENCES ChungCu(ID) ON DELETE SET NULL
);

-- Bảng Chủ Hộ
CREATE TABLE ChuHo (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ID_CuDan INT NOT NULL,
    ID_CanHo INT NOT NULL,
    ID_ChungCu INT NOT NULL,
    NgayBatDau DATETIME NOT NULL DEFAULT GETDATE(),
    GhiChu NVARCHAR(MAX) NULL,
    FOREIGN KEY (ID_CuDan) REFERENCES CuDan(ID) ON DELETE CASCADE,
    FOREIGN KEY (ID_CanHo) REFERENCES CanHo(ID) ON DELETE CASCADE,
    FOREIGN KEY (ID_ChungCu) REFERENCES ChungCu(ID) ON DELETE CASCADE
);

-- Bảng Dịch Vụ
CREATE TABLE DichVu (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    TenDichVu NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(MAX) NULL,
    Gia DECIMAL(18,2) NOT NULL
);

-- Bảng Hóa Đơn Dịch Vụ
CREATE TABLE HoaDonDichVu (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ID_CanHo INT NOT NULL,
    ID_ChungCu INT NOT NULL,
    SoTien DECIMAL(18,2) NOT NULL,
    NgayLap DATETIME NOT NULL DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) NOT NULL CHECK (TrangThai IN (N'Chưa thanh toán', N'Đã thanh toán')),
    FOREIGN KEY (ID_CanHo) REFERENCES CanHo(ID) ON DELETE NO ACTION,
    FOREIGN KEY (ID_ChungCu) REFERENCES ChungCu(ID) ON DELETE CASCADE
);

-- Bảng Liên Kết Hóa Đơn Dịch Vụ và Dịch Vụ
CREATE TABLE HoaDonDichVu_DichVu (
    ID_HoaDon INT NOT NULL,
    ID_DichVu INT NOT NULL,
    PRIMARY KEY (ID_HoaDon, ID_DichVu),
    FOREIGN KEY (ID_HoaDon) REFERENCES HoaDonDichVu(ID) ON DELETE CASCADE,
    FOREIGN KEY (ID_DichVu) REFERENCES DichVu(ID) ON DELETE CASCADE
);

-- Bảng Đăng Ký Dịch Vụ
CREATE TABLE DangKyDichVu (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ID_CuDan INT NOT NULL,
    ID_DichVu INT NOT NULL,
    NgayDangKy DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (ID_CuDan) REFERENCES CuDan(ID) ON DELETE CASCADE,
    FOREIGN KEY (ID_DichVu) REFERENCES DichVu(ID) ON DELETE CASCADE
);

-- Bảng Phản Ánh & Khiếu Nại
CREATE TABLE PhanAnh (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ID_NguoiDung INT NOT NULL,
    NoiDung NVARCHAR(MAX) NOT NULL,
    TrangThai NVARCHAR(50) NOT NULL CHECK (TrangThai IN (N'Chưa xử lý', N'Chờ xử lý', N'Hoàn thành')) DEFAULT N'Chưa xử lý',
    NgayGui DATETIME NOT NULL DEFAULT GETDATE(),
    PhanHoi NVARCHAR(MAX) NULL,
    HinhAnh NVARCHAR(MAX) NULL,
    FOREIGN KEY (ID_NguoiDung) REFERENCES NguoiDung(ID) ON DELETE CASCADE
);

-- Bảng Hình Ảnh Chung Cư
CREATE TABLE HinhAnhChungCu (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ID_ChungCu INT NOT NULL,
    DuongDan NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (ID_ChungCu) REFERENCES ChungCu(ID) ON DELETE CASCADE Meu
);

CREATE TABLE HinhAnhCanHo (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    DuongDan NVARCHAR(255) NOT NULL,
    ID_CanHo INT NOT NULL,
    FOREIGN KEY (ID_CanHo) REFERENCES CanHo(ID) ON DELETE CASCADE
);

CREATE TABLE HinhAnhDichVu (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    DuongDan NVARCHAR(255) NOT NULL,
    ID_DichVu INT NOT NULL,
    FOREIGN KEY (ID_DichVu) REFERENCES DichVu(ID) ON DELETE CASCADE
);


CREATE TABLE ChuHo (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ID_CuDan INT NOT NULL,
    ID_CanHo INT NOT NULL,
    ID_ChungCu INT NOT NULL,
    NgayBatDau DATETIME NOT NULL DEFAULT GETDATE(),
    GhiChu NVARCHAR(MAX) NULL
);

------------------------------------------------------------------------

INSERT INTO ChungCus (Ten, DiaChi, ChuDauTu, NamXayDung, SoTang, MoTa)
VALUES 
('Chung cư Hoàng Anh Gold House', '456 Đường Lê Văn Sỹ, Quận 3, TP. Hồ Chí Minh', 'Công ty TNHH Đầu Tư Bất Động Sản Hoàng Anh', 2021, 20, 'Chung cư Hoàng Anh Gold House tọa lạc tại vị trí đắc địa, gần trung tâm thành phố, thuận tiện cho việc di chuyển đến các khu vực lân cận. Với thiết kế hiện đại và sang trọng, đây là một trong những dự án bất động sản được yêu thích nhất.'),
('Chung cư Sunrise City', '58 Đường Nguyễn Hữu Thọ, Quận 7, TP. Hồ Chí Minh', 'Tập đoàn Novaland', 2018, 25, 'Sunrise City là một trong những dự án nổi bật tại Quận 7, với phong cách thiết kế hiện đại và tiện nghi cao cấp. Nằm gần khu vực mua sắm và giải trí, dự án hứa hẹn mang lại cuộc sống thuận tiện cho cư dân.'),
('Chung cư Vinhome Central Park', '208 Nguyễn Hữu Cảnh, Quận Bình Thạnh, TP. Hồ Chí Minh', 'Vingroup', 2017, 45, 'Vinhome Central Park là một trong những dự án lớn nhất tại TP. Hồ Chí Minh, nổi bật với công viên cây xanh rộng lớn và các tiện ích cao cấp. Đây là nơi lý tưởng cho cuộc sống hiện đại.'),
('Chung cư Eco Green Saigon', '5 Đường Tôn Thất Thuyết, Quận 7, TP. Hồ Chí Minh', 'Công ty TNHH Sài Gòn Nam Long', 2020, 30, 'Eco Green Saigon được thiết kế với tiêu chí sống xanh, mang lại không gian sống trong lành và hiện đại. Dự án nằm gần trung tâm thương mại và các tiện ích xã hội.'),
('Chung cư The Vista An Phú', '628 Đường Nguyễn Hữu Cảnh, Quận 2, TP. Hồ Chí Minh', 'Tập đoàn CapitaLand', 2014, 30, 'The Vista An Phú là một trong những dự án cao cấp tại Quận 2, với thiết kế hiện đại và đầy đủ tiện ích cao cấp. Nằm gần khu vực trung tâm tài chính, nơi đây hứa hẹn mang đến cuộc sống thuận tiện cho cư dân.'),
('Chung cư City Garden', '59 Ngô Tất Tố, Bình Thạnh, TP. Hồ Chí Minh', 'Công ty TNHH Đầu tư Địa ốc Phú Nhuận', 2014, 17, 'Chung cư City Garden nằm trong khu vực yên tĩnh nhưng vẫn gần các tiện ích chính của thành phố. Với thiết kế hiện đại và không gian sống xanh, đây là lựa chọn lý tưởng cho những ai tìm kiếm sự bình yên giữa lòng đô thị.'),
('Chung cư The Estella', '88 Đường Song Hành, Quận 2, TP. Hồ Chí Minh', 'Tập đoàn Kiến Á', 2015, 27, 'The Estella là một trong những dự án cao cấp tại Quận 2, với thiết kế đẳng cấp và nhiều tiện ích hiện đại. Nằm gần khu vực trung tâm kinh tế mới, đây là lựa chọn hoàn hảo cho những ai làm việc tại khu vực này.'),
('Chung cư Richstar', '7 Đường Hòa Bình, Quận Tân Phú, TP. Hồ Chí Minh', 'Công ty TNHH Đầu tư Xây dựng Hưng Thịnh', 2018, 20, 'Richstar là một trong những dự án mới và hiện đại tại Quận Tân Phú, với thiết kế đẹp mắt và đầy đủ tiện ích. Nằm gần các trung tâm thương mại lớn, đây là nơi lý tưởng cho cuộc sống năng động.'),
('Chung cư Saigon Pearl', '92 Nguyễn Hữu Cảnh, Bình Thạnh, TP. Hồ Chí Minh', 'Công ty Cổ phần Đầu tư Saigon Pearl', 2011, 35, 'Saigon Pearl là một trong những dự án cao cấp nổi bật tại khu vực Bình Thạnh, với tầm nhìn ra sông Sài Gòn. Dự án mang đến không gian sống hiện đại và tiện nghi cho cư dân.'),
('Chung cư The Manor', '91 Nguyễn Hữu Cảnh, Bình Thạnh, TP. Hồ Chí Minh', 'Tập đoàn Bitexco', 2006, 22, 'The Manor là một trong những dự án bất động sản cao cấp đầu tiên tại TP. Hồ Chí Minh, nổi bật với kiến trúc độc đáo và không gian sống sang trọng. Đây là nơi lý tưởng cho những ai tìm kiếm một cuộc sống đẳng cấp.');

INSERT INTO HinhAnhChungCus (ID_ChungCu, DuongDan)
VALUES 
(1, 'images/ChungCu/Hoang Anh Gold House.jpg'),  -- Hình ảnh cho Chung cư Hoàng Anh Gold House
(2, 'images/ChungCu/Sunrise City.jpg'),        -- Hình ảnh cho Chung cư Sunrise City
(3, 'images/ChungCu/Vinhome Central Park.jpg'), -- Hình ảnh cho Chung cư Vinhome Central Park
(4, 'images/ChungCu/Eco Green Saigon.jpg'),     -- Hình ảnh cho Chung cư Eco Green Saigon
(5, 'images/ChungCu/The Vista AnPhu.jpg'),     -- Hình ảnh cho Chung cư The Vista An Phú
(6, 'images/ChungCu/City Garden.jpg'),          -- Hình ảnh cho Chung cư City Garden
(7, 'images/ChungCu/The Estella.jpg'),         -- Hình ảnh cho Chung cư The Estella
(8, 'images/ChungCu/Richstar.jpg'),            -- Hình ảnh cho Chung cư Richstar
(9, 'images/ChungCu/Saigon Pearl.jpg'),
(10, 'images/ChungCu/The Manor.jpg');



SELECT * 
FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS 
WHERE CONSTRAINT_NAME = 'CK__CanHo__TrangThai__58C70E7C';


INSERT INTO CanHos (MaCan, ID_ChungCu, DienTich, SoPhong, Gia, TrangThai, MoTa, URLs)
VALUES 
(N'010', 1, 140, 3, 4500000000, N'Đang bán', N'Thông tin phòng khách và bếp\nThông tin phòng 1\nThông tin phòng 2\nCăn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true"]'),
(N'010', 2, 130, 2, 4200000000, N'Đã bán', N'Thông tin phòng khách và bếp\nThông tin phòng 1\nThông tin phòng 2\nCăn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true"]'),
(N'010', 3, 120, 3, 3900000000, N'Cho thuê', N'Thông tin phòng khách và bếp\nThông tin phòng 1\nThông tin phòng 2\nCăn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true"]'),
(N'010', 4, 110, 1, 3600000000, N'Đã thuê', N'Thông tin phòng khách và bếp\nThông tin phòng 1\nThông tin phòng 2\nCăn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true"]'),
(N'010', 5, 100, 2, 3300000000, N'Đang bán', N'Thông tin phòng khách và bếp\nThông tin phòng 1\nThông tin phòng 2\nCăn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true"]'),
(N'010', 6, 90, 3, 3000000000, N'Đã bán', N'Thông tin phòng khách và bếp\nThông tin phòng 1\nThông tin phòng 2\nCăn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true"]'),
(N'010', 7, 80, 2, 2700000000, N'Cho thuê', N'Thông tin phòng khách và bếp\nThông tin phòng 1\nThông tin phòng 2\nCăn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true"]'),
(N'010', 8, 70, 1, 2400000000, N'Đã thuê', N'Thông tin phòng khách và bếp\nThông tin phòng 1\nThông tin phòng 2\nCăn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true"]'),
(N'010', 9, 60, 2, 2100000000, N'Đang bán', N'Thông tin phòng khách và bếp\nThông tin phòng 1\nThông tin phòng 2\nCăn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true"]'),
(N'010', 10, 50, 1, 1500000000, N'Đã bán', N'Thông tin phòng khách và bếp\nThông tin phòng 1\nThông tin phòng 2\nCăn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true", "https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true"]');

INSERT INTO HinhAnhCanHos (DuongDan, ID_CanHo)
VALUES 
(N'images/CanHo/CanHo10.jpg', 101),
(N'images/CanHo/CanHo10.jpg', 102),
(N'images/CanHo/CanHo10.jpg', 103),
(N'images/CanHo/CanHo10.jpg', 104),
(N'images/CanHo/CanHo10.jpg', 105),
(N'images/CanHo/CanHo10.jpg', 106),
(N'images/CanHo/CanHo10.jpg', 107),
(N'images/CanHo/CanHo10.jpg', 108),
(N'images/CanHo/CanHo10.jpg', 109),
(N'images/CanHo/CanHo10.jpg', 110);


Alter table CuDans Add Ngaycapquyen Date null
---------------------------------------------------------
INSERT INTO ChungCu (Ten, DiaChi, ChuDauTu, NamXayDung, SoTang, MoTa) VALUES
(N'Hoàng Anh Gold House', N'456 Đường Lê Văn Sỹ, Quận 3, TP. Hồ Chí Minh', N'Công ty TNHH Đầu Tư Bất Động Sản Hoàng Anh', 2021, 20, N'Chung cư Hoàng Anh Gold House tọa lạc tại 456 Đường Lê Văn Sỹ, Quận 3, TP. Hồ Chí Minh, do Công ty TNHH Đầu tư Bất Động Sản Hoàng Anh làm chủ đầu tư và được xây dựng vào năm 2021 với 20 tầng. Dự án nổi bật với vị trí đắc địa, gần các tiện ích chính của thành phố. Hoàng Anh Gold House mang đến cho cư dân không gian sống hiện đại với hồ bơi vô cực, phòng gym đầy đủ trang thiết bị, khu vui chơi trẻ em an toàn và nhà hàng phục vụ đa dạng món ăn. Các căn hộ được thiết kế thông minh, tối ưu hóa không gian sống với nhiều loại hình từ 1 đến 3 phòng ngủ. Cộng đồng cư dân thân thiện, chủ yếu là các gia đình trẻ và người đi làm, thường xuyên tổ chức các hoạt động giao lưu, tạo dựng mối quan hệ thân thiết.'),
(N'Sunrise City', N'58 Đường Nguyễn Hữu Thọ, Quận 7, TP. Hồ Chí Minh', N'Tập đoàn Novaland', 2018, 25, N'Chung cư Sunrise City, tọa lạc tại 58 Đường Nguyễn Hữu Thọ, Quận 7, TP. Hồ Chí Minh, là một trong những dự án nổi bật của Tập đoàn Novaland được xây dựng vào năm 2018 với 25 tầng. Dự án này không chỉ có thiết kế hiện đại mà còn nằm gần khu vực mua sắm và giải trí sôi động. Với hồ bơi lớn, phòng gym, và khu vực giải trí đa dạng, Sunrise City hứa hẹn mang đến cuộc sống tiện nghi cho cư dân. Các căn hộ tại đây có diện tích linh hoạt từ 50m² đến 150m², thiết kế mở giúp tận dụng ánh sáng tự nhiên. Cư dân đến từ nhiều nền văn hóa khác nhau, tạo nên một cộng đồng đa dạng và thân thiện.'),
(N'Vinhome Central Park', N'208 Nguyễn Hữu Cảnh, Quận Bình Thạnh, TP. Hồ Chí Minh', N'Vingroup', 2017, 45, N'Vinhome Central Park, với địa chỉ 208 Nguyễn Hữu Cảnh, Quận Bình Thạnh, TP. Hồ Chí Minh, là một trong những dự án lớn của Vingroup, hoàn thành vào năm 2017 với 45 tầng. Dự án nổi bật với công viên cây xanh rộng lớn bên sông Sài Gòn, mang lại không gian sống trong lành và gần gũi với thiên nhiên. Các tiện ích cao cấp như hồ bơi, trung tâm thương mại, và nhà hàng sang trọng đều hiện hữu tại đây. Các căn hộ được thiết kế hiện đại, đa dạng từ 1 đến 4 phòng ngủ, phục vụ nhu cầu của nhiều đối tượng cư dân. Cộng đồng cư dân tại Vinhome Central Park chủ yếu là các gia đình và chuyên gia, thường xuyên tổ chức các hoạt động giao lưu văn hóa.'),
(N'Eco Green Saigon', N'5 Đường Tôn Thất Thuyết, Quận 7, TP. Hồ Chí Minh', N'Công ty TNHH Sài Gòn Nam Long', 2020, 30, N'Chung cư Eco Green Saigon, tọa lạc tại 5 Đường Tôn Thất Thuyết, Quận 7, TP. Hồ Chí Minh, được xây dựng bởi Công ty TNHH Sài Gòn Nam Long vào năm 2020 với 30 tầng. Dự án được thiết kế với tiêu chí sống xanh, mang lại không gian sống trong lành và hiện đại. Các tiện ích như hồ bơi tràn bờ, khu vườn nhiệt đới, và phòng gym đều được trang bị đầy đủ. Các căn hộ tại Eco Green có diện tích linh hoạt, tối ưu hóa không gian sống và ánh sáng tự nhiên. Cư dân chủ yếu là các gia đình trẻ và người yêu thích lối sống xanh, thường xuyên tham gia các hoạt động bảo vệ môi trường.'),
(N'The Vista An Phú', N'628 Đường Nguyễn Hữu Cảnh, Quận 2, TP. Hồ Chí Minh', N'Tập đoàn CapitaLand', 2014, 30, N'Chung cư The Vista An Phú, tọa lạc tại 628 Đường Nguyễn Hữu Cảnh, Quận 2, TP. Hồ Chí Minh, do Tập đoàn CapitaLand phát triển và hoàn thành vào năm 2014 với 30 tầng. Dự án nổi bật với thiết kế hiện đại và đầy đủ tiện ích cao cấp, nằm gần khu vực trung tâm tài chính, thuận tiện cho việc di chuyển. The Vista An Phú mang đến cho cư dân không gian sống sang trọng với hồ bơi lớn, phòng gym hiện đại, và khu vực vui chơi cho trẻ em. Các căn hộ tại đây có nhiều loại diện tích từ 2 đến 4 phòng ngủ, được thiết kế tối ưu hóa không gian sống và ánh sáng tự nhiên. Cộng đồng cư dân tại The Vista An Phú chủ yếu là các chuyên gia và gia đình trẻ, thường xuyên tổ chức các hoạt động giao lưu văn hóa, tạo nên một không gian sống thân thiện và văn minh.'),
(N'City Garden', N'59 Ngô Tất Tố, Bình Thạnh, TP. Hồ Chí Minh', N'Công ty TNHH Đầu tư Địa ốc Phú Nhuận', 2014, 17, N'City Garden, nằm tại 59 Ngô Tất Tố, Bình Thạnh, TP. Hồ Chí Minh, là một dự án đáng chú ý do Công ty TNHH Đầu tư Địa ốc Phú Nhuận phát triển, hoàn thành vào năm 2014 với 17 tầng. Dự án nằm trong khu vực yên tĩnh nhưng vẫn gần các tiện ích chính của thành phố. City Garden mang đến không gian sống xanh với hồ bơi, phòng gym và khu vui chơi trẻ em an toàn. Các căn hộ được thiết kế với diện tích từ 1 đến 3 phòng ngủ, có cửa sổ lớn cho ánh sáng tự nhiên và tầm nhìn đẹp ra thành phố. Cộng đồng cư dân tại đây chủ yếu là các gia đình trẻ và người đi làm, tạo nên một không gian sống thân thiện và hòa đồng.'),
(N'The Estella', N'88 Đường Song Hành, Quận 2, TP. Hồ Chí Minh', N'Tập đoàn Kiến Á', 2015, 27, N'The Estella, tọa lạc tại 88 Đường Song Hành, Quận 2, TP. Hồ Chí Minh, do Tập đoàn Kiến Á phát triển và được xây dựng vào năm 2015 với 27 tầng. Dự án nổi bật với thiết kế đẳng cấp và nhiều tiện ích hiện đại. Nằm gần khu vực trung tâm kinh tế mới, The Estella là lựa chọn hoàn hảo cho những ai làm việc tại khu vực này. Các tiện ích như hồ bơi tràn bờ, khu vui chơi cho trẻ em, và trung tâm mua sắm đều được trang bị đầy đủ. Căn hộ tại đây có nhiều loại diện tích từ 2 đến 4 phòng ngủ, được thiết kế tối ưu hóa không gian sống với nội thất sang trọng. Cộng đồng cư dân đa dạng, thường xuyên tổ chức các hoạt động giao lưu văn hóa.'),
(N'Richstar', N'7 Đường Hòa Bình, Quận Tân Phú, TP. Hồ Chí Minh', N'Công ty TNHH Đầu tư Xây dựng Hưng Thịnh', 2018, 20, N'Richstar, tọa lạc tại 7 Đường Hòa Bình, Quận Tân Phú, TP. Hồ Chí Minh, là một dự án mới và hiện đại do Công ty TNHH Đầu tư Xây dựng Hưng Thịnh phát triển, hoàn thành vào năm 2018 với 20 tầng. Dự án nằm gần các trung tâm thương mại lớn, mang đến sự thuận tiện cho cuộc sống năng động. Richstar cung cấp các tiện ích như hồ bơi, phòng gym, khu vui chơi trẻ em và nhà hàng phục vụ đa dạng món ăn. Các căn hộ được thiết kế từ 1 đến 3 phòng ngủ, với không gian sống thoải mái và hiện đại. Cư dân chủ yếu là những người trẻ tuổi, tạo nên một không gian sống sôi động và thân thiện.'),
(N'Saigon Pearl', N'92 Nguyễn Hữu Cảnh, Bình Thạnh, TP. Hồ Chí Minh', N'Công ty Cổ phần Đầu tư Saigon Pearl', 2011, 35, N'Saigon Pearl, nằm tại 92 Nguyễn Hữu Cảnh, Bình Thạnh, TP. Hồ Chí Minh, là một dự án cao cấp nổi bật do Công ty Cổ phần Đầu tư Saigon Pearl phát triển, hoàn thành vào năm 2011 với 35 tầng. Dự án mang đến tầm nhìn đẹp ra sông Sài Gòn, không gian sống hiện đại và tiện nghi. Saigon Pearl cung cấp các tiện ích như hồ bơi lớn, phòng gym, spa và khu vui chơi trẻ em. Các căn hộ được thiết kế với phong cách hiện đại, từ 1 đến 4 phòng ngủ, phục vụ nhu cầu đa dạng của cư dân. Cộng đồng cư dân tại đây chủ yếu là người nước ngoài và chuyên gia, tạo nên một môi trường đa văn hóa và thân thiện.'),
(N'The Manor', N'91 Nguyễn Hữu Cảnh, Bình Thạnh, TP. Hồ Chí Minh', N'Tập đoàn Bitexco', 2006, 22, N'The Manor, tọa lạc tại 91 Nguyễn Hữu Cảnh, Bình Thạnh, TP. Hồ Chí Minh, được xây dựng bởi Tập đoàn Bitexco vào năm 2006 với 22 tầng. Dự án nổi bật với kiến trúc độc đáo và không gian sống sang trọng, lý tưởng cho những ai tìm kiếm một cuộc sống đẳng cấp. The Manor cung cấp các tiện ích như hồ bơi, phòng gym và khu vui chơi trẻ em. Các căn hộ được thiết kế hiện đại, từ 1 đến 3 phòng ngủ, với nội thất sang trọng mang lại sự thoải mái cho cư dân. Cộng đồng cư dân tại The Manor chủ yếu là giới thượng lưu và chuyên gia, tạo nên một không gian sống văn minh và đẳng cấp.');


INSERT INTO CanHo (MaCan, ID_ChungCu, DienTich, SoPhong, Gia, TrangThai, MoTa, URLs) VALUES
(N'001', 1, 70, 2, 2000000000.00, N'Đang bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75.95&size=medium&display-plan=true","https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=-46.27&pitch=6.6&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/350328cef4e04822a9728d8938125729/embed",null]'),
(N'002', 1, 70, 2, 2200000000.00, N'Đang bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/e04a80b91a4a4e9699c0797136a1f5cb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/243bc70e84a94cd4abc77b9667d2da7d?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/3934e10e48a34d10ac3fe68c3a243fba?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'003', 1, 80, 2, 2400000000.00, N'Đang bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/a123e303bcdf46b9b0d459c33e30d5a6?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/228c36f7e1464c61b9aa16c98de8b1b2?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'001', 2, 140, 3, 4500000000.00, N'Đang bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75.95&size=medium&display-plan=true","https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=-46.27&pitch=6.6&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/350328cef4e04822a9728d8938125729/embed",null]'),
(N'002', 2, 130, 2, 4200000000.00, N'Đã bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/e04a80b91a4a4e9699c0797136a1f5cb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/243bc70e84a94cd4abc77b9667d2da7d?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/3934e10e48a34d10ac3fe68c3a243fba?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'003', 2, 120, 3, 7500000.00, N'Cho thuê', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/a123e303bcdf46b9b0d459c33e30d5a6?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/228c36f7e1464c61b9aa16c98de8b1b2?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'001', 3, 140, 3, 4500000000.00, N'Đang bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75.95&size=medium&display-plan=true","https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=-46.27&pitch=6.6&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/350328cef4e04822a9728d8938125729/embed",null]'),
(N'002', 3, 130, 2, 4200000000.00, N'Đã bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/e04a80b91a4a4e9699c0797136a1f5cb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/243bc70e84a94cd4abc77b9667d2da7d?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/3934e10e48a34d10ac3fe68c3a243fba?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'003', 3, 120, 3, 8500000.00, N'Cho thuê', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/a123e303bcdf46b9b0d459c33e30d5a6?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/228c36f7e1464c61b9aa16c98de8b1b2?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'001', 4, 140, 3, 4500000000.00, N'Đang bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75.95&size=medium&display-plan=true","https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=-46.27&pitch=6.6&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/350328cef4e04822a9728d8938125729/embed",null]'),
(N'002', 4, 130, 2, 4200000000.00, N'Đã bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/e04a80b91a4a4e9699c0797136a1f5cb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/243bc70e84a94cd4abc77b9667d2da7d?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/3934e10e48a34d10ac3fe68c3a243fba?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'003', 4, 120, 3, 8500000.00, N'Cho thuê', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/a123e303bcdf46b9b0d459c33e30d5a6?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/228c36f7e1464c61b9aa16c98de8b1b2?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'001', 5, 120, 3, 8500000.00, N'Cho thuê', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75.95&size=medium&display-plan=true","https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=-46.27&pitch=6.6&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/350328cef4e04822a9728d8938125729/embed",null]'),
(N'002', 5, 110, 1, 8500000.00, N'Đã thuê', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/e04a80b91a4a4e9699c0797136a1f5cb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/243bc70e84a94cd4abc77b9667d2da7d?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/3934e10e48a34d10ac3fe68c3a243fba?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'003', 5, 100, 2, 3300000000.00, N'Đang bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/a123e303bcdf46b9b0d459c33e30d5a6?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/228c36f7e1464c61b9aa16c98de8b1b2?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'001', 7, 120, 3, 8500000.00, N'Cho thuê', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75.95&size=medium&display-plan=true","https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=-46.27&pitch=6.6&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/350328cef4e04822a9728d8938125729/embed",null]'),
(N'002', 7, 110, 1, 8500000.00, N'Đã thuê', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/e04a80b91a4a4e9699c0797136a1f5cb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/243bc70e84a94cd4abc77b9667d2da7d?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/3934e10e48a34d10ac3fe68c3a243fba?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'003', 7, 100, 2, 3300000000.00, N'Đang bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/a123e303bcdf46b9b0d459c33e30d5a6?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/228c36f7e1464c61b9aa16c98de8b1b2?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'001', 8, 120, 3, 8500000.00, N'Cho thuê', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như lake bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75.95&size=medium&display-plan=true","https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=-46.27&pitch=6.6&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/350328cef4e04822a9728d8938125729/embed",null]'),
(N'002', 8, 110, 1, 8500000.00, N'Đã thuê', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/e04a80b91a4a4e9699c0797136a1f5cb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/243bc70e84a94cd4abc77b9667d2da7d?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/3934e10e48a34d10ac3fe68c3a243fba?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'003', 8, 100, 2, 3300000000.00, N'Đang bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/a123e303bcdf46b9b0d459c33e30d5a6?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/228c36f7e1464c61b9aa16c98de8b1b2?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'001', 9, 120, 3, 8500000.00, N'Cho thuê', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75.95&size=medium&display-plan=true","https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=-46.27&pitch=6.6&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/350328cef4e04822a9728d8938125729/embed",null]'),
(N'002', 9, 110, 1, 8500000.00, N'Đã thuê', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/e04a80b91a4a4e9699c0797136a1f5cb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/243bc70e84a94cd4abc77b9667d2da7d?utm_campaign=embed&utm_source=other&sheading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/3934e10e48a34d10ac3fe68c3a243fba?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'003', 9, 100, 2, 3300000000.00, N'Đang bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/a123e303bcdf46b9b0d459c33e30d5a6?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/228c36f7e1464c61b9aa16c98de8b1b2?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'001', 10, 120, 3, 8500000.00, N'Cho thuê', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/bfd74e5df79a48a79b932c15373dc818?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75.95&size=medium&display-plan=true","https://momento360.com/e/u/675ddbf033f34127a9df86543d290a24?utm_campaign=embed&utm_source=other&heading=-46.27&pitch=6.6&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/5aef599958674690bb3ebd09f87de7fb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/350328cef4e04822a9728d8938125729/embed",null]'),
(N'002', 10, 110, 1, 8500000.00, N'Đã thuê', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/e04a80b91a4a4e9699c0797136a1f5cb?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/243bc70e84a94cd4abc77b9667d2da7d?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/3934e10e48a34d10ac3fe68c3a243fba?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]'),
(N'003', 10, 100, 2, 3300000000.00, N'Đang bán', N'Căn hộ này được thiết kế theo phong cách hiện đại với không gian mở, tận dụng ánh sáng tự nhiên. Nội thất được trang bị đầy đủ, bao gồm bếp hiện đại, phòng khách rộng rãi và phòng ngủ ấm cúng. Căn hộ nằm ở tầng trung, mang đến tầm nhìn đẹp ra khu vực xung quanh. Chung cư có nhiều tiện ích như hồ bơi, phòng gym, và khu vui chơi cho trẻ em, tạo điều kiện sống thoải mái cho cư dân. Vị trí thuận lợi, gần các trung tâm mua sắm và trường học.', N'["https://momento360.com/e/u/a123e303bcdf46b9b0d459c33e30d5a6?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://momento360.com/e/u/228c36f7e1464c61b9aa16c98de8b1b2?utm_campaign=embed&utm_source=other&heading=0&pitch=0&field-of-view=75&size=medium&display-plan=true","https://sketchfab.com/models/80425c18729e44a8a614c5678fbc5589/embed"]');


INSERT INTO DichVu (TenDichVu, MoTa, Gia) VALUES
(N'Dịch vụ sửa chữa điện nước trong căn hộ', N'Hỗ trợ sửa chữa các vấn đề về điện và nước trong căn hộ như rò rỉ đường ống, hỏng vòi nước, chập điện, hoặc mất điện cục bộ. Đội ngũ kỹ thuật viên sẽ đến tận nơi để kiểm tra và khắc phục nhanh chóng, đảm bảo sinh hoạt của cư dân không bị gián đoạn.', 200000.00),
(N'Dịch vụ vệ sinh căn hộ', N'Cung cấp dịch vụ dọn dẹp toàn diện cho căn hộ, bao gồm lau chùi sàn nhà, cửa kính, vệ sinh bếp, phòng tắm, và hút bụi thảm. Đội ngũ nhân viên sử dụng các dụng cụ và hóa chất an toàn, giúp căn hộ luôn sạch sẽ và thoải mái.', 500000.00),
(N'Dịch vụ lắp đặt và bảo trì điều hòa', N'Hỗ trợ lắp đặt điều hòa mới hoặc bảo trì điều hòa hiện có trong căn hộ. Dịch vụ bao gồm vệ sinh máy lạnh, kiểm tra gas, và sửa chữa các lỗi nhỏ để đảm bảo điều hòa hoạt động hiệu quả, đặc biệt trong mùa nóng.', 300000.00),
(N'Dịch vụ giặt sấy', N'Cung cấp dịch vụ giặt, sấy và ủi quần áo cho cư dân ngay tại chung cư. Quần áo được xử lý bằng máy móc hiện đại, đảm bảo sạch sẽ, thơm tho và được giao trả tận cửa căn hộ trong thời gian nhanh nhất.', 20000.00),
(N'Dịch vụ cung cấp nước đóng chai', N'Giao nước uống đóng bình tinh khiết đến tận cửa căn hộ theo yêu cầu. Dịch vụ đảm bảo nước đạt tiêu chuẩn an toàn vệ sinh, phù hợp cho việc uống trực tiếp hoặc nấu ăn, giúp cư dân tiết kiệm thời gian.', 50000.00),
(N'Dịch vụ sơn và làm tường căn hộ', N'Hỗ trợ sơn lại tường hoặc làm mới không gian sống trong căn hộ. Dịch vụ bao gồm xử lý bề mặt tường, sơn lót, và sơn phủ với màu sắc theo lựa chọn của cư dân, giúp căn hộ trở nên sáng sủa và mới mẻ hơn.', 500000.00),
(N'Dịch vụ kiểm tra và xử lý côn trùng', N'Kiểm tra và xử lý các loại côn trùng như gián, muỗi, kiến trong căn hộ. Dịch vụ sử dụng các biện pháp an toàn, không gây hại cho sức khỏe cư dân, đảm bảo không gian sống sạch sẽ và không bị côn trùng quấy nhiễu.', 400000.00),
(N'Dịch vụ chuyển đồ', N'Hỗ trợ cư dân di chuyển đồ đạc hoặc sắp xếp lại nội thất trong căn hộ, hoặc khi chuyển sang căn hộ khác trong cùng chung cư. Đội ngũ nhân viên sẽ giúp vận chuyển an toàn, tránh làm hỏng đồ đạc.', 300000.00),
(N'Dịch vụ lắp đặt và bảo trì đèn ánh sáng', N'Hỗ trợ lắp đặt đèn chiếu sáng mới hoặc bảo trì hệ thống đèn hiện có trong căn hộ. Dịch vụ bao gồm thay bóng đèn, sửa chữa ổ điện, và kiểm tra an toàn điện để đảm bảo ánh sáng đầy đủ và an toàn cho cư dân.', 150000.00),
(N'Dịch vụ lắp đặt nội thất', N'Hỗ trợ lắp đặt các món đồ nội thất như tủ, kệ, giường, hoặc bàn ghế trong căn hộ. Đội ngũ nhân viên sẽ đảm bảo lắp đặt đúng kỹ thuật, chắc chắn và thẩm mỹ, giúp cư dân tiết kiệm thời gian và công sức.', 400000.00);



INSERT INTO HinhAnhCanHo (DuongDan, ID_CanHo) VALUES
(N'/images/ab36a8c3-39cb-4aef-a46b-6a1b532174fa_Canho6.jpg', 11),
(N'/images/37500fac-6531-4b45-8718-f94658463a71_Canho7.jpg', 12),
(N'/images/ff93088e-2ca5-4e7e-9ff1-e1e56e586568_Canho8.jpg', 13),
(N'/images/30471508-6dcc-43d5-b8cd-70c876e4c796_CanHo1.jpg', 21),
(N'/images/9a1268ea-1250-4820-9adf-8ff7ba0e029d_canho2.jpg', 22),
(N'/images/12bd013c-d876-4285-a5e8-1d8ce3c357f3_canho3.jpg', 23),
(N'/images/37500fac-6531-4b45-8718-f94658463a71_Canho7.jpg', 31),
(N'/images/ff93088e-2ca5-4e7e-9ff1-e1e56e586568_Canho8.jpg', 32),
(N'/images/68ad3b93-eddf-4b5c-848a-7ba8f3f131de_Canho9.jpg', 33),
(N'/images/b608c8ee-0a54-4218-8cac-f821b69eaa2a_canho2.jpg', 36),
(N'/images/ea38c57b-71e7-44ce-b657-aab6ccb8831e_canho3.jpg', 37),
(N'/images/2a772c2e-50e1-4156-9881-e410fe9101b4_canho4.jpg', 38),
(N'/images/ff93088e-2ca5-4e7e-9ff1-e1e56e586568_Canho8.jpg', 43),
(N'/images/37500fac-6531-4b45-8718-f94658463a71_Canho7.jpg', 44),
(N'/images/17b7493f-3e33-4d5d-8d36-a20cc27d0db2_CanHo1.jpg', 45),
(N'/images/834e0d5d-0daa-4b9f-ba99-5cc87165afd7_canho4.jpg', 48),
(N'/images/c8d8fdc8-f0b2-42af-9028-66a2e384790a_Canho5.jpg', 49),
(N'/images/e116c7ea-c090-4543-89f9-76be23ded557_Canho9.jpg', 50),
(N'/images/cd454f45-7cdb-4638-99a5-246ca5174ff2_Canho10.jpg', 53),
(N'/images/ab36a8c3-39cb-4aef-a46b-6a1b532174fa_Canho6.jpg', 54),
(N'/images/37500fac-6531-4b45-8718-f94658463a71_Canho7.jpg', 55),
(N'/images/cd454f45-7cdb-4638-99a5-246ca5174ff2_Canho10.jpg', 58),
(N'/images/30471508-6dcc-43d5-b8cd-70c876e4c796_CanHo1.jpg', 59),
(N'/images/b608c8ee-0a54-4218-8cac-f821b69eaa2a_canho2.jpg', 60),
(N'/images/b0f25ba0-ce95-49bd-b735-6501f438a494_3ba1cfa7-c3eb-4757-a323-33bcafaadf8b_Canho5.jpg', 63),
(N'/images/2a810f66-c5e3-4d56-8929-98a40a504c5e_2a772c2e-50e1-4156-9881-e410fe9101b4_canho4.jpg', 64),
(N'/images/da8b7f8b-e5ac-4caa-a781-02323af43b9d_0adea74e-7fe9-457a-bdb7-ed06ce8c1cd9_Canho10.jpg', 65);

INSERT INTO HinhAnhChungCu (ID_ChungCu, DuongDan) VALUES
(1, N'images/ChungCu/Hoang Anh Gold House.jpg'),
(2, N'images/ChungCu/Sunrise City.jpg'),
(3, N'images/ChungCu/Vinhome Central Park.jpg'),
(4, N'images/ChungCu/Eco Green Saigon.jpg'),
(5, N'images/ChungCu/The Vista AnPhu.jpg'),
(6, N'images/ChungCu/City Garden.jpg'),
(7, N'images/ChungCu/The Estella.jpg'),
(8, N'images/ChungCu/Richstar.jpg'),
(9, N'images/ChungCu/Saigon Pearl.jpg'),
(10, N'images/ChungCu/The Manor.jpg');

INSERT INTO HinhAnhDichVu (DuongDan, ID_DichVu) VALUES
(N'/images/suanuoc.webp', 1),
(N'/images/vesinh.jpg', 2),
(N'/images/maylanh.jpg', 3),
(N'/images/giat.jpg', 4),
(N'/images/nuocdongbinh.jpg', 5),
(N'/images/sontuong.jpg', 6),
(N'/images/contrung.jpg', 7),
(N'/images/chuyendo.jpg', 8),
(N'/images/lapden.jpg', 9),
(N'/images/lapdo.jpg', 10);