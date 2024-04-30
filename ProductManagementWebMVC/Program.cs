using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductManagementWebMVC.Data;
using System.Globalization;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ProductManagementWebMVCContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ProductManagementWebMVCContext") ?? throw new InvalidOperationException("Connection string 'ProductManagementWebMVCContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Language Config
var euUS = new CultureInfo("en-US");
var localizationSettings = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(euUS),
    SupportedCultures = [euUS],
    SupportedUICultures = [euUS]
};

app.UseRequestLocalization(localizationSettings);

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