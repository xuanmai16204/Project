## PROMPT TỔNG HỢP CẤU TRÚC DỰ ÁN - WEBSITE ĐIỆN NHÀ

Hãy xây dựng một hệ thống WebsiteDienNha_DoAnChuyenNganh hoàn chỉnh theo yêu cầu sau. Mục tiêu là triển khai một website bán hàng thiết bị điện gia dụng/chung cư với đầy đủ tính năng quản trị, bán hàng, vận chuyển, marketing, loyalty, và bảo mật. Dự án dùng ASP.NET Core MVC, EF Core, phân chia multi-areas (Admin/Employee/Customer), áp dụng Repository + Service Layer, và có tài liệu/SQL scripts đầy đủ để triển khai nhanh.

---

### 1) Thông tin dự án
- Tên dự án: WebsiteDienNha_DoAnChuyenNganh
- Công nghệ: ASP.NET Core MVC (.NET 8.0)
- Database: SQL Server (Entity Framework Core 8)
- Mô hình: Multi-area application (Admin, Employee, Customer)
- Xác thực/Phân quyền: ASP.NET Core Identity (ApplicationUser), Role-based + Permissions

---

### 2) Cấu trúc thư mục

Tạo cấu trúc sau với số lượng controller/view tương tự mô tả, có thể scaffold từng phần sau khi có DbContext/Migrations.

#### Areas/

1. Areas/Admin/
   - Controllers/ (20):
     - CategoryController.cs
     - ChatbotController.cs
     - CustomerController.cs
     - DiscountCodeController.cs
     - EmailController.cs
     - EmployeeController.cs
     - ExportController.cs
     - InventoryController.cs
     - LogController.cs
     - LogsController.cs
     - NotificationController.cs
     - OrderController.cs
     - PerformanceController.cs
     - ProductController.cs
     - PromotionController.cs
     - ReportsController.cs
     - RoleController.cs
     - SecurityController.cs
     - SettingsController.cs
     - StatisticsController.cs
   - Views/ (khoảng 70+ file), gồm các thư mục: Categories, Chatbot, Customer, DiscountCode, Email, Employee, Export, Inventory, Log, Logs, Notification, Order, Product, Promotion, Reports, Role, Security, Settings, Statistics, Shared (có _LayoutAdmin, _LoginPartial, _ValidationScriptsPartial, Error, _CartCountPartial)

2. Areas/Customer/
   - Controllers/:
     - NotificationController.cs
   - Views/:
     - Home/ (Index, Privacy)
     - ShoppingCart/ (Index, Checkout, ConfirmPayment, OrderCompleted)
     - Notification/ (Index)

3. Areas/Employee/
   - Controllers/ (8): CategoryController.cs, CustomerController.cs, DashboardController.cs, DiscountCodeController.cs, NotificationController.cs, OrderController.cs, ProductController.cs, StatisticsController.cs
   - Views/ (khoảng 25 file): Categories, Customer, Dashboard, DiscountCode, Notification, Order, Product, Shared (_EmployeeLayout, _EmployeeLayout.css, _LoginPartial, _ValidationScriptsPartial, _CartCountPartial, Error), Statistics

4. Areas/Identity/
   - Pages/Account/ (Login, Logout, Register + code-behind)

#### Controllers/ (root)
- AccountController.cs, AddressController.cs, AdminLoginController.cs, ApiController.cs, BaseController.cs, ChatbotController.cs, ClearSessionController.cs, CustomerFeedbackController.cs, DiscountCodeController.cs, GameController.cs, GuestNotificationController.cs, HomeController.cs, NotificationController.cs, OrderController.cs, OrderTrackingController.cs, PaymentController.cs (VNPay), ProductController.cs, ProfileController.cs, PromotionController.cs, ServiceController.cs, ShippingController.cs, ShoppingCartController.cs

