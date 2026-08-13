-- ============================================================================
-- SQL SCRIPTS - CÁC TRUY VẤN THƯỜNG DÙNG CHO HỆ THỐNG QUẢN LÝ PHÒNG KHÁM
-- ============================================================================

-- NOTE: Những script này có thể chạy trực tiếp trên SQL Server hoặc qua Entity Framework

-- ============================================================================
-- 1. QUẢN LÝ KHO - INVENTORY MANAGEMENT
-- ============================================================================

-- 1.1 Xem tất cả sản phẩm còn tồn kho
SELECT 
	p.ProductCode,
	p.ProductName,
	p.Unit,
	p.Category,
	i.Quantity,
	i.MinimumQuantity,
	i.MaximumQuantity,
	p.RetailPrice,
	i.WarehouseLocation,
	CASE 
		WHEN i.Quantity < i.MinimumQuantity THEN 'HẾT THIẾU'
		WHEN p.ExpiryDate < GETDATE() THEN 'HẾT HẠN'
		WHEN p.ExpiryDate < DATEADD(DAY, 30, GETDATE()) THEN 'SẮP HẾT HẠN'
		ELSE 'OK'
	END AS Status
FROM Products p
LEFT JOIN Inventories i ON p.Id = i.ProductId
WHERE i.Quantity > 0 OR i.Quantity IS NULL
ORDER BY i.Quantity ASC;

-- 1.2 Cảnh báo hàng hết thiếu (dưới mức tối thiểu)
SELECT 
	p.ProductCode,
	p.ProductName,
	i.Quantity,
	i.MinimumQuantity,
	(i.MinimumQuantity - i.Quantity) AS SoLuongCanNhap
FROM Products p
JOIN Inventories i ON p.Id = i.ProductId
WHERE i.Quantity < i.MinimumQuantity
ORDER BY (i.MinimumQuantity - i.Quantity) DESC;

-- 1.3 Cảnh báo hàng sắp hết hạn (30 ngày)
SELECT 
	p.ProductCode,
	p.ProductName,
	p.ExpiryDate,
	DATEDIFF(DAY, GETDATE(), p.ExpiryDate) AS NgayConLai,
	i.Quantity,
	s.SupplierName
FROM Products p
JOIN Inventories i ON p.Id = i.ProductId
LEFT JOIN Suppliers s ON p.SupplierId = s.Id
WHERE p.ExpiryDate BETWEEN GETDATE() AND DATEADD(DAY, 30, GETDATE())
AND i.Quantity > 0
ORDER BY p.ExpiryDate ASC;

-- 1.4 Giá trị kho (tính theo giá nhập)
SELECT 
	p.ProductCode,
	p.ProductName,
	i.Quantity,
	p.CostPrice,
	(i.Quantity * p.CostPrice) AS GiaTriKho
FROM Products p
JOIN Inventories i ON p.Id = i.ProductId
WHERE i.Quantity > 0
ORDER BY (i.Quantity * p.CostPrice) DESC;

-- 1.5 Lợi suất sản phẩm
SELECT 
	p.ProductCode,
	p.ProductName,
	p.CostPrice,
	p.RetailPrice,
	(p.RetailPrice - p.CostPrice) AS LoiNhuan,
	ROUND(((p.RetailPrice - p.CostPrice) / p.CostPrice * 100), 2) AS LoiNhuanPercent
FROM Products p
ORDER BY LoiNhuanPercent DESC;

-- ============================================================================
-- 2. QUẢN LÝ NHẬP HÀNG - IMPORT MANAGEMENT
-- ============================================================================

-- 2.1 Xem tất cả đơn nhập
SELECT 
	io.ImportCode,
	io.ImportDate,
	s.SupplierName,
	io.InvoiceNumber,
	io.TotalAmount,
	io.PaidAmount,
	(io.TotalAmount - io.PaidAmount) AS DuThanhtoan,
	io.Status,
	io.PaymentStatus,
	u.FullName AS NguoiTao
FROM ImportOrders io
JOIN Suppliers s ON io.SupplierId = s.Id
LEFT JOIN Users u ON io.CreatedByUserId = u.Id
ORDER BY io.ImportDate DESC;

-- 2.2 Đơn nhập chưa thanh toán
SELECT 
	io.ImportCode,
	io.ImportDate,
	s.SupplierName,
	io.TotalAmount,
	io.PaidAmount,
	(io.TotalAmount - io.PaidAmount) AS ChuaThanhtoan
FROM ImportOrders io
JOIN Suppliers s ON io.SupplierId = s.Id
WHERE io.PaymentStatus IN ('Chưa thanh toán', 'Một phần')
ORDER BY io.ImportDate DESC;

