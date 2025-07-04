using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CityExplorer.Data;
using CityExplorer.Models;
using CityExplorer.Models.ViewModels;
using CityExplorer.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CityExplorer.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionService _sessionService;

        public AdminController(ApplicationDbContext context, SessionService sessionService)
        {
            _context = context;
            _sessionService = sessionService;
        }

        private bool IsAdminLoggedIn()
        {
            return _sessionService.IsLoggedIn() && _sessionService.IsAdmin();
        }

        public IActionResult Dashboard()
        {
            if (!IsAdminLoggedIn())
            {
                TempData["Error"] = "Access denied. Admin login required.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                ViewBag.TotalCities = _context.Cities.Count();
                ViewBag.TotalCustomers = _context.Users.Where(u => u.Role == "Customer").Count();
                ViewBag.TotalBookings = _context.Bookings.Count();
                ViewBag.TotalRevenue = _context.Bookings.Sum(b => b.TotalAmount).ToString("F2");
                ViewBag.Username = _sessionService.GetSession<string>("Username");

                var recentBookings = _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.City)
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(5)
                    .ToList();
                ViewBag.RecentBookings = recentBookings;

                var popularCities = _context.Cities
                    .Select(c => new
                    {
                        c.Name,
                        c.Country,
                        BookingCount = c.Bookings.Count()
                    })
                    .OrderByDescending(c => c.BookingCount)
                    .Take(5)
                    .ToList();
                ViewBag.PopularCities = popularCities;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading dashboard: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Cities()
        {
            if (!IsAdminLoggedIn())
            {
                TempData["Error"] = "Access denied. Admin login required.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var cities = _context.Cities.OrderBy(c => c.Name).ToList();
                ViewBag.Username = _sessionService.GetSession<string>("Username");
                return View(cities);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading cities: " + ex.Message;
                return View(new List<City>());
            }
        }

        [HttpGet]
        public IActionResult CreateCity()
        {
            if (!IsAdminLoggedIn())
            {
                TempData["Error"] = "Access denied. Admin login required.";
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Username = _sessionService.GetSession<string>("Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCity(CityViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                TempData["Error"] = "Access denied. Admin login required.";
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var city = new City
                    {
                        Name = model.Name,
                        Country = model.Country,
                        Description = model.Description,
                        Price = model.Price,
                        Duration = model.Duration,
                        ImageUrl = model.ImageUrl,
                        IsActive = model.IsActive
                    };

                    _context.Cities.Add(city);
                    _context.SaveChanges();

                    TempData["Success"] = "City created successfully!";
                    return RedirectToAction("Cities");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating city: " + ex.Message);
                }
            }

            ViewBag.Username = _sessionService.GetSession<string>("Username");
            return View(model);
        }

        [HttpGet]
        public IActionResult EditCity(int id)
        {
            if (!IsAdminLoggedIn())
            {
                TempData["Error"] = "Access denied. Admin login required.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var city = _context.Cities.Find(id);
                if (city == null)
                {
                    TempData["Error"] = "City not found.";
                    return RedirectToAction("Cities");
                }

                var model = new CityViewModel
                {
                    Id = city.Id,
                    Name = city.Name,
                    Country = city.Country,
                    Description = city.Description,
                    Price = city.Price,
                    Duration = city.Duration,
                    ImageUrl = city.ImageUrl,
                    IsActive = city.IsActive
                };

                ViewBag.Username = _sessionService.GetSession<string>("Username");
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading city: " + ex.Message;
                return RedirectToAction("Cities");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCity(CityViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                TempData["Error"] = "Access denied. Admin login required.";
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var city = _context.Cities.Find(model.Id);
                    if (city == null)
                    {
                        TempData["Error"] = "City not found.";
                        return RedirectToAction("Cities");
                    }

                    city.Name = model.Name;
                    city.Country = model.Country;
                    city.Description = model.Description;
                    city.Price = model.Price;
                    city.Duration = model.Duration;
                    city.ImageUrl = model.ImageUrl;
                    city.IsActive = model.IsActive;

                    _context.SaveChanges();

                    TempData["Success"] = "City updated successfully!";
                    return RedirectToAction("Cities");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating city: " + ex.Message);
                }
            }

            ViewBag.Username = _sessionService.GetSession<string>("Username");
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteCity([FromBody] int id)
        {
            if (!IsAdminLoggedIn())
            {
                return Json(new { success = false, message = "Access denied." });
            }

            try
            {
                var city = _context.Cities.Find(id);
                if (city == null)
                {
                    return Json(new { success = false, message = "City not found." });
                }

                _context.Cities.Remove(city);
                _context.SaveChanges();
                return Json(new { success = true, message = "City deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting city: " + ex.Message });
            }
        }

        public IActionResult Customers()
        {
            if (!IsAdminLoggedIn())
            {
                TempData["Error"] = "Access denied. Admin login required.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var customers = _context.Users
                    .Where(u => u.Role == "Customer")
                    .OrderBy(u => u.FullName)
                    .ToList();

                ViewBag.Username = _sessionService.GetSession<string>("Username");
                return View(customers);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading customers: " + ex.Message;
                return View(new List<User>());
            }
        }

        public IActionResult Bookings()
        {
            if (!IsAdminLoggedIn())
            {
                TempData["Error"] = "Access denied. Admin login required.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var bookings = _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.City)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToList();

                ViewBag.Username = _sessionService.GetSession<string>("Username");
                return View(bookings);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading bookings: " + ex.Message;
                return View(new List<Booking>());
            }
        }

        [HttpPost]
        public IActionResult UpdateBookingStatus([FromBody] dynamic data)
        {
            if (!IsAdminLoggedIn())
            {
                return Json(new { success = false, message = "Access denied." });
            }

            try
            {
                int id = data.id;
                string status = data.status;

                var booking = _context.Bookings.Find(id);
                if (booking == null)
                {
                    return Json(new { success = false, message = "Booking not found." });
                }

                booking.Status = status;
                _context.SaveChanges();

                return Json(new { success = true, message = "Booking status updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating booking: " + ex.Message });
            }
        }
    }
}
