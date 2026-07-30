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

            // =========================================
            // Authentication & Authorization
            // =========================================

            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "LoanServicingAuth";
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.AccessDeniedPath = "/unauthorized";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                });

            builder.Services.AddAuthorization();

            // =========================================
            // Blazor Services
            // =========================================

            builder.Services.AddRazorComponents()
                            .AddInteractiveServerComponents();

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            builder.Services.AddApexCharts();

            // =========================================
            // Application Services
            // =========================================

            builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

            builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

            builder.Services.AddTransient<IDatabaseConnection>(_ =>
                new DatabaseConnection(
                    builder.Configuration.GetConnectionString("LSSConnection")));

            builder.Services.AddScoped<IDbConnection>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

                var connectionString =
                    configuration.GetConnectionString("LSSConnection")
                    ?? throw new InvalidOperationException(
                        "Connection string 'LSSConnection' not found.");

                return new SqlConnection(connectionString);
            });

            var app = builder.Build();

            // =========================================
            // Middleware Pipeline
            // =========================================

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAntiforgery();

            // =========================================
            // Endpoint Mapping
            // =========================================

            app.MapRazorComponents<Loan_Servicing_System.Components.App>()
               .AddInteractiveServerRenderMode();

            app.MapGet("/logout", async context =>
            {
                await context.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);

                context.Response.Redirect("/login");
            });

            app.Run();
        }
    }
}