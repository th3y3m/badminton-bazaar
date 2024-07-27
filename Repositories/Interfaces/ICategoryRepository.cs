using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task Add(Category category);
        Task Update(Category category);
        Task<Category> GetById(string id);
        Task<List<Category>> GetAll();
        Task<DbSet<Category>> GetDbSet();
        Task Delete(string id);
    }
}
