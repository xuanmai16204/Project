using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Data
{
	public interface IDbInitializer
	{
		Task InitializeAsync();
	}

	public class DbInitializer : IDbInitializer
	{
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_db = db;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task InitializeAsync()
		{
			await _db.Database.MigrateAsync();

			// Roles
			if (!await _roleManager.RoleExistsAsync(SD.RoleAdmin))
			{
				await _roleManager.CreateAsync(new IdentityRole(SD.RoleAdmin));
			}
			if (!await _roleManager.RoleExistsAsync(SD.RoleEmployee))
			{
				await _roleManager.CreateAsync(new IdentityRole(SD.RoleEmployee));
			}
			if (!await _roleManager.RoleExistsAsync(SD.RoleCustomer))
			{
				await _roleManager.CreateAsync(new IdentityRole(SD.RoleCustomer));
			}

			// Admin user (password should be set via secrets or environment)
			var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@diennha.local";
			var adminPass = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "Admin!23456"; // dev only
			// Sử dụng FirstOrDefaultAsync thay vì FindByEmailAsync để tránh lỗi khi có nhiều user cùng email
			var admin = await _db.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
			if (admin == null)
			{
				admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
				var createResult = await _userManager.CreateAsync(admin, adminPass);
				if (createResult.Succeeded)
				{
					await _userManager.AddToRoleAsync(admin, SD.RoleAdmin);
				}
			}
			else
			{
				// Đảm bảo admin có role Admin nếu đã tồn tại
				if (!await _userManager.IsInRoleAsync(admin, SD.RoleAdmin))
				{
					await _userManager.AddToRoleAsync(admin, SD.RoleAdmin);
				}
			}

			// Sample Customer user for testing
			var customerEmail = "khachhang@gmail.com";
			var customerPass = "123456"; // Simple password for testing
			// Sử dụng FirstOrDefaultAsync thay vì FindByEmailAsync để tránh lỗi khi có nhiều user cùng email
			var customer = await _db.Users.FirstOrDefaultAsync(u => u.Email == customerEmail);
			if (customer == null)
			{
				customer = new ApplicationUser { UserName = customerEmail, Email = customerEmail, EmailConfirmed = true };
				var createCustomerResult = await _userManager.CreateAsync(customer, customerPass);
				if (createCustomerResult.Succeeded)
				{
					await _userManager.AddToRoleAsync(customer, SD.RoleCustomer);
				}
			}
			else
			{
				// Đảm bảo customer có role Customer nếu đã tồn tại
				if (!await _userManager.IsInRoleAsync(customer, SD.RoleCustomer))
				{
					await _userManager.AddToRoleAsync(customer, SD.RoleCustomer);
				}
			}

			// Basic categories/products seed (idempotent)
			if (!await _db.Categories.AnyAsync())
			{
				_db.Categories.AddRange(
					new Category { Name = "Công tắc", Slug = "cong-tac" },
					new Category { Name = "Ổ cắm", Slug = "o-cam" },
					new Category { Name = "Đèn LED", Slug = "den-led" }
				);
				await _db.SaveChangesAsync();
			}

			if (!await _db.Products.AnyAsync())
			{
				var cat = await _db.Categories.FirstAsync();
				_db.Products.AddRange(
					new Product { Name = "Công tắc 1 chiều", CategoryId = cat.Id, Price = 50000, Stock = 100, Slug = "cong-tac-1-chieu" },
					new Product { Name = "Ổ cắm 3 chấu", CategoryId = cat.Id, Price = 70000, Stock = 80, Slug = "o-cam-3-chau" }
				);
				await _db.SaveChangesAsync();
			}
		}
	}
}


