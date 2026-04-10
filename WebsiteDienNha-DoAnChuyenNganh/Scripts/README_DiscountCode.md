# Hướng dẫn sử dụng và test Mã Giảm Giá

## 1. Cấu trúc Mã Giảm Giá

### Model: `DiscountCode`
```csharp
- Id: int
- Code: string (mã giảm giá, ví dụ: "LUCKY5")
- Description: string (mô tả)
- DiscountPercent: decimal? (% giảm giá)
- DiscountAmount: decimal? (số tiền giảm cố định)
- StartDate: DateTime (ngày bắt đầu)
- EndDate: DateTime (ngày kết thúc)
- UsageLimit: int (giới hạn lượt sử dụng, 0 = không giới hạn)
- UsageCount: int (số lần đã sử dụng)
- MinimumOrderAmount: decimal? (giá trị đơn hàng tối thiểu)
- IsActive: bool (trạng thái hoạt động)
- CreatedBy: string (người tạo)
```

## 2. Các điều kiện áp dụng mã

### Điều kiện hợp lệ:
1. ✅ Mã phải tồn tại trong database
2. ✅ `IsActive = true`
3. ✅ `StartDate <= Now <= EndDate`
4. ✅ `UsageLimit = 0` HOẶC `UsageCount < UsageLimit`
5. ✅ `CartTotal >= MinimumOrderAmount` (nếu có)

### Thông báo lỗi:
- ❌ "Mã giảm giá không tồn tại"
- ❌ "Mã giảm giá đã bị vô hiệu hóa"
- ❌ "Mã giảm giá chưa có hiệu lực"
- ❌ "Mã giảm giá đã hết hạn"
- ❌ "Mã giảm giá đã hết lượt sử dụng"
- ❌ "Đơn hàng tối thiểu X đ để sử dụng mã này"

## 3. Cách tính giảm giá

### Giảm theo phần trăm:
```
DiscountAmount = CartTotal * (DiscountPercent / 100)
FinalTotal = CartTotal - DiscountAmount
```

### Giảm số tiền cố định:
```
DiscountAmount = DiscountAmount (từ database)
FinalTotal = CartTotal - DiscountAmount
```

**Lưu ý:** Discount không được vượt quá CartTotal

## 4. Test Mã Giảm Giá

### Chạy script test:
```sql
-- File: Scripts/TestDiscountCode.sql
-- Chạy script này để:
-- 1. Xem danh sách mã giảm giá
-- 2. Kiểm tra trạng thái mã LUCKY5
-- 3. Test áp dụng với các giá trị đơn hàng khác nhau
-- 4. Sửa mã LUCKY5 để đảm bảo hoạt động
```

### Test trên website:
1. Đăng nhập vào tài khoản customer
2. Thêm sản phẩm vào giỏ hàng (tổng >= 100,000 đ)
3. Vào trang giỏ hàng
4. Nhập mã "LUCKY5" và click "Áp dụng"
5. Kiểm tra:
   - Thông báo thành công
   - Giá giảm 5%
   - Tổng tiền được cập nhật
   - Trang reload sau 1.5 giây

### Mở Console để debug (F12):
```javascript
// Xem request
console.log('Applying discount code:', code);

// Xem response
console.log('Discount code response:', response);
```

## 5. Các mã giảm giá mẫu

| Mã | Loại | Giá trị | Đơn tối thiểu | Mô tả |
|----|------|---------|---------------|-------|
| LUCKY5 | % | 5% | 100,000 đ | Giảm 5% cho mọi đơn hàng |
| WELCOME10 | % | 10% | 200,000 đ | Chào mừng khách hàng mới |
| SAVE50K | Cố định | 50,000 đ | 500,000 đ | Giảm 50K |
| SUMMER20 | % | 20% | 300,000 đ | Khuyến mãi mùa hè |
| VIP15 | % | 15% | 1,000,000 đ | Dành cho VIP |

## 6. Xử lý lỗi thường gặp

### Lỗi: "Mã giảm giá không tồn tại"
**Nguyên nhân:** Mã không có trong database hoặc gõ sai
**Giải pháp:** Kiểm tra lại mã trong database

### Lỗi: "Đơn hàng tối thiểu X đ"
**Nguyên nhân:** Giá trị giỏ hàng < MinimumOrderAmount
**Giải pháp:** Thêm sản phẩm vào giỏ hàng

### Lỗi: "Mã giảm giá đã hết hạn"
**Nguyên nhân:** EndDate < Now
**Giải pháp:** Chạy script update EndDate

### Lỗi: AJAX request không hoạt động
**Nguyên nhân:** Anti-forgery token hoặc session
**Giải pháp:** Đã thêm `[IgnoreAntiforgeryToken]`

## 7. Flow áp dụng mã giảm giá

```
1. User nhập mã → Click "Áp dụng"
2. JavaScript gửi AJAX request
3. Controller kiểm tra:
   - Mã có tồn tại?
   - IsActive = true?
   - Trong thời hạn?
   - Đủ điều kiện đơn tối thiểu?
4. Nếu hợp lệ:
   - Tính discount
   - Lưu vào Session
   - Trả về JSON success
   - JavaScript reload trang
5. Nếu không hợp lệ:
   - Trả về JSON error
   - Hiển thị thông báo lỗi
```

## 8. Session Management

### Lưu vào Session:
```csharp
HttpContext.Session.SetString("AppliedDiscountCode", code.Code);
HttpContext.Session.SetString("DiscountCodeId", code.Id.ToString());
```

### Đọc từ Session:
```csharp
var discountCode = HttpContext.Session.GetString("AppliedDiscountCode");
var discountCodeId = HttpContext.Session.GetString("DiscountCodeId");
```

### Xóa khỏi Session:
```csharp
HttpContext.Session.Remove("AppliedDiscountCode");
HttpContext.Session.Remove("DiscountCodeId");
```

## 9. Checklist Test

- [ ] Nhập mã đúng → Áp dụng thành công
- [ ] Nhập mã sai → Hiển thị lỗi
- [ ] Đơn hàng < tối thiểu → Hiển thị lỗi
- [ ] Mã hết hạn → Hiển thị lỗi
- [ ] Mã đã vô hiệu hóa → Hiển thị lỗi
- [ ] Reload trang → Discount vẫn được áp dụng
- [ ] Checkout → Discount được tính vào order
- [ ] OrderCompleted → Hiển thị discount

## 10. Troubleshooting

### Debug Controller:
- Thêm breakpoint tại `ApplyDiscountCode` action
- Kiểm tra giá trị `cartTotal` và `code.MinimumOrderAmount`
- Kiểm tra điều kiện date range

### Debug JavaScript:
- Mở Console (F12)
- Xem log "Applying discount code"
- Xem log "Discount code response"
- Kiểm tra Network tab

### Debug Session:
- Kiểm tra Session có được lưu không
- Kiểm tra Session có bị xóa sau reload không
- Đảm bảo `app.UseSession()` trong Program.cs

