using ISpanShop.Models.EfModels;
using ISpanShop.Models.Seeding;
using ISpanShop.MVC.Middleware;

// Repository namespaces
using ISpanShop.Repositories.Admins;
using ISpanShop.Repositories.Members;
using ISpanShop.Repositories.Members.Implementations; // 補上實作命名空間
using ISpanShop.Repositories.Products;
using ISpanShop.Repositories.Categories;
using ISpanShop.Repositories.Orders;
using ISpanShop.Repositories.Inventories;
using ISpanShop.Repositories.ContentModeration;
using ISpanShop.Repositories.Support;
using ISpanShop.Repositories.Stores;
using ISpanShop.Repositories.Promotions;
using ISpanShop.Repositories.Brands;

// Service namespaces
using ISpanShop.Services.Admins;
using ISpanShop.Services.Members;
using ISpanShop.Services.Products;
using ISpanShop.Services.Categories;
using ISpanShop.Services.Orders;
using ISpanShop.Services.Inventories;
using ISpanShop.Services.ContentModeration;
using ISpanShop.Services.Support;
using ISpanShop.Services.Payments;
using ISpanShop.Services.Stores;
using ISpanShop.Services.Promotions;
using ISpanShop.Services.Brands;
using ISpanShop.Services.Coupons;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ISpanShop.Services.Auth;
using ISpanShop.Services;

namespace ISpanShop.MVC
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// 1. 註冊控制器與視圖
			builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IFrontAuthService, FrontAuthService>();

			// 2. CORS 跨域配置
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("FrontendPolicy", policy =>
				{
					policy.WithOrigins("http://localhost:5173") 
						  .AllowAnyHeader()
						  .AllowAnyMethod()
						  .AllowCredentials();
				});
			});

			// 3. 資料庫連線註冊
			builder.Services.AddDbContext<ISpanShopDBContext>(options => 
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
				sqlServerOptionsAction: sqlOptions =>
				{
					sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
				}));

			// 4. 身份驗證 (Cookie 與 JWT 徹底分離)
			var jwtSettings = builder.Configuration.GetSection("Jwt");
			builder.Services.AddAuthentication(options => {
				options.DefaultScheme = "AdminCookieAuth";
			})
			.AddCookie("AdminCookieAuth", options =>
			{
				options.Cookie.Name = "ISpanShop.Admin.Session";
				options.LoginPath = "/Admin/Auth/Login";
				options.AccessDeniedPath = "/Admin/Auth/AccessDenied";
				options.ExpireTimeSpan = TimeSpan.FromDays(7);
			})
			.AddJwtBearer("FrontendJwt", options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings["Issuer"],
					ValidAudience = jwtSettings["Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
				};
			});

			// 5. 註冊所有倉儲層 (Repositories)
			builder.Services.AddScoped<IMemberRepository, MemberRepository>();
			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<IAdminRepository, AdminRepository>();
			builder.Services.AddScoped<IAdminRoleRepository, AdminRoleRepository>();
			builder.Services.AddScoped<ILoginHistoryRepository, LoginHistoryRepository>();
			builder.Services.AddScoped<IProductRepository, ProductRepository>();
			builder.Services.AddScoped<ICategoryAttributeRepository, CategoryAttributeRepository>();
			builder.Services.AddScoped<ICategoryManageRepository, CategoryManageRepository>();
			builder.Services.AddScoped<IOrderRepository, OrderRepository>();
			builder.Services.AddScoped<IOrderReviewRepository, OrderReviewRepository>();
			builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
			builder.Services.AddScoped<IPointRepository, PointRepository>();
			builder.Services.AddScoped<ISensitiveWordRepository, SensitiveWordRepository>();
			builder.Services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
			builder.Services.AddScoped<IStoreRepository, StoreRepository>();
			builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
			builder.Services.AddScoped<IBrandRepository, BrandRepository>();

			// 6. 註冊所有服務層 (Services)
			builder.Services.AddScoped<IMemberService, MemberService>();
			builder.Services.AddScoped<IAdminService, AdminService>();
			builder.Services.AddScoped<ILoginHistoryService, LoginHistoryService>();
			builder.Services.AddScoped<IProductService, ProductService>();
			builder.Services.AddScoped<CategoryAttributeService>();
			builder.Services.AddScoped<CategoryManageService>();
			builder.Services.AddScoped<IOrderService, OrderService>();
			builder.Services.AddScoped<IFrontOrderService, FrontOrderService>();
			builder.Services.AddScoped<IOrderDashboardService, OrderDashboardService>();
			builder.Services.AddScoped<IOrderReviewService, OrderReviewService>();
			builder.Services.AddScoped<IInventoryService, InventoryService>();
			builder.Services.AddScoped<PointService>();
			builder.Services.AddScoped<PaymentService>();
			builder.Services.AddScoped<CheckoutService>();
			builder.Services.AddScoped<NewebPayService>();
			builder.Services.AddScoped<ISensitiveWordService, SensitiveWordService>();
			builder.Services.AddScoped<ISupportTicketService, SupportTicketService>();
			builder.Services.AddScoped<IStoreService, StoreService>();
			builder.Services.AddScoped<IFrontStoreService, FrontStoreService>();
			builder.Services.AddScoped<PromotionService>();
			builder.Services.AddScoped<BrandService>();
			builder.Services.AddScoped<ICouponService, CouponService>();
			builder.Services.AddHostedService<CouponCleanupService>();

			// 7. Swagger / OpenAPI
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "ISpanShop API", Version = "v1" });
			});

			var app = builder.Build();

			// 8. 中間件管線 (Middleware Pipeline)
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();

			// CORS 必須在 Routing 與 Auth 之前
			app.UseCors("FrontendPolicy");

			app.UseMiddleware<ExceptionHandlingMiddleware>();

			app.UseAuthentication(); 
			app.UseAuthorization();

			// 9. 路由映射
			app.MapAreaControllerRoute(
				name: "admin",
				areaName: "Admin",
				pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Orders}/{action=Dashboard}/{id?}",
				defaults: new { area = "Admin" });

			app.MapControllers();

			// 10. 種子資料與資料庫檢查
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var context = services.GetRequiredService<ISpanShopDBContext>();
				await DataSeeder.SeedAsync(context);
				await DataSeeder.EnsureAdminUserAsync(context);
			}

			app.Run();
		}
	}
}
