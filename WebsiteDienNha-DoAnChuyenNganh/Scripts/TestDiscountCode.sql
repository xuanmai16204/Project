-- =============================================
-- Script: Test Discount Code
-- Mục đích: Kiểm tra và test mã giảm giá
-- =============================================

USE [DienNhaDB];
GO

-- 1. Kiểm tra các mã giảm giá đang có
PRINT N'=== DANH SÁCH MÃ GIẢM GIÁ ===';
SELECT 
    Id,
    Code,
    Description,
    DiscountPercent,
    DiscountAmount,
    StartDate,
    EndDate,
    UsageLimit,
    UsageCount,
    MinimumOrderAmount,
    IsActive,
    CASE 
        WHEN GETUTCDATE() < StartDate THEN N'Chưa có hiệu lực'
        WHEN GETUTCDATE() > EndDate THEN N'Đã hết hạn'
        WHEN IsActive = 0 THEN N'Đã vô hiệu hóa'
        WHEN UsageLimit > 0 AND UsageCount >= UsageLimit THEN N'Đã hết lượt sử dụng'
        ELSE N'Đang hoạt động'
    END AS Status
FROM DiscountCodes
ORDER BY Id;
GO

-- 2. Tìm mã giảm giá LUCKY5
PRINT N'';
PRINT N'=== THÔNG TIN MÃ LUCKY5 ===';
SELECT 
    Id,
    Code,
    Description,
    DiscountPercent,
    DiscountAmount,
    FORMAT(StartDate, 'dd/MM/yyyy HH:mm:ss') AS StartDate,
    FORMAT(EndDate, 'dd/MM/yyyy HH:mm:ss') AS EndDate,
    FORMAT(GETUTCDATE(), 'dd/MM/yyyy HH:mm:ss') AS CurrentTime,
    UsageLimit,
    UsageCount,
    MinimumOrderAmount,
    IsActive,
    CASE 
        WHEN GETUTCDATE() < StartDate THEN N'❌ Chưa có hiệu lực'
        WHEN GETUTCDATE() > EndDate THEN N'❌ Đã hết hạn'
        WHEN IsActive = 0 THEN N'❌ Đã vô hiệu hóa'
        WHEN UsageLimit > 0 AND UsageCount >= UsageLimit THEN N'❌ Đã hết lượt sử dụng'
        ELSE N'✅ Đang hoạt động'
    END AS Status
FROM DiscountCodes
WHERE Code = 'LUCKY5';
GO

-- 3. Kiểm tra các mã đang hoạt động
PRINT N'';
PRINT N'=== CÁC MÃ ĐANG HOẠT ĐỘNG ===';
SELECT 
    Code,
    Description,
    CASE 
        WHEN DiscountPercent IS NOT NULL THEN CONCAT(DiscountPercent, '%')
        WHEN DiscountAmount IS NOT NULL THEN CONCAT(FORMAT(DiscountAmount, 'N0'), ' đ')
        ELSE N'Không có giảm giá'
    END AS Discount,
    FORMAT(MinimumOrderAmount, 'N0') + N' đ' AS MinOrder,
    DATEDIFF(DAY, GETUTCDATE(), EndDate) AS DaysRemaining
FROM DiscountCodes
WHERE IsActive = 1
    AND GETUTCDATE() >= StartDate
    AND GETUTCDATE() <= EndDate
    AND (UsageLimit = 0 OR UsageCount < UsageLimit)
ORDER BY MinimumOrderAmount;
GO

-- 4. Test áp dụng mã với giá trị đơn hàng khác nhau
PRINT N'';
PRINT N'=== TEST ÁP DỤNG MÃ VỚI GIÁ TRỊ ĐƠN HÀNG ===';

DECLARE @TestOrderValues TABLE (OrderValue DECIMAL(18,2));
INSERT INTO @TestOrderValues VALUES (50000), (100000), (200000), (500000), (1000000), (2000000);

SELECT 
    dc.Code,
    dc.Description,
    tov.OrderValue AS OrderValue,
    dc.MinimumOrderAmount AS MinRequired,
    CASE 
        WHEN dc.MinimumOrderAmount IS NOT NULL AND tov.OrderValue < dc.MinimumOrderAmount 
        THEN N'❌ Không đủ điều kiện'
        ELSE N'✅ Đủ điều kiện'
    END AS CanApply,
    CASE 
        WHEN dc.DiscountPercent IS NOT NULL 
        THEN tov.OrderValue * dc.DiscountPercent / 100
        WHEN dc.DiscountAmount IS NOT NULL 
        THEN dc.DiscountAmount
        ELSE 0
    END AS DiscountAmount,
    CASE 
        WHEN dc.DiscountPercent IS NOT NULL 
        THEN tov.OrderValue - (tov.OrderValue * dc.DiscountPercent / 100)
        WHEN dc.DiscountAmount IS NOT NULL 
        THEN tov.OrderValue - dc.DiscountAmount
        ELSE tov.OrderValue
    END AS FinalAmount
FROM DiscountCodes dc
CROSS JOIN @TestOrderValues tov
WHERE dc.IsActive = 1
    AND GETUTCDATE() >= dc.StartDate
    AND GETUTCDATE() <= dc.EndDate
    AND (dc.UsageLimit = 0 OR dc.UsageCount < dc.UsageLimit)
ORDER BY dc.Code, tov.OrderValue;
GO

-- 5. Sửa mã LUCKY5 để đảm bảo hoạt động
PRINT N'';
PRINT N'=== SỬA MÃ LUCKY5 ===';

UPDATE DiscountCodes
SET 
    IsActive = 1,
    StartDate = DATEADD(DAY, -1, GETUTCDATE()),
    EndDate = DATEADD(MONTH, 12, GETUTCDATE()),
    MinimumOrderAmount = 100000,
    UsageLimit = 0,
    UsageCount = 0
WHERE Code = 'LUCKY5';

PRINT N'✅ Đã cập nhật mã LUCKY5';
GO

-- 6. Kiểm tra lại sau khi sửa
PRINT N'';
PRINT N'=== KIỂM TRA LẠI MÃ LUCKY5 ===';
SELECT 
    Code,
    Description,
    DiscountPercent,
    DiscountAmount,
    FORMAT(StartDate, 'dd/MM/yyyy HH:mm:ss') AS StartDate,
    FORMAT(EndDate, 'dd/MM/yyyy HH:mm:ss') AS EndDate,
    FORMAT(GETUTCDATE(), 'dd/MM/yyyy HH:mm:ss') AS CurrentTime,
    MinimumOrderAmount,
    IsActive,
    UsageLimit,
    UsageCount,
    CASE 
        WHEN GETUTCDATE() < StartDate THEN N'❌ Chưa có hiệu lực'
        WHEN GETUTCDATE() > EndDate THEN N'❌ Đã hết hạn'
        WHEN IsActive = 0 THEN N'❌ Đã vô hiệu hóa'
        WHEN UsageLimit > 0 AND UsageCount >= UsageLimit THEN N'❌ Đã hết lượt sử dụng'
        ELSE N'✅ Đang hoạt động'
    END AS Status
FROM DiscountCodes
WHERE Code = 'LUCKY5';
GO

PRINT N'';
PRINT N'=== HOÀN THÀNH ===';

