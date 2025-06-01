using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FUNewsManagementSystem.BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FUNewsManagementSystem.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using FUNewsManagementSystem.BusinessObject.Enums;

namespace NewsManagementMVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Categories
        public IActionResult Index(string search)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1)
            {
                return NotFound();
            }

            ViewData["ShowSearch"] = true;

            var categories = _categoryService.GetCategories().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                categories = categories.Where(c => c.CategoryName.ToLower().Contains(search.ToLower()));
            }

            var listCategories = categories.ToList();

            return View(listCategories);
        }

        // GET: Categories/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _categoryService.GetCategoryById((int)id);

            if (category == null)
            {
                return NotFound();
            }
            if (category.ParentCategoryId != null)
            {
                category.ParentCategory = _categoryService.GetCategoryById((int)category.ParentCategoryId);
            }
            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["ParentCategoryId"] = new SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName,CategoryDesciption,ParentCategoryId,IsActive")] Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryService.CreateCategory(category);
                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            else {
                TempData["ErrorMessage"] = $"Failed to created category";
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _categoryService.GetCategoryById((int)id);
            if (category == null)
            {
                return NotFound();
            }

            var allCategories = _categoryService.GetCategories().ToList();

            var parentCandidates = allCategories
                .Where(c => c.CategoryId != category.CategoryId)
                .ToList();

            ViewData["ParentCategoryId"] = new SelectList(
                parentCandidates,
                "CategoryId",
                "CategoryName",
                category.ParentCategoryId
            );

            return View(category);
        }


        // POST: Categories/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CategoryId,CategoryName,CategoryDesciption,ParentCategoryId,IsActive")] Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _categoryService.UpdateCategory(category);
                    TempData["SuccessMessage"] = "Category updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Failed to updated category";
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete?id=5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _categoryService.GetCategoryById((int)id);

            if (category == null)
            {
                return NotFound();
            }
            if (category.ParentCategoryId != null)
            {
                category.ParentCategory = _categoryService.GetCategoryById((int)category.ParentCategoryId);
            }

            return View(category);
        }

        // POST: Categories/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var cate = _categoryService.GetCategoryById(id);
            if (cate != null)
            {
                if(cate.NewsArticles.Count == 0){
                    _categoryService.DeleteCategory(id);
                    TempData["SuccessMessage"] = "Category deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to delete category, it belong to {cate.NewsArticles.Count} articles";
                }
            }
                return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(short id)
        {
            var tmp = _categoryService.GetCategoryById(id);
            return (tmp == null) ? false : true;
        }
    }
}
