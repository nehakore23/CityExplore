using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CityExplorer.Data;
using CityExplorer.Models;
using CityExplorer.Models.ViewModels;
using CityExplorer.Services;
using System;

namespace CityExplorer.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionService _sessionService;

        public CustomerController(ApplicationDbContext context, SessionService sessionService)
        {
            _context = context;
            _sessionService = sessionService;
        }

        // Check if user is customer before each action
        private bool IsCustomerLoggedIn()
        {
            return _sessionService.IsLoggedIn() && _sessionService.IsCustomer();
        }

        private int GetCurrentUserId()
        {
            return _sessionService.GetSession<int>("UserId");
        }

        public IActionResult Dashboard()
        {
            if (!IsCustomerLoggedIn())
            {
                TempData["Error"] = "Access denied. Customer login required.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var userId = GetCurrentUserId();

                var userBookings = _context.Bookings.Where(b => b.UserId == userId);
                ViewBag.TotalBookings = userBookings.Count();
                ViewBag.CitiesVisited = userBookings.Select(b => b.CityId).Distinct().Count();
                ViewBag.TotalSpent = userBookings.Sum(b => b.TotalAmount).ToString("F2");
                ViewBag.FullName = _sessionService.GetSession<string>("FullName");
                ViewBag.Username = _sessionService.GetSession<string>("Username");

                var recentBookings = _context.Bookings
                    .Include(b => b.City)
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(5)
                    .ToList();
                ViewBag.RecentBookings = recentBookings;

                var bookedCityIds = userBookings.Select(b => b.CityId).ToList();
                var recommendedCities = _context.Cities
                    .Where(c => c.IsActive && !bookedCityIds.Contains(c.Id))
                    .OrderBy(c => c.Price)
                    .Take(3)
                    .ToList();
                ViewBag.RecommendedCities = recommendedCities;

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
            if (!IsCustomerLoggedIn())
            {
                TempData["Error"] = "Access denied. Customer login required.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var cities = _context.Cities
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToList();

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
        public IActionResult BookTour(int id)
        {
            if (!IsCustomerLoggedIn())
            {
                TempData["Error"] = "Access denied. Customer login required.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var city = _context.Cities.Find(id);
                if (city == null || !city.IsActive)
                {
                    TempData["Error"] = "City not found or not available.";
                    return RedirectToAction("Cities");
                }

                var model = new BookingViewModel
                {
                    CityId = city.Id,
                    CityName = city.Name,
                    Country = city.Country,
                    Price = city.Price,
                    BookingDate = DateTime.Now.AddDays(1) // Default to tomorrow
                };

                ViewBag.City = city;
                ViewBag.Username = _sessionService.GetSession<string>("Username");
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading booking form: " + ex.Message;
                return RedirectToAction("Cities");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BookTour(BookingViewModel model)
        {
            if (!IsCustomerLoggedIn())
            {
                TempData["Error"] = "Access denied. Customer login required.";
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var city = _context.Cities.Find(model.CityId);
                    if (city == null || !city.IsActive)
                    {
                        ModelState.AddModelError("", "Selected city is not available.");
                        ViewBag.City = city;
                        ViewBag.Username = _sessionService.GetSession<string>("Username");
                        return View(model);
                    }

                    if (model.BookingDate <= DateTime.Now)
                    {
                        ModelState.AddModelError("BookingDate", "Booking date must be in the future.");
                        ViewBag.City = city;
                        ViewBag.Username = _sessionService.GetSession<string>("Username");
                        return View(model);
                    }

                    var booking = new Booking
                    {
                        UserId = GetCurrentUserId(),
                        CityId = model.CityId,
                        BookingDate = model.BookingDate,
                        NumberOfPeople = model.NumberOfPeople,
                        TotalAmount = city.Price * model.NumberOfPeople,
                        SpecialRequests = model.SpecialRequests,
                        Status = "Pending"
                    };

                    _context.Bookings.Add(booking);
                    _context.SaveChanges();

                    TempData["Success"] = "Tour booked successfully! Your booking is pending confirmation.";
                    return RedirectToAction("MyBookings");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating booking: " + ex.Message);
                }
            }

            var cityData = _context.Cities.Find(model.CityId);
            ViewBag.City = cityData;
            ViewBag.Username = _sessionService.GetSession<string>("Username");
            return View(model);
        }

        public IActionResult MyBookings()
        {
            if (!IsCustomerLoggedIn())
            {
                TempData["Error"] = "Access denied. Customer login required.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var userId = GetCurrentUserId();
                var bookings = _context.Bookings
                    .Include(b => b.City)
                    .Where(b => b.UserId == userId)
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
        public IActionResult CancelBooking([FromBody] int id)
        {
            if (!IsCustomerLoggedIn())
            {
                return Json(new { success = false, message = "Access denied." });
            }

            try
            {
                var userId = GetCurrentUserId();
                var booking = _context.Bookings.FirstOrDefault(b => b.Id == id && b.UserId == userId);

                if (booking == null)
                {
                    return Json(new { success = false, message = "Booking not found." });
                }

                if (booking.Status == "Cancelled")
                {
                    return Json(new { success = false, message = "Booking is already cancelled." });
                }

                if (booking.BookingDate <= DateTime.Now.AddDays(1))
                {
                    return Json(new { success = false, message = "Cannot cancel booking less than 24 hours before the tour date." });
                }

                booking.Status = "Cancelled";
                _context.SaveChanges();

                return Json(new { success = true, message = "Booking cancelled successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error cancelling booking: " + ex.Message });
            }
        }

        public IActionResult BookingDetails(int id)
        {
            if (!IsCustomerLoggedIn())
            {
                TempData["Error"] = "Access denied. Customer login required.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var userId = GetCurrentUserId();
                var booking = _context.Bookings
                    .Include(b => b.City)
                    .Include(b => b.User)
                    .FirstOrDefault(b => b.Id == id && b.UserId == userId);

                if (booking == null)
                {
                    TempData["Error"] = "Booking not found.";
                    return RedirectToAction("MyBookings");
                }

                ViewBag.Username = _sessionService.GetSession<string>("Username");
                return View(booking);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading booking details: " + ex.Message;
                return RedirectToAction("MyBookings");
            }
        }
    }
}
