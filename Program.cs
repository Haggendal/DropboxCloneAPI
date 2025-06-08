using Microsoft.EntityFrameworkCore;
using DropboxCloneAPI;
using DropboxCloneAPI.Repositories;
using DropboxCloneAPI.Services;

namespace FileStorageAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString)
            );

            // Register repositories
            builder.Services.AddScoped<IFileRepository, EfFileRepository>();
            builder.Services.AddScoped<IFolderRepository, EfFolderRepository>();

            // Register services
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IFolderService, FolderService>();

            // Configure file upload limits
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 100_000_000; // 100 MB
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:3000",
                            "http://localhost:5173" 
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            app.UseCors("AllowFrontend");

            app.MapControllers();

            app.Run();
        }
    }
}