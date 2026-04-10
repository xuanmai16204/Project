-- =============================================
-- Script Kiểm Tra Tài Khoản Admin
-- Database: WebsiteDienNha_DoAnChuyenNganh
-- =============================================

USE [WebsiteDienNha_DoAnChuyenNganh]
GO

PRINT N'========================================';
PRINT N'KIỂM TRA TÀI KHOẢN ADMIN';
PRINT N'========================================';
PRINT N'';

-- Kiểm tra Role Admin
PRINT N'1. KIỂM TRA ROLE ADMIN:';
IF EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [Name] = 'Admin')
BEGIN
    PRINT N'   ✓ Role "Admin" đã tồn tại';
    SELECT [Id], [Name], [NormalizedName] FROM [AspNetRoles] WHERE [Name] = 'Admin';
END
ELSE
BEGIN
    PRINT N'   ✗ Role "Admin" CHƯA TỒN TẠI!';
END
PRINT N'';

-- Kiểm tra tài khoản Admin
PRINT N'2. KIỂM TRA TÀI KHOẢN ADMIN:';
DECLARE @AdminUserId NVARCHAR(450);
DECLARE @AdminEmail NVARCHAR(256);
DECLARE @AdminPhoneNumber NVARCHAR(MAX);
DECLARE @AdminUserName NVARCHAR(256);

SELECT 
    @AdminUserId = u.[Id],
    @AdminEmail = u.[Email],
    @AdminPhoneNumber = u.[PhoneNumber],
    @AdminUserName = u.[UserName]
FROM [AspNetUsers] u
INNER JOIN [AspNetUserRoles] ur ON u.[Id] = ur.[UserId]
INNER JOIN [AspNetRoles] r ON ur.[RoleId] = r.[Id]
WHERE r.[Name] = 'Admin';

IF @AdminUserId IS NOT NULL
BEGIN
    PRINT N'   ✓ Tìm thấy tài khoản Admin:';
    PRINT N'   - User ID: ' + @AdminUserId;
    PRINT N'   - Email: ' + ISNULL(@AdminEmail, N'(NULL)');
    PRINT N'   - Phone Number: ' + ISNULL(@AdminPhoneNumber, N'(NULL)');
    PRINT N'   - User Name: ' + ISNULL(@AdminUserName, N'(NULL)');
    PRINT N'';
    PRINT N'   Chi tiết tài khoản:';
    SELECT 
        u.[Id],
        u.[UserName],
        u.[Email],
        u.[EmailConfirmed],
        u.[PhoneNumber],
        u.[PhoneNumberConfirmed],
        u.[LockoutEnabled],
        u.[LockoutEnd],
        u.[AccessFailedCount]
    FROM [AspNetUsers] u
    WHERE u.[Id] = @AdminUserId;
END
ELSE
BEGIN
    PRINT N'   ✗ KHÔNG TÌM THẤY tài khoản Admin!';
    PRINT N'   Cần tạo tài khoản Admin mới.';
END
PRINT N'';

-- Kiểm tra tất cả user có role Admin
PRINT N'3. TẤT CẢ TÀI KHOẢN CÓ ROLE ADMIN:';
SELECT 
    u.[Id],
    u.[UserName],
    u.[Email],
    u.[PhoneNumber],
    u.[EmailConfirmed],
    u.[PhoneNumberConfirmed],
    r.[Name] AS [RoleName]
FROM [AspNetUsers] u
INNER JOIN [AspNetUserRoles] ur ON u.[Id] = ur.[UserId]
INNER JOIN [AspNetRoles] r ON ur.[RoleId] = r.[Id]
WHERE r.[Name] = 'Admin';
PRINT N'';

-- Kiểm tra tài khoản theo email mặc định
PRINT N'4. KIỂM TRA TÀI KHOẢN THEO EMAIL MẶC ĐỊNH (admin@diennha.local):';
IF EXISTS (SELECT 1 FROM [AspNetUsers] WHERE [Email] = 'admin@diennha.local')
BEGIN
    DECLARE @DefaultAdminId NVARCHAR(450);
    SELECT @DefaultAdminId = [Id] FROM [AspNetUsers] WHERE [Email] = 'admin@diennha.local';
    
    PRINT N'   ✓ Tìm thấy tài khoản với email admin@diennha.local';
    SELECT 
        u.[Id],
        u.[UserName],
        u.[Email],
        u.[PhoneNumber],
        u.[EmailConfirmed],
        u.[PhoneNumberConfirmed],
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM [AspNetUserRoles] ur
                INNER JOIN [AspNetRoles] r ON ur.[RoleId] = r.[Id]
                WHERE ur.[UserId] = u.[Id] AND r.[Name] = 'Admin'
            ) THEN N'Có'
            ELSE N'Không'
        END AS [IsAdmin]
    FROM [AspNetUsers] u
    WHERE u.[Id] = @DefaultAdminId;
END
ELSE
BEGIN
    PRINT N'   ✗ Không tìm thấy tài khoản với email admin@diennha.local';
END
PRINT N'';

-- Lưu ý về cách đăng nhập
PRINT N'========================================';
PRINT N'LƯU Ý VỀ ĐĂNG NHẬP:';
PRINT N'========================================';
PRINT N'Hệ thống đăng nhập theo SỐ ĐIỆN THOẠI (PhoneNumber), không phải Email!';
PRINT N'Để đăng nhập vào Admin Dashboard, bạn cần:';
PRINT N'1. Tài khoản phải có PhoneNumber được set';
PRINT N'2. Tài khoản phải có Role "Admin"';
PRINT N'3. Đăng nhập bằng PhoneNumber và Password';
PRINT N'';
PRINT N'========================================';
GO

