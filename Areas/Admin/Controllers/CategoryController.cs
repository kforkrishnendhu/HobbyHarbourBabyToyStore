using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using HobbyHarbour.Data;
using HobbyHarbour.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HobbyHarbour.Areas.Admin.Controllers
{

    [Area("Admin")] // Specify the area
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)    //Here it is actually doing the dependancy injection inside  
        {                                                     //the constructor for using the application db context that 
            _db = db;                                         //is registered as a service in the program.cs file. Now we can 
        }                                                     //use the database tables using this _db object.

        // GET: /<controller>/
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Use this attribute to prevent cross-site request forgery (CSRF) attacks
        public IActionResult Create(Category model)
        {
            if (ModelState.IsValid)
            {
                // Model is valid; create and save the category
                _db.Categories.Add(model);
                _db.SaveChanges();

                // Save the category to the database or data store
                // Example: _dbContext.Categories.Add(category);
                // Example: _dbContext.SaveChanges();

                return RedirectToAction("Index"); // Redirect to the category list after successful creation
            }

            // Model is not valid; redisplay the form with validation errors
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Retrieve the category with the specified ID from the database
            var category = _db.Categories.Find(id);

            if (category == null)
            {
                // Category not found, return a not found view or redirect
                return NotFound();
            }

            // Map the category data to a CategoryViewModel or edit model
            var editModel = new Category
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                Description = category.Description
                // Map other properties as needed
            };

            return View(editModel); // Display the Edit view with the edit model
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category editModel)
        {
            if (ModelState.IsValid)
            {
                //Find the category in the database by its ID
                //var category = _db.Categories.Find(editModel.CategoryID);

                //if (category == null)
                //{
                //    // Category not found, return a not found view or redirect
                //    return NotFound();
                //}

                // Update the category with the edited data
                //category.CategoryName = editModel.CategoryName;
                //category.Description = editModel.Description;
                // Update other properties as needed
                _db.Categories.Update(editModel);
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
            var category = _db.Categories.Find(id);

            if (category == null)
            {
                // Category not found, return a not found view or redirect
                return NotFound();
            }

            return View(category); // Display the Delete view with the category data
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Use this attribute to prevent cross-site request forgery (CSRF) attacks
        public IActionResult DeleteConfirmed(int id)
        {
            // Find the category in the database by its ID
            var category = _db.Categories.Find(id);

            if (category == null)
            {
                // Category not found, return a not found view or redirect
                return NotFound();
            }

            // Reassign products and then delete the category
            ReassignProductsToUncategorized(category);

            // Delete the category
            _db.Categories.Remove(category);
            _db.SaveChanges();

            return RedirectToAction("Index"); // Redirect to the category list after successful deletion
        }

        private void ReassignProductsToUncategorized(Category category)
        {
            // Retrieve all products associated with the category
            var productsToReassign = _db.Products.Where(p => p.CategoryID == category.CategoryID).ToList();

            // Get the "Uncategorized" category
            var uncategorizedCategory = _db.Categories.SingleOrDefault(c => c.CategoryName == "Uncategorized");

            if (uncategorizedCategory != null)
            {
                // Reassign products to the "Uncategorized" category
                foreach (var product in productsToReassign)
                {
                    product.CategoryID = uncategorizedCategory.CategoryID;
                }

                _db.SaveChanges();
            }
        }



    }
}