#### Models/
- Cốt lõi (đã có): ApplicationUser, Category, Product, Cart, CartItem, Order, OrderItem
- Bổ sung để hoàn chỉnh (28+ models):
  - ProductImage, DiscountCode, RewardPoints
  - LoyaltyLevel (10 cấp), GameResult, Notification, ShippingInfo, CustomerFeedback, Promotion
  - EmailCampaign, EmailTemplate, EmailRecipient
  - SystemNotification, NotificationRecipient, GuestNotification
  - ChatConversation, ChatMessage
  - Employee, EmployeePerformance, Role, Permission, RolePermission, UserRole
  - SystemLog, ExportReport, ErrorViewModel, SD (static roles/constants)

#### Repositories/ (14 repos)
- EFCategoryRepository, ProductRepository, OrderRepository, DiscountCodeRepository, StatisticsRepository, CustomerFeedbackRepository, PromotionRepository, EmailMarketingRepository, NotificationRepository, ExportReportRepository, EmployeeRepository, RoleRepository, EmployeePerformanceRepository, SystemLogRepository

#### IRepository/ (17 interfaces)
- ICategoryRepository, IProductRepository, IOrderRepository, IDiscountCodeRepository, IStatisticsRepository, ICustomerFeedbackRepository, IPromotionRepository, IEmailMarketingRepository, INotificationRepository, IExportReportRepository, IEmployeeRepository, IRoleRepository, IEmployeePerformanceRepository, ISystemLogRepository, IChatbotService, IShippingService, InvoiceGenerator

#### Services/ (8 services)
- LoyaltyService, RecommendationService, EmailService, NotificationService, LayoutService, ChatbotService, ShippingService, LoggingService

#### DTO/
- DTO chính: EmployeePerformanceDTO, OrderStatisticDTO, PagedResult, RevenueByDateDTO, SystemLogDTO, TopProductDTO
- DTO/Chart/: DataPoint
- DTO/Shipping/: CreateShippingOrderRequest, OrderTrackingRequest, OrderTrackingResponse, ShippingCalculateRequest, ShippingCalculateResponse
- DTO/Shipping/GHN/: GHNCalculateFeeRequest, GHNCalculateFeeResponse, GHNCreateOrderRequest, GHNCreateOrderResponse
- DTO/Shipping/GoogleMaps/: DistanceMatrixResponse, GeocodingResponse

#### Views/
- Account/, AdminLogin/, CustomerFeedback/, Game/, GuestNotification/, Home/ (Dashboard, Index, Privacy), Order/ (BankTransferInstructions, Completed, Details, History, Index, Track), OrderTracking/ (Index, TrackingResult), Product/ (Details, Index, Order), Profile/ (Address, Edit, Index, Loyalty, Orders), Promotion/ (Game, Index, Loyalty), Service/ (Delivery), Shipping/ (Track), ShoppingCart/ (Checkout, ConfirmPayment, Index, OrderCompleted), Shared/ (_CartCountPartial, _EmployeeLayout, _Header, _Layout, _Layout.css, _LayoutAdmin, _LayoutCustomer, _LayoutGuest, _LoginPartial, _ValidationScriptsPartial, Error)

#### wwwroot/
- css/: cart-fix.css, enhanced-cart.css, enhanced-home.css, chatbot.css, header.css, site.css
- js/: chatbot.js, header.js, realtime-update.js, site.js
- lib/: bootstrap, jquery, jquery-validation, jquery-validation-unobtrusive
- Images/: ảnh sản phẩm, logo/banner
- exports/: báo cáo xuất (PDF, Excel)

#### Config/
- VNPaySetting.cs

#### Context/
- ApplicationDbContext.cs (Identity, model configs, relationships, seed LoyaltyLevels, 28+ DbSets)

#### Extensions/
- SessionExtensions.cs

#### Middleware/
- LoggingMiddleware.cs

#### Utils/
- PayLib.cs (VNPay)

#### Migrations/
- Các file migrations (InitialCreate, UpdateDatabase, Snapshot)

