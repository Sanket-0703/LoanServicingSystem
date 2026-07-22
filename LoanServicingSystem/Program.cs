using System.Data;
using Microsoft.Data.SqlClient;

namespace Loan_Servicing_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Adding Razor Pages and Blazor Server services
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            // DB Coonection
            builder.Services.AddScoped<IDbConnection>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                return new SqlConnection(connectionString);
            });

            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();

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

            app.Run();
        }
    }
}
