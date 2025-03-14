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

        public async Task<Story> CreateStoryAsync(Story story)
        {
            await _context.Stories.AddAsync(story);
            await _context.SaveChangesAsync();

            return story;
        }
    }
}
