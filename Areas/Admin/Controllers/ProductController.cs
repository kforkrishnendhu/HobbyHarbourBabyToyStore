using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using HobbyHarbour.Data;
using HobbyHarbour.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HobbyHarbour.Areas.Admin.Controllers
{
    [Area("Admin")] // Specify the area
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)    //Here it is actually doing the dependancy injection inside  
        {                                                     //the constructor for using the application db context that 
            _db = db;                                           //is registered as a service in the program.cs file. Now we can
            _webHostEnvironment = webHostEnvironment;           //use the database tables using this _db object.
        }                                                     

        // GET: /<controller>/
        public IActionResult Index()
        {
            //List<Product> objProductList = _db.Products.ToList();
            List<Product> objProductList = _db.Products.Include(p => p.Category).ToList();

            return View(objProductList);
        }

        public IActionResult Create()
        {
            // Retrieve a list of categories from your data source (e.g., a database)
            var categories = _db.Categories.ToList(); // Replace "_db.Categories" with your actual data retrieval logic

            // Create a SelectList of categories to use with the dropdown
            ViewBag.CategoryList = new SelectList(categories, "CategoryID", "CategoryName");
            var model = new Product
            {
                ImgUrl = string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Use this attribute to prevent cross-site request forgery (CSRF) attacks
        public IActionResult Create(Product model,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file!=null)
                {
                    string fileName = Guid.NewGuid().ToString()+ Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images/product");

                    using(var fileStream=new FileStream(Path.Combine(productPath,fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    model.ImgUrl = @"/images/product/" + fileName;
                }
                // Model is valid; create and save the category
                _db.Products.Add(model);
                _db.SaveChanges();

                // Save the category to the database or data store
                // Example: _dbContext.Categories.Add(category);
                // Example: _dbContext.SaveChanges();

                return RedirectToAction("Index"); // Redirect to the category list after successful creation
            }


            // Model is not valid; redisplay the form with validation errors

            // Retrieve a list of categories from your data source (e.g., a database)
            var categories = _db.Categories.ToList(); // Replace "_db.Categories" with your actual data retrieval logic

            // Create a SelectList of categories to use with the dropdown
            ViewBag.CategoryList = new SelectList(categories, "CategoryID", "CategoryName");


            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var categories = _db.Categories.ToList(); // Replace "_db.Categories" with your actual data retrieval logic

            // Create a SelectList of categories to use with the dropdown
            ViewBag.CategoryList = new SelectList(categories, "CategoryID", "CategoryName");

            // Retrieve the category with the specified ID from the database
            var product = _db.Products.Find(id);

            if (product == null)
            {
                // Category not found, return a not found view or redirect
                return NotFound();
            }

            // Map the category data to a CategoryViewModel or edit model
            var editModel = new Product
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Description = product.Description,
                Price=product.Price,
                StockQuantity=product.StockQuantity,
                CategoryID=product.CategoryID,
                ImgUrl=product.ImgUrl
                // Map other properties as needed
            };

            return View(editModel); // Display the Edit view with the edit model
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product editModel,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images/product");
                    if(!string.IsNullOrEmpty(editModel.ImgUrl))
                    {
                        //delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, editModel.ImgUrl.TrimStart('/','\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    editModel.ImgUrl = @"/images/product/" + fileName;
                }

                _db.Products.Update(editModel);
                // Save the changes to the database
                _db.SaveChanges();

                return RedirectToAction("Index"); // Redirect to the category list after successful edit
            }

            // Model is not valid; redisplay the Edit view with validation errors
            return View(editModel);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            // Retrieve the category with the specified ID from the database
            var product = _db.Products.Find(id);

            if (product == null)
            {
                // Category not found, return a not found view or redirect
                return NotFound();
            }

            return View(product); // Display the Delete view with the category data
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Use this attribute to prevent cross-site request forgery (CSRF) attacks
        public IActionResult DeleteConfirmed(int id)
        {
            // Find the category in the database by its ID
            var product = _db.Products.Find(id);

            if (product == null)
            {
                // Category not found, return a not found view or redirect
                return NotFound();
            }

           
            //delete the old image
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImgUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            // Delete the category
            _db.Products.Remove(product);
            _db.SaveChanges();

            return RedirectToAction("Index"); // Redirect to the category list after successful deletion
        }


    }
}

