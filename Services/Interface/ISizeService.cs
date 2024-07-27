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
    public interface ISizeService
    {
        Task<List<SizeModel>> GetSizesOfProduct(string productId);
        Task<Size> GetById(string id);
        Task Update(Size size);
        Task<Size> Add(string sizeName);
        Task<PaginatedList<Size>> GetPaginatedSizes(
            string searchQuery,
            string sortBy,
            int pageIndex,
            int pageSize);
        Task Delete(string id);
        Task<List<Size>> GetAll();
    }
}
