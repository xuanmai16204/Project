-- =============================================
-- Script Insert Dữ Liệu Mẫu - Website Điện Nhà
-- Database: WebsiteDienNha_DoAnChuyenNganh
-- =============================================

USE [WebsiteDienNha_DoAnChuyenNganh]
GO

-- =============================================
-- 1. XÓA DỮ LIỆU CŨ (Nếu cần)
-- =============================================
-- Lưu ý: Chỉ chạy khi muốn reset dữ liệu
-- Bỏ comment phần này nếu muốn xóa toàn bộ dữ liệu cũ
/*
-- Xóa theo thứ tự để tránh lỗi Foreign Key
DELETE FROM [OrderItems];
DELETE FROM [Orders];
DELETE FROM [CartItems];
DELETE FROM [Products];
DELETE FROM [Categories];
DELETE FROM [DiscountCodes];
GO
*/

-- =============================================
-- 2. INSERT/UPDATE DANH MỤC (Categories)
-- =============================================
SET IDENTITY_INSERT [Categories] ON;
GO

-- Sử dụng MERGE để insert hoặc update
MERGE [Categories] AS target
USING (VALUES
    (1, N'Công tắc', N'cong-tac', N'Các loại công tắc điện dân dụng và công nghiệp', 1),
    (2, N'Ổ cắm điện', N'o-cam-dien', N'Ổ cắm điện 2 chấu, 3 chấu, đa năng', 1),
    (3, N'Đèn LED', N'den-led', N'Đèn LED chiếu sáng tiết kiệm điện', 1),
    (4, N'Dây điện', N'day-dien', N'Dây điện các loại: dây đơn, dây đôi, dây 3 lõi', 1),
    (5, N'Thiết bị đóng cắt', N'thiet-bi-dong-cat', N'CB, MCB, ELCB, Contactor, Relay', 1),
    (6, N'Ổn áp - Biến áp', N'on-ap-bien-ap', N'Máy ổn áp, biến áp tự ngẫu', 1),
    (7, N'Thiết bị chiếu sáng', N'thiet-bi-chieu-sang', N'Đèn chùm, đèn treo, đèn âm trần', 1),
    (8, N'Phụ kiện điện', N'phu-kien-dien', N'Băng keo điện, kẹp cáp, ống luồn dây điện', 1),
    (9, N'Quạt điện', N'quat-dien', N'Quạt trần, quạt treo tường, quạt cây', 1),
    (10, N'Thiết bị an toàn điện', N'thiet-bi-an-toan-dien', N'Găng tay cách điện, ủng cách điện, thảm cách điện', 1)
) AS source ([Id], [Name], [Slug], [Description], [IsActive])
ON target.[Id] = source.[Id]
WHEN MATCHED THEN
    UPDATE SET 
        [Name] = source.[Name],
        [Slug] = source.[Slug],
        [Description] = source.[Description],
        [IsActive] = source.[IsActive]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name], [Slug], [Description], [IsActive], [CreatedAt])
    VALUES (source.[Id], source.[Name], source.[Slug], source.[Description], source.[IsActive], GETUTCDATE());
GO

SET IDENTITY_INSERT [Categories] OFF;
GO

-- =============================================
-- 3. INSERT/UPDATE SẢN PHẨM (Products)
-- =============================================
SET IDENTITY_INSERT [Products] ON;
GO

