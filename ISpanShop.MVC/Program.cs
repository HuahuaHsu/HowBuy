using ISpanShop.Models.EfModels;
using ISpanShop.Services;
using Microsoft.EntityFrameworkCore;


namespace ISpanShop.MVC
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			// �o�̪� "DefaultConnection" ������z appsettings.json��appsettings.Development�̪��W�r�@�Ҥ@��
			builder.Services.AddDbContext<ISpanShopDBContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			//2�� �̿�`�J (Dependency Injection)�G�O�ѤF�b Program.cs �����U�o�� Service�A�_�h����ɷ|�����G

			builder.Services.AddScoped<PointService>();
			builder.Services.AddScoped<PaymentService>();
			builder.Services.AddScoped<CheckoutService>();
			builder.Services.AddScoped<NewebPayService>();


			//2�� ���a


			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
