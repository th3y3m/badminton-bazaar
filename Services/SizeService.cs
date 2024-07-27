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
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _sizeRepository;
        private readonly IProductVariantRepository _productVariantRepository;

        public SizeService(ISizeRepository sizeRepository, IProductVariantRepository productVariantRepository)
        {
            _sizeRepository = sizeRepository;
            _productVariantRepository = productVariantRepository;
        }

        public async Task<List<SizeModel>> GetSizesOfProduct(string productId)
        {
            var allProduct = await _productVariantRepository.GetAll();
            var productVariants = allProduct.Where(p => p.ProductId == productId).ToList();
            var sizeIds = productVariants.Select(p => p.SizeId).ToList();
            var allSizes = await GetAll();
            var sizes = allSizes.Where(p => sizeIds.Contains(p.SizeId)).ToList();
            var sizeModels = new List<SizeModel>();
            foreach (var size in sizes)
            {
                var sizeModel = new SizeModel
                {
                    SizeId = size.SizeId,
                    SizeName = size.SizeName
                };
                sizeModels.Add(sizeModel);
            }
            return sizeModels;
        }

        public async Task<Size> Add(string sizeName)
        {
            var size = new Size
            {
                SizeId = "SZ" + GenerateId.GenerateRandomId(5),
                SizeName = sizeName
            };
            await _sizeRepository.Add(size);
            return size;
        }

        public async Task Update(Size size)
        {
            await _sizeRepository.Update(size);
        }

        public async Task<Size> GetById(string id)
        {
            return await _sizeRepository.GetById(id);
        }

        public async Task<List<Size>> GetAll()
        {
            return await _sizeRepository.GetAll();
        }

        public async Task Delete(string id)
        {
            await _sizeRepository.Delete(id);
        }

        public async Task<PaginatedList<Size>> GetPaginatedSizes(
            string searchQuery,
            string sortBy,
            int pageIndex,
            int pageSize)
        {
            var dbSet = await _sizeRepository.GetDbSet();
            var source = dbSet.AsNoTracking();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                source = source.Where(p => p.SizeName.ToLower().Contains(searchQuery.ToLower()));
            }

            source = sortBy switch
            {
                "size_asc" => source.OrderBy(p => p.SizeName),
                "size_desc" => source.OrderByDescending(p => p.SizeName),
                _ => source
            };

            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<Size>(items, count, pageIndex, pageSize);
        }


    }
}