-- Chỉ giữ lại 5 sản phẩm đại diện
MERGE [Products] AS target
USING (VALUES
    (1, N'Công tắc 1 chiều Panasonic WES6101', N'cong-tac-1-chieu-panasonic', N'Công tắc 1 chiều Panasonic, màu trắng, chịu dòng 10A, điện áp 250V. Thiết kế hiện đại, dễ lắp đặt.', 45000, 22500, 150, 1, N'https://cdn.tgdd.vn/Products/Images/1986/295366/may-khoan-dong-luc-bosch-gsb-550w-1-600x600.jpg', 1),
    (2, N'Ổ cắm 2 chấu Panasonic WES8101', N'o-cam-2-chau-panasonic', N'Ổ cắm 2 chấu Panasonic, màu trắng, chịu dòng 10A, có nắp đậy an toàn.', 55000, 27500, 200, 1, N'https://cdn.tgdd.vn/Products/Images/1986/289557/may-mai-goc-bosch-gws-060-600x600.jpg', 2),
    (3, N'Đèn LED bulb 9W Panasonic', N'den-led-bulb-9w', N'Bóng đèn LED 9W, tương đương 60W đèn sợi đốt, tuổi thọ 25000 giờ, ánh sáng trắng.', 85000, 59500, 300, 1, N'https://meta.vn/Data/image/2022/09/05/may-han-que-maxpro-mma-200-1.jpg', 3),
    (4, N'Dây điện đơn 1.5mm2 Cadivi', N'day-dien-don-1.5mm', N'Dây điện đơn 1.5mm2, vỏ PVC, dài 100m, chịu dòng 16A, màu đỏ, vàng, xanh.', 850000, 680000, 50, 1, N'https://cdn.tgdd.vn/Products/Images/1986/240571/dong-ho-do-vom-victor-vc890d-1-600x600.jpg', 4),
    (5, N'CB 1 pha 10A Schneider', N'cb-1-pha-10a', N'Cầu dao tự động 1 pha 10A, bảo vệ quá tải và ngắn mạch, màu trắng.', 180000, 80000, 100, 1, N'https://cdn.tgdd.vn/Products/Images/1986/286265/may-cua-loc-bosch-gks-190-1-600x600.jpg', 5)
) AS source ([Id], [Name], [Slug], [Description], [Price], [PromotionPrice], [Stock], [IsActive], [ImageUrl], [CategoryId])
ON target.[Id] = source.[Id]
WHEN MATCHED THEN
    UPDATE SET 
        [Name] = source.[Name],
        [Slug] = source.[Slug],
        [Description] = source.[Description],
        [Price] = source.[Price],
        [PromotionPrice] = source.[PromotionPrice],
        [Stock] = source.[Stock],
        [IsActive] = source.[IsActive],
        [ImageUrl] = source.[ImageUrl],
        [CategoryId] = source.[CategoryId]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name], [Slug], [Description], [Price], [PromotionPrice], [Stock], [IsActive], [ImageUrl], [CategoryId], [CreatedAt])
    VALUES (source.[Id], source.[Name], source.[Slug], source.[Description], source.[Price], source.[PromotionPrice], source.[Stock], source.[IsActive], source.[ImageUrl], source.[CategoryId], GETUTCDATE());
GO

SET IDENTITY_INSERT [Products] OFF;
GO

-- =============================================
-- 4. INSERT/UPDATE MÃ GIẢM GIÁ (DiscountCodes)
-- =============================================
SET IDENTITY_INSERT [DiscountCodes] ON;
GO

MERGE [DiscountCodes] AS target
USING (VALUES
    (1, N'WELCOME10', N'Mã giảm giá chào mừng khách hàng mới - Giảm 10%', 10, NULL, GETUTCDATE(), DATEADD(MONTH, 3, GETUTCDATE()), 0, 0, 200000, 1, N'Admin'),
    (2, N'SAVE50K', N'Giảm 50.000đ cho đơn hàng từ 500.000đ', NULL, 50000, GETUTCDATE(), DATEADD(MONTH, 6, GETUTCDATE()), 0, 0, 500000, 1, N'Admin'),
    (3, N'SUMMER20', N'Khuyến mãi mùa hè - Giảm 20%', 20, NULL, GETUTCDATE(), DATEADD(MONTH, 2, GETUTCDATE()), 1000, 0, 300000, 1, N'Admin'),
    (4, N'VIP15', N'Mã dành cho khách hàng VIP - Giảm 15%', 15, NULL, GETUTCDATE(), DATEADD(MONTH, 12, GETUTCDATE()), 0, 0, 1000000, 1, N'Admin'),
    (5, N'NEW100K', N'Giảm 100.000đ cho đơn hàng từ 1.000.000đ', NULL, 100000, GETUTCDATE(), DATEADD(MONTH, 3, GETUTCDATE()), 500, 0, 1000000, 1, N'Admin'),
    (6, N'FLASH30', N'Flash sale - Giảm 30% trong 24h', 30, NULL, GETUTCDATE(), DATEADD(DAY, 1, GETUTCDATE()), 100, 0, 500000, 1, N'Admin'),
    (7, N'FREESHIP', N'Miễn phí vận chuyển cho đơn hàng từ 500.000đ', NULL, 50000, GETUTCDATE(), DATEADD(MONTH, 6, GETUTCDATE()), 0, 0, 500000, 1, N'Admin'),
    (8, N'BIRTHDAY', N'Mã giảm giá sinh nhật - Giảm 25%', 25, NULL, GETUTCDATE(), DATEADD(MONTH, 1, GETUTCDATE()), 0, 0, 200000, 1, N'Admin'),
    (9, N'LUCKY5', N'Giảm 5% cho mọi đơn hàng', 5, NULL, GETUTCDATE(), DATEADD(MONTH, 12, GETUTCDATE()), 0, 0, 100000, 1, N'Admin'),
    (10, N'BIG200K', N'Giảm 200.000đ cho đơn hàng từ 2.000.000đ', NULL, 200000, GETUTCDATE(), DATEADD(MONTH, 6, GETUTCDATE()), 200, 0, 2000000, 1, N'Admin')
) AS source ([Id], [Code], [Description], [DiscountPercent], [DiscountAmount], [StartDate], [EndDate], [UsageLimit], [UsageCount], [MinimumOrderAmount], [IsActive], [CreatedBy])
ON target.[Id] = source.[Id]
WHEN MATCHED THEN
    UPDATE SET 
        [Code] = source.[Code],
        [Description] = source.[Description],
        [DiscountPercent] = source.[DiscountPercent],
        [DiscountAmount] = source.[DiscountAmount],
        [StartDate] = source.[StartDate],
        [EndDate] = source.[EndDate],
        [UsageLimit] = source.[UsageLimit],
        [MinimumOrderAmount] = source.[MinimumOrderAmount],
        [IsActive] = source.[IsActive],
        [CreatedBy] = source.[CreatedBy]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Code], [Description], [DiscountPercent], [DiscountAmount], [StartDate], [EndDate], [UsageLimit], [UsageCount], [MinimumOrderAmount], [IsActive], [CreatedBy])
    VALUES (source.[Id], source.[Code], source.[Description], source.[DiscountPercent], source.[DiscountAmount], source.[StartDate], source.[EndDate], source.[UsageLimit], source.[UsageCount], source.[MinimumOrderAmount], source.[IsActive], source.[CreatedBy]);
