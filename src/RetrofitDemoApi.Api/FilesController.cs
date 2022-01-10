using Microsoft.AspNetCore.Mvc;

namespace RetrofitDemoApi.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public FilesController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (InMB(file.Length) > 5L)
            {
                return BadRequest();
            }

            var extension = Path.GetExtension(file.FileName);
            var fileName = Path.GetRandomFileName();
            var newName = Path.ChangeExtension(fileName, extension);
            var filePath = Path.Combine(_environment.WebRootPath, newName);
            using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            return Ok(new { fileName = newName });

            static long InMB(long value)
            {
                return value / 1000000L;
            }
        }
    }
}