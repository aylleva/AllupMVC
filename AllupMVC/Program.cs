using AllupMVC.DAL;
using AllupMVC.Models;
using AllupMVC.Services.Implementations;
using AllupMVC.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AllupMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDBContext>(opt=>
            
            opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
            
            );

            builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
            { 
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 8;
                opt.User.RequireUniqueEmail = true;

                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                opt.Lockout.MaxFailedAccessAttempts = 3;
            
            }
            ).AddEntityFrameworkStores<AppDBContext>().AddDefaultTokenProviders();

            builder.Services.AddScoped<ILayoutServices,LayoutServices>();
            builder.Services.AddScoped<IBasketService, BasketService>();    

            var app = builder.Build();

            app.UseStaticFiles();
            
            app.UseAuthentication();
            app.UseAuthorization(); 

            app.MapControllerRoute(
                             "admin",
                             "{area:exists}/{controller=home}/{action=index}/{Id?}"

                            );
            app.MapControllerRoute(
                 "default",
                 "{controller=home}/{action=index}/{Id?}"

                );

            app.Run();
        }
    }
}
