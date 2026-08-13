# 📊 CẬP NHẬT DATABASE - HỆSYSTEM QUẢN LÝ PHÒNG KHÁM

## ✅ HOÀN THÀNH

Tôi đã thiết kế và implement đầy đủ database schema cho hệ thống quản lý phòng khám với các tính năng:
- ✅ Quản lý sản phẩm/thuốc
- ✅ Quản lý kho
- ✅ Quản lý nhà cung cấp
- ✅ Quản lý nhập hàng
- ✅ Quản lý bán hàng
- ✅ Quản lý hóa đơn
- ✅ Quản lý toa bệnh
- ✅ Phân quyền chi tiết

---

## 📋 DANH SÁCH MODELS ĐÃ TẠO

### 1. **Product (Sản phẩm/Thuốc)**
- mã sản phẩm (unique)
- tên, tên khác (VD: tên chứng chỉ TC bệnh)
- loại sản phẩm (Thuốc, Nước, Thiết bị y tế)
- đơn vị tính (Vỉ, Lọ, Chai, Tuýp)
- hàm lượng/liều lượng
- số lô, ngày hết hạn
- giá nhập (cost), giá bán lẻ, giá buôn
- nhà cung cấp (foreign key)
- mô tả, ghi chú
- trạng thái hoạt động

### 2. **Supplier (Nhà cung cấp)**
- mã nhà cung cấp (unique)
- tên, người đại diện/liên hệ
- điện thoại, email, địa chỉ
- ngân hàng: tên tài khoản, số TK, tên ngân hàng
- mã số thuế
- điều khoản thanh toán
- chiết khấu mặc định
- trạng thái hoạt động

### 3. **Inventory (Quản kho)**
- sản phẩm (foreign key)
- số lượng tồn kho hiện tại
- số lượng tối thiểu (để nhắc nhở)
- số lượng tối đa
- vị trí kho (VD: Kệ A1, Ngăn B2)
- ngày nhập lần cuối, ngày xuất lần cuối
- ngày kiểm kho lần cuối
- số lần bán trong tháng
- status (Sẵn, Hư hỏng, Hết hạn)
- **Methods**: GetInventoryValue(), IsLowStock(), IsNearExpiry(), IsExpired()

### 4. **ImportOrder (Đơn nhập hàng)**
- số hiệu đơn nhập (unique, VD: IM-2026-001)
- ngày nhập, ngày hết hạn thanh toán
- nhà cung cấp (foreign key)
- số hóa đơn, ngày hóa đơn
- **Tính toán tài chính**:
  - tổng tiền hàng (SubTotal)
  - chiết khấu, thuế VAT, chi phí vận chuyển
  - tổng tiền phải thanh toán
  - số đã thanh toán
  - trạng thái thanh toán (Chưa, Một phần, Đã TT)
- trạng thái đơn (Chờ xác nhận, Đã nhập, Hủy)
- người tạo, người xác nhận (foreign key)
- ghi chú
- **Methods**: CalculateTotal(), IsFullyPaid(), GetRemainingAmount()

### 5. **ImportOrderDetail (Chi tiết đơn nhập)**
- đơn nhập (foreign key)
- sản phẩm (foreign key)
- số lượng, đơn giá
- thành tiền
- số lô, hạn sử dụng
- hàng nhập thực tế, hàng hư hỏng/mất
- ghi chú (không đủ lô, khác kích cỡ, v.v.)
- **Method**: CalculateTotal()

### 6. **Sale (Đơn bán hàng)**
- số hiệu đơn bán (unique, VD: SALE-2026-001)
- ngày bán
- khách hàng (foreign key, nullable - khách lẻ)
- nhân viên bán hàng (foreign key)
- **Tính toán tài chính**:
  - tổng tiền hàng, chiết khấu (%, tiền), VAT
  - tổng tiền phải thanh toán
  - tiền khách trả, tiền thối lại/nợ
- phương thức thanh toán (Tiền mặt, Chuyển khoản, QR)
- trạng thái thanh toán, trạng thái đơn (Mới, Hoàn thành, Hủy)
- ghi chú
- người lập (foreign key)
- **Methods**: CalculateTotal(), IsFullyPaid(), GetRemainingAmount()

### 7. **SaleDetail (Chi tiết bán hàng)**
- đơn bán (foreign key)
- sản phẩm (foreign key)
- số lượng, đơn giá
- thành tiền
- chiết khấu (%, tiền)
- ghi chú (hàng tặng kèm, hết hạn sắp, v.v.)
- **Methods**: CalculateTotal(), GetFinalTotal()

