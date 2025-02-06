using CircleApp.Data;
using CircleApp.Data.Helpers;
using CircleApp.Data.Models;
using CircleApp.Data.Services;
using CircleApp.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CircleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IPostService _postService;

        public HomeController(ILogger<HomeController> logger, AppDbContext contect, IPostService postService)
        {
            _logger = logger;
            _context = contect;
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUserId = 1;

            var allPosts = await _postService.GetAllPostsAsync(loggedInUserId);
            return View(allPosts);
        }

        public async Task<IActionResult> CreatePost(PostVM post)
        {
            // Get the logged in user
            int loggedInUser = 1;

            // Create a new Post/Status
            var newPost = new Post
            {
                Content = post.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                ImageUrl = "",
                NrOfReposts = 0,
                UserId = loggedInUser

            };

            await _postService.CreatePostAsync(newPost, post.Image);

            // Find and Store Hashtags
            var postHashtags = HashtagHelper.GetHashtags(post.Content);

            foreach(var hashtag in postHashtags)
            {
                var hashtagDb = await _context.Hashtags.FirstOrDefaultAsync(n => n.Name == hashtag);

                if (hashtagDb != null)
                {
                    hashtagDb.Count += 1;
                    hashtagDb.DateUpdated = DateTime.UtcNow;

                    _context.Hashtags.Update(hashtagDb);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var newHashtag = new Hashtag()
                    {
                        Name = hashtag,
                        Count = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    };
                    await _context.Hashtags.AddAsync(newHashtag);
                    await _context.SaveChangesAsync();
                }
            }

            //Redirect the user to home page
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            int loggedInUserId = 1;

            await _postService.TogglePostLikeAsync(postLikeVM.PostId, loggedInUserId);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            int loggedInUserId = 1;
            await _postService.TogglePostFavoriteAsync(postFavoriteVM.PostId, loggedInUserId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVM)
        {
            int loggedInUserId = 1;

            await _postService.TogglePostVisibilityAsync(postVisibilityVM.PostId, loggedInUserId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            //get id for the logged in user
            int loggedInUserId = 1;
            
            await _postService.ReportPostAsync(postReportVM.PostId, loggedInUserId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostComment (PostCommentVM postCommentVM)
        {
            //get id for the logged in user
            int loggedInUserId = 1;

            // create a post object
            var newComment = new Comment()
            {
                UserId = loggedInUserId,
                PostId = postCommentVM.PostId,
                Content = postCommentVM.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };
            await _postService.AddPostCommentAsync(newComment);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemovePostComment(RemoveCommentVM removeCommentVM)
        {
            

            await _postService.RemovePostCommentAsync(removeCommentVM.CommentId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PostRemove(PostRemoveVM postRemoveVM)
        {
            await _postService.RemovePostAsync(postRemoveVM.PostId);

            //Update Hashtags
            //var postHashtags = HashtagHelper.GetHashtags(postDb.Content);
            //foreach (var tag in postHashtags)
            //{
            //    var hashtagDb = await _context.Hashtags.FirstOrDefaultAsync(n => n.Name == tag);
            //    if (hashtagDb != null)
            //    {
            //        hashtagDb.Count -= 1;
            //        hashtagDb.DateUpdated = DateTime.Now;

            //        _context.Hashtags.Update(hashtagDb);
            //        await _context.SaveChangesAsync();
            //    }
            //}

            return RedirectToAction("Index");
        }
    }
}
