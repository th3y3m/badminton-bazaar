using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class NewsService
    {
        private readonly NewsRepository _newsRepository;

        public NewsService(NewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public PaginatedList<News> GetPaginatedOrders(
            string searchQuery,
            string sortBy,
            string status,
            int pageIndex,
            int pageSize)
        {
            var source = _newsRepository.GetDbSet().AsNoTracking();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                source = source.Where(p => p.Title.ToLower().Contains(searchQuery.ToLower()) || p.Content.ToLower().Contains(searchQuery.ToLower()));
            }

            if (!string.IsNullOrEmpty(status))
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
    }
}
