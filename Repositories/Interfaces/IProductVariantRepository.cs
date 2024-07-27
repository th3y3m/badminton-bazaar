using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IProductVariantRepository
    {
        Task Add(ProductVariant productVariant);
        Task Update(ProductVariant productVariant);
        Task<ProductVariant> GetById(string id);
        Task<List<ProductVariant>> GetAll();
        Task<DbSet<ProductVariant>> GetDbSet();
        Task Delete(string id);
    }
}
