using Microsoft.Extensions.FileProviders;
using FileShare.Models;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

var environ = app.Services.GetService<IWebHostEnvironment>();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(environ!.ContentRootPath, "files")),
    RequestPath = "/api/v1/files"
});

app.Run();
