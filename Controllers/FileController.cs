using Microsoft.AspNetCore.Mvc;
using DropboxCloneAPI.DTOs;
using DropboxCloneAPI.Services;

namespace DropboxCloneAPI.Controllers
{
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService fileService;

        public FileController(IFileService fileService)
        {
            this.fileService = fileService;
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile([FromForm] FileRequest fileRequest)
        {
            try
            {
                var uploadedFile = await fileService.UploadFileAsync(fileRequest);
                return CreatedAtAction(nameof(UploadFile), FileResponse.FromEntity(uploadedFile));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-file/{id}")]
        public async Task<IActionResult> GetFile(Guid id)
        {
            try
            {
                var file = await fileService.GetFileAsync(id);
                return Ok(FileResponse.FromEntity(file));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("download-file/{id}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            try
            {
                var (content, fileName, contentType) = await fileService.DownloadFileAsync(id);
                return File(content, contentType, fileName);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("delete-file/{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            try
            {
                var result = await fileService.DeleteFileAsync(id);
                if (!result)
                    return NotFound("File not found.");
                
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("get-files-by-folder/{folderId}")]
        public async Task<IActionResult> GetFilesByFolder(Guid folderId)
        {
            try
            {
                var files = await fileService.GetFilesByFolderAsync(folderId);
                return Ok(files);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving files", message = ex.Message });
            }
        }
    }
}