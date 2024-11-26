using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using EventureMVC.Models;
using System.Net.Http.Headers;
using EventureMVC.Middleware;

namespace EventureMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            
            // Fetch Base URL from configuration
            var baseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl");

            //builder.Services.AddHttpClient();
            builder.Services.AddHttpClient("APIClient", client =>
            {
                client.BaseAddress = new Uri(baseUrl); 
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/User/Login";
                options.LogoutPath = "/User/Logout";
                options.AccessDeniedPath = "/";

                // Cookie expiration settings
                options.ExpireTimeSpan = TimeSpan.FromDays(30); // Set this to a reasonable expiration time
                options.SlidingExpiration = true; // Optional: Automatically refresh the expiration time
            });
           


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

            app.UseSession();
            app.UseMiddleware<RoleBasedAccessMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

    }
}
