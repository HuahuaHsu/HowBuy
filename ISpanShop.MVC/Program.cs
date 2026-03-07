using ISpanShop.Models.EfModels;
using ISpanShop.Repositories.Implementations;
using ISpanShop.Repositories.Interfaces;
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

			// 這裡的 "DefaultConnection" 必須跟 appsettings.json或appsettings.Development裡的名字一模一樣
			builder.Services.AddDbContext<ISpanShopDBContext>
				(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
				);
			builder.Services.AddScoped<IMemberRepository, MemberRepository>();
			builder.Services.AddScoped<MemberService>();


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
