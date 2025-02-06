﻿using CircleApp.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Services
{
    public class StoriesService : IStoriesServices
    {
        private readonly AppDbContext _context;

        public StoriesService(AppDbContext contect)
        {
            _context = contect;
        }


        public async Task<List<Story>> GetAllStoriesAsync()
        {
            var allStories = await _context.Stories.Where(n => n.DateCreated >= DateTime.UtcNow.AddHours(-24)).Include(s => s.User).ToListAsync();
            return allStories;
        }

        public async Task<Story> CreateStoryAsync(Story story, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                if (image.ContentType.Contains("image"))
                {
                    string rootFolderPathImage = Path.Combine(rootFolderPath, "images/stories/");
                    Directory.CreateDirectory(rootFolderPathImage);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string filePath = Path.Combine(rootFolderPathImage, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await image.CopyToAsync(stream);

                    // Set the URL to the newPost object
                    story.ImageUrl = "/images/stories/" + fileName;
                }
            }
            await _context.Stories.AddAsync(story);
            await _context.SaveChangesAsync();

            return story;
        }
    }
}
