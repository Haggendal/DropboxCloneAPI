using DropboxCloneAPI.Models;

namespace DropboxCloneAPI.DTOs
{
    public class FileResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid FolderId { get; set; }
        public string FolderName { get; set; }

        public static FileResponse FromEntity(FileEntity file)
        {
            return new FileResponse
            {
                Id = file.Id,
                Name = file.Name,
                ContentType = file.ContentType,
                Size = file.Size,
                CreatedAt = file.CreatedAt,
                UpdatedAt = file.UpdatedAt,
                FolderId = file.FolderId,
                FolderName = file.Folder?.Name ?? ""
            };
        }
    }
}