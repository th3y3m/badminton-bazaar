using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task Add(Product product);
        Task Update(Product product);
        Task<Product> GetById(string id);
        Task<List<Product>> GetAll();
        Task<DbSet<Product>> GetDbSet();
        Task Delete(string id);
    }
}
