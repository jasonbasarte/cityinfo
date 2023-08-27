using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider) 
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ?? throw new System.ArgumentNullException(nameof(fileExtensionContentTypeProvider));
        }

        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            var pathFile = "sample-pdf.pdf";

            if (!System.IO.File.Exists(pathFile)) return NotFound();

            // Content negotiation is the process of selecting the best representation for a
            // given response when there are multiple representations available

            if (!_fileExtensionContentTypeProvider.TryGetContentType(pathFile, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(pathFile);

            // Use the File method on ControllerBase to return files
            // Important is to set the correct media type on the response
            // to ensure that the operating system knows the correct viewer to use.

            return File(bytes, contentType, Path.GetFileName(pathFile));
        }
    }
}
