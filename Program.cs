using PBL3Hos.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace FIlePBL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            var connectionString = builder.Configuration.GetConnectionString("default");

            // Add services to the container.

            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(connectionString));
            builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Admin",
                    areaName: "Admin",
                    pattern: "Admin/{controller=User}/{action=Index}"
                    );
                endpoints.MapControllerRoute(
                   name: "areaRoute",

                   pattern: "{area:exists}/{controller}/{action}/{id?}"
                   );
                endpoints.MapControllerRoute(
                   name: "default",

                   pattern: "{controller=Home}/{action=Index}/{id?}"
                   );
            }
                );


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "Manager", "Customer", "Doctor" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                string email = "admin@admin.com";
                string password = "Test1234,";
                if (await userManager.FindByEmailAsync(email)==null)
                {
                    var user = new AppUser();
                    user.UserName=email;
                    user.Email=email;
                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Admin");

                }
            }
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext=services.GetRequiredService<AppDbContext>();
                dbContext.Appointments.RemoveRange(dbContext.Appointments.Where(x => x.Date<DateTime.Today));
                dbContext.SaveChanges();
            }


            app.Run();
        }
    }

}