#### Scripts/
- SQL: CheckAdminEmployeeAccounts.sql, CheckCategoryData.sql, CleanData.sql, CreateAdminEmployeeUsers.sql, DeleteAndRecreateAdmin.sql, DropTables.sql, SeedData_Fixed.sql, TEST_SHIPPING_SAMPLE_DATA.sql, UpdateCategoryId.sql
- Docs: DISCOUNT_CODES_SUMMARY.md, README_CREATE_ADMIN.md, README_SEED_DATA.md, TROUBLESHOOTING.md, UNSPLASH_IMAGES_SUMMARY.md

#### Root Files
- appsettings.json, appsettings.Development.json, Program.cs, WebsiteDienNha-DoAnChuyenNganh.csproj, Properties/launchSettings.json
- Documentation: ADMIN_LOGIN_GUIDE.md, CHATBOT_IMPLEMENTATION_SUMMARY.md, CHATBOT_README.md, FIX_AND_RUN.md, GOOGLE_MAPS_SHIPPING_INTEGRATION_GUIDE.md, HOW_TO_GET_API_KEYS.md, IMPLEMENTATION_SUMMARY.md, IMPLEMENTATION_SUMMARY_SHIPPING.md, MIGRATION_GUIDE_LOYALTY_POINTS.md, QUICK_START_ADMIN.md, QUICK_START_SHIPPING.md, README_ENHANCED_FEATURES.md, README_SHIPPING_TRACKING.md

---

### 3) Tính năng chính
1. Quản lý sản phẩm điện nhà: danh mục, sản phẩm, tồn kho, hình ảnh
2. Đơn hàng: tạo đơn, thanh toán VNPay, theo dõi, trạng thái
3. Loyalty 10 cấp: Seed → Brew → Roast → Aroma → Steam → Pour → Sip → Blend → Barista → Master (có thể đổi tên theo chủ đề điện nhà nếu cần)
4. Game & Gợi ý: minigame tích điểm, recommendation sản phẩm
5. Thông báo: email, real-time polling, notification center từng role, khách vãng lai
6. Vận chuyển & Tracking: mã vận đơn, timeline, Google Maps API, GHN API
7. Email marketing: chiến dịch, template, gửi hàng loạt, theo dõi người nhận
8. Nhân viên: CRUD, hiệu suất, phân quyền, dashboard
9. Phân quyền & Bảo mật: Roles/Permissions, System logging, Security controller
10. Báo cáo & Thống kê: doanh thu, bán hàng, khách hàng, export PDF/Excel, dashboard
11. Chatbot: hỗ trợ khách hàng, lưu lịch sử
12. Phản hồi khách hàng: tạo/xem/quản trị
13. Mã giảm giá & Khuyến mãi: CRUD, sử dụng mã, khuyến mãi theo nhóm sản phẩm
14. Layout động theo Role: Guest/Customer/Employee/Admin

---

### 4) Kiến trúc hệ thống
- MVC Pattern, Repository Pattern, Service Layer, DTO Pattern, DI
- SQL Server + EF Core 8 + Migrations
- Identity + Role-based Authorization + Custom Permissions
- VNPay payment, Google Maps API, GHN API
- Frontend: Bootstrap 5, jQuery, AJAX polling, responsive

---

### 5) Database schema (mô tả bảng chính)
- Core: AspNetUsers, AspNetRoles, Products, Categories, Orders, OrderDetails, ShoppingCarts, CartItems, DiscountCodes, RewardPoints
- Enhanced: LoyaltyLevels, GameResults, Notifications, ShippingInfos, CustomerFeedbacks, Promotions, PromotionUsages
- Email Marketing: EmailCampaigns, EmailTemplates, EmailRecipients
- Administration: Employees, Roles, Permissions, RolePermissions, UserRoles, EmployeePerformances, SystemLogs, ExportReports, SystemNotifications, NotificationRecipients
- Chatbot: ChatConversations, ChatMessages
- Guest: GuestNotifications

---

