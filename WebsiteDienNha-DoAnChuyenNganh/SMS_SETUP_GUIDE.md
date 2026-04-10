# Hướng dẫn cấu hình SMS OTP

## 1. Chế độ Development (Console)

Mặc định, hệ thống sử dụng chế độ Console - OTP sẽ được hiển thị trong Console/Log thay vì gửi SMS thật.

```json
"SMS": {
  "Provider": "Console",
  "ApiKey": "",
  "ApiSecret": "",
  "SenderId": "OTP"
}
```

**Cách sử dụng:** Chạy ứng dụng và kiểm tra Console để xem mã OTP.

---

## 2. Tích hợp với ESMS.vn (Khuyến nghị cho Việt Nam)

### Bước 1: Đăng ký tài khoản
- Truy cập: https://esms.vn/
- Đăng ký tài khoản và nạp tiền

### Bước 2: Lấy API Key

**Cách 1: Truy cập trực tiếp qua URL**
Thử các URL sau (đăng nhập trước):
- https://account.esms.vn/API/Info
- https://account.esms.vn/API
- https://account.esms.vn/Settings/API
- https://account.esms.vn/Account/API

**Cách 2: Tìm trong menu chính**
API có thể nằm ở:
- Menu **"Cài đặt"** hoặc **"Settings"** (nếu có)
- Menu **"Tài khoản"** hoặc **"Account"** (nếu có)
- Menu **"Tích hợp"** hoặc **"Integration"** (nếu có)
- Hoặc là một menu item riêng biệt ở sidebar

**Cách 3: Tìm trong Developer/API section**
- Tìm menu có tên **"Developer"**, **"API"**, **"Tích hợp"**, hoặc **"Integration"**
- Có thể nằm ở phần trên cùng của sidebar hoặc trong dropdown menu

**Cách 4: Liên hệ hỗ trợ**
Nếu không tìm thấy:
- Click nút **"GỬI YÊU CẦU HỖ TRỢ"** (màu đỏ ở bên phải)
- Hoặc gọi hotline: **0901 888 484** (Hỗ trợ & Kinh Doanh)
- Hoặc dùng **"Chat Facebook"** ở footer
- Yêu cầu: "Tôi cần lấy API Key và Secret Key để tích hợp API gửi SMS"

**Trong trang API (khi tìm thấy), bạn sẽ thấy:**
- **API Key** (hoặc **ApiKey**) - Copy giá trị này
- **Secret Key** (hoặc **ApiSecret**) - Copy giá trị này
- Có thể có thêm thông tin về Brandname, Sender ID

**Lưu ý:** 
- Một số tài khoản có thể cần kích hoạt tính năng API trước
- Có thể cần đăng ký gói dịch vụ API (nếu chưa có)
- API Key thường được hiển thị dạng text có thể copy

### Bước 3: Cấu hình appsettings.json

```json
"SMS": {
  "Provider": "esms",
  "ApiKey": "YOUR_ESMS_API_KEY",
  "ApiSecret": "YOUR_ESMS_SECRET_KEY",
  "SenderId": "YOUR_BRAND_NAME"
}
```

**Lưu ý:**
- `SenderId`: Tên thương hiệu đã đăng ký với ESMS (ví dụ: "DIENNHA", "SHOP")
- Số điện thoại sẽ được tự động chuẩn hóa thành format: 84xxxxxxxxx (bỏ số 0 đầu)
- API sử dụng POST method với JSON body
- `SmsType`: 8 (thường dùng cho OTP/CSKH)
- `IsUnicode`: 0 (không unicode) hoặc 1 (unicode)
- `Sandbox`: 0 (gửi thật) hoặc 1 (test mode)

---

## 3. Tích hợp với BrandSMS.vn

### Bước 1: Đăng ký tài khoản
- Truy cập: https://brandsms.vn/
- Đăng ký và nạp tiền

### Bước 2: Lấy thông tin API
- Vào **Quản lý** → **API**
- Copy **API Key** và **API Secret**

