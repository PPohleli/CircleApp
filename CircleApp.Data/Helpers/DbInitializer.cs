﻿using CircleApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Helpers
{
    public static class DbInitializer
    {
        public static async Task SeedAsyn(AppDbContext appDbContext)
        {
            if (!appDbContext.Users.Any() && !appDbContext.Posts.Any())
            {
                var newUser = new User()
                {
                    FullName = "Litha Cele",
                    ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_5.jpg"
                };
                await appDbContext.Users.AddAsync(newUser);
                await appDbContext.SaveChangesAsync();

                var newPostWithoutImage = new Post()
                {
                    Content = "This is our first post which is being loaded from the database without an image/photo. Created using our test user data.",
                    ImageUrl = "",
                    NrOfReposts = 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    
                    UserId = newUser.Id
                };
                var newPostWithImage = new Post()
                {
                    Content = "This is our second post which is being loaded from the database with an image/photo. Created using our test user data.",
                    ImageUrl = "https://unsplash.com/photos/foggy-mountain-summit-1Z2niiBPg5A",
                    NrOfReposts = 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,

                    UserId = newUser.Id
                };
                await appDbContext.AddRangeAsync(newPostWithoutImage, newPostWithImage);
                await appDbContext.SaveChangesAsync();
            }
        }
    }
}
