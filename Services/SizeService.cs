using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Helper;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class SizeService
    {
        private readonly SizeRepository _sizeRepository;
        private readonly ProductVariantRepository _productVariantRepository;

        public SizeService(SizeRepository sizeRepository, ProductVariantRepository productVariantRepository)
        {
            _sizeRepository = sizeRepository;
            _productVariantRepository = productVariantRepository;
        }

        public List<SizeModel> GetSizesOfProduct(string productId)
        {
            var productVariants = _productVariantRepository.GetAll().Where(p => p.ProductId == productId).ToList();
            var sizeIds = productVariants.Select(p => p.SizeId).ToList();
            var sizes = GetAll().Where(p => sizeIds.Contains(p.SizeId)).ToList();
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



        public Size Add(string sizeName)
        {
            var size = new Size
            {
                SizeId = "SZ" + GenerateId.GenerateRandomId(5),
                SizeName = sizeName
            };
            _sizeRepository.Add(size);
            return size;
        }

        public void Update(Size size)
        {
            _sizeRepository.Update(size);
        }

        public Size GetById(string id)
        {
            return _sizeRepository.GetById(id);
        }

        public List<Size> GetAll()
        {
            return _sizeRepository.GetAll();
        }

        public void Delete(string id)
        {
            _sizeRepository.Delete(id);
        }

        public PaginatedList<Size> GetPaginatedSizes(
            string searchQuery,
            string sortBy,
            int pageIndex,
            int pageSize)
        {
            var source = _sizeRepository.GetDbSet().AsNoTracking();

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
