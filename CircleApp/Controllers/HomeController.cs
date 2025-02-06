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
        private readonly IHashtagsService _hashtagsService;
        private readonly IPostService _postService;

        public HomeController(ILogger<HomeController> logger, IPostService postService, IHashtagsService hashtagsService)
        {
            _logger = logger;
            _hashtagsService = hashtagsService;
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
            await _hashtagsService.ProcessHashtagsForNewPostAsync(post.Content);

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
            var postRemoved = await _postService.RemovePostAsync(postRemoveVM.PostId);
            await _hashtagsService.ProcessHashtagsForRemovedPostAsync(postRemoved.Content);

            return RedirectToAction("Index");
        }
    }
}
