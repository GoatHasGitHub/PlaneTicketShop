using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PlaneTicketShop.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace PlaneTicketShop.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PlaneTicketShop.Infrastructure.Data.ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, PlaneTicketShop.Infrastructure.Data.ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> Index(string? departureCity, string? arrivalCity, DateTime? departureDate, int? passengers)
    {
        var query = _context.Flights.AsQueryable();
        if (!string.IsNullOrEmpty(departureCity))
            query = query.Where(f => f.DepartureCity.Contains(departureCity));
        if (!string.IsNullOrEmpty(arrivalCity))
            query = query.Where(f => f.ArrivalCity.Contains(arrivalCity));
        if (departureDate.HasValue)
            query = query.Where(f => f.DepartureTime.Date == departureDate.Value.Date);
        if (passengers.HasValue)
            query = query.Where(f => f.AvailableSeats >= passengers.Value);
        var flights = await query.ToListAsync();
        ViewBag.SearchResults = flights;
        return View();
    }
}
