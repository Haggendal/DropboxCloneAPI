using DropboxCloneAPI.Models;
using DropboxCloneAPI.DTOs;
using DropboxCloneAPI.Repositories;

namespace DropboxCloneAPI.Services
{
    public interface IFolderService
    {
        Task<FolderEntity> CreateFolderAsync(FolderRequest folderRequest);
        Task<FolderEntity> GetFolderAsync(Guid id);
        Task<List<FolderResponse>> GetAllFoldersAsync();
        Task<bool> DeleteFolderAsync(Guid id);
    }

    public class FolderService : IFolderService
    {
        private readonly IFolderRepository folderRepository;

        public FolderService(IFolderRepository folderRepository)
        {
            this.folderRepository = folderRepository;
        }

        public async Task<FolderEntity> CreateFolderAsync(FolderRequest folderRequest)
        {
            if (string.IsNullOrWhiteSpace(folderRequest.Name))
            {
                throw new ArgumentException("Folder name cannot be empty.");
            }

            // Verify parent folder exists if specified
            if (folderRequest.ParentFolderId.HasValue && folderRequest.ParentFolderId != Guid.Empty)
            {
                var parentFolder = await folderRepository.GetFolderByIdAsync(folderRequest.ParentFolderId.Value);
                if (parentFolder == null)
                {
                    throw new InvalidOperationException($"Parent folder with ID '{folderRequest.ParentFolderId}' not found.");
                }
            }

            // Check if folder with same name already exists in the same parent
            if (await folderRepository.FolderExistsByNameAsync(folderRequest.Name, folderRequest.ParentFolderId))
            {
                throw new InvalidOperationException($"A folder with the name '{folderRequest.Name}' already exists in this location.");
            }

            var folderEntity = new FolderEntity(folderRequest.Name, folderRequest.ParentFolderId);
            await folderRepository.AddFolderAsync(folderEntity);

            return folderEntity;
        }

        public async Task<FolderEntity> GetFolderAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid folder ID.");
            }

            var folder = await folderRepository.GetFolderByIdAsync(id);
            if (folder == null)
            {
                throw new InvalidOperationException($"Folder with ID '{id}' not found.");
            }

            return folder;
        }

        public async Task<List<FolderResponse>> GetAllFoldersAsync()
        {
            var folders = await folderRepository.GetAllFoldersAsync();
            return folders.Select(FolderResponse.FromEntity).ToList();
        }

        public async Task<bool> DeleteFolderAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid folder ID.");
            }

            // Check if folder has content
            if (await folderRepository.FolderHasContentAsync(id))
            {
                throw new InvalidOperationException("Cannot delete folder that contains files or subfolders.");
            }

            return await folderRepository.DeleteFolderAsync(id);
        }
    }
}