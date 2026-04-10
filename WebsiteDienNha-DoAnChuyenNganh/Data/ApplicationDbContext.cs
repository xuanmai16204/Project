using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Category> Categories { get; set; } = null!;
		public DbSet<Product> Products { get; set; } = null!;
		public DbSet<Cart> Carts { get; set; } = null!;
		public DbSet<CartItem> CartItems { get; set; } = null!;
		public DbSet<Order> Orders { get; set; } = null!;
		public DbSet<OrderItem> OrderItems { get; set; } = null!;
		public DbSet<LoyaltyLevel> LoyaltyLevels { get; set; } = null!;
		public DbSet<ProductImage> ProductImages { get; set; } = null!;
		public DbSet<DiscountCode> DiscountCodes { get; set; } = null!;
		public DbSet<RewardPointTransaction> RewardPointTransactions { get; set; } = null!;
		public DbSet<GameResult> GameResults { get; set; } = null!;
		public DbSet<Notification> Notifications { get; set; } = null!;
		public DbSet<SystemNotification> SystemNotifications { get; set; } = null!;
		public DbSet<NotificationRecipient> NotificationRecipients { get; set; } = null!;
		public DbSet<GuestNotification> GuestNotifications { get; set; } = null!;
		public DbSet<ShippingInfo> ShippingInfos { get; set; } = null!;
		public DbSet<CustomerFeedback> CustomerFeedbacks { get; set; } = null!;
		public DbSet<Promotion> Promotions { get; set; } = null!;
		public DbSet<PromotionProduct> PromotionProducts { get; set; } = null!;
		public DbSet<PromotionUsage> PromotionUsages { get; set; } = null!;
		public DbSet<EmailCampaign> EmailCampaigns { get; set; } = null!;
		public DbSet<EmailTemplate> EmailTemplates { get; set; } = null!;
		public DbSet<EmailRecipient> EmailRecipients { get; set; } = null!;
		public DbSet<ChatConversation> ChatConversations { get; set; } = null!;
		public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
		public DbSet<Employee> Employees { get; set; } = null!;
		public DbSet<EmployeePerformance> EmployeePerformances { get; set; } = null!;
		public DbSet<Role> BusinessRoles { get; set; } = null!;
		public DbSet<Permission> Permissions { get; set; } = null!;
		public DbSet<RolePermission> RolePermissions { get; set; } = null!;
		public DbSet<UserRole> BusinessUserRoles { get; set; } = null!;
		public DbSet<SystemLog> SystemLogs { get; set; } = null!;
		public DbSet<ExportReport> ExportReports { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Category>()
				.HasIndex(c => c.Slug)
				.IsUnique(false);

		builder.Entity<Product>()
			.Property(p => p.Price)
			.HasPrecision(18, 2);

		builder.Entity<Product>()
			.Property(p => p.PromotionPrice)
			.HasPrecision(18, 2);

			builder.Entity<Product>()
				.HasOne(p => p.Category)
				.WithMany()
				.HasForeignKey(p => p.CategoryId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Product>()
				.HasMany(p => p.Images)
				.WithOne(i => i.Product)
				.HasForeignKey(i => i.ProductId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Product>()
				.HasMany(p => p.PromotionProducts)
				.WithOne(pp => pp.Product)
				.HasForeignKey(pp => pp.ProductId);

			builder.Entity<CartItem>()
				.Property(ci => ci.UnitPrice)
				.HasPrecision(18, 2);

			builder.Entity<CartItem>()
				.HasOne(ci => ci.Cart)
				.WithMany(c => c.Items)
				.HasForeignKey(ci => ci.CartId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<CartItem>()
				.HasOne(ci => ci.Product)
				.WithMany()
				.HasForeignKey(ci => ci.ProductId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<OrderItem>()
				.Property(oi => oi.UnitPrice)
				.HasPrecision(18, 2);

			builder.Entity<OrderItem>()
				.HasOne(oi => oi.Order)
				.WithMany(o => o.Items)
				.HasForeignKey(oi => oi.OrderId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<OrderItem>()
				.HasOne(oi => oi.Product)
				.WithMany()
				.HasForeignKey(oi => oi.ProductId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Order>()
				.Property(o => o.Total)
				.HasPrecision(18, 2);

			builder.Entity<Order>()
				.Property(o => o.PaymentMethod)
				.HasConversion<int>(); // Store enum as int in database

			builder.Entity<Order>()
				.HasMany(o => o.PromotionUsages)
				.WithOne(pu => pu.Order)
				.HasForeignKey(pu => pu.OrderId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<ShippingInfo>()
				.HasOne(si => si.Order)
				.WithOne(o => o.ShippingInfo)
				.HasForeignKey<ShippingInfo>(si => si.OrderId);

			builder.Entity<ShippingInfo>()
				.HasOne(si => si.User)
				.WithMany(u => u.ShippingInfos)
				.HasForeignKey(si => si.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<LoyaltyLevel>()
				.HasMany<ApplicationUser>()
				.WithOne(u => u.LoyaltyLevel)
				.HasForeignKey(u => u.LoyaltyLevelId);

			builder.Entity<RewardPointTransaction>()
				.HasOne(r => r.User)
				.WithMany(u => u.RewardTransactions)
				.HasForeignKey(r => r.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<RewardPointTransaction>()
				.HasOne(r => r.Order)
				.WithMany()
				.HasForeignKey(r => r.OrderId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Notification>()
				.HasOne(n => n.User)
				.WithMany(u => u.Notifications)
				.HasForeignKey(n => n.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<NotificationRecipient>()
				.HasOne(nr => nr.SystemNotification)
				.WithMany(sn => sn.Recipients)
				.HasForeignKey(nr => nr.SystemNotificationId);

			builder.Entity<NotificationRecipient>()
				.HasOne(nr => nr.User)
				.WithMany()
				.HasForeignKey(nr => nr.UserId);

			builder.Entity<CustomerFeedback>()
				.HasOne(cf => cf.Product)
				.WithMany(p => p.Feedbacks)
				.HasForeignKey(cf => cf.ProductId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<CustomerFeedback>()
				.HasOne(cf => cf.User)
				.WithMany()
				.HasForeignKey(cf => cf.UserId);

			builder.Entity<PromotionProduct>()
				.HasKey(pp => new { pp.PromotionId, pp.ProductId });

			builder.Entity<PromotionProduct>()
				.HasOne(pp => pp.Promotion)
				.WithMany(p => p.PromotionProducts)
				.HasForeignKey(pp => pp.PromotionId);

			builder.Entity<PromotionUsage>()
				.HasOne(pu => pu.Promotion)
				.WithMany(p => p.PromotionUsages)
				.HasForeignKey(pu => pu.PromotionId);

			builder.Entity<Promotion>()
				.Property(p => p.DiscountPercent)
				.HasPrecision(5, 2);

			builder.Entity<Promotion>()
				.Property(p => p.DiscountAmount)
				.HasPrecision(18, 2);

			builder.Entity<Promotion>()
				.Property(p => p.MaxDiscount)
				.HasPrecision(18, 2);

			builder.Entity<Promotion>()
				.Property(p => p.MinOrderTotal)
				.HasPrecision(18, 2);

			builder.Entity<DiscountCode>()
				.HasIndex(dc => dc.Code)
				.IsUnique();

			builder.Entity<DiscountCode>()
				.Property(dc => dc.DiscountAmount)
				.HasPrecision(18, 2);

			builder.Entity<DiscountCode>()
				.Property(dc => dc.DiscountPercent)
				.HasPrecision(5, 2);

			builder.Entity<EmailCampaign>()
				.HasMany(ec => ec.Recipients)
				.WithOne(r => r.Campaign)
				.HasForeignKey(r => r.CampaignId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<ApplicationUser>()
				.HasOne(u => u.EmployeeProfile)
				.WithOne(e => e.User)
				.HasForeignKey<Employee>(e => e.UserId);

			builder.Entity<Employee>()
				.HasMany(e => e.Performances)
				.WithOne(p => p.Employee)
				.HasForeignKey(p => p.EmployeeId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Role>()
				.HasIndex(r => r.Name)
				.IsUnique();

			builder.Entity<RolePermission>()
				.HasOne(rp => rp.Role)
				.WithMany(r => r.RolePermissions)
				.HasForeignKey(rp => rp.RoleId);

			builder.Entity<RolePermission>()
				.HasOne(rp => rp.Permission)
				.WithMany(p => p.RolePermissions)
				.HasForeignKey(rp => rp.PermissionId);

			builder.Entity<UserRole>()
				.HasOne(ur => ur.Role)
				.WithMany(r => r.UserRoles)
				.HasForeignKey(ur => ur.RoleId);

			builder.Entity<UserRole>()
				.HasOne(ur => ur.User)
				.WithMany()
				.HasForeignKey(ur => ur.UserId);

			builder.Entity<UserRole>()
				.HasIndex(ur => new { ur.UserId, ur.RoleId })
				.IsUnique();

			builder.Entity<ChatConversation>()
				.HasMany(c => c.Messages)
				.WithOne(m => m.Conversation)
				.HasForeignKey(m => m.ConversationId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<LoyaltyLevel>().HasData(
				new LoyaltyLevel { Id = 1, Name = "Seed", DiscountPercent = 0, ThresholdPoints = 0 },
				new LoyaltyLevel { Id = 2, Name = "Brew", DiscountPercent = 2, ThresholdPoints = 100 },
				new LoyaltyLevel { Id = 3, Name = "Roast", DiscountPercent = 4, ThresholdPoints = 300 },
				new LoyaltyLevel { Id = 4, Name = "Aroma", DiscountPercent = 6, ThresholdPoints = 600 },
				new LoyaltyLevel { Id = 5, Name = "Steam", DiscountPercent = 8, ThresholdPoints = 1000 },
				new LoyaltyLevel { Id = 6, Name = "Pour", DiscountPercent = 10, ThresholdPoints = 1500 },
				new LoyaltyLevel { Id = 7, Name = "Sip", DiscountPercent = 12, ThresholdPoints = 2100 },
				new LoyaltyLevel { Id = 8, Name = "Blend", DiscountPercent = 14, ThresholdPoints = 2800 },
				new LoyaltyLevel { Id = 9, Name = "Barista", DiscountPercent = 17, ThresholdPoints = 3600 },
				new LoyaltyLevel { Id = 10, Name = "Master", DiscountPercent = 20, ThresholdPoints = 4500 }
			);
		}
	}
}


