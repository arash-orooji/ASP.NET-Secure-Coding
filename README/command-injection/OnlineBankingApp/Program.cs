using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineBankingApp.Data;
using OnlineBankingApp.Models;
using OnlineBankingApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<SecureBackupService>();
builder.Services.AddScoped<UnsecureBackupService>();

void ConfigureDatabase(DbContextOptionsBuilder options)
{
    var configuration = builder.Configuration;
    var connectionString = configuration.GetConnectionString("OnlineBankingAppContext")
        ?? throw new InvalidOperationException("Connection string 'OnlineBankingAppContext' was not found.");

    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
}

builder.Services.AddDbContextFactory<OnlineBankingAppContext>(ConfigureDatabase);
builder.Services.AddScoped(sp => sp.GetRequiredService<IDbContextFactory<OnlineBankingAppContext>>().CreateDbContext());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Startup");
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();