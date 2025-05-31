using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FUNewsManagementSystem.Services;
using FUNewsManagementSystem.BusinessObject;

namespace NewsManagementMVC.Controllers
{
    public class NewsArticlesController : Controller
    {
        private readonly INewsArticleService _contextNewsArticle;
        private readonly ICategoryService _contextCategory;

        public NewsArticlesController(INewsArticleService contextNewsArticle, ICategoryService contextCategory)
        {
            _contextNewsArticle = contextNewsArticle;
            _contextCategory = contextCategory;
        }

        // GET: NewsArticles
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            var listNewsArticles = _contextNewsArticle.GetNewsArticles();
            if (role == "Staff" || role == "Admin")
            {
                return View(listNewsArticles.ToList());
            }
            var activeNewsArticles =  listNewsArticles.Where(n => n.NewsStatus == true)
                                                      .OrderByDescending(n => n.CreatedDate)
                                                      .ToList(); ;
            return View(activeNewsArticles);
        }

        // GET: NewsArticles/Details/5
        public IActionResult Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle = _contextNewsArticle.GetNewsArticleById(id);
            if (newsArticle == null)
            {
                return NotFound();
            }

            return View(newsArticle);
        }

        // GET: NewsArticles/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_contextCategory.GetCategories(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: NewsArticles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NewsArticleId,NewsTitle,Headline,CreatedDate,NewsContent,NewsSource,CategoryId,NewsStatus,CreatedById")] NewsArticle newsArticle)
        {
            if (ModelState.IsValid)
            {
                _contextNewsArticle.SaveNewsArticle(newsArticle);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_contextCategory.GetCategories(), "CategoryId", "CategoryId", newsArticle.CategoryId);
            return View(newsArticle);
        }

        // GET: NewsArticles/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle =  _contextNewsArticle.GetNewsArticleById(id);
            if (newsArticle == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_contextCategory.GetCategories(), "CategoryId", "CategoryId", newsArticle.CategoryId);
            return View(newsArticle);
        }

        // POST: NewsArticles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NewsArticleId,NewsTitle,Headline,CreatedDate,NewsContent,NewsSource,CategoryId,NewsStatus,CreatedById,UpdatedById,ModifiedDate")] NewsArticle newsArticle)
        {
            if (id != newsArticle.NewsArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _contextNewsArticle.SaveNewsArticle(newsArticle);
                }
                catch (Exception)
                {
                    if (!NewsArticleExists(newsArticle.NewsArticleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_contextCategory.GetCategories(), "CategoryId", "CategoryId", newsArticle.CategoryId);
            return View(newsArticle);
        }

        // GET: NewsArticles/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle = _contextNewsArticle.GetNewsArticleById(id);
            if (newsArticle == null)
            {
                return NotFound();
            }

            return View(newsArticle);
        }

        // POST: NewsArticles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var newsArticle = _contextNewsArticle.GetNewsArticleById(id);
            if (newsArticle != null)
            {
                _contextNewsArticle.DeleteNewsArticle(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool NewsArticleExists(string id)
        {
            var tmp = _contextNewsArticle.GetNewsArticleById(id);
            return (tmp == null) ? false : true;
        }

        // GET: NewsArticles/Report
        public IActionResult Report(DateTime? startDate, DateTime? endDate)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Forbid();
            }

            List<NewsArticle> reportData = new();
            if (startDate.HasValue && endDate.HasValue)
            {
                reportData = _contextNewsArticle.GetNewsArticlesByPeriod(startDate.Value, endDate.Value);
            }

            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;
            return View(reportData);
        }

    }
}
