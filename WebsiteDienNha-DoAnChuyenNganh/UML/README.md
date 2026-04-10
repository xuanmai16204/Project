# UML Diagrams - Website Điện Nhà

Thư mục này chứa các sơ đồ UML cho hệ thống Website Điện Nhà, bao gồm Use Case Diagram và Sequence Diagrams được chia theo từng role.

## Cấu trúc thư mục

```
UML/
├── UseCaseDiagram.puml              # Use Case Diagram tổng thể
├── ClassDiagram_Activity.puml       # Class Diagram: Activity Model
├── ERD_Activity.puml                 # ERD: Activity Model và relationships
├── ERD_Activity_Detailed.puml        # ERD chi tiết: Activity với đầy đủ SQL types
├── ERD_Full_Database.puml            # ERD đầy đủ: Tất cả các table trong hệ thống
├── SequenceDiagrams/
│   ├── Customer_Register.puml        # Customer: Đăng ký tài khoản
│   ├── Customer_Login.puml          # Customer: Đăng nhập
│   ├── Customer_AddToCart.puml       # Customer: Thêm sản phẩm vào giỏ
│   ├── Customer_Checkout.puml        # Customer: Thanh toán đơn hàng
│   ├── Customer_PaymentVietQR.puml  # Customer: Thanh toán VietQR
│   ├── Customer_Chatbot.puml         # Customer: Chat với chatbot
│   ├── Admin_ManageProduct.puml      # Admin: Quản lý sản phẩm
│   ├── Admin_ManageOrder.puml        # Admin: Quản lý đơn hàng
│   ├── Admin_Dashboard.puml          # Admin: Xem dashboard
│   └── Employee_Dashboard.puml       # Employee: Xem dashboard
└── README.md                          # File này
```

## Cách sử dụng

### 1. Cài đặt PlantUML

#### Option 1: Sử dụng VS Code Extension
1. Mở VS Code
2. Cài đặt extension "PlantUML" (by jebbs)
3. Mở file `.puml` và nhấn `Alt + D` để preview

#### Option 2: Sử dụng PlantUML Server Online
1. Truy cập: http://www.plantuml.com/plantuml/uml/
2. Copy nội dung file `.puml`
3. Paste vào editor và xem kết quả

#### Option 3: Cài đặt PlantUML locally
1. Tải Java: https://www.java.com/
2. Tải PlantUML jar: http://plantuml.com/download
3. Chạy: `java -jar plantuml.jar file.puml`

### 2. Xem các diagram

#### Use Case Diagram
- File: `UseCaseDiagram.puml`
- Mô tả: Tổng quan các use case của hệ thống, chia theo 3 roles: Customer, Admin, Employee

#### Class Diagrams
- `ClassDiagram_Activity.puml`: Class diagram cho Activity model, mô tả cấu trúc và mối quan hệ với ApplicationUser

#### ERD (Entity Relationship Diagrams)
- `ERD_Activity_Detailed.puml`: ERD chi tiết cho Activity model (PlantUML) với đầy đủ kiểu dữ liệu SQL, ràng buộc (NOT NULL, NULL, DEFAULT), indexes clustered, foreign key constraints và ON DELETE behavior
- `ERD_Activity_Detailed.mmd`: ERD chi tiết cho Activity model (Mermaid) với đầy đủ kiểu dữ liệu SQL, ràng buộc và mối quan hệ

#### Sequence Diagrams

**Customer:**
- `Customer_Register.puml`: Quy trình đăng ký với OTP
- `Customer_Login.puml`: Quy trình đăng nhập
- `Customer_AddToCart.puml`: Quy trình thêm sản phẩm vào giỏ hàng
- `Customer_Checkout.puml`: Quy trình thanh toán đơn hàng
- `Customer_PaymentVietQR.puml`: Quy trình thanh toán qua VietQR
- `Customer_Chatbot.puml`: Quy trình tương tác với chatbot

**Admin:**
- `Admin_ManageProduct.puml`: Quy trình quản lý sản phẩm (CRUD)
- `Admin_ManageOrder.puml`: Quy trình quản lý đơn hàng
- `Admin_Dashboard.puml`: Quy trình xem dashboard và thống kê

**Employee:**
- `Employee_Dashboard.puml`: Quy trình xem dashboard nhân viên

## Các thành phần chính trong diagrams

### Actors (Người dùng)
- **Customer**: Khách hàng mua hàng
- **Admin**: Quản trị viên hệ thống
- **Employee**: Nhân viên

### Controllers
- `AccountController`: Xử lý đăng nhập/đăng ký
- `ShoppingCartController`: Xử lý giỏ hàng
- `PaymentController`: Xử lý thanh toán
- `ProductController`: Xử lý sản phẩm
- `OrderController`: Xử lý đơn hàng
- `ChatbotController`: Xử lý chatbot

### Services
- `CartService`: Quản lý giỏ hàng (session)
- `OtpService`: Quản lý mã OTP
- `SmsService`: Gửi SMS
- `VietQRService`: Tạo QR code thanh toán
- `ChatbotService`: Xử lý logic chatbot

### Repositories
- `ProductRepository`: Truy vấn sản phẩm
- `OrderRepository`: Truy vấn đơn hàng
- `CategoryRepository`: Truy vấn danh mục

## Lưu ý

- Tất cả các diagram sử dụng định dạng PlantUML
- Các diagram có thể được export sang PNG, SVG, PDF
- Có thể chỉnh sửa các diagram để phù hợp với thay đổi trong code
- Sequence diagrams mô tả luồng xử lý chi tiết, bao gồm cả database interactions

## Cập nhật

Khi có thay đổi trong code, vui lòng cập nhật các diagram tương ứng để đảm bảo tính nhất quán.

