-- =============================================
-- Script Tạo/Cập Nhật Tài Khoản Admin
-- Database: WebsiteDienNha_DoAnChuyenNganh
-- =============================================
-- Lưu ý: Hệ thống đăng nhập theo PhoneNumber, không phải Email
-- =============================================

USE [WebsiteDienNha_DoAnChuyenNganh]
GO

-- =============================================
-- 1. Đảm bảo Role Admin tồn tại
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [Name] = 'Admin')
BEGIN
    INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());
    PRINT N'✓ Đã tạo Role "Admin"';
END
ELSE
BEGIN
    PRINT N'✓ Role "Admin" đã tồn tại';
END
GO

-- =============================================
-- 2. Tìm hoặc tạo tài khoản Admin
-- =============================================
DECLARE @AdminEmail NVARCHAR(256) = 'admin@diennha.local';
DECLARE @AdminPhoneNumber NVARCHAR(MAX) = '0900000000'; -- Số điện thoại để đăng nhập
DECLARE @AdminPassword NVARCHAR(MAX) = 'Admin!23456'; -- Mật khẩu mặc định
DECLARE @AdminUserId NVARCHAR(450);

-- Tìm tài khoản theo Email
SELECT @AdminUserId = [Id] FROM [AspNetUsers] WHERE [Email] = @AdminEmail;

IF @AdminUserId IS NULL
BEGIN
    -- Tạo tài khoản mới
    SET @AdminUserId = NEWID();
    
    INSERT INTO [AspNetUsers] (
        [Id],
        [UserName],
        [NormalizedUserName],
        [Email],
        [NormalizedEmail],
        [EmailConfirmed],
        [PhoneNumber],
        [PhoneNumberConfirmed],
        [PasswordHash],
        [SecurityStamp],
        [ConcurrencyStamp],
        [LockoutEnabled],
        [AccessFailedCount],
        [TwoFactorEnabled],
        [FullName],
        [LoyaltyPoints],
        [LoyaltyLevelId]
    )
    VALUES (
        @AdminUserId,
        @AdminPhoneNumber, -- UserName = PhoneNumber
        UPPER(@AdminPhoneNumber),
        @AdminEmail,
        UPPER(@AdminEmail),
        1, -- EmailConfirmed
        @AdminPhoneNumber,
        1, -- PhoneNumberConfirmed
        -- Password hash cho "Admin!23456" - cần hash thực tế từ ASP.NET Identity
        -- Tạm thời để NULL, sẽ cần reset password sau
        NULL,
        NEWID(),
        NEWID(),
        0, -- LockoutEnabled
        0, -- AccessFailedCount
        0, -- TwoFactorEnabled
        N'Quản trị viên', -- FullName
        0, -- LoyaltyPoints
        1  -- LoyaltyLevelId (mặc định)
    );
    
    PRINT N'✓ Đã tạo tài khoản Admin mới';
    PRINT N'  - Email: ' + @AdminEmail;
    PRINT N'  - Phone Number: ' + @AdminPhoneNumber;
    PRINT N'  - User ID: ' + @AdminUserId;
    PRINT N'';
    PRINT N'⚠ LƯU Ý: Password hash chưa được set.';
    PRINT N'   Bạn cần reset password qua ứng dụng hoặc sử dụng AccountController.ResetPassword';
END
ELSE
BEGIN
    -- Cập nhật PhoneNumber nếu chưa có
    UPDATE [AspNetUsers]
    SET 
        [PhoneNumber] = @AdminPhoneNumber,
        [PhoneNumberConfirmed] = 1,
        [UserName] = COALESCE([UserName], @AdminPhoneNumber),
        [NormalizedUserName] = COALESCE([NormalizedUserName], UPPER(@AdminPhoneNumber))
    WHERE [Id] = @AdminUserId;
    
    PRINT N'✓ Đã cập nhật tài khoản Admin hiện có';
    PRINT N'  - Email: ' + @AdminEmail;
    PRINT N'  - Phone Number: ' + @AdminPhoneNumber;
    PRINT N'  - User ID: ' + @AdminUserId;
END
GO

-- =============================================
-- 3. Gán Role Admin cho tài khoản
-- =============================================
DECLARE @AdminEmail2 NVARCHAR(256) = 'admin@diennha.local';
DECLARE @AdminUserId2 NVARCHAR(450);
DECLARE @AdminRoleId NVARCHAR(450);

SELECT @AdminUserId2 = [Id] FROM [AspNetUsers] WHERE [Email] = @AdminEmail2;
SELECT @AdminRoleId = [Id] FROM [AspNetRoles] WHERE [Name] = 'Admin';

IF @AdminUserId2 IS NOT NULL AND @AdminRoleId IS NOT NULL
BEGIN
    -- Kiểm tra xem đã có role chưa
    IF NOT EXISTS (
        SELECT 1 FROM [AspNetUserRoles] 
        WHERE [UserId] = @AdminUserId2 AND [RoleId] = @AdminRoleId
    )
    BEGIN
        INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
        VALUES (@AdminUserId2, @AdminRoleId);
        PRINT N'✓ Đã gán Role "Admin" cho tài khoản';
    END
    ELSE
    BEGIN
        PRINT N'✓ Tài khoản đã có Role "Admin"';
    END
END
GO

-- =============================================
-- 4. Hiển thị thông tin tài khoản Admin
-- =============================================
PRINT N'';
PRINT N'========================================';
PRINT N'THÔNG TIN TÀI KHOẢN ADMIN:';
PRINT N'========================================';

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
    END AS [IsAdmin],
    u.[LockoutEnabled],
    u.[LockoutEnd],
    u.[AccessFailedCount]
FROM [AspNetUsers] u
WHERE u.[Email] = 'admin@diennha.local' OR u.[PhoneNumber] = '0900000000';

PRINT N'';
PRINT N'========================================';
PRINT N'HƯỚNG DẪN ĐĂNG NHẬP:';
PRINT N'========================================';
PRINT N'1. Truy cập: /Account/Login';
PRINT N'2. Số điện thoại: 0900000000';
PRINT N'3. Mật khẩu: Admin!23456';
PRINT N'';
PRINT N'⚠ Nếu không đăng nhập được, có thể password hash chưa được set.';
PRINT N'   Sử dụng endpoint: /Account/ResetPassword?email=admin@diennha.local&newPassword=Admin!23456';
PRINT N'   (Chỉ hoạt động trong môi trường Development)';
PRINT N'';
GO

