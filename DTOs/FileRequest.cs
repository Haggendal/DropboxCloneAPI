using System.ComponentModel.DataAnnotations;

namespace DropboxCloneAPI.DTOs
{
    public class FileRequest
    {
        [Required]
        public IFormFile File { get; set; }
        
        [Required]
        public Guid FolderId { get; set; }
    }
}