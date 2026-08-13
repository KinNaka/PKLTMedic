# 📊 ER DIAGRAM - DATABASE STRUCTURE

## Entity Relationships

```
┌─────────────────────────────────────────────────────────────────────┐
│                         USER MANAGEMENT                             │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌──────────────┐              ┌──────────────┐                     │
│  │    Role      │◄─────────────│     User     │                     │
│  │ - Id (PK)    │  1:Many      │ - Id (PK)    │                     │
│  │ - Name       │              │ - Username   │                     │
│  │ - Description                │ - PasswordHash
│  └──────────────┘              │ - FullName   │                     │
│                                 │ - Email      │                     │
│       │                         │ - Phone      │                     │
│       │                         │ - IsActive   │                     │
│       │                         │ - RoleId (FK)                      │
│       │                         └──────────────┘                     │
│       │                              ▲                               │
│       │                              │                               │
│       └──► Permission (Role → Module → Action)                       │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                    PRODUCT & SUPPLIER MANAGEMENT                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌──────────────┐         ┌──────────────┐                          │
│  │  Supplier    │◄────────│   Product    │                          │
│  │ - Id (PK)    │ 1:Many  │ - Id (PK)    │                          │
│  │ - Name       │         │ - Code (UQ)  │                          │
│  │ - Phone      │         │ - Name       │                          │
│  │ - Email      │         │ - Category   │                          │
│  │ - Address    │         │ - Unit       │                          │
│  │ - Bank Info  │         │ - Strength   │                          │
│  │ - Tax Code   │         │ - CostPrice  │                          │
│  │ - Discount % │         │ - RetailPrice                           │
│  └──────────────┘         │ - SupplierId (FK)                       │
│         ▲                  │ - ExpiryDate │                          │
│         │                  └──────────────┘                          │
│         │                        │                                    │
│         └────────────────────────┘                                    │
│                    Supplies                                           │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                      INVENTORY MANAGEMENT                            │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌──────────────────┐                                               │
│  │   Inventory      │───────────┐                                   │
│  │ - Id (PK)        │           │                                   │
│  │ - ProductId (FK) │           ├─────► Product                    │
│  │ - Quantity       │           │                                   │
│  │ - MinQuantity    │───────────┘                                   │
│  │ - MaxQuantity    │                                               │
│  │ - WarehouseLocation                                              │
│  │ - Status         │                                               │
│  │ - LastReceivedDate                                               │
│  │ - LastIssuedDate │                                               │
│  │ - LastCountDate  │                                               │
│  └──────────────────┘                                               │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                    IMPORT MANAGEMENT (Nhập Hàng)                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌──────────────┐          ┌─────────────────────┐                 │
│  │Supplier      │◄─────────│   ImportOrder       │                 │
│  └──────────────┘ 1:Many   │ - Id (PK)           │                 │
│                             │ - Code (UQ)         │                 │
│  ┌──────────────┐           │ - ImportDate        │                 │
│  │    User      │◄─────────│ - DueDate           │                 │
│  │(CreatedBy)   │1:Many    │ - InvoiceNumber     │                 │
│  └──────────────┘          │ - SubTotal, Discount                  │
│                             │ - VAT, TaxAmount    │                 │
│  ┌──────────────┐           │ - ShippingCost      │                 │
│  │    User      │◄─────────│ - TotalAmount       │                 │
│  │(Confirmed)   │1:Many    │ - PaidAmount        │                 │
│  └──────────────┘          │ - Status            │                 │
│                             │ - PaymentStatus     │                 │
│                             │ - SupplierId (FK)   │                 │
│                             └─────────────────────┘                 │
│                                      │                               │
│                                      │ 1:Many                        │
│                                      ▼                               │
│                         ┌─────────────────────────┐                 │
│                         │ ImportOrderDetail       │                 │
│                         │ - Id (PK)               │                 │
│                         │ - ImportOrderId (FK)    │                 │
│                         │ - ProductId (FK)        │                 │
│                         │ - Quantity              │                 │
│                         │ - UnitPrice             │                 │
│                         │ - Total                 │                 │
│                         │ - ExpiryDate            │                 │
│                         │ - BatchNumber           │                 │
│                         │ - ReceivedQuantity      │                 │
│                         │ - DamagedQuantity       │                 │
│                         └─────────────────────────┘                 │
│                                      │                               │
│                                      └──────── Product                │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                    SALES MANAGEMENT (Bán Hàng)                       │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌──────────────┐          ┌──────────────────┐                    │
│  │ Customer     │◄─────────│     Sale         │                    │
│  │              │1:Many    │ - Id (PK)        │                    │
│  └──────────────┘          │ - Code (UQ)      │                    │
│                             │ - SaleDate       │                    │
│  ┌──────────────┐           │ - SubTotal       │                    │
│  │    User      │◄─────────│ - DiscountAmount │                    │
│  │(SalesPerson) │1:Many    │ - VATAmount      │                    │
│  └──────────────┘          │ - TotalAmount    │                    │
│                             │ - PaidAmount     │                    │
│                             │ - ChangeAmount   │                    │
│                             │ - PaymentMethod  │                    │
│                             │ - PaymentStatus  │                    │
│                             │ - Status         │                    │
│                             └──────────────────┘                    │
│                                      │                              │
│                                      │ 1:Many                       │
│                                      ▼                              │
│                         ┌──────────────────────┐                   │
│                         │   SaleDetail         │                   │
│                         │ - Id (PK)            │                   │
│                         │ - SaleId (FK)        │                   │
│                         │ - ProductId (FK)     │                   │
│                         │ - Quantity           │                   │
│                         │ - UnitPrice          │                   │
│                         │ - Total              │                   │
│                         │ - DiscountPercent    │                   │
│                         │ - DiscountAmount     │                   │
│                         └──────────────────────┘                   │
│                                      │                              │
│                                      └──► Product                   │
│                                                                      │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                    INVOICE MANAGEMENT (Hóa Đơn)                      │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌──────────────┐          ┌──────────────────┐                    │
│  │    Sale      │◄─────────│    Invoice       │                    │
│  │              │1:Many    │ - Id (PK)        │                    │
│  └──────────────┘          │ - Number (UQ)    │                    │
│                             │ - InvoiceDate    │                    │
│  ┌──────────────┐           │ - InvoiceType    │                    │
│  │    User      │◄─────────│ - CustomerName   │                    │
│  │(CreatedBy)   │1:Many    │ - CustomerAddress│                    │
│  └──────────────┘          │ - CustomerPhone  │                    │
│                             │ - SubTotal       │                    │
│                             │ - DiscountAmount │                    │
│                             │ - VATAmount      │                    │
│                             │ - TotalAmount    │                    │
│                             │ - PaidAmount     │                    │
│                             │ - Status         │                    │
│                             │ - PrintCount     │                    │
│                             │ - LastPrintedDate                     │
│                             └──────────────────┘                    │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│               PRESCRIPTION MANAGEMENT (Toa Bệnh)                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌──────────────┐          ┌──────────────────┐                    │
│  │ Customer     │◄─────────│  Prescription    │                    │
│  │(Patient)     │1:Many    │ - Id (PK)        │                    │
│  └──────────────┘          │ - Code (UQ)      │                    │
│                             │ - PrescriptionDate                    │
│  ┌──────────────┐           │ - EffectiveDate  │                    │
│  │    User      │◄─────────│ - ExpiryDate     │                    │
│  │(CreatedBy)   │1:Many    │ - Diagnosis      │                    │
│  └──────────────┘          │ - Symptoms       │                    │
│                             │ - Instructions   │                    │
│                             │ - Weight, Height │                    │
│                             │ - BloodPressure  │                    │
│                             │ - Temperature    │                    │
│                             │ - Status         │                    │
│                             │ - IsPrinted      │                    │
│                             └──────────────────┘                    │
│                                      │                              │
│                                      │ 1:Many                       │
│                                      ▼                              │
│                    ┌──────────────────────────────┐                │
│                    │  PrescriptionDetail          │                │
│                    │ - Id (PK)                    │                │
│                    │ - PrescriptionId (FK)        │                │
│                    │ - ProductId (FK)             │                │
│                    │ - Quantity                   │                │
│                    │ - Unit (viên, ống, ml, ...)  │                │
│                    │ - Dosage (liều lượng)        │                │
│                    │ - Frequency (tần suất)       │                │
│                    │ - Route (đường dùng)         │                │
│                    │ - Duration (thời gian)       │                │
│                    │ - TotalDoses                 │                │
│                    │ - Instructions               │                │
│                    │ - Contraindication           │                │
│                    │ - HasDrugInteraction         │                │
│                    │ - SideEffects                │                │
│                    │ - Status                     │                │
│                    │ - StartDate, EndDate         │                │
│                    └──────────────────────────────┘                │
│                                      │                              │
│                                      └──► Product                   │
│                                                                      │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                   PERMISSION MANAGEMENT (Quyền Hạn)                  │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌──────────────┐          ┌──────────────────┐                    │
│  │    Role      │◄─────────│  Permission      │                    │
│  │              │1:Many    │ - Id (PK)        │                    │
│  └──────────────┘          │ - RoleId (FK)    │                    │
│                             │ - ModuleName     │                    │
│                             │  (enum: Product,  │                    │
│                             │   Inventory,      │                    │
│                             │   Sale,           │                    │
│                             │   Invoice, etc.)  │                    │
│                             │ - Action         │                    │
│                             │  (enum: View,     │                    │
│                             │   Create, Edit,   │                    │
│                             │   Delete, Print,│                    │
│                             │   Export, ...)    │                    │
│                             │ - IsGranted      │                    │
│                             │ - Description    │                    │
│                             └──────────────────┘                    │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 🔑 KEY BUSINESS LOGIC

### 1. **Cập nhật Inventory từ ImportOrder**
```
ImportOrder + ImportOrderDetail
	↓
