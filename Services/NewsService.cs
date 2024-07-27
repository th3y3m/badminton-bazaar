using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;

        public NewsService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<PaginatedList<News>> GetPaginatedNews(
            string searchQuery,
            string sortBy,
            bool? IsHomepageBanner,
            bool? IsHomepageSlideShow,
            bool? status,
            int pageIndex,
            int pageSize)
        {
            var dbSet = await _newsRepository.GetDbSet();
            var source = dbSet.AsNoTracking();

            if (IsHomepageBanner.HasValue)
            {
                source = source.Where(p => p.IsHomepageBanner == IsHomepageBanner);
            }
            if (IsHomepageSlideShow.HasValue)
            {
                source = source.Where(p => p.IsHomepageSlideshow == IsHomepageSlideShow);
            }


            if (!string.IsNullOrEmpty(searchQuery))
            {
                source = source.Where(p => p.Title.ToLower().Contains(searchQuery.ToLower()) || p.Content.ToLower().Contains(searchQuery.ToLower()));
            }

            if (status.HasValue)
            {
                source = source.Where(p => p.Status == status);
            }

            source = sortBy switch
            {
                "publicationdate_asc" => source.OrderBy(p => p.PublicationDate),
                "publicationdate_desc" => source.OrderByDescending(p => p.PublicationDate),
                "title_asc" => source.OrderBy(p => p.Title),
                "title_desc" => source.OrderByDescending(p => p.Title),
                _ => source
            };

            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<News>(items, count, pageIndex, pageSize);
        }

        public async Task<News> GetNewsById(string id)
        {
            return await _newsRepository.GetById(id);
        }

        public async Task<News> AddNews(NewsModel newsModel)
        {
            var news = new News
            {
                NewId = "N" + GenerateId.GenerateRandomId(5),
                Title = newsModel.Title,
                Content = newsModel.Content,
                Image = newsModel.Image,
                PublicationDate = DateTime.Now,
                IsHomepageSlideshow = newsModel.IsHomepageSlideshow,
                IsHomepageBanner = newsModel.IsHomepageBanner,
                Status = newsModel.Status
            };
            await _newsRepository.Add(news);
            return news;
        }

        public async Task<News> UpdateNews(string id, NewsModel newsModel)
        {
            var news = await _newsRepository.GetById(id);
            if (news == null)
            {
                return null;
            }

            news.Title = newsModel.Title;
            news.Content = newsModel.Content;
            news.Image = newsModel.Image;
            news.IsHomepageSlideshow = newsModel.IsHomepageSlideshow;
            news.IsHomepageBanner = newsModel.IsHomepageBanner;
            news.Status = newsModel.Status;

            await _newsRepository.Update(news);
            return news;
        }

        public async Task<News?> DeleteNews(string id) {
            var news = await _newsRepository.GetById(id);
            if (news == null)
            {
                return null;
            }
            await _newsRepository.Delete(id);
            return news;
        }

        public async Task<List<News>> GetSlideshowNews() {
            List<News> news = await _newsRepository.GetAll();
            return news.Where(p => p.IsHomepageSlideshow == true).ToList();
        }

        public async Task<List<News>> GetBannerNews() {
            List<News> news = await _newsRepository.GetAll();
            return news.Where(p => p.IsHomepageBanner == true).ToList();
        }
    }
}