### 6) Cấu hình & phụ thuộc
- .NET 8.0 SDK
- NuGet (phiên bản 8.x):
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore
  - Microsoft.AspNetCore.Identity.UI
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.SqlServer
  - Microsoft.EntityFrameworkCore.Tools
  - QuestPDF (tạo PDF báo cáo) – phiên bản ổn định gần nhất
- appsettings.json: ConnectionStrings, VNPay, Email SMTP
- Program.cs: đăng ký DbContext, Identity, Repositories, Services, Routing (Areas + default)
- Areas routing:
  - /Admin/{controller}/{action}
  - /Employee/{controller}/{action}
  - /Customer/{controller}/{action}
  - Default: /{controller}/{action}

---

### 7) Deploy & chạy
1) Database:
```
dotnet ef migrations add InitialCreate
dotnet ef database update
```
2) Chạy ứng dụng:
```
dotnet run
```
3) Điểm truy cập:
- Home: /
- Admin: /Admin/Statistics
- Employee: /Employee/Product
- Customer: /Customer/Notification
- Login: /Identity/Account/Login

---

### 8) Security & Performance
- RBAC, input validation, EF chống SQL injection, XSS/CSRF protection, session management, logging/auditing
- Tối ưu EF Core query, lazy-loading ảnh, AJAX polling với debounce, caching, phân trang, minified CSS/JS

---

### 9) UI/UX
- Thiết kế hiện đại/đáp ứng, màu chủ đạo phù hợp thương hiệu thiết bị điện nhà (ví dụ: xanh dương tin cậy + cam nổi bật)
- Bootstrap Icons, card-based layouts, timeline tracking, real-time updates

---

### 10) Yêu cầu thực thi
- Tạo đầy đủ skeleton theo cấu trúc ở trên (areas, controllers, views, models, repositories, services, DTO, wwwroot, config, extensions, middleware, utils, scripts, docs)
- Hoàn thiện DbContext với DbSet, Fluent API, seed LoyaltyLevels
- Áp dụng Repository + Service Layer cho các nghiệp vụ chính (sản phẩm, đơn hàng, khuyến mãi, thống kê, thông báo, email, nhân viên, logs)
- Tạo migrations và cập nhật DB; cung cấp scripts hỗ trợ kiểm tra/seed dữ liệu
- Scaffold các trang CRUD chính (Category, Product, DiscountCode, Promotion, Employee, Role, Order)
- Cấu hình VNPay, Google Maps, GHN (mock/test keys ở môi trường dev), Email SMTP

---

### 11) Ghi chú
- Mặc định dùng .NET 8.0 (đồng bộ với project hiện tại)
- Có thể kế thừa UI/UX và luồng nghiệp vụ từ phiên bản coffee shop nhưng đổi ngữ cảnh sang thiết bị điện nhà
- Ưu tiên viết code rõ ràng, tách lớp, có thể scaffold trước rồi tinh chỉnh



---

### 12) Checklist triển khai (theo thứ tự)
1. Cấu hình NuGet + Packages (.NET 8): Identity, Identity.UI, EFCore, EFCore.SqlServer, EFCore.Tools, CodeGeneration.Design, QuestPDF
2. Khởi tạo DbContext, `ApplicationUser`, models cốt lõi và Fluent API (đã có)
3. Tạo migrations đầu tiên và cập nhật DB
4. Seed data: Roles, Admin user, LoyaltyLevels, Categories, Products mẫu
5. Scaffold Areas + Controllers + Views (đã có skeleton); mở rộng CRUD các module chính
6. Áp dụng Repository + Service Layer cho nghiệp vụ sản phẩm/đơn hàng/khuyến mãi/thống kê
7. Cấu hình VNPay, Email SMTP, GHN, Google Maps (dev keys)
8. Thêm Middleware logging, Session extensions, cấu hình cache/static files
9. Viết DTO và endpoints/Actions trả về DTO cho thống kê/báo cáo
10. UI/UX: hoàn thiện layout theo role, partials, CSS/JS, AJAX polling
11. Kiểm thử smoke test và hardening bảo mật

---