GO

SET IDENTITY_INSERT [DiscountCodes] OFF;
GO

-- =============================================
-- 5. INSERT/UPDATE KHUYẾN MÃI (Promotions)
-- =============================================
SET IDENTITY_INSERT [Promotions] ON;
GO

MERGE [Promotions] AS target
USING (VALUES
    -- Khuyến mãi giảm 50% cho một số sản phẩm (ví dụ: 600.000 -> 300.000)
    (1, N'Siêu khuyến mãi giảm 50%', N'Giảm giá 50% cho các sản phẩm công tắc và ổ cắm điện', NULL, 
     DATEADD(DAY, -7, GETUTCDATE()), DATEADD(MONTH, 1, GETUTCDATE()), 
     50.00, NULL, NULL, NULL, 1),
    
    -- Khuyến mãi giảm 30% cho đèn LED
    (2, N'Khuyến mãi đèn LED giảm 30%', N'Giảm giá 30% cho tất cả sản phẩm đèn LED', NULL,
     DATEADD(DAY, -5, GETUTCDATE()), DATEADD(MONTH, 2, GETUTCDATE()),
     30.00, NULL, NULL, NULL, 1),
    
    -- Khuyến mãi giảm 20% cho dây điện
    (3, N'Khuyến mãi dây điện giảm 20%', N'Giảm giá 20% cho các loại dây điện', NULL,
     DATEADD(DAY, -3, GETUTCDATE()), DATEADD(MONTH, 1, GETUTCDATE()),
     20.00, NULL, NULL, NULL, 1),
    
    -- Khuyến mãi giảm 100.000đ cho thiết bị đóng cắt
    (4, N'Giảm 100.000đ thiết bị đóng cắt', N'Giảm trực tiếp 100.000đ cho các thiết bị đóng cắt', NULL,
     DATEADD(DAY, -2, GETUTCDATE()), DATEADD(MONTH, 1, GETUTCDATE()),
     NULL, 100000, NULL, NULL, 1),
    
    -- Khuyến mãi giảm 25% nhưng tối đa 200.000đ
    (5, N'Giảm 25% tối đa 200.000đ', N'Giảm 25% nhưng không quá 200.000đ cho quạt điện', NULL,
     DATEADD(DAY, -1, GETUTCDATE()), DATEADD(MONTH, 1, GETUTCDATE()),
     25.00, NULL, NULL, 200000, 1),
    
    -- Khuyến mãi giảm 15% cho phụ kiện điện
    (6, N'Khuyến mãi phụ kiện giảm 15%', N'Giảm giá 15% cho các phụ kiện điện', NULL,
     GETUTCDATE(), DATEADD(MONTH, 1, GETUTCDATE()),
     15.00, NULL, NULL, NULL, 1)
) AS source ([Id], [Name], [Description], [BannerUrl], [StartDate], [EndDate], 
             [DiscountPercent], [DiscountAmount], [MinOrderTotal], [MaxDiscount], [IsActive])