### 8. **Invoice (Hóa đơn)**
- số hiệu hóa đơn (unique, VD: HĐ001/2026)
- ngày lập
- loại hóa đơn (Tờ rơi, Hóa đơn GTGT, Chứng từ tự in)
- đơn bán (foreign key)
- **Thông tin khách**:
  - tên, địa chỉ, điện thoại, email
  - (có thể khác với DB nếu khách lẻ)
- **Tài chính**: bản sao từ Sale (SubTotal, Discount, VAT, Total, Paid)
- trạng thái (Chưa in, Đã in, Huỷ)
- số lần in, lần in cuối
- người tạo (foreign key)
- ghi chú
- **Methods**: MarkAsPrinted(), SyncFromSale()

### 9. **Prescription (Toa bệnh)**
- số hiệu toa (unique, VD: TOA-2026-001)
- ngày lập, ngày hiệu lực, ngày hết hiệu lực
- bệnh nhân (foreign key)
- bác sĩ/người lập toa (foreign key)
- **Thông tin chẩn đoán**:
  - chẩn đoán bệnh, triệu chứng
  - chỉ định/hướng dẫn điều trị
- **Thông tin bệnh nhân**:
  - cân nặng, chiều cao, huyết áp, nhiệt độ
- trạng thái (Hoạt động, Hết hiệu lực, Huỷ)
- đã in, lần in cuối
- ghi chú bác sĩ
- **Methods**: IsValid(), MarkAsPrinted()

### 10. **PrescriptionDetail (Chi tiết toa bệnh)**
- toa bệnh (foreign key)
- sản phẩm/thuốc (foreign key)
- **Liều dùng**:
  - số lượng, đơn vị (viên, ống, ml, v.v.)
  - liều lượng mỗi lần (VD: 500mg, 1 viên)
  - tần suất (3 lần/ngày, 2 lần/ngày sáng tối)
  - đường dùng (Uống, Tiêm, Bôi, Nhỏ, Hít)
  - thời gian dùng (7 ngày, 2 tuần)
  - tổng số lần dùng
- **Hướng dẫn**:
  - ghi chú/hướng dẫn (sau khi ăn, uống với nước ấm)
  - cảnh báo/chống chỉ định
  - tương tác thuốc (flag)
  - tác dụng phụ cần lưu ý
- trạng thái (Chưa dùng, Đang dùng, Đã dùng xong)
- ngày bắt đầu, ngày kết thúc

### 11. **Permission (Quyền hạn)**
- vai trò (foreign key)
- tên module/tính năng (Product, Inventory, Sale, v.v.)
  - Danh sách: Product, Inventory, Supplier, ImportOrder, Sale, Invoice, Prescription, Customer, User, Role, Report, Settings
- hành động (View, Create, Edit, Delete, Print, Export, Approve, Reject)
- mô tả
- được phép hay không
- **Unique constraint**: RoleId + ModuleName + Action

---

## 🔗 RELATIONSHIPS (Quan hệ)

```
Product ← → Supplier (Many-to-One)
Product ← → Inventory (One-to-Many)
Product ← → PrescriptionDetail (One-to-Many)
Product ← → SaleDetail (One-to-Many)

Inventory → Product (Many-to-One)

ImportOrder ← → Supplier (Many-to-One)
ImportOrder ← → User (Many-to-One, who created)
ImportOrder ← → ImportOrderDetail (One-to-Many)
ImportOrderDetail → Product (Many-to-One)

Sale ← → Customer (Many-to-One, nullable)
Sale ← → User (Many-to-One, SalesPerson)
Sale ← → SaleDetail (One-to-Many)
Sale ← → Invoice (One-to-Many)
SaleDetail → Product (Many-to-One)

Invoice → Sale (Many-to-One)
Invoice → User (Many-to-One, CreatedBy)

Prescription → Customer (Many-to-One)
Prescription → User (Many-to-One, CreatedBy)
Prescription ← → PrescriptionDetail (One-to-Many)
PrescriptionDetail → Product (Many-to-One)

Permission → Role (Many-to-One)
```

---

## 📊 DATABASE STRUCTURE SUMMARY

| Module | Tables | Fields | Purpose |
|--------|--------|--------|---------|
| **Product** | Product, Supplier | 40+ | Quản lý sản phẩm, nhà cung cấp |
| **Inventory** | Inventory | 15+ | Quản lý tồn kho |
| **Import** | ImportOrder, ImportOrderDetail | 30+ | Quản lý nhập hàng |
| **Sale** | Sale, SaleDetail | 40+ | Quản lý bán hàng |
| **Invoice** | Invoice | 25+ | Quản lý hóa đơn in |
| **Prescription** | Prescription, PrescriptionDetail | 50+ | Quản lý toa bệnh |
| **Permission** | Permission | 6+ | Quản lý quyền hạn |

