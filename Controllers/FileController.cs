using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using FileShare.Models;
using FileShare.Services;
using System.Transactions;
using NCrontab;
using Hangfire;

namespace FileShare.Controllers
{
    [Route("api/v1/file")]
    [ApiController]
    public class FileController
    {
        private readonly IFileProvider _fileProvider;
        private readonly IWebHostEnvironment _env;
        private readonly DatabaseContext _context;
        private readonly Deleter _deleter;
        public FileController(IFileProvider fileProvider,
                              IWebHostEnvironment env,
                              DatabaseContext context,
                              Deleter deleter)
        {
            _fileProvider = fileProvider;
            _env = env;
            _context = context;
            _deleter = deleter;
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

            byte[] salt = SodiumLibrary.CreateSalt();
            var hashedPassword = SodiumLibrary.HashPassword(password, salt);

            CrontabSchedule schedule = CrontabSchedule.Parse("* * * * *");           

            CustomFile newFile = new CustomFile
            {
                Path = filePath,
                Salt = salt,
                PasswordToDel = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                DelTime = schedule.GetNextOccurrence(DateTime.UtcNow)
            };

            _context.Files.Add(newFile);
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
            return Results.Ok(new
            {
                Data = _context.Files.ToList(),
                StatusCode = StatusCodes.Status200OK,
                Success = true
            }
            );
        }

        [HttpPost("deleteFile")]
        public async Task<IResult> DeleteFileWithPassword(FileDeleteDto deleteRequest)
        {
            var fileToDelete = await _context.Files.FirstOrDefaultAsync(file => file.Id == deleteRequest.Id);
            if (fileToDelete == null || !File.Exists(fileToDelete.Path)) return Results.NotFound();

            var isVerified = SodiumLibrary.VerifyPassword(deleteRequest.PasswordToDel, fileToDelete.Salt, fileToDelete.PasswordToDel);
            if (!isVerified) return Results.BadRequest();

            await _deleter.DeleteFileAsync(fileToDelete);
            return Results.Ok();
        }
    }
}
