# Script SQL - Insert Dữ Liệu Mẫu

## Mô tả
Script SQL này chèn dữ liệu thực tế vào database cho hệ thống Website Điện Nhà, bao gồm:
- **Categories**: 10 danh mục sản phẩm
- **Products**: 48 sản phẩm điện dân dụng và công nghiệp
- **DiscountCodes**: 10 mã giảm giá

## Cách sử dụng

### 1. Mở SQL Server Management Studio (SSMS)

### 2. Kết nối đến database
- Server: `THANHTRUCNGUYEN` (hoặc server của bạn)
- Database: `WebsiteDienNha_DoAnChuyenNganh`

### 3. Mở file script
- Mở file `InsertSampleData.sql` trong SSMS

### 4. Chạy script
- Nhấn `F5` hoặc click nút `Execute` để chạy toàn bộ script
- Hoặc chọn từng phần và chạy riêng lẻ

## Cấu trúc dữ liệu

### Categories (10 danh mục)
1. Công tắc
2. Ổ cắm điện
3. Đèn LED
4. Dây điện
5. Thiết bị đóng cắt
6. Ổn áp - Biến áp
7. Thiết bị chiếu sáng
8. Phụ kiện điện
9. Quạt điện
10. Thiết bị an toàn điện

### Products (48 sản phẩm)
- **Công tắc**: 5 sản phẩm (Panasonic, Schneider, Xiaomi)
- **Ổ cắm điện**: 5 sản phẩm (2 chấu, 3 chấu, USB, đa năng)
- **Đèn LED**: 6 sản phẩm (bulb, tube, downlight, panel, dây LED)
- **Dây điện**: 5 sản phẩm (đơn, đôi, 3 lõi, 4 lõi, cáp ngầm)
- **Thiết bị đóng cắt**: 5 sản phẩm (CB, ELCB, Contactor)
- **Ổn áp - Biến áp**: 4 sản phẩm (1KVA, 2KVA, 5KVA, biến áp)
- **Thiết bị chiếu sáng**: 4 sản phẩm (chùm, treo tường, âm trần, sân vườn)
- **Phụ kiện điện**: 5 sản phẩm (băng keo, kẹp cáp, ống luồn, đầu cốt)
- **Quạt điện**: 4 sản phẩm (trần, treo tường, cây, hộp)
- **Thiết bị an toàn**: 5 sản phẩm (găng tay, ủng, thảm, bút thử, kìm)

### DiscountCodes (10 mã giảm giá)
1. **WELCOME10**: Giảm 10% - Đơn từ 200.000đ
2. **SAVE50K**: Giảm 50.000đ - Đơn từ 500.000đ
3. **SUMMER20**: Giảm 20% - Đơn từ 300.000đ (giới hạn 1000 lượt)
4. **VIP15**: Giảm 15% - Đơn từ 1.000.000đ
5. **NEW100K**: Giảm 100.000đ - Đơn từ 1.000.000đ (giới hạn 500 lượt)
6. **FLASH30**: Giảm 30% - Đơn từ 500.000đ (24h, giới hạn 100 lượt)
7. **FREESHIP**: Miễn phí ship 50.000đ - Đơn từ 500.000đ
8. **BIRTHDAY**: Giảm 25% - Đơn từ 200.000đ
9. **LUCKY5**: Giảm 5% - Đơn từ 100.000đ
10. **BIG200K**: Giảm 200.000đ - Đơn từ 2.000.000đ (giới hạn 200 lượt)

## Lưu ý

### Reset dữ liệu
Nếu muốn xóa dữ liệu cũ trước khi insert, bỏ comment phần:
```sql
DELETE FROM [OrderItems]
DELETE FROM [Orders]
DELETE FROM [CartItems]
DELETE FROM [Products]
DELETE FROM [Categories]
DELETE FROM [DiscountCodes]
```

**CẢNH BÁO**: Chỉ chạy phần DELETE khi thực sự cần reset dữ liệu!

### Identity Insert
Script sử dụng `SET IDENTITY_INSERT ON/OFF` để có thể chỉ định ID cụ thể cho các bản ghi. Điều này giúp:
- Dữ liệu nhất quán giữa các lần chạy
- Dễ dàng tham chiếu trong code/test

### Giá cả
Tất cả giá sản phẩm được tính bằng **VND (Việt Nam Đồng)**.

### Hình ảnh
Các đường dẫn hình ảnh (`ImageUrl`) là đường dẫn tương đối, bạn cần:
- Upload hình ảnh vào thư mục `wwwroot/images/products/`
- Hoặc cập nhật URL sau khi insert

## Kiểm tra sau khi chạy

Sau khi chạy script, kiểm tra:
```sql
-- Đếm số lượng
SELECT COUNT(*) AS TotalCategories FROM [Categories];
SELECT COUNT(*) AS TotalProducts FROM [Products];
SELECT COUNT(*) AS TotalDiscountCodes FROM [DiscountCodes];

-- Xem chi tiết
SELECT * FROM [Categories];
SELECT * FROM [Products];
SELECT * FROM [DiscountCodes];
```

## Troubleshooting

### Lỗi: "Cannot insert explicit value for identity column"
- **Nguyên nhân**: Bảng đã có dữ liệu và IDENTITY_INSERT chưa được bật
- **Giải pháp**: Đảm bảo chạy `SET IDENTITY_INSERT [TableName] ON;` trước khi INSERT

### Lỗi: "Foreign key constraint"
- **Nguyên nhân**: Sản phẩm tham chiếu CategoryId không tồn tại
- **Giải pháp**: Đảm bảo chạy INSERT Categories trước Products

### Lỗi: "String or binary data would be truncated"
- **Nguyên nhân**: Dữ liệu vượt quá độ dài cho phép của cột
- **Giải pháp**: Kiểm tra lại độ dài dữ liệu trong script

## Cập nhật dữ liệu

Để thêm/sửa dữ liệu sau này:
1. Thêm sản phẩm mới: INSERT vào [Products] với CategoryId hợp lệ
2. Thêm mã giảm giá: INSERT vào [DiscountCodes]
3. Sửa giá/số lượng: UPDATE [Products] SET Price = ..., Stock = ...

## Hỗ trợ

Nếu gặp vấn đề, kiểm tra:
- Kết nối database
- Quyền truy cập (cần quyền INSERT, UPDATE, DELETE)
- Cấu trúc bảng có đúng với Models trong code không

