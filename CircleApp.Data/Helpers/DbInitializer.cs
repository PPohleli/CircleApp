using CircleApp.Data.Helpers.Constants;
using CircleApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Helpers
{
    public static class DbInitializer
    {
        public static async Task SeedUsersAndRolesAsyn(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            //Roles
            if (!roleManager.Roles.Any())
            {
                foreach (var roleName in AppRoles.All)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                    }
                }
            }

            //Users with Roles
            if (!userManager.Users.Any(n => !string.IsNullOrEmpty(n.Email)))
            {
                var userPassword = "Coding@1234";
                var newUser = new User()
                {
                    UserName = "litha.cele",
                    Email = "litha.cele@gmail.com",
                    FullName = "Litha Cele",
                    ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_3.jpg",
                    EmailConfirmed = true,

                };
                var result = await userManager.CreateAsync(newUser, userPassword);

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(newUser, AppRoles.User);

                
                var newAdmin = new User()
                {
                    UserName = "phiwe.admin",
                    Email = "admin@gmail.com",
                    FullName = "Phiwe Pohleli",
                    ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_2.jpg",
                    EmailConfirmed = true,

                };
                var resultNewAdmin = await userManager.CreateAsync(newAdmin, userPassword);

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(newAdmin, AppRoles.Admin);
            }

        }
        public static async Task SeedAsyn(AppDbContext appDbContext)
        {
            //if (!appDbContext.Users.Any() && !appDbContext.Posts.Any())
            //{
            //    var newUser = new User()
            //    {
            //        FullName = "Litha Cele",
            //        ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_5.jpg"
            //    };
            //    await appDbContext.Users.AddAsync(newUser);
            //    await appDbContext.SaveChangesAsync();

            //    var newPostWithoutImage = new Post()
            //    {
            //        Content = "This is our first post which is being loaded from the database without an image/photo. Created using our test user data.",
            //        ImageUrl = "",
            //        NrOfReposts = 0,
            //        DateCreated = DateTime.UtcNow,
            //        DateUpdated = DateTime.UtcNow,
                    
            //        UserId = newUser.Id
            //    };
            //    var newPostWithImage = new Post()
            //    {
            //        Content = "This is our second post which is being loaded from the database with an image/photo. Created using our test user data.",
            //        ImageUrl = "https://unsplash.com/photos/foggy-mountain-summit-1Z2niiBPg5A",
            //        NrOfReposts = 0,
            //        DateCreated = DateTime.UtcNow,
            //        DateUpdated = DateTime.UtcNow,

            //        UserId = newUser.Id
            //    };
            //    await appDbContext.AddRangeAsync(newPostWithoutImage, newPostWithImage);
            //    await appDbContext.SaveChangesAsync();
            //}
        }
    }
}
