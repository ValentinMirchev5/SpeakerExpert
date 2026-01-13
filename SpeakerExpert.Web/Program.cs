using Microsoft.AspNetCore.Authentication.Cookies;
using SpeakerExpert.Business.Interfaces;
using SpeakerExpert.Business.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "EmployeeOnly");
    options.Conventions.AuthorizePage("/Shop/Checkout");
});

builder.Services.AddSession();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmployeeOnly", policy =>
        policy.RequireRole("Employee"));
});



// Data helpers
builder.Services.AddScoped<SpeakerExpert.Data.Repositories.Db>();

// Repositories (Data implements Business interfaces)
builder.Services.AddScoped<SpeakerExpert.Business.Interfaces.ISpeakerRepository, SpeakerExpert.Data.Repositories.SpeakerRepository>();
builder.Services.AddScoped<SpeakerExpert.Business.Interfaces.IUserRepository, SpeakerExpert.Data.Repositories.UserRepository>();
builder.Services.AddScoped<SpeakerExpert.Business.Interfaces.IOrderRepository,SpeakerExpert.Data.Repositories.OrderRepository>();
builder.Services.AddScoped<IReviewRepository, SpeakerExpert.Data.Repositories.ReviewRepository>();
builder.Services.AddScoped<IReviewService, SpeakerExpert.Business.Services.ReviewService>();



// Services
builder.Services.AddScoped<SpeakerExpert.Business.Interfaces.ISpeakerService, SpeakerExpert.Business.Services.SpeakerService>();
builder.Services.AddScoped<SpeakerExpert.Business.Interfaces.IAuthService, SpeakerExpert.Business.Services.AuthService>();
builder.Services.AddScoped<SpeakerExpert.Business.Interfaces.IOrderService,SpeakerExpert.Business.Services.OrderService>();
builder.Services.AddScoped<SpeakerExpert.Business.Interfaces.ICartService,SpeakerExpert.Business.Services.CartService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

