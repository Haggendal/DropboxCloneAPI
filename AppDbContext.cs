using Microsoft.EntityFrameworkCore;
using DropboxCloneAPI.Models;

namespace DropboxCloneAPI
{
    public class AppDbContext : DbContext
    {
        public DbSet<FileEntity> Files { get; set; }
        public DbSet<FolderEntity> Folders { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FileEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.ContentType).HasMaxLength(100);
                
                entity.HasOne(e => e.Folder)
                      .WithMany(e => e.Files)
                      .HasForeignKey(e => e.FolderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<FolderEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                
                entity.HasOne(e => e.ParentFolder)
                      .WithMany(e => e.ChildFolders)
                      .HasForeignKey(e => e.ParentFolderId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}