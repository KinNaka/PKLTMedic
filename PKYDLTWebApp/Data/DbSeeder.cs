using ClinicManagement.Models;

namespace ClinicManagement.Data
{
    /// <summary>
    /// Khởi tạo dữ liệu mẫu cho ứng dụng
    /// Chạy một lần khi ứng dụng khởi động
    /// </summary>
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Database.CanConnect())
            {
                return;
            }

            // ============ 1. ROLES ============
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { Name = "Admin", Description = "Quản trị hệ thống - toàn quyền" },
                    new Role { Name = "Nhân viên", Description = "Nhân viên phòng khám" }
                );
                context.SaveChanges();
            }

            // ============ 2. ADMIN USER ============
            // Luôn đảm bảo có tài khoản admin để đăng nhập
            var adminRole = context.Roles.First(r => r.Name == "Admin");
            var adminUser = context.Users.FirstOrDefault(u => u.Username == "admin");

            if (adminUser == null)
            {
                // Chưa có tài khoản admin → tạo mới
                context.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FullName = "Quản trị viên",
                    Email = "admin@clinic.com",
                    Phone = "0900000000",
                    IsActive = true,
                    RoleId = adminRole.Id
                });
                context.SaveChanges();
            }
            else
            {
                // Đã có tài khoản admin → đặt lại mật khẩu mặc định
                adminUser.RoleId = adminRole.Id;
                adminUser.IsActive = true;
                adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
                context.SaveChanges();
            }

            // ============ 3. SUPPLIERS ============
            if (!context.Suppliers.Any())
            {
                context.Suppliers.AddRange(
                    new Supplier
                    {
                        SupplierCode = "NCC001",
                        SupplierName = "Công ty Dược phẩm Việt Nam",
                        ContactPerson = "Nguyễn Văn A",
                        Phone = "0912345678",
                        Email = "contact@duocvietnam.vn",
                        Address = "123 Lê Lợi, Quận 1, TP.HCM"
                    },
                    new Supplier
                    {
                        SupplierCode = "NCC002",
                        SupplierName = "Nhà phân phối Y tế An Khang",
                        ContactPerson = "Trần Thị B",
                        Phone = "0987654321",
                        Email = "info@ankhang.com",
                        Address = "456 Nguyễn Trãi, Quận 5, TP.HCM"
                    },
                    new Supplier
                    {
                        SupplierCode = "NCC003",
                        SupplierName = "Công ty Y tế Phương Đông",
                        ContactPerson = "Lê Văn C",
                        Phone = "0934567890",
                        Email = "sale@phuongdong.vn",
                        Address = "789 Cộng Hòa, Tân Bình, TP.HCM"
                    }
                );
                context.SaveChanges();
            }

            // ============ 4. PRODUCTS & INVENTORY ============
            if (!context.Products.Any())
            {
                var sup1 = context.Suppliers.First(s => s.SupplierCode == "NCC001");
                var sup2 = context.Suppliers.First(s => s.SupplierCode == "NCC002");
                var sup3 = context.Suppliers.First(s => s.SupplierCode == "NCC003");

                var products = new List<Product>
                {
                    new Product
                    {
                        ProductCode = "THUOC001",
                        ProductName = "Paracetamol 500mg",
                        Category = "Thuốc",
                        Unit = "Vỉ",
                        Strength = "500mg",
                        CostPrice = 5000,
                        RetailPrice = 12000,
                        WholesalePrice = 9000,
                        SupplierId = sup1.Id,
                        ExpiryDate = DateTime.Now.AddMonths(12),
                        IsActive = true
                    },
                    new Product
                    {
                        ProductCode = "THUOC002",
                        ProductName = "Amoxicillin 500mg",
                        Category = "Thuốc",
                        Unit = "Vỉ",
                        Strength = "500mg",
                        CostPrice = 8000,
                        RetailPrice = 20000,
                        WholesalePrice = 15000,
                        SupplierId = sup1.Id,
                        ExpiryDate = DateTime.Now.AddMonths(18),
                        IsActive = true
                    },
                    new Product
                    {
                        ProductCode = "THUOC003",
                        ProductName = "Vitamin C 1000mg",
                        Category = "Thực phẩm chức năng",
                        Unit = "Lọ",
                        Strength = "1000mg",
                        CostPrice = 30000,
                        RetailPrice = 65000,
                        WholesalePrice = 50000,
                        SupplierId = sup2.Id,
                        ExpiryDate = DateTime.Now.AddMonths(24),
                        IsActive = true
                    },
                    new Product
                    {
                        ProductCode = "THUOC004",
                        ProductName = "Đan sâm hoàn",
                        Category = "Đông y",
                        Unit = "Hộp",
                        Strength = "100 viên",
                        CostPrice = 40000,
                        RetailPrice = 85000,
                        SupplierId = sup3.Id,
                        ExpiryDate = DateTime.Now.AddMonths(9),
                        IsActive = true
                    },
                    new Product
                    {
                        ProductCode = "VTYT001",
                        ProductName = "Khẩu trang y tế 4 lớp",
                        Category = "Vật tư y tế",
                        Unit = "Hộp",
                        CostPrice = 15000,
                        RetailPrice = 30000,
                        SupplierId = sup2.Id,
                        ExpiryDate = null,
                        IsActive = true
                    }
                };

                context.Products.AddRange(products);
                context.SaveChanges();

                // ============ INVENTORY (tự động tạo cho từng sản phẩm) ============
                if (!context.Inventories.Any())
                {
                    context.Inventories.AddRange(
                        new Inventory
                        {
                            ProductId = products[0].Id,
                            Quantity = 150,
                            MinimumQuantity = 30,
                            MaximumQuantity = 500,
                            WarehouseLocation = "Kệ A1"
                        },
                        new Inventory
                        {
                            ProductId = products[1].Id,
                            Quantity = 80,
                            MinimumQuantity = 30,
                            MaximumQuantity = 300,
                            WarehouseLocation = "Kệ A2"
                        },
                        new Inventory
                        {
                            ProductId = products[2].Id,
                            Quantity = 25,
                            MinimumQuantity = 20,
                            MaximumQuantity = 200,
                            WarehouseLocation = "Kệ B1"
                        },
                        new Inventory
                        {
                            ProductId = products[3].Id,
                            Quantity = 10,
                            MinimumQuantity = 30,
                            MaximumQuantity = 100,
                            WarehouseLocation = "Kệ B2"
                        },
                        new Inventory
                        {
                            ProductId = products[4].Id,
                            Quantity = 200,
                            MinimumQuantity = 50,
                            MaximumQuantity = 800,
                            WarehouseLocation = "Kệ C1"
                        }
                    );
                    context.SaveChanges();
                }
            }

            // ============ 5. CUSTOMERS (khách hàng mẫu) ============
            if (!context.Customers.Any())
            {
                context.Customers.AddRange(
                    new Customer
                    {
                        CustomerCode = "KH001",
                        FullName = "Nguyễn Thị Hồng",
                        DateOfBirth = new DateTime(1990, 5, 15),
                        Gender = "Nữ",
                        Phone = "0901112222",
                        Address = "12 Nguyễn Huệ, Quận 1, TP.HCM"
                    },
                    new Customer
                    {
                        CustomerCode = "KH002",
                        FullName = "Trần Văn Minh",
                        DateOfBirth = new DateTime(1985, 8, 20),
                        Gender = "Nam",
                        Phone = "0913334444",
                        Address = "34 Lê Văn Sĩ, Quận 3, TP.HCM"
                    },
                    new Customer
                    {
                        CustomerCode = "KH003",
                        FullName = "Lê Thị Mai",
                        DateOfBirth = new DateTime(1995, 12, 1),
                        Gender = "Nữ",
                        Phone = "0925556666",
                        Address = "56 Ngô Quyền, Quận 10, TP.HCM"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}