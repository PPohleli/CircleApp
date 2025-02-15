using CircleApp.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CircleApp.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly IPostService _postService;
        public FavoritesController(IPostService postService) 
        {
            _postService = postService;
        }
        public async Task<IActionResult> Index()
        {
            var loggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var myFavoritePosts = await _postService.GetAllFavoritedPostsAsync(int.Parse(loggedInUser));
            return View(myFavoritePosts);
        }
    }
}
