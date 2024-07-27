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
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;
        private readonly IProductVariantRepository _productVariantRepository;

        public ColorService(IColorRepository colorRepository, IProductVariantRepository productVariantRepository)
        {
            _colorRepository = colorRepository;
            _productVariantRepository = productVariantRepository;
        }

        public async Task<List<ColorModel>> GetColorsOfProduct(string productId)
        {
            var allProduct = await _productVariantRepository.GetAll();
            var productVariants = allProduct.Where(p => p.ProductId == productId).ToList();
            var colorIds = productVariants.Select(p => p.ColorId).ToList();
            var allColors = await GetAll();
            var colors = allColors.Where(p => colorIds.Contains(p.ColorId)).ToList();
            var colorModels = new List<ColorModel>();
            foreach (var color in colors)
            {
                var colorModel = new ColorModel
                {
                    ColorId = color.ColorId,
                    ColorName = color.ColorName
                };
                colorModels.Add(colorModel);
            }
            return colorModels;
        }

        public async Task<Color> Add(string colorName)
        {
            var color = new Color
            {
                ColorId = "CL" + GenerateId.GenerateRandomId(5),
                ColorName = colorName
            };

            await _colorRepository.Add(color);
            return color;
        }

        public async Task Update(Color color)
        {
            await _colorRepository.Update(color);
        }

        public async Task<Color> GetById(string id) => await _colorRepository.GetById(id);

        public async Task<List<Color>> GetAll()
        {
            return await _colorRepository.GetAll();
        }

        public async Task Delete(string id)
        {
            await _colorRepository.Delete(id);
        }

        public async Task<PaginatedList<Color>> GetPaginatedColors(
            string searchQuery,
            string sortBy,
            int pageIndex,
            int pageSize)
        {

            var dbSet = await _colorRepository.GetDbSet();
            var source = dbSet.AsNoTracking();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                source = source.Where(p => p.ColorName.ToLower().Contains(searchQuery.ToLower()));
            }

            source = sortBy switch
            {
                "color_asc" => source.OrderBy(p => p.ColorName),
                "color_desc" => source.OrderByDescending(p => p.ColorName),
                _ => source
            };

            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<Color>(items, count, pageIndex, pageSize);
        }
    }
}
