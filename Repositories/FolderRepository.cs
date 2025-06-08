using Microsoft.EntityFrameworkCore;
using DropboxCloneAPI.Models;

namespace DropboxCloneAPI.Repositories
{
    public interface IFolderRepository
    {
        Task AddFolderAsync(FolderEntity folder);
        Task<FolderEntity?> GetFolderByIdAsync(Guid id);
        Task<bool> DeleteFolderAsync(Guid id);
        Task<List<FolderEntity>> GetAllFoldersAsync();
        Task<bool> FolderExistsByNameAsync(string name, Guid? parentFolderId = null);
        Task<bool> FolderHasContentAsync(Guid id);
    }

    public class EfFolderRepository : IFolderRepository
    {
        private readonly AppDbContext context;

        public EfFolderRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task AddFolderAsync(FolderEntity folder)
        {
            await context.Folders.AddAsync(folder);
            await context.SaveChangesAsync();
        }

        public async Task<FolderEntity?> GetFolderByIdAsync(Guid id)
        {
            return await context.Folders
                .Include(f => f.ParentFolder)
                .Include(f => f.Files)
                .Include(f => f.ChildFolders)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<bool> DeleteFolderAsync(Guid id)
        {
            var folder = await context.Folders.FindAsync(id);
            if (folder == null)
                return false;

            context.Folders.Remove(folder);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<FolderEntity>> GetAllFoldersAsync()
        {
            return await context.Folders
                .Include(f => f.ParentFolder)
                .ToListAsync();
        }

        public async Task<bool> FolderExistsByNameAsync(string name, Guid? parentFolderId = null)
        {
            return await context.Folders.AnyAsync(f => 
                f.Name.ToLower() == name.ToLower() && 
                f.ParentFolderId == parentFolderId);
        }

        public async Task<bool> FolderHasContentAsync(Guid id)
        {
            var folder = await context.Folders
                .Include(f => f.Files)
                .Include(f => f.ChildFolders)
                .FirstOrDefaultAsync(f => f.Id == id);
            
            return folder != null && (folder.Files.Any() || folder.ChildFolders.Any());
        }
    }
}