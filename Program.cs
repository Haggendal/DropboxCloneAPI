using Microsoft.EntityFrameworkCore;
using DropboxCloneAPI.Repositories;
using DropboxCloneAPI.Services;

namespace DropboxCloneAPI
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

            builder.Services.AddScoped<IFileRepository, EfFileRepository>();
            builder.Services.AddScoped<IFolderRepository, EfFolderRepository>();

            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IFolderService, FolderService>();

            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 100_000_000;
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