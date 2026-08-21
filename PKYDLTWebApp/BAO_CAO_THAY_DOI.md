# BÁO CÁO THAY ĐỔI — Tính năng Cập nhật Hóa Đơn

**Ngày:** 21/08/2026
**Người thực hiện:** Senior Developer
**Dự án:** PKYDLTWebApp (Quản lý phòng khám / Bán hàng - Kho)

---

## 1. MỤC ĐÍCH

Chỉnh sửa tính năng cập nhật hóa đơn để cho phép:
1. **Thêm / bớt sản phẩm** trong hóa đơn.
2. **Chỉnh sửa ghi chú** của hóa đơn và của từng dòng sản phẩm.
3. **Điều chỉnh kho tự động**:
   - Bớt sản phẩm / giảm số lượng → **cộng lại kho**.
   - Thêm sản phẩm / tăng số lượng → **trừ kho**.
4. Khi **xóa hóa đơn** → **cộng lại toàn bộ sản phẩm** vào kho.

---

## 2. KIẾN TRÚC HIỆN TẠI (TRƯỚC KHI SỬA)

- Hóa đơn (`Invoice`) **không có** dòng sản phẩm riêng.
- Sản phẩm của hóa đơn được lấy từ **`Sale.SaleDetails`** (đơn bán hàng liên kết qua `Invoice.SaleId`).
- Tồn kho lưu trong **`Inventory.Quantity`**.
- Khi tạo đơn bán nhanh (`Pages/Sales/Quick`), hệ thống trừ kho và tự sinh hóa đơn.
- Trang Sửa hóa đơn gốc chỉ cho sửa thông tin (số HĐ, ngày, khách, chiết khấu, VAT, trả trước, ghi chú) — **không** cho thêm/bớt sản phẩm và **không** đụng tới kho.

---

## 3. CÁC THAY ĐỔI ĐÃ THỰC HIỆN

### 3.1. Trang `Pages/Invoices/Edit.cshtml.cs` (Controller — viết lại)

- Thêm `[BindProperty] List<SaleDetail> SaleDetails` để nhận danh sách dòng sản phẩm từ form.
- Thêm các trường `Products`, `SaleDetailsJson`, `ProductsJson` để render giao diện động.
- (**OnPostAsync**) Xử lý đầy đủ luồng:
  1. **Chuẩn hóa** danh sách dòng gửi lên (bỏ dòng không hợp lệ).
  2. **Tính tổng số lượng cũ – mới** theo từng sản phẩm.
  3. **Kiểm tra đủ kho** cho phần thêm mới (nếu thiếu → báo lỗi, không lưu).
  4. **Cập nhật kho theo chênh lệch**:
     - `stockDelta = oldQty - newQty`
     - `stockDelta > 0` (bớt sản phẩm) → **cộng lại kho**.
     - `stockDelta < 0` (thêm sản phẩm) → **trừ kho**.
     - Cập nhật `LastReceivedDate`/`LastIssuedDate`, `UpdatedAt`.
     - Nếu chưa có dòng kho mà cần trả về → tự tạo dòng kho mới.
  5. **Cập nhật dòng sản phẩm** của đơn bán: xóa dòng cũ, tạo lại theo danh sách mới (đã gộp theo sản phẩm).
  6. **Tính lại tổng tiền** của đơn bán (`CalculateTotal`) và đồng bộ sang hóa đơn (`SubTotal`, `TotalAmount`, `PaymentStatus`).
  7. Cập nhật các trường thông tin hóa đơn và `UpdatedAt`.
- Thêm các helper: `LoadInvoiceAsync` (nạp Invoice + Sale + SaleDetails + Product + Customer) và `BuildFormDataAsync` (nạp sản phẩm, tồn kho, dựng JSON cho giao diện).

### 3.2. Trang `Pages/Invoices/Edit.cshtml` (View — viết lại)

- Giữ nguyên phần thông tin hóa đơn (số HĐ, ngày, khách, chiết khấu, VAT, trả trước, trạng thái, ghi chú…).
- **Thêm khối “Sản phẩm trong hóa đơn”**:
  - Bảng danh sách sản phẩm hiện tại: cho **sửa số lượng**, **đơn giá**, **ghi chú từng dòng**, và nút **Xóa (🗑)**.
  - Form **thêm sản phẩm** (chọn sản phẩm + số lượng + ghi chú → nhấn “Thêm”).
  - Hiển thị tồn kho hiện tại của từng sản phẩm và “Thành tiền sản phẩm”.
- **JavaScript** quản lý danh sách dòng: `addProduct`, `updateQty`, `updatePrice`, `updateNote`, `removeItem`, `renderTable`, `renderHiddenInputs`, `calculateTotal`. Dữ liệu được đưa vào form thông qua các hidden input `SaleDetails[i].Id/.ProductId/.Quantity/.UnitPrice/.Notes`.

