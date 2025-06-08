using DropboxCloneAPI.Models;
using DropboxCloneAPI.DTOs;
using DropboxCloneAPI.Repositories;

namespace DropboxCloneAPI.Services
{
    public interface IFileService
    {
        Task<FileEntity> UploadFileAsync(FileRequest fileRequest);
        Task<FileEntity> GetFileAsync(Guid id);
        Task<(byte[] content, string fileName, string contentType)> DownloadFileAsync(Guid id);
        Task<bool> DeleteFileAsync(Guid id);
        Task<List<FileResponse>> GetFilesByFolderAsync(Guid folderId);
    }

    public class FileService : IFileService
    {
        private readonly IFileRepository fileRepository;
        private readonly IFolderRepository folderRepository;

        public FileService(IFileRepository fileRepository, IFolderRepository folderRepository)
        {
            this.fileRepository = fileRepository;
            this.folderRepository = folderRepository;
        }

        public async Task<FileEntity> UploadFileAsync(FileRequest fileRequest)
        {
            if (fileRequest.File == null || fileRequest.File.Length == 0)
            {
                throw new ArgumentException("File cannot be empty.");
            }

            if (fileRequest.FolderId == Guid.Empty)
            {
                throw new ArgumentException("Invalid folder ID.");
            }

            // Check if folder exists
            var folder = await folderRepository.GetFolderByIdAsync(fileRequest.FolderId);
            if (folder == null)
            {
                throw new InvalidOperationException($"Folder with ID '{fileRequest.FolderId}' not found.");
            }

            // Check if file with same name already exists in folder
            if (await fileRepository.FileExistsByNameInFolderAsync(fileRequest.File.FileName, fileRequest.FolderId))
            {
                throw new InvalidOperationException($"A file with the name '{fileRequest.File.FileName}' already exists in this folder.");
            }

            // Convert IFormFile to byte array
            byte[] fileContent;
            using (var memoryStream = new MemoryStream())
            {
                await fileRequest.File.CopyToAsync(memoryStream);
                fileContent = memoryStream.ToArray();
            }

            var fileEntity = new FileEntity(
                fileRequest.File.FileName,
                fileContent,
                fileRequest.File.ContentType ?? "application/octet-stream",
                fileRequest.File.Length,
                fileRequest.FolderId
            );

            await fileRepository.AddFileAsync(fileEntity);
            return fileEntity;
        }

        public async Task<FileEntity> GetFileAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid file ID.");
            }

            var file = await fileRepository.GetFileByIdAsync(id);
            if (file == null)
            {
                throw new InvalidOperationException($"File with ID '{id}' not found.");
            }

            return file;
        }

        public async Task<(byte[] content, string fileName, string contentType)> DownloadFileAsync(Guid id)
        {
            var file = await GetFileAsync(id);
            return (file.Content, file.Name, file.ContentType);
        }

        public async Task<bool> DeleteFileAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid file ID.");
            }

            return await fileRepository.DeleteFileAsync(id);
        }

        public async Task<List<FileResponse>> GetFilesByFolderAsync(Guid folderId)
        {
            if (folderId == Guid.Empty)
            {
                throw new ArgumentException("Invalid folder ID.");
            }

            var files = await fileRepository.GetFilesByFolderIdAsync(folderId);
            return files.Select(FileResponse.FromEntity).ToList();
        }
    }
}