using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Book_Store.Data;
using Book_Store.Models;

namespace Book_Store
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			
			builder.Services.AddDbContext<BookStoreContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("Book_StoreContext")
				?? throw new InvalidOperationException("Connection string 'Book_StoreContext' not found.")));

			builder.Services.AddDefaultIdentity<IdentityUser>(options =>
			{
				options.SignIn.RequireConfirmedAccount = true;
			}).AddEntityFrameworkStores<BookStoreContext>();

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // Register IHttpContextAccessor
            builder.Services.AddSession();  // Enable session support
            builder.Services.AddControllersWithViews();


            builder.Services.AddSession(options =>
			{
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			
			builder.Services.AddRazorPages();

			builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			builder.Services.AddScoped<Cart>(sp => Cart.GetCart(sp));

			builder.Services.AddControllersWithViews();

			var app = builder.Build();

			
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}
            


            app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();
			app.UseSession();

			
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Store}/{action=Index}/{id?}");

			
			app.MapRazorPages();

			
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					SetData.Initialize(services);
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred while attempting to seed the database");
				}
			}

			app.Run();
		}
	}
}
