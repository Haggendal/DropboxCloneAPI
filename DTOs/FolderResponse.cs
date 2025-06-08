using DropboxCloneAPI.Models;

namespace DropboxCloneAPI.DTOs
{
    public class FolderResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? ParentFolderId { get; set; }
        public string? ParentFolderName { get; set; }
        public List<FileResponse> Files { get; set; } = new();
        public List<FolderResponse> ChildFolders { get; set; } = new();

        public static FolderResponse FromEntity(FolderEntity folder)
        {
            return new FolderResponse
            {
                Id = folder.Id,
                Name = folder.Name,
                CreatedAt = folder.CreatedAt,
                UpdatedAt = folder.UpdatedAt,
                ParentFolderId = folder.ParentFolderId,
                ParentFolderName = folder.ParentFolder?.Name,
                Files = folder.Files?.Select(FileResponse.FromEntity).ToList() ?? new(),
                ChildFolders = folder.ChildFolders?.Select(FromEntity).ToList() ?? new()
            };
        }
    }
}