using Microsoft.AspNetCore.Mvc;
using CityExplorer.Data;
using CityExplorer.Services;

namespace CityExplorer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionService _sessionService;

        public HomeController(ApplicationDbContext context, SessionService sessionService)
        {
            _context = context;
            _sessionService = sessionService;
        }

        public IActionResult Index()
        {
            var cities = _context.Cities.Where(c => c.IsActive).Take(6).ToList();
            ViewBag.IsLoggedIn = _sessionService.IsLoggedIn();
            ViewBag.UserRole = _sessionService.GetSession<string>("UserRole");
            ViewBag.Username = _sessionService.GetSession<string>("Username");
            return View(cities);
        }

        public IActionResult About()
        {
            ViewBag.IsLoggedIn = _sessionService.IsLoggedIn();
            ViewBag.UserRole = _sessionService.GetSession<string>("UserRole");
            ViewBag.Username = _sessionService.GetSession<string>("Username");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
