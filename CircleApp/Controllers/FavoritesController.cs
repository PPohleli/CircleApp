using CircleApp.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace CircleApp.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly IPostService _postService;
        public FavoritesController(IPostService postService) 
        {
            _postService = postService;
        }
        public async Task<IActionResult> Index()
        {
            int loggedInUser = 1;
            var myFavoritePosts = await _postService.GetAllFavoritedPostsAsync(loggedInUser);
            return View(myFavoritePosts);
        }
    }
}