### 3.3. Trang `Pages/Invoices/Delete.cshtml.cs` (Controller — bổ sung)

- (**OnPostAsync**) Trước khi xóa hóa đơn:
  - Gộp toàn bộ số lượng sản phẩm của đơn bán theo từng sản phẩm.
  - **Cộng lại toàn bộ** vào kho (tạo dòng kho mới nếu chưa có).
  - Cập nhật `LastReceivedDate`, `UpdatedAt`.
  - Sau đó mới xóa hóa đơn.
- (**OnGetAsync**) Nạp thêm `Sale.SaleDetails.Product` để hiển thị danh sách sản phẩm trên trang xác nhận.

### 3.4. Trang `Pages/Invoices/Delete.cshtml` (View — cập nhật nội dung)

- Đổi toàn bộ nội dung sang tiếng Việt.
- Thêm thông báo rõ: “Khi xóa hóa đơn, toàn bộ sản phẩm sẽ được cộng lại vào kho”.
- **Liệt kê** các sản phẩm sẽ được hoàn về kho trước khi xác nhận.
- Đổi nút thành “🗑️ Xóa hóa đơn + cộng lại kho”.

---

## 4. QUY TẮC ĐIỀU CHỈNH KHO (TÓM TẮT)

| Trường hợp | Thay đổi kho |
|---|---|
| Thêm sản phẩm mới vào hóa đơn | Trừ kho đúng số lượng thêm |
| Tăng số lượng của một sản phẩm | Trừ kho phần tăng thêm |
| Bớt / xóa sản phẩm khỏi hóa đơn | Cộng lại kho đúng số lượng bớt |
| Xóa hóa đơn | Cộng lại **toàn bộ** sản phẩm của hóa đơn vào kho |

**Bảo vệ:** Nếu thêm sản phẩm mà kho không đủ, hệ thống chặn và báo lỗi, không lưu thay đổi.

---

## 5. GHI CHÚ / GIỚI HẠN

- Sản phẩm của hóa đơn vẫn được lưu thông qua **đơn bán hàng liên kết** (`Sale.SaleDetails`) — không thay đổi cấu trúc CSDL, **không cần migration** mới.
- Giả định một hóa đơn gắn với một đơn bán (luồng hiện tại của hệ thống).
- Khi sửa hóa đơn, giá/ghi chú của các dòng được gộp theo từng sản phẩm để tạo lại dòng cho đơn bán.
- **Chưa tự động xóa hóa đơn khi đã “Đã in”**; nếu cần kiểm soát thêm, có thể bổ sung sau.

---

## 6. DANH SÁCH FILE THAY ĐỔI

1. `Pages/Invoices/Edit.cshtml.cs` — Controller cập nhật hóa đơn (viết lại, thêm logic kho + sản phẩm).
2. `Pages/Invoices/Edit.cshtml` — View cập nhật hóa đơn (thêm bảng sản phẩm động + JS).
3. `Pages/Invoices/Delete.cshtml.cs` — Controller xóa hóa đơn (thêm logic cộng lại kho).
4. `Pages/Invoices/Delete.cshtml` — View xóa hóa đơn (cập nhật nội dung + danh sách hoàn kho).
5. `BAO_CAO_THAY_DOI.md` — File báo cáo này.

---

## 7. KẾT QUẢ

- ✅ Cho phép thêm/bớt sản phẩm khi sửa hóa đơn.
- ✅ Cho phép sửa ghi chú của hóa đơn và từng dòng.
- ✅ Bớt sản phẩm → cộng lại kho; thêm sản phẩm → trừ kho.
- ✅ Xóa hóa đơn → cộng lại toàn bộ sản phẩm vào kho.
- ✅ Có kiểm tra đủ kho trước khi lưu.
- ✅ **Build thành công**: `dotnet build -c Debug` → **0 Error / 23 Warning**. Các cảnh báo còn lại đều là cảnh báo nullable-reference (CS8602/CS8618) có sẵn từ trước và rải rác khắp dự án, không phải lỗi biên dịch.
- ✅ Sửa lỗi biên dịch: dùng tên đầy đủ `ClinicManagement.Models.Inventory` (do tên `Inventory` bị xung đột với namespace trong ngữ cảnh biên dịch), khớp với cách viết sẵn có trong `Pages/Adjustments/Create.cshtml.cs`.

---

## 8. LỊCH SỬ THAY ĐỔI / XÁC MINH

- **Lần build 1 (trước khi build):** 2 lỗi `CS0118: 'Inventory' is a namespace but is used like a type` tại `Edit.cshtml.cs` và `Delete.cshtml.cs` → đã sửa bằng cách dùng `ClinicManagement.Models.Inventory`.
- **Build cuối (đã xác minh):** `Build succeeded.` — **23 Warning(s), 0 Error(s)**.
