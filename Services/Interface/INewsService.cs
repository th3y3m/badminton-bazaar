using BusinessObjects;
using Services.Helper;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface INewsService
    {
        Task<PaginatedList<News>> GetPaginatedNews(
            string searchQuery,
            string sortBy,
            bool? IsHomepageBanner,
            bool? IsHomepageSlideShow,
            bool? status,
            int pageIndex,
            int pageSize);

        Task<News> GetNewsById(string id);
        Task<News> AddNews(NewsModel newsModel);
        Task<News> UpdateNews(string id, NewsModel newsModel);
        Task<News?> DeleteNews(string id);
        Task<List<News>> GetBannerNews();
        Task<List<News>> GetSlideshowNews();
    }
}
