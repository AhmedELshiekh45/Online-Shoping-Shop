using BussinessLayer.Repos;
using DataAccess;
using DataAccess.Constaats;
using DataAccess.Context;
using DataAccess.Model;
using DataAccess.Repos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceLayer;
using ServiceLayer.Repos;
using Services_Layer.Services;
using Stripe;
using System.Reflection;





var builder = WebApplication.CreateBuilder(args);
// Configure Stripe settings from appsettings.json

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MyDbContext>(
    opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<User, IdentityRole>(
    p =>
    {
        p.Password.RequiredLength = 5;
        p.Password.RequireUppercase = false;
        p.Password.RequireLowercase = true;
        p.Password.RequireDigit = false;
        p.Password.RequireNonAlphanumeric = false;
        
    }

    ).AddEntityFrameworkStores<MyDbContext>();
builder.Services.AddScoped<MyDbContext>();
builder.Services.AddScoped<RoleRepo>();
builder.Services.AddScoped<IdentityRole>();
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<OrderRepo>();
builder.Services.AddScoped<Services_Layer.Services.ProductService>();
builder.Services.AddScoped<ShopingCartRepo>();
builder.Services.AddScoped<ShopingCart>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
//using for useing auto mapper on all classes not eavery on i will register it 
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());




var app = builder.Build();
using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    context.Database.Migrate();
    context.SeedData();
}
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
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
