using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;

        public ColorService(IColorRepository colorRepository, IProductVariantRepository productVariantRepository, IConnectionMultiplexer redisConnection)
        {
            _colorRepository = colorRepository;
            _productVariantRepository = productVariantRepository;
            _redisConnection = redisConnection;
            _redisDb = _redisConnection.GetDatabase();
        }

        public async Task<List<ColorModel>> GetColorsOfProduct(string productId)
        {
            try
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
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving colors of the product.", ex);
            }
        }

        public async Task<Color> Add(string colorName)
        {
            try
            {
                var color = new Color
                {
                    ColorId = "CL" + GenerateId.GenerateRandomId(5),
                    ColorName = colorName
                };

                await _colorRepository.Add(color);
                return color;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while adding the color.", ex);
            }
        }

        public async Task Update(Color color)
        {
            try
            {
                await _colorRepository.Update(color);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while updating the color.", ex);
            }
        }

        public async Task<Color> GetById(string id)
        {
            try
            {
                var cachedColor = await _redisDb.StringGetAsync($"color:{id}");

                if (!cachedColor.IsNullOrEmpty)
                    return JsonConvert.DeserializeObject<Color>(cachedColor);
                var color = await _colorRepository.GetById(id);

                await _redisDb.StringSetAsync($"color:{id}", JsonConvert.SerializeObject(color), TimeSpan.FromHours(1));
                return color;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving the color by ID.", ex);
            }
        }

        public async Task<List<Color>> GetAll()
        {
            try
            {
                return await _colorRepository.GetAll();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving all colors.", ex);
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                await _colorRepository.Delete(id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while deleting the color.", ex);
            }
        }

        public async Task<PaginatedList<Color>> GetPaginatedColors(
            string searchQuery,
            string sortBy,
            int pageIndex,
            int pageSize)
        {
            try
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

                var count = await source.CountAsync();
                var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

                return new PaginatedList<Color>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving paginated colors.", ex);
            }
        }
    }
}
