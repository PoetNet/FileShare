using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

var app = builder.Build();
app.MapControllers();

var environ = app.Services.GetService<IWebHostEnvironment>();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(environ!.ContentRootPath, "files")),
    RequestPath = "/images"
});

app.Run();