### Bước 3: Cấu hình appsettings.json

```json
"SMS": {
  "Provider": "brandsms",
  "ApiKey": "YOUR_BRANDSMS_API_KEY",
  "ApiSecret": "YOUR_BRANDSMS_API_SECRET",
  "SenderId": "YOUR_BRAND_NAME"
}
```

---

## 4. Tích hợp với Twilio (Quốc tế)

### Bước 1: Đăng ký tài khoản
- Truy cập: https://www.twilio.com/
- Đăng ký tài khoản (có free trial)

### Bước 2: Lấy thông tin
- Vào **Console** → **Account** → **API Credentials**
- Copy **Account SID** (dùng làm ApiKey)
- Copy **Auth Token** (dùng làm ApiSecret)
- Lấy số điện thoại Twilio (dùng làm SenderId)

### Bước 3: Cấu hình appsettings.json

```json
"SMS": {
  "Provider": "twilio",
  "ApiKey": "YOUR_TWILIO_ACCOUNT_SID",
  "ApiSecret": "YOUR_TWILIO_AUTH_TOKEN",
  "SenderId": "+1234567890"
}
```

**Lưu ý:** SenderId phải là số điện thoại Twilio (format: +1234567890)

---

## 5. Các nhà cung cấp SMS khác ở Việt Nam

Bạn có thể mở rộng thêm các provider khác như:
- **SMS Brandname** (smsbrandname.com)
- **VietGuys** (vietguys.com)
- **SMS Việt** (smsviet.com)

Để thêm provider mới, cập nhật file `Services/SmsService.cs` và thêm method mới tương tự như `SendViaESMSAsync`.

---

## 6. Kiểm tra và Test

### Test trong Development:
1. Đặt `Provider: "Console"`
2. Chạy ứng dụng và đăng ký
3. Kiểm tra Console để xem OTP

### Test với SMS thật:
1. Cấu hình đúng thông tin API
2. Đặt `Provider` tương ứng
3. Test với số điện thoại thật
4. Kiểm tra log nếu có lỗi

---

## 7. Troubleshooting

### OTP không được gửi:
- Kiểm tra API Key và Secret có đúng không
- Kiểm tra số điện thoại format (84xxxxxxxxx)
- Kiểm tra tài khoản SMS còn tiền không
- Xem log để biết lỗi cụ thể

### Lỗi "Invalid API Key":
- Kiểm tra lại ApiKey và ApiSecret
- Đảm bảo không có khoảng trắng thừa
- Kiểm tra provider name có đúng không (chữ thường)

### Rate Limiting:
- Hệ thống tự động giới hạn 5 lần gửi trong 15 phút
- Nếu vượt quá, đợi 15 phút hoặc reset cache

---

## 8. Bảo mật

⚠️ **QUAN TRỌNG:**
- **KHÔNG** commit file `appsettings.json` có chứa API keys thật lên Git
- Sử dụng `appsettings.Development.json` cho development
- Sử dụng **User Secrets** hoặc **Environment Variables** cho production
- Hoặc sử dụng **Azure Key Vault** / **AWS Secrets Manager**

### Cách sử dụng User Secrets (Development):
```bash
dotnet user-secrets set "SMS:ApiKey" "YOUR_API_KEY"
dotnet user-secrets set "SMS:ApiSecret" "YOUR_API_SECRET"
```

### Sử dụng Environment Variables (Production):
```bash
export SMS__Provider="esms"
export SMS__ApiKey="YOUR_API_KEY"
export SMS__ApiSecret="YOUR_API_SECRET"
```

---

## 9. Chi phí ước tính

- **ESMS.vn**: ~200-300 VNĐ/SMS
- **BrandSMS.vn**: ~250-350 VNĐ/SMS  
- **Twilio**: ~$0.0075/SMS (quốc tế)

**Lưu ý:** Giá có thể thay đổi, vui lòng kiểm tra website chính thức của nhà cung cấp.