Product
	↓
Inventory (Quantity += ImportOrderDetail.ReceivedQuantity)
```

### 2. **Tạo Invoice từ Sale**
```
Sale + SaleDetail
	↓
Invoice (copy tất cả thông tin từ Sale)
	↓
In/Print (MarkAsPrinted())
```

### 3. **Giảm Inventory khi bán**
```
Sale + SaleDetail
	↓
Inventory (Quantity -= SaleDetail.Quantity)
	↓
Check nếu còn < minimum → Cảnh báo
```

### 4. **Kiểm Tra Hàng Hết Hạn**
```
Inventory.Product.ExpiryDate
	↓
IsNearExpiry() → True nếu < 30 ngày
	↓
IsExpired() → True nếu quá hạn
	↓
Status = "Hết hạn" hoặc từng báo
```

### 5. **Phân Quyền**
```
User.Role.Permissions
	↓
Check Permission (Module, Action)
	↓
IsGranted == true? → Allow | Deny
```

---

## 📋 UNIQUE CONSTRAINTS

| Table | Field(s) | Constraint |
|-------|----------|-----------|
| Product | ProductCode | UNIQUE |
| Supplier | SupplierCode | UNIQUE (nullable) |
| ImportOrder | ImportCode | UNIQUE |
| Sale | SaleCode | UNIQUE |
| Invoice | InvoiceNumber | UNIQUE |
| Prescription | PrescriptionCode | UNIQUE |
| Permission | RoleId + ModuleName + Action | UNIQUE |

---

## 🔍 KEY INDEXES

- Product: ProductCode (UNIQUE), ProductName, SupplierId
- Inventory: ProductId, Quantity
- ImportOrder: ImportCode (UNIQUE), ImportDate
- Sale: SaleCode (UNIQUE), SaleDate, CustomerId
- Invoice: InvoiceNumber (UNIQUE), InvoiceDate
- Prescription: PrescriptionCode (UNIQUE), PrescriptionDate, CustomerId
- Permission: RoleId + ModuleName + Action (UNIQUE)

---

**This ER Diagram represents a complete clinic management system with product, inventory, import, sales, invoice, and prescription management.** ✅
