using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HobbyHarbour.Models;
using HobbyHarbour.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace HobbyHarbour.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = "Customer")]
//[Authorize(Policy = "Customer")]
public class CustomerHomeController : Controller
{
    private readonly ILogger<CustomerHomeController> _logger;
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CustomerHomeController(ILogger<CustomerHomeController> logger, ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _db = db;                                           //is registered as a service in the program.cs file. Now we can
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        List<Product> objProductList = _db.Products.Include(p => p.Category).ToList();

        return View(objProductList);
    }

    public IActionResult Details(int id)
    {
        //var product = _db.Products.Find(id);
        var product = _db.Products.Include(p => p.Category) // Include the Category navigation property
                        .FirstOrDefault(p => p.ProductID == id); 
        return View(product);
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
}

