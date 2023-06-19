using Microsoft.Extensions.FileProviders;
using FileShare.Models;
using FileShare.Services;
using Microsoft.EntityFrameworkCore;
using Hangfire.PostgreSql;
using Hangfire;

DotNetEnv.Env.Load("./.env");
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
var env = Environment.GetEnvironmentVariables();
string connection =
$"""
    Host={env["DB_HOST"]};
    Port={env["DB_PORT"]};
    Database={env["DB_DATABASE"]};
    Username={env["DB_USERNAME"]};
    Password={env["DB_PASSWORD"]};
""";

builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(connection));
// builder.Services.AddHangfire(configuration => configuration.UsePostgreSqlStorage(connection));
builder.Services.AddSingleton<Deleter>();

builder.Services.AddControllers();
// builder.Services.AddHostedService<FileCleanupTask>();
// builder.Services.AddScoped<FileCleanupTask>();
var app = builder.Build();

// app.UseHangfireDashboard();
// app.UseHangfireServer();

app.MapControllers();

var environ = app.Services.GetService<IWebHostEnvironment>();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(environ!.ContentRootPath, "files")),
    RequestPath = "/api/v1/files"
});

app.Run();
