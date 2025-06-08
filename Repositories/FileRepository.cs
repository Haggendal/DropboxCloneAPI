using Microsoft.EntityFrameworkCore;
using DropboxCloneAPI.Models;

namespace DropboxCloneAPI.Repositories
{
    public interface IFileRepository
    {
        Task AddFileAsync(FileEntity file);
        Task<FileEntity?> GetFileByIdAsync(Guid id);
        Task<bool> DeleteFileAsync(Guid id);
        Task<List<FileEntity>> GetFilesByFolderIdAsync(Guid folderId);
        Task<bool> FileExistsByNameInFolderAsync(string name, Guid folderId);
    }

    public class EfFileRepository : IFileRepository
    {
        private readonly AppDbContext context;

        public EfFileRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task AddFileAsync(FileEntity file)
        {
            await context.Files.AddAsync(file);
            await context.SaveChangesAsync();
        }

        public async Task<FileEntity?> GetFileByIdAsync(Guid id)
        {
            return await context.Files
                .Include(f => f.Folder)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<bool> DeleteFileAsync(Guid id)
        {
            var file = await context.Files.FindAsync(id);
            if (file == null)
                return false;

            context.Files.Remove(file);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<FileEntity>> GetFilesByFolderIdAsync(Guid folderId)
        {
            return await context.Files
                .Include(f => f.Folder)
                .Where(f => f.FolderId == folderId)
                .ToListAsync();
        }

        public async Task<bool> FileExistsByNameInFolderAsync(string name, Guid folderId)
        {
            return await context.Files.AnyAsync(f => f.Name.ToLower() == name.ToLower() && f.FolderId == folderId);
        }
    }
}