ON target.[Id] = source.[Id]
WHEN MATCHED THEN
    UPDATE SET 
        [Name] = source.[Name],
        [Description] = source.[Description],
        [BannerUrl] = source.[BannerUrl],
        [StartDate] = source.[StartDate],
        [EndDate] = source.[EndDate],
        [DiscountPercent] = source.[DiscountPercent],
        [DiscountAmount] = source.[DiscountAmount],
        [MinOrderTotal] = source.[MinOrderTotal],
        [MaxDiscount] = source.[MaxDiscount],
        [IsActive] = source.[IsActive]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name], [Description], [BannerUrl], [StartDate], [EndDate], 
            [DiscountPercent], [DiscountAmount], [MinOrderTotal], [MaxDiscount], [IsActive])
    VALUES (source.[Id], source.[Name], source.[Description], source.[BannerUrl], 
            source.[StartDate], source.[EndDate], source.[DiscountPercent], 
            source.[DiscountAmount], source.[MinOrderTotal], source.[MaxDiscount], 
            source.[IsActive]);
GO

SET IDENTITY_INSERT [Promotions] OFF;
GO

-- =============================================
-- 6. INSERT/UPDATE SẢN PHẨM KHUYẾN MÃI (PromotionProducts)
-- =============================================

-- Promotion 1: Giảm 50% cho công tắc và ổ cắm (sản phẩm ID 1, 2)
MERGE [PromotionProducts] AS target
USING (
    SELECT 1 AS [PromotionId], [Id] AS [ProductId] FROM [Products] WHERE [Id] IN (1, 2)
) AS source ([PromotionId], [ProductId])
ON target.[PromotionId] = source.[PromotionId] AND target.[ProductId] = source.[ProductId]
WHEN NOT MATCHED THEN
    INSERT ([PromotionId], [ProductId])
    VALUES (source.[PromotionId], source.[ProductId]);
GO

-- Promotion 2: Giảm 30% cho đèn LED (sản phẩm ID 3)
MERGE [PromotionProducts] AS target
USING (
    SELECT 2 AS [PromotionId], [Id] AS [ProductId] FROM [Products] WHERE [Id] = 3
) AS source ([PromotionId], [ProductId])
ON target.[PromotionId] = source.[PromotionId] AND target.[ProductId] = source.[ProductId]
WHEN NOT MATCHED THEN
    INSERT ([PromotionId], [ProductId])
    VALUES (source.[PromotionId], source.[ProductId]);
GO

-- Promotion 3: Giảm 20% cho dây điện (sản phẩm ID 4)
MERGE [PromotionProducts] AS target
USING (
    SELECT 3 AS [PromotionId], [Id] AS [ProductId] FROM [Products] WHERE [Id] = 4
) AS source ([PromotionId], [ProductId])
ON target.[PromotionId] = source.[PromotionId] AND target.[ProductId] = source.[ProductId]
WHEN NOT MATCHED THEN
    INSERT ([PromotionId], [ProductId])
    VALUES (source.[PromotionId], source.[ProductId]);
GO

-- Promotion 4: Giảm 100.000đ cho thiết bị đóng cắt (sản phẩm ID 5)
MERGE [PromotionProducts] AS target
USING (
    SELECT 4 AS [PromotionId], [Id] AS [ProductId] FROM [Products] WHERE [Id] = 5
) AS source ([PromotionId], [ProductId])
ON target.[PromotionId] = source.[PromotionId] AND target.[ProductId] = source.[ProductId]
WHEN NOT MATCHED THEN
    INSERT ([PromotionId], [ProductId])
    VALUES (source.[PromotionId], source.[ProductId]);
GO

-- =============================================
-- 7. KIỂM TRA DỮ LIỆU ĐÃ INSERT
-- =============================================
DECLARE @CategoryCount INT;
DECLARE @ProductCount INT;
DECLARE @DiscountCodeCount INT;
DECLARE @PromotionCount INT;
DECLARE @PromotionProductCount INT;

SELECT @CategoryCount = COUNT(*) FROM [Categories];
SELECT @ProductCount = COUNT(*) FROM [Products];
SELECT @DiscountCodeCount = COUNT(*) FROM [DiscountCodes];
SELECT @PromotionCount = COUNT(*) FROM [Promotions];
SELECT @PromotionProductCount = COUNT(*) FROM [PromotionProducts];

PRINT N'========================================';
PRINT N'KIỂM TRA DỮ LIỆU ĐÃ INSERT';
PRINT N'========================================';
PRINT N'Số lượng danh mục: ' + CAST(@CategoryCount AS NVARCHAR(10));
PRINT N'Số lượng sản phẩm: ' + CAST(@ProductCount AS NVARCHAR(10));
PRINT N'Số lượng mã giảm giá: ' + CAST(@DiscountCodeCount AS NVARCHAR(10));
PRINT N'Số lượng khuyến mãi: ' + CAST(@PromotionCount AS NVARCHAR(10));
PRINT N'Số lượng sản phẩm khuyến mãi: ' + CAST(@PromotionProductCount AS NVARCHAR(10));
PRINT N'';
PRINT N'Danh sách danh mục:';
SELECT [Id], [Name], [Slug], [IsActive] FROM [Categories] ORDER BY [Id];
PRINT N'';
PRINT N'Danh sách sản phẩm (10 sản phẩm đầu):';
SELECT TOP 10 [Id], [Name], [Price], [PromotionPrice], [Stock], [CategoryId] FROM [Products] ORDER BY [Id];
PRINT N'';
PRINT N'Danh sách mã giảm giá:';
SELECT [Id], [Code], [DiscountPercent], [DiscountAmount], [StartDate], [EndDate], [IsActive] FROM [DiscountCodes] ORDER BY [Id];
PRINT N'';
PRINT N'Danh sách khuyến mãi:';
SELECT [Id], [Name], [DiscountPercent], [DiscountAmount], [MaxDiscount], [StartDate], [EndDate], [IsActive] 
FROM [Promotions] ORDER BY [Id];
PRINT N'';
PRINT N'Danh sách sản phẩm có khuyến mãi (ví dụ):';
SELECT TOP 10 
    p.[Id] AS [ProductId],
    p.[Name] AS [ProductName],
    p.[Price] AS [OriginalPrice],
    p.[PromotionPrice] AS [PromotionPriceFromDB],
    pr.[Name] AS [PromotionName],
    pr.[DiscountPercent],
    pr.[DiscountAmount],
    pr.[MaxDiscount],
    CASE 
        WHEN p.[PromotionPrice] IS NOT NULL THEN p.[PromotionPrice]
        WHEN pr.[DiscountPercent] IS NOT NULL THEN 
            CAST(p.[Price] * (1 - pr.[DiscountPercent] / 100.0) AS DECIMAL(18,2))
        WHEN pr.[DiscountAmount] IS NOT NULL THEN 
            CAST(p.[Price] - pr.[DiscountAmount] AS DECIMAL(18,2))
        ELSE p.[Price]
    END AS [CalculatedPromotionPrice]
FROM [Products] p
INNER JOIN [PromotionProducts] pp ON p.[Id] = pp.[ProductId]
INNER JOIN [Promotions] pr ON pp.[PromotionId] = pr.[Id]
WHERE pr.[IsActive] = 1 
    AND pr.[StartDate] <= GETUTCDATE() 
    AND pr.[EndDate] >= GETUTCDATE()
ORDER BY p.[Id];
PRINT N'';
PRINT N'========================================';
PRINT N'HOÀN TẤT INSERT DỮ LIỆU!';
PRINT N'========================================';
PRINT N'';
PRINT N'VÍ DỤ TÍNH GIÁ KHUYẾN MÃI:';
PRINT N'- Sản phẩm ID 1 (Công tắc 1 chiều): Giá gốc 45.000đ, giảm 50% = 22.500đ';
PRINT N'- Sản phẩm ID 2 (Ổ cắm 2 chấu): Giá gốc 55.000đ, giảm 50% = 27.500đ';
PRINT N'- Sản phẩm ID 3 (Đèn LED 9W): Giá gốc 85.000đ, giảm 30% = 59.500đ';
PRINT N'- Sản phẩm ID 4 (Dây điện 1.5mm): Giá gốc 850.000đ, giảm 20% = 680.000đ';
PRINT N'- Sản phẩm ID 5 (CB 1 pha 10A): Giá gốc 180.000đ, giảm 100.000đ = 80.000đ';
GO

