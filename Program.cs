using SpeakerExpert.Business.Services;
using SpeakerExpert.Data.Repositories;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});


builder.Services.AddScoped<ISpeakerRepository>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connStr = config.GetConnectionString("SpeakerExpertDB");
    return new SpeakerRepository(connStr);
});

builder.Services.AddScoped<ISpeakerService, SpeakerService>();

var app = builder.Build();
var cultureInfo = new CultureInfo("nl-NL");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
