    using PBL3Hos.Identity;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System;
    using PBL3Hos.Models.DbModel;
using PBL3Hos.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Identity.Client;

namespace FIlePBL
    {
        public class Program
        {

            public static async Task Main(string[] args)
            {
                
                var builder = WebApplication.CreateBuilder(args);
                builder.Services.AddRazorPages();
                var connectionString = builder.Configuration.GetConnectionString("default");
                builder.Services.AddHttpContextAccessor();

                // Add services to the container.

                builder.Services.AddDbContext<AppDbContext>(
                    options => options.UseSqlServer(connectionString));
                builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            builder.Services.AddScoped<MyAuthenFilter>();
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

            app.UseAuthentication();
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
                    user.Fullname="Admin";
                        await userManager.CreateAsync(user, password);
                        await userManager.AddToRoleAsync(user, "Admin");

                    }
                }
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var dbContext = services.GetRequiredService<AppDbContext>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var AppointmentHistory = dbContext.Appointments.Where(x => x.Date<DateTime.Today).ToList();
                foreach(var item in AppointmentHistory)
                {
                    var appointmentHistory = new AppointmentHistory();
                    appointmentHistory.Date=item.Date;
                    appointmentHistory.DoctorId=item.DoctorId;
                    appointmentHistory.PatientId=item.PatientId;
                    dbContext.AppointmentHistory.Add(appointmentHistory);

                }
                // Lưu các thay đổi của AppointmentHistories
                dbContext.SaveChanges();
                // Xóa các appointment cũ
                dbContext.Appointments.RemoveRange(dbContext.Appointments.Where(x => x.Date<DateTime.Today));
                // Lưu các thay đổi của Appointments
                dbContext.SaveChanges();
            
                }


                app.Run();
            }

    }

    }
