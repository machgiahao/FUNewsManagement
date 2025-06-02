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
        public async Task<IActionResult> Index(string? searchField, string? searchString, int? tagId)
        {
            var role = HttpContext.Session.GetInt32("Role");
            var listNewsArticles = _contextNewsArticle.SearchNewsArticles(searchField, searchString, tagId);
            var viewModels = listNewsArticles.Select(NewsArticleViewModel.FromNewsArticle).ToList();

            // Prepare edit models for each article
            var editModels = listNewsArticles
                .Select(EditNewsArticleViewModel.FromNewsArticle)
                .ToDictionary(m => m.NewsArticleId);

            ViewData["CategoryId"] = new SelectList(_contextCategory.GetCategories(), "CategoryId", "CategoryName");
            ViewData["TagIds"] = new MultiSelectList(_contextTag.GetTags(), "TagId", "TagName");
            ViewBag.Role = role;
            ViewBag.EditModels = editModels;

            if (role == (int)AccountRole.Staff || role == (int)AccountRole.Admin)
            {
                return View(viewModels);
            }
            var activeViewModels = viewModels
                                    .Where(n => n.NewsStatus == true)
                                    .OrderByDescending(n => n.NewsArticleId) // or another property if you prefer
                                    .OrderByDescending(n => n.CreatedDate)
                                    .ToList();
            return View(activeViewModels);
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
        [CustomAuthorize(AccountRole.Staff)]
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
        [CustomAuthorize(AccountRole.Staff)]

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
        [CustomAuthorize(AccountRole.Staff)]
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
            var viewModel = EditNewsArticleViewModel.FromNewsArticle(newsArticle);
            ViewData["CategoryId"] = new SelectList(_contextCategory.GetCategories(), "CategoryId", "CategoryId", newsArticle.CategoryId);
            ViewData["TagIds"] = new MultiSelectList(_contextTag.GetTags(), "TagId", "TagName", viewModel.SelectedTagIds);
            return View(viewModel);
        }

        // POST: NewsArticles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(AccountRole.Staff)]
        public async Task<IActionResult> Edit(string id, EditNewsArticleViewModel model)
        {
            if (id != model.NewsArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var newsArticle = model.ToNewsArticle((short)HttpContext.Session.GetInt32("UserId").Value);
                    _contextNewsArticle.UpdateNewsArticle(newsArticle);
                }
                catch (Exception)
                {
                    if (!NewsArticleExists(model.NewsArticleId))
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
            ViewData["CategoryId"] = new SelectList(_contextCategory.GetCategories(), "CategoryId", "CategoryId", model.CategoryId);
            ViewData["TagIds"] = new MultiSelectList(_contextTag.GetTags(), "TagId", "TagName", model.SelectedTagIds);
            return View(model);
        }

        // GET: NewsArticles/Delete/5
        [CustomAuthorize(AccountRole.Staff)]
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
        [CustomAuthorize(AccountRole.Staff)]
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
        [CustomAuthorize(AccountRole.Admin)]
        public IActionResult Report(DateTime? startDate, DateTime? endDate)
        {
            List<NewsArticle> reportData = new();
            if (startDate.HasValue && endDate.HasValue)
            {
                reportData = _contextNewsArticle.GetNewsArticlesByPeriod(startDate.Value, endDate.Value);
            }

            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;
            return View(reportData);
        }

        [CustomAuthorize(AccountRole.Staff)]
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
    }
}
