using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FUNewsManagementSystem.Services;
using FUNewsManagementSystem.BusinessObject;
using NewsManagementMVC.Models.ViewModels.NewsArticle;
using FUNewsManagementSystem.BusinessObject.Enums;
using NewsManagementMVC.Attributes;

namespace NewsManagementMVC.Controllers
{
    public class NewsArticlesController : Controller
    {
        private readonly INewsArticleService _contextNewsArticle;
        private readonly ICategoryService _contextCategory;
        private readonly ITagService _contextTag;

        public NewsArticlesController(INewsArticleService contextNewsArticle, ICategoryService contextCategory, ITagService contextTag)
        {
            _contextNewsArticle = contextNewsArticle;
            _contextCategory = contextCategory;
            _contextTag = contextTag;
        }

        // GET: NewsArticles
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetInt32("Role");
            var listNewsArticles = _contextNewsArticle.GetNewsArticles();
            if (role == (int)AccountRole.Staff || role == (int)AccountRole.Admin)
            {
                return View(listNewsArticles.ToList());
            }
            var activeNewsArticles = listNewsArticles.Where(n => n.NewsStatus == true)
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
            ViewData["TagIds"] = new MultiSelectList(_contextTag.GetTags(), "TagId", "TagName");
            return View();
        }

        // POST: NewsArticles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(CreateNewsArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var newsArticle = model.ToNewsArticle((short)userId.Value);
                _contextNewsArticle.SaveNewsArticle(newsArticle);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_contextCategory.GetCategories(), "CategoryId", "CategoryId", model.CategoryId);
            ViewData["TagIds"] = new MultiSelectList(_contextTag.GetTags(), "TagId", "TagName", model.SelectedTagIds);
            return View(model);
        }

        // GET: NewsArticles/Edit/5
        public async Task<IActionResult> Edit(string? id)
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

        public IActionResult MyNewsArticle()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var newsListEntities = _contextNewsArticle.GetListNewsArticlesByCreator(userId.Value);

            var newsListViewModels = newsListEntities.Select(n => new NewsArticleSummaryViewModel
            {
                NewsArticleId = n.NewsArticleId,
                NewsTitle = n.NewsTitle,
                CreatedDate = n.CreatedDate,
                CategoryName = n.Category?.CategoryName,
                NewsStatus = n.NewsStatus,
                ModifiedDate = n.ModifiedDate
            }).ToList();

            return View(newsListViewModels);
        }

        public IActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var newsArticle = _contextNewsArticle.GetDetailNewsArticleById(id);
            if (newsArticle == null)
            {
                return NotFound();
            }

            var viewModel = new NewsArticleDetailViewModel
            {
                NewsTitle = newsArticle.NewsTitle,
                Headline = newsArticle.Headline,
                CreatedDate = newsArticle.CreatedDate,
                NewsContent = newsArticle.NewsContent,
                NewsSource = newsArticle.NewsSource,
                CategoryName = newsArticle.Category?.CategoryName,
                NewsStatus = newsArticle.NewsStatus,
                CreatedName = newsArticle.CreatedBy?.AccountName ?? "Unknown",
                UpdatedName = newsArticle.UpdatedBy?.AccountName ?? "Unknown",
                ModifiedDate = newsArticle.ModifiedDate,
                TagNames = newsArticle.Tags?.Select(t => t.TagName).ToList() ?? new List<string>()
            };

            return View("MyDetailNewsArticle", viewModel);
        }
    }
}
