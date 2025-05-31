using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntityNewsArticle = FUNewsManagementSystem.BusinessObject.NewsArticle;
using FUNewsManagementSystem.BusinessObject.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using FUNewsManagementSystem.BusinessObject;

namespace NewsManagementMVC.Models.ViewModels.NewsArticle
{
    public class CreateNewsArticleViewModel
    {
        [Required(ErrorMessage = "News title is required.")]
        public string NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required.")]
        public string Headline { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? NewsContent { get; set; }

        public string? NewsSource { get; set; }

        public short? CategoryId { get; set; }

        [Required]
        public bool? NewsStatus { get; set; }

        public List<int> SelectedTagIds { get; set; } = new();

        public EntityNewsArticle ToNewsArticle(short createdById)
        {
            return new EntityNewsArticle
            {
                NewsArticleId = Guid.NewGuid().ToString("N").Substring(0, 20),
                NewsTitle = this.NewsTitle,
                Headline = this.Headline,
                CreatedDate = this.CreatedDate ?? DateTime.Now,
                NewsContent = this.NewsContent,
                NewsSource = this.NewsSource,
                CategoryId = this.CategoryId,
                NewsStatus = this.NewsStatus ?? false,
                CreatedById = createdById,
                ModifiedDate = null,
                Tags = this.SelectedTagIds.Select(tagId => new Tag { TagId = tagId }).ToList()
            };
        }
    }

}