### 13) Roles & Permissions (SD constants)
- Roles: `Admin`, `Employee`, `Customer`
- Quyền (gợi ý): `ManageProducts`, `ManageOrders`, `ManagePromotions`, `ManageDiscountCodes`, `ViewReports`, `ManageEmployees`, `ManageRoles`, `ManagePermissions`, `ManageNotifications`, `ManageInventory`, `ExportReports`
- Mapping gợi ý:
  - Admin: tất cả quyền
  - Employee: `ManageProducts`, `ManageOrders`, `ViewReports`, `ManageInventory`
  - Customer: không có quyền quản trị, chỉ thao tác mua hàng và hồ sơ

---

### 14) Kế hoạch Seed Data
- Roles: Admin/Employee/Customer
- User admin mặc định: email `admin@diennha.local`, mật khẩu mạnh (đặt trong secrets); gán role Admin
- LoyaltyLevels (10 cấp): Seed → Brew → Roast → Aroma → Steam → Pour → Sip → Blend → Barista → Master, kèm phần trăm ưu đãi 0 → 20%
- Categories mẫu: Công tắc, Ổ cắm, Đèn LED, Cáp điện, Thiết bị bảo vệ
- Products mẫu: 2-3 sản phẩm cho mỗi danh mục với giá/stock/hình ảnh

---

### 15) Cấu hình appsettings mẫu
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=THANHTRUCNGUYEN;Database=WebsiteDienNha_DoAnChuyenNganh;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "VNPay": {
    "TmnCode": "your_tmn",
    "HashSecret": "your_secret",
    "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "ReturnUrl": "https://localhost:5001/payment/return"
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "User": "your_email@gmail.com",
    "Password": "app_password",
    "EnableSsl": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

### 16) Lệnh scaffolding và migrations (tham khảo)
```bash
# Cài packages cần thiết
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.10
dotnet add package Microsoft.AspNetCore.Identity.UI --version 8.0.10
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.10
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.10
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 8.0.5

# Clear cache + restore nếu lỗi
dotnet nuget locals all --clear
dotnet restore

# Migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

# Scaffold CRUD ví dụ (Category)
dotnet-aspnet-codegenerator controller \
  -name CategoryController \
  -m WebsiteDienNha_DoAnChuyenNganh.Models.Category \
  -dc WebsiteDienNha_DoAnChuyenNganh.Data.ApplicationDbContext \
  -outDir Areas/Admin/Controllers \
  -udl -f
```

---

### 17) Đăng ký DI (nhắc lại)
- `ICategoryRepository` → `EFCategoryRepository`
- `IProductRepository` → `ProductRepository`
- `IOrderRepository` → `OrderRepository`
- Services: `LoyaltyService`, `EmailService`, `NotificationService`
- `Configure<VNPaySetting>` với section `VNPay`

---

### 18) Quy ước migrations & đặt tên
- Mỗi thay đổi lớn: `Add_`, `Update_`, `Rename_`, `Seed_`
- Ví dụ: `Add_DiscountCodes`, `Update_Order_AddShippingInfo`, `Seed_LoyaltyLevels`

---

### 19) Smoke test sau build
- Truy cập `/Admin/Category`, `/Admin/Product`, `/Admin/Order` thấy trang Index
- `/Employee/Dashboard` hoạt động
- `/Customer/Notification` hoạt động
- Đăng ký/Đăng nhập Identity (sau khi scaffold Identity UI nếu cần)
- Tạo danh mục/sản phẩm (sau scaffold CRUD + migrations)

---

### 20) Công việc tương lai (Future Work)
- Tích hợp VNPay thực: tạo URL thanh toán, xác minh checksum, cập nhật trạng thái Order
- GHN + Google Maps: tính phí, tạo vận đơn, tracking, hiển thị timeline
- Email marketing: template, campaign, recipients, theo dõi mở/nhấp (basic)
- Dashboard phân quyền theo role với biểu đồ doanh thu, top sản phẩm
- Báo cáo PDF/Excel (QuestPDF + ClosedXML nếu cần)
