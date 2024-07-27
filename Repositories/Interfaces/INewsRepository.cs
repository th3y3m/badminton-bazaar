using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface INewsRepository
    {
        Task Add(News news);
        Task Update(News news);
        Task<News> GetById(string id);
        Task<List<News>> GetAll();
        Task<DbSet<News>> GetDbSet();
        Task Delete(string id);
    }
}
