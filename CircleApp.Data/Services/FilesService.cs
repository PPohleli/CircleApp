using CircleApp.Data.Helpers.Enums;
using CircleApp.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CircleApp.Data.Services
{
    public class FilesService : IFilesService
    {
        public async Task<string> UploadImageAsync(IFormFile file, ImageFileType imageFileType)
        {
            string filePathUpload = imageFileType switch
            {
                ImageFileType.PostImage => Path.Combine("images","posts"),
                ImageFileType.StoryImage => Path.Combine("images", "stories"),
                ImageFileType.ProfilePicture => Path.Combine("images", "profilePictures"),
                ImageFileType.CoverImage => Path.Combine("images", "covers"),
                _ => throw new ArgumentException("Invalid file type")
            };

            if (file != null && file.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                if (file.ContentType.Contains("image"))
                {
                    string rootFolderPathImage = Path.Combine(rootFolderPath, filePathUpload);
                    Directory.CreateDirectory(rootFolderPathImage);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(rootFolderPathImage, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await file.CopyToAsync(stream);

                    return $"{filePathUpload}/{fileName}";
                }
            }
            return "";
        }
    }
}
