using Microsoft.AspNetCore.Mvc;
using DropboxCloneAPI.DTOs;
using DropboxCloneAPI.Services;

namespace DropboxCloneAPI.Controllers
{
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService folderService;

        public FolderController(IFolderService folderService)
        {
            this.folderService = folderService;
        }

        [HttpPost("create-folder")]
        public async Task<IActionResult> CreateFolder([FromBody] FolderRequest folderRequest)
        {
            try
            {
                var createdFolder = await folderService.CreateFolderAsync(folderRequest);
                return CreatedAtAction(nameof(CreateFolder), FolderResponse.FromEntity(createdFolder));
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

        [HttpGet("get-folder/{id}")]
        public async Task<IActionResult> GetFolder(Guid id)
        {
            try
            {
                var folder = await folderService.GetFolderAsync(id);
                return Ok(FolderResponse.FromEntity(folder));
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

        [HttpGet("get-all-folders")]
        public async Task<IActionResult> GetAllFolders()
        {
            try
            {
                var folders = await folderService.GetAllFoldersAsync();
                return Ok(folders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving folders", message = ex.Message });
            }
        }

        [HttpDelete("delete-folder/{id}")]
        public async Task<IActionResult> DeleteFolder(Guid id)
        {
            try
            {
                var result = await folderService.DeleteFolderAsync(id);
                if (!result)
                    return NotFound("Folder not found.");
                
                return NoContent();
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
    }
}