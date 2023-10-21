using Demo.BLL;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Data;
using Demo.DAL.Models;
using Demo.PL.Extensions;
using Demo.PL.Helpers;
using Demo.PL.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region configure services that allow dependancy injection


            //Dependancy Injection
            builder.Services.AddControllersWithViews(); // register mvc services to the container
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            }, ServiceLifetime.Scoped); // Scoped => when request end ... object of database will be killed       
            //services.AddApplicationServices(); //Extension Method 
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));

            ///Add DI for security services + add configration ??? ?? ??????  => ?? ???? ???? ???? ?????? ??????? ??????? 
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.Password.RequiredUniqueChars = 2; //minimum number uniq is 2
                config.Password.RequireDigit = true; //default
                config.Password.RequireNonAlphanumeric = true; //default
                config.Password.RequiredLength = 5;
                config.Password.RequireUppercase = true;
                config.Password.RequireLowercase = true;

                config.User.RequireUniqueEmail = true;

                config.Lockout.MaxFailedAccessAttempts = 3;
                config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(10);
                config.Lockout.AllowedForNewUsers = true;


            })
                    .AddEntityFrameworkStores<AppDbContext>() //contain ef code
                     .AddDefaultTokenProviders(); //Add services to create token only to reset pass or change eamil..etc 

            ///services.AddScoped<UserManager<ApplicationUser>>();
            ///services.AddScoped<SignInManager<ApplicationUser>>();
            ///services.AddScoped<RoleManager<IdentityRole >>();

            //default schema is Cookies => if you have token => go to Login
            //if you want to change => LoginPath for Cookies :

            builder.Services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Auth/SignIn";
                config.ExpireTimeSpan = TimeSpan.FromMinutes(10);

            });
            //services.AddAuthentication("Cookies"); // Register services requored by Authentication services => called entrly => to create schema of token => each token have schema
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(config =>
                    {
                        config.LoginPath = "/Auth/SignIn";
                        config.AccessDeniedPath = "/Home/Error";
                    });

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("MailSettings"));

            builder.Services.AddTransient<IMailSettings, EmailSettings>();

            builder.Services.Configure<TwillioSettings>(builder.Configuration.GetSection("Twillio"));
			
            builder.Services.AddTransient<ISmsMessage, SmsSettings>();
			//google Authentication
			builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            }
            ).AddGoogle(o=>
            {
                IConfiguration GoogleAuthSection = builder.Configuration.GetSection("Authentication:Google");
                o.ClientId = GoogleAuthSection["ClientId"];
                o.ClientSecret = GoogleAuthSection["ClientSecret"];

			}); 
			#endregion

			#region BIBLINES
			var app = builder.Build();
            #endregion

            #region Configure Http Request Pipline

            var env = builder.Environment;
          
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
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
            ///run project :
            /// 1. "/Home/Index" => [Authorize]
            /// 2. Validate that you have token from =>app.UseAuthentication(); and app.UseAuthorization();
            /// 3. if yo have token => applay => "/Auth/SignIn"
            /// services.AddAuthentication("Cookies") 
            ///         .AddCookie("Admin", config =>
            ///	         {
            ///	         config.LoginPath = "/Auth/SignIn"; 
            ///           config.AccessDeniedPath = "/Home/Error"; });
            ///           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            #endregion

            #region Run App
            app.Run();
            #endregion

        }


    }
}
