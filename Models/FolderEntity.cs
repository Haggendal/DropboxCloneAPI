using System.ComponentModel.DataAnnotations;

namespace DropboxCloneAPI.Models
{
    public class FolderEntity
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public Guid? ParentFolderId { get; set; }
        public FolderEntity? ParentFolder { get; set; }
        
        public ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();
        public ICollection<FolderEntity> ChildFolders { get; set; } = new List<FolderEntity>();

        public FolderEntity(string name, Guid? parentFolderId = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            ParentFolderId = parentFolderId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public FolderEntity() { }
    }
}