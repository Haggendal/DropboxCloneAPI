using System.ComponentModel.DataAnnotations;

namespace DropboxCloneAPI.Models
{
    public class FileEntity
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        
        [Required]
        public byte[] Content { get; set; }
        
        public string ContentType { get; set; }
        
        public long Size { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public Guid FolderId { get; set; }
        
        public FolderEntity Folder { get; set; }

        public FileEntity(string name, byte[] content, string contentType, long size, Guid folderId)
        {
            Id = Guid.NewGuid();
            Name = name;
            Content = content;
            ContentType = contentType;
            Size = size;
            FolderId = folderId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public FileEntity() { }
    }
}