-- 2.3 Chi tiết một đơn nhập
DECLARE @ImportOrderId INT = 1; -- Thay ID phù hợp
SELECT 
	iod.Id,
	p.ProductCode,
	p.ProductName,
	iod.Quantity,
	iod.UnitPrice,
	iod.Total,
	iod.ExpiryDate,
	iod.BatchNumber,
	iod.ReceivedQuantity,
	iod.DamagedQuantity
FROM ImportOrderDetails iod
JOIN Products p ON iod.ProductId = p.Id
WHERE iod.ImportOrderId = @ImportOrderId
ORDER BY iod.Id;

-- 2.4 Nhà cung cấp nào cung cấp sản phẩm gì
SELECT 
	s.SupplierCode,
	s.SupplierName,
	s.Phone,
	p.ProductCode,
	p.ProductName,
	p.CostPrice
FROM Suppliers s
LEFT JOIN Products p ON s.Id = p.SupplierId
ORDER BY s.SupplierName, p.ProductName;

-- ============================================================================
-- 3. QUẢN LÝ BÁN HÀNG - SALES MANAGEMENT
-- ============================================================================

-- 3.1 Xem tất cả đơn bán trong tháng này
SELECT 
	s.SaleCode,
	s.SaleDate,
	c.FullName AS KhachHang,
	u.FullName AS NhanVien,
	s.TotalAmount,
	s.PaidAmount,
	s.ChangeAmount,
	s.PaymentMethod,
	s.PaymentStatus,
	s.Status
FROM Sales s
LEFT JOIN Customers c ON s.CustomerId = c.Id
LEFT JOIN Users u ON s.SalesPersonUserId = u.Id
WHERE MONTH(s.SaleDate) = MONTH(GETDATE()) 
  AND YEAR(s.SaleDate) = YEAR(GETDATE())
ORDER BY s.SaleDate DESC;

-- 3.2 Doanh thu từng nhân viên (theo hôm nay/tuần/tháng)
SELECT 
	u.FullName AS NhanVien,
	COUNT(s.Id) AS SoDonBan,
	SUM(s.TotalAmount) AS TongDoanhthu,
	AVG(s.TotalAmount) AS DoanhThuTrunhBinh
FROM Sales s
JOIN Users u ON s.SalesPersonUserId = u.Id
WHERE s.SaleDate >= DATEADD(DAY, -30, GETDATE())
GROUP BY u.FullName
ORDER BY TongDoanhthu DESC;

-- 3.3 Sản phẩm bán chạy nhất
SELECT 
	p.ProductCode,
	p.ProductName,
	SUM(sd.Quantity) AS TongSoLuong,
	COUNT(DISTINCT sd.SaleId) AS SoDonBan,
	SUM(sd.Total) AS TongDoanhthu,
	AVG(sd.UnitPrice) AS GiaTrungBinh
FROM SaleDetails sd
JOIN Products p ON sd.ProductId = p.Id
JOIN Sales s ON sd.SaleId = s.Id
WHERE s.SaleDate >= DATEADD(DAY, -30, GETDATE())
GROUP BY p.ProductCode, p.ProductName
ORDER BY TongSoLuong DESC;

-- 3.4 Chi tiết một đơn bán
DECLARE @SaleId INT = 1; -- Thay ID phù hợp
SELECT 
	p.ProductCode,
	p.ProductName,
	sd.Quantity,
	sd.UnitPrice,
	sd.Total,
	sd.DiscountPercent,
	sd.DiscountAmount,
	(sd.Total - sd.DiscountAmount) AS ThanhTienThucTe
FROM SaleDetails sd
JOIN Products p ON sd.ProductId = p.Id
WHERE sd.SaleId = @SaleId
ORDER BY sd.Id;

-- 3.5 Khách hàng mua nhiều nhất
SELECT 
	c.FullName,
	c.Phone,
	COUNT(s.Id) AS SoDonMua,
	SUM(s.TotalAmount) AS TongTienDaMua,
	MAX(s.SaleDate) AS LanMuaCuoi
FROM Sales s
JOIN Customers c ON s.CustomerId = c.Id
GROUP BY c.FullName, c.Phone
ORDER BY TongTienDaMua DESC;

-- ============================================================================
-- 4. QUẢN LÝ HÓA ĐƠN - INVOICE MANAGEMENT
-- ============================================================================

-- 4.1 Xem tất cả hóa đơn trong tháng
SELECT 
	i.InvoiceNumber,
	i.InvoiceDate,
	i.CustomerName,
	i.InvoiceType,
	i.TotalAmount,
	i.Status,
	i.PrintCount,
	i.LastPrintedDate
FROM Invoices i
WHERE MONTH(i.InvoiceDate) = MONTH(GETDATE())
  AND YEAR(i.InvoiceDate) = YEAR(GETDATE())
ORDER BY i.InvoiceDate DESC;

-- 4.2 Hóa đơn chưa in
SELECT 
	i.InvoiceNumber,
	i.InvoiceDate,
	i.CustomerName,
	i.TotalAmount,
	s.SaleCode,
	u.FullName AS NguoiTao
FROM Invoices i
JOIN Sales s ON i.SaleId = s.Id
LEFT JOIN Users u ON i.CreatedByUserId = u.Id
WHERE i.Status = 'Chưa in'
ORDER BY i.InvoiceDate ASC;

-- 4.3 Thống kê in hóa đơn
SELECT 
	i.InvoiceNumber,
	i.PrintCount,
	i.LastPrintedDate,
	CASE 
		WHEN i.PrintCount = 0 THEN 'Chưa in'
		WHEN i.PrintCount = 1 THEN 'In 1 lần'
		ELSE CONCAT('In ', i.PrintCount, ' lần')
	END AS TrangThai
FROM Invoices i
ORDER BY i.LastPrintedDate DESC;

-- ============================================================================
-- 5. QUẢN LÝ TOA BỆNH - PRESCRIPTION MANAGEMENT
-- ============================================================================

-- 5.1 Xem tất cả toa bệnh của một bệnh nhân
DECLARE @CustomerId INT = 1; -- Thay ID bệnh nhân
SELECT 
	p.PrescriptionCode,
	p.PrescriptionDate,
	p.EffectiveDate,
	p.ExpiryDate,
	p.Diagnosis,
	p.Symptoms,
	u.FullName AS BacSi,
	p.Status,
	p.IsPrinted
FROM Prescriptions p
LEFT JOIN Users u ON p.CreatedByUserId = u.Id
WHERE p.CustomerId = @CustomerId
ORDER BY p.PrescriptionDate DESC;

-- 5.2 Chi tiết toa bệnh
DECLARE @PrescriptionId INT = 1; -- Thay ID toa
SELECT 
	p.ProductCode,
	p.ProductName,
	pd.Quantity,
	pd.Unit,
	pd.Dosage,
	pd.Frequency,
	pd.Route,
	pd.Duration,
	pd.TotalDoses,
	pd.Instructions,
	pd.Contraindication,
	pd.SideEffects,
	pd.Status
FROM PrescriptionDetails pd
JOIN Products p ON pd.ProductId = p.Id
WHERE pd.PrescriptionId = @PrescriptionId
ORDER BY pd.Id;

-- 5.3 Toa bệnh có tương tác thuốc
SELECT 
	p.PrescriptionCode,
	p.PrescriptionDate,
	c.FullName AS BenhNhan,
	u.FullName AS BacSi,
	COUNT(*) AS SoThuocCoTuongTac
FROM Prescriptions p
JOIN PrescriptionDetails pd ON p.Id = pd.PrescriptionId
JOIN Customers c ON p.CustomerId = c.Id
LEFT JOIN Users u ON p.CreatedByUserId = u.Id
WHERE pd.HasDrugInteraction = 1
GROUP BY p.PrescriptionCode, p.PrescriptionDate, c.FullName, u.FullName
ORDER BY p.PrescriptionDate DESC;

-- 5.4 Toa bệnh chưa in
SELECT 
	p.PrescriptionCode,
	p.PrescriptionDate,
	c.FullName AS BenhNhan,
	u.FullName AS BacSi,
	COUNT(pd.Id) AS SoThuoc
FROM Prescriptions p
JOIN Customers c ON p.CustomerId = c.Id
LEFT JOIN Users u ON p.CreatedByUserId = u.Id
LEFT JOIN PrescriptionDetails pd ON p.Id = pd.PrescriptionId
WHERE p.IsPrinted = 0 AND p.Status = 'Hoạt động'
GROUP BY p.PrescriptionCode, p.PrescriptionDate, c.FullName, u.FullName
ORDER BY p.PrescriptionDate DESC;

-- ============================================================================
-- 6. BÁOCÁO TỔNG HỢP - SUMMARY REPORTS
-- ============================================================================

-- 6.1 Tổng hợp doanh thu theo ngày/tháng/quý/năm (ngày)
SELECT 
	CAST(s.SaleDate AS DATE) AS Ngay,
	COUNT(s.Id) AS SoDonBan,
	SUM(s.TotalAmount) AS DoanhhThuBrutto,
	SUM(s.DiscountAmount) AS ChietKhau,
	SUM(s.TotalAmount - s.DiscountAmount) AS DoanhThuThucTe
FROM Sales s
WHERE s.Status = 'Hoàn thành'
GROUP BY CAST(s.SaleDate AS DATE)
ORDER BY Ngay DESC;

-- 6.2 Tổng hợp doanh thu theo nhân viên
SELECT 
	u.FullName AS NhanVien,
	COUNT(s.Id) AS SoDonBan,
	SUM(s.TotalAmount) AS DoanhThu,
	MIN(s.SaleDate) AS NgayMuaDau,
	MAX(s.SaleDate) AS NgayMuaCuoi
FROM Sales s
JOIN Users u ON s.SalesPersonUserId = u.Id
GROUP BY u.FullName
ORDER BY DoanhThu DESC;

-- 6.3 Tổng hợp theo loại sản phẩm (category)
SELECT 
	p.Category,
	COUNT(DISTINCT p.Id) AS SoLuongSanPham,
	SUM(sd.Quantity) AS TongSoLuongBan,
	SUM(sd.Total) AS TongDoanhthu,
	AVG(i.Quantity) AS TonKhoTrungBinh
FROM Products p
LEFT JOIN SaleDetails sd ON p.Id = sd.ProductId
LEFT JOIN Inventories i ON p.Id = i.ProductId
GROUP BY p.Category
ORDER BY TongDoanhthu DESC;

-- 6.4 Khuyến cáo: SẮP HẾT KHO
SELECT 
	'CẢNH BÁO' AS Alert,
	p.ProductCode,
	p.ProductName,
	i.Quantity AS TonHienTai,
	i.MinimumQuantity AS MucToiThieu,
	(i.MinimumQuantity - i.Quantity) AS DuThieuLuong
FROM Products p
JOIN Inventories i ON p.Id = i.ProductId
WHERE i.Quantity < i.MinimumQuantity AND i.Quantity > 0
ORDER BY (i.MinimumQuantity - i.Quantity) DESC;

-- 6.5 Khuyến cáo: HẾT HẠN / SẮP HẾT HẠN
SELECT 
	CASE 
		WHEN p.ExpiryDate < GETDATE() THEN 'HẾT HẠN'
		WHEN p.ExpiryDate < DATEADD(DAY, 30, GETDATE()) THEN 'SẮP HẾT HẠN'
	END AS CanhBao,
	p.ProductCode,
	p.ProductName,
	p.ExpiryDate,
	DATEDIFF(DAY, GETDATE(), p.ExpiryDate) AS NgayConLai,
	i.Quantity
FROM Products p
JOIN Inventories i ON p.Id = i.ProductId
WHERE p.ExpiryDate <= DATEADD(DAY, 30, GETDATE()) AND i.Quantity > 0
ORDER BY p.ExpiryDate ASC;

-- 6.6 Danh sách nợ (đơn bán chưa thanh toán)
SELECT 
	s.SaleCode,
	s.SaleDate,
	c.FullName AS KhachHang,
	c.Phone,
	s.TotalAmount,
	s.PaidAmount,
	(s.TotalAmount - s.PaidAmount) AS ChuaThanhtoan,
	DATEDIFF(DAY, s.SaleDate, GETDATE()) AS SoNgayNo
FROM Sales s
LEFT JOIN Customers c ON s.CustomerId = c.Id
WHERE s.PaymentStatus IN ('Chưa thanh toán', 'Một phần')
ORDER BY (s.TotalAmount - s.PaidAmount) DESC;

-- 6.7 Danh sách nợ nhà cung cấp (đơn nhập chưa thanh toán)
SELECT 
	io.ImportCode,
	io.ImportDate,
	s.SupplierName,
	s.Phone,
	io.TotalAmount,
	io.PaidAmount,
	(io.TotalAmount - io.PaidAmount) AS ChuaThanhtoan,
	DATEDIFF(DAY, io.ImportDate, GETDATE()) AS SoNgayNo
FROM ImportOrders io
JOIN Suppliers s ON io.SupplierId = s.Id
WHERE io.PaymentStatus IN ('Chưa thanh toán', 'Một phần')
ORDER BY (io.TotalAmount - io.PaidAmount) DESC;

-- ============================================================================
-- NOTES:
-- ============================================================================
-- * Thay ID/Date phù hợp khi sử dụng DECLARE
-- * Có thể bind các tham số từ C# code
-- * Performance: Nên thêm indexes trên các cột thường dùng trong WHERE
-- * Backup trước khi chạy UPDATE/DELETE scripts
-- ============================================================================
