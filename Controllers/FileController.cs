using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace FileShare.Controllers
{
    [Route("api/v1/file")]
    [ApiController]
    public class FileController
    {
        private readonly IFileProvider _fileProvider;
        private readonly IWebHostEnvironment _env;
        public FileController(IFileProvider fileProvider,
                              IWebHostEnvironment env)
        {
            _fileProvider = fileProvider;
            _env = env;
        }

        [HttpPost("uploadFile")]
        public async Task<IResult> UploadFiles(IFormFile file)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(_env.ContentRootPath, "files", fileName);
        }
    }
}
