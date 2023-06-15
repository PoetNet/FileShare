using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using FileShare.Models;

namespace FileShare.Controllers
{
    [Route("api/v1/file")]
    [ApiController]
    public class FileController
    {
        private readonly IFileProvider _fileProvider;
        private readonly IWebHostEnvironment _env;
        private readonly DatabaseContext _context;
        public FileController(IFileProvider fileProvider,
                              IWebHostEnvironment env,
                              DatabaseContext context)
        {
            _fileProvider = fileProvider;
            _env = env;
            _context = context;
        }

        [HttpPost("uploadFile")]
        public async Task<IResult> UploadFiles(IFormFile file, [FromForm] string password)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(_env.ContentRootPath, "files", fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file!.CopyToAsync(fileStream);
            }

            _context.Files.Add(new CustomFile
            {
                Path = filePath,
                PasswordToDel = password
            });
            _context.SaveChanges();

            return Results.Ok();
        }

        [HttpGet("{filename}")]
        public IResult GetFile(string fileName)
        {
            var fileInfo = _fileProvider.GetFileInfo(fileName);
            if (!fileInfo.Exists) return Results.NotFound();

            return Results.File(fileInfo.PhysicalPath!);
        }

        [HttpGet]
        public IResult GetAllFiles()
        {
            return Results.Ok( new
                {
                    Data = _context.Files.ToList(),
                    StatusCode = StatusCodes.Status200OK,
                    Success = true
                }
            );
        }


        [HttpDelete("{id:Guid}/{password}")]
        public async Task<IResult> DeleteFile(Guid id, string password)
        {
            var fileToDelete = await _context.Files.FirstOrDefaultAsync(file => file.Id == id);

            if (fileToDelete == null) return Results.NotFound();
            if (password != fileToDelete.PasswordToDel) return Results.BadRequest();

            _context.Files.Remove(fileToDelete);
            _context.SaveChanges();

            return Results.Ok();
        }
    }
}
