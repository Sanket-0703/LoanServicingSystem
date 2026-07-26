using System.Data;
using ApexCharts;
using CoreData;
using CoreData.Dashboard;
using CoreData.Dashboard.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;


namespace Loan_Servicing_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Add Authentication Services
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
     .AddCookie(options =>
     {
         options.Cookie.Name = "LoanServicingAuth";
         options.LoginPath = "/login";
         options.LogoutPath = "/logout";
         options.AccessDeniedPath = "/unauthorized";
         options.ExpireTimeSpan = TimeSpan.FromHours(8);
     });

            builder.Services.AddAuthorization();


            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Adding Razor Pages and Blazor Server services
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
            builder.Services.AddApexCharts();

            // Register database connection
            builder.Services.AddTransient<IDatabaseConnection>(db =>
                new DatabaseConnection(builder.Configuration.GetConnectionString("LSSConnection")));

            // DB Coonection
            builder.Services.AddScoped<IDbConnection>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("LSSConnection")
                                       ?? throw new InvalidOperationException("Connection string 'LSSConnection' not found.");
                return new SqlConnection(connectionString);
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAntiforgery();

            // Map your root component (standard for modern Blazor templates)
            app.MapRazorComponents<Loan_Servicing_System.Components.App>()
                .AddInteractiveServerRenderMode();

            app.MapGet("/logout", async context =>
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                context.Response.Redirect("/login");
            });

            app.Run();
        }
    }
}