**Tổng cộng**: ~13 tables, ~250+ fields

---

## ✨ FEATURES CHỎ YỂU

### 1. **Quản Lý Sản Phẩm**
- Danh sách thuốc/sản phẩm đầy đủ
- Theo dõi hạn sử dụng
- Quản lý nhiều giá (nhập, lẻ, buôn)
- Mã sản phẩm unique, dễ tìm kiếm

### 2. **Quản Lý Kho**
- Tồn kho theo sản phẩm
- Cảnh báo khi hàng hết thiếu
- Cảnh báo khi hàng sắp hết hạn
- Theo dõi vị trí kho
- Lịch sử kiểm kho

### 3. **Quản Lý Nhập Hàng**
- Ghi nhận đơn nhập từ nhà cung cấp
- Theo dõi hóa đơn nhà cung cấp
- Tính toán tổng tiền (chiết khấu, VAT, vận chuyển)
- Quản lý thanh toán (chưa, một phần, đã)
- Chi tiết từng sản phẩm trong đơn

### 4. **Quản Lý Bán Hàng**
- Ghi nhận bán hàng cho khách lẻ hoặc khách hàng trong hệ
- Tính chiết khấu theo dòng hoặc toàn đơn
- Quản lý thanh toán (tiền mặt, chuyển khoản)
- Tính tiền thối lại/nợ tự động
- Theo dõi nhân viên bán hàng

### 5. **Quản Lý Hóa Đơn**
- Tạo hóa đơn từ đơn bán
- In hóa đơn (theo dõi số lần in)
- Thông tin khách hàng (có thể custom cho khách lẻ)
- Trạng thái hóa đơn (chưa in, đã in, huỷ)
- Bảng ghi nếu từ đơn bán

### 6. **Quản Lý Toa Bệnh**
- Lập toa bệnh cho bệnh nhân
- Chi tiết từng thuốc: liều lượng, tần suất, đường dùng, thời gian
- Thông tin bệnh nhân (cân nặng, huyết áp, v.v.)
- Cảnh báo về tương tác thuốc
- In toa bệnh

### 7. **Phân Quyền Chi Tiết**
- Quyền theo module (12 modules)
- Quyền theo action (8 actions: View, Create, Edit, Delete, Print, Export, Approve, Reject)
- Dễ mở rộng khi thêm tính năng mới

---

## 🗄️ DATABASE INDEXES

Các index đã được tạo tối ưu hiệu năng:

```
Product:
  - ProductCode (UNIQUE)
  - ProductName
  - SupplierId

Supplier:
  - SupplierCode (UNIQUE)

Inventory:
  - ProductId
  - Quantity

ImportOrder:
  - ImportCode (UNIQUE)
  - ImportDate

ImportOrderDetail:
  - ImportOrderId
  - ProductId

Sale:
  - SaleCode (UNIQUE)
  - SaleDate
  - CustomerId
  - SalesPersonUserId

SaleDetail:
  - SaleId
  - ProductId

Invoice:
  - InvoiceNumber (UNIQUE)
  - InvoiceDate

Prescription:
  - PrescriptionCode (UNIQUE)
  - PrescriptionDate
  - CustomerId

PrescriptionDetail:
  - PrescriptionId
  - ProductId

Permission:
  - RoleId + ModuleName + Action (UNIQUE)
```

---

## 🚀 CÁCH SỬ DỤNG

### 1. **Thêm Nhà Cung Cấp Mới**
```csharp
var supplier = new Supplier
{
	SupplierName = "Công ty Y Tế ABC",
	SupplierCode = "NCC001",
	Phone = "0123456789",
	Email = "supplier@example.com"
};
_context.Suppliers.Add(supplier);
await _context.SaveChangesAsync();
```

### 2. **Thêm Sản Phẩm Mới**
```csharp
var product = new Product
{
	ProductCode = "THUOC001",
	ProductName = "Paracetamol 500mg",
	Category = "Thuốc",
	Unit = "Vỉ",
	Strength = "500mg",
	CostPrice = 5000,
	RetailPrice = 8000,
	SupplierId = 1
};
_context.Products.Add(product);
await _context.SaveChangesAsync();
```

### 3. **Ghi Nhận Nhập Hàng**
```csharp
var importOrder = new ImportOrder
{
	ImportCode = "IM-2026-001",
	ImportDate = DateTime.Now,
	SupplierId = 1,
	InvoiceNumber = "HĐ123/2026"
};

// Thêm chi tiết
importOrder.ImportDetails.Add(new ImportOrderDetail
{
	ProductId = 1,
	Quantity = 100,
	UnitPrice = 5000,
	ExpiryDate = DateTime.Now.AddMonths(24)
});

importOrder.CalculateTotal();
_context.ImportOrders.Add(importOrder);
await _context.SaveChangesAsync();

// Cập nhật tồn kho
var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == 1);
if (inventory == null)
{
	inventory = new Inventory { ProductId = 1, Quantity = 0 };
	_context.Inventories.Add(inventory);
}
inventory.Quantity += 100;
await _context.SaveChangesAsync();
```

