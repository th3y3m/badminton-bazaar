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
    public interface IColorService
    {
        Task<List<ColorModel>> GetColorsOfProduct(string productId);
        Task<Color> Add(string colorName);
        Task Update(Color color);
        Task<Color> GetById(string id);
        Task<List<Color>> GetAll();
        Task<PaginatedList<Color>> GetPaginatedColors(
            string searchQuery,
            string sortBy,
            int pageIndex,
            int pageSize);
        Task Delete(string id);
    }
}
