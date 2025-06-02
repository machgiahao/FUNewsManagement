using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntityNewsArticle = FUNewsManagementSystem.BusinessObject.NewsArticle;
using FUNewsManagementSystem.BusinessObject.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using FUNewsManagementSystem.BusinessObject;

namespace NewsManagementMVC.Models.ViewModels.NewsArticle
{
    public class EditNewsArticleViewModel
    {
        public string NewsArticleId { get; set; }
        [Required(ErrorMessage = "News title is required.")]
        public string NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required.")]
        public string Headline { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? NewsContent { get; set; }

        public string? NewsSource { get; set; }

        public short? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public bool? NewsStatus { get; set; }

        public List<int> SelectedTagIds { get; set; } = new();

        public EntityNewsArticle ToNewsArticle(short id)
        {
            return new EntityNewsArticle
            {
                NewsArticleId = this.NewsArticleId,
                NewsTitle = this.NewsTitle,
                Headline = this.Headline,
                ModifiedDate = this.ModifiedDate ?? DateTime.Now,
                NewsContent = this.NewsContent,
                NewsSource = this.NewsSource,
                CategoryId = this.CategoryId,
                NewsStatus = this.NewsStatus,
                UpdatedById = id,
                Tags = this.SelectedTagIds.Select(tagId => new Tag { TagId = tagId }).ToList()
            };
        }

        public static EditNewsArticleViewModel FromNewsArticle(EntityNewsArticle entity)
        {
            return new EditNewsArticleViewModel
            {
                NewsArticleId = entity.NewsArticleId,
                NewsTitle = entity.NewsTitle,
                Headline = entity.Headline,
                ModifiedDate = entity.ModifiedDate,
                NewsContent = entity.NewsContent,
                NewsSource = entity.NewsSource,
                CategoryId = entity.CategoryId,

                NewsStatus = entity.NewsStatus,
                SelectedTagIds = entity.Tags != null
                    ? new List<int>(entity.Tags.Select(t => t.TagId))
                    : new List<int>()
            };
        }

    }

}