### 4. **Ghi Bán Hàng**
```csharp
var sale = new Sale
{
	SaleCode = "SALE-2026-001",
	SaleDate = DateTime.Now,
	CustomerId = 1, // hoặc null nếu khách lẻ
	SalesPersonUserId = 1
};

// Thêm chi tiết sản phẩm
sale.SaleDetails.Add(new SaleDetail
{
	ProductId = 1,
	Quantity = 2,
	UnitPrice = 8000,
	DiscountPercent = 10
});

sale.CalculateTotal();
_context.Sales.Add(sale);
await _context.SaveChangesAsync();

// Tạo hóa đơn
var invoice = new Invoice
{
	InvoiceNumber = "HĐ001/2026",
	InvoiceDate = DateTime.Now,
	SaleId = sale.Id
};
invoice.SyncFromSale();
_context.Invoices.Add(invoice);
await _context.SaveChangesAsync();
```

### 5. **Lập Toa Bệnh**
```csharp
var prescription = new Prescription
{
	PrescriptionCode = "TOA-2026-001",
	PrescriptionDate = DateTime.Now,
	CustomerId = 1,
	CreatedByUserId = 1,
	Diagnosis = "Cảm cúm"
};

// Thêm thuốc
prescription.PrescriptionDetails.Add(new PrescriptionDetail
{
	ProductId = 1,
	Quantity = 1,
	Dosage = "500mg",
	Frequency = "3 lần/ngày",
	Route = "Uống",
	Duration = "7 ngày",
	Instructions = "Uống sau khi ăn"
});

_context.Prescriptions.Add(prescription);
await _context.SaveChangesAsync();
```

---

## ⚠️ LƯU Ý QUAN TRỌNG

### 1. **Decimal Precision**
Database mặc định sử dụng `decimal(18,2)` cho giá tiền. Từ model có thể tùy chỉnh:
```csharp
modelBuilder.Entity<Product>()
	.Property(p => p.CostPrice)
	.HasPrecision(18, 2);
```

### 2. **Transactions**
Khi thực hiện bán hàng + nhập kho, cần transaction để đảm bảodata consistency:
```csharp
using (var transaction = await _context.Database.BeginTransactionAsync())
{
	try
	{
		// Thêm Sale
		// Cập nhật Inventory
		// Tạo Invoice
		await _context.SaveChangesAsync();
		await transaction.CommitAsync();
	}
	catch
	{
		await transaction.RollbackAsync();
		throw;
	}
}
```

### 3. **Soft Delete (Optional)**
Nếu muốn giữ lịch sử thay vì xóa, thêm:
```csharp
public bool IsDeleted { get; set; } = false;
public DateTime? DeletedAt { get; set; }
```

### 4. **Audit Trail (Optional)**
Để theo dõi ai thay đổi gì, thêm:
```csharp
public int? ModifiedByUserId { get; set; }
public DateTime? ModifiedAt { get; set; }
```

---

## 📝 NEXT STEPS

1. **Tạo Controllers/Pages** cho từng module (Product, Sale, Invoice, v.v.)
2. **Implement Business Logic** (tính chiết khấu, kiểm tra hàng hết hạn, v.v.)
3. Implement **Authorization** dựa trên Permission model
4. Tạo **Reports** (báo cáo bán hàng, bao cáo kho, v.v.)
5. Thêm **Validations** phức tạp hơn
6. Tối ưu **Performance** (caching, batch operations)

---

## 📞 CÁC CÂUĐÓ THƯỜNG GẶP

### Q: Làm sao để thêm sản phẩm vào tương tác thuốc?
**A:** Tạo bảng `DrugInteraction` với hai khóa ngoài tới `Product`.

### Q: Làm sao để theo dõi lịch sử thay đổi giá?
**A:** Tạo bảng `PriceHistory` với timestamp.

### Q: Làm sao để quản lý kho nhiều nơi?
**A:** Thêm `Warehouse` model, `Inventory` link tới warehouse.

### Q: Làm sao để theo dõi hàng bán lô?
**A:** Thêm `BatchNumber` vào `SaleDetail`.

### Q:
 Làm sao để pull report bán hàng?
**A:** Query `Sale` với `SaleDetail`, group by Product/Date.

---

**✅ DATABASE ĐÃSẴN SÀNGdevices THỨ PHÁT TRIỂN TIẾP!** 🚀
