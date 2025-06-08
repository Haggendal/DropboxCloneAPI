using System.ComponentModel.DataAnnotations;

namespace DropboxCloneAPI.DTOs
{
    public class FolderRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        
        public Guid? ParentFolderId { get; set; }
    }
}
