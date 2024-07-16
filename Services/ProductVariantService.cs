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
    public class ProductVariantService
    {
        private readonly ProductVariantRepository _productVariantRepository;

        public ProductVariantService(ProductVariantRepository productVariantRepository)
        {
            _productVariantRepository = productVariantRepository;
        }

        public ProductVariant Add(ProductVariantModel productVariantModel)
        {
            var productVariant = new ProductVariant
            {
                ProductVariantId = "PV" + GenerateId.GenerateRandomId(5),
                ProductId = productVariantModel.ProductId,
                SizeId = productVariantModel.SizeId,
                ColorId = productVariantModel.ColorId,
                Price = productVariantModel.Price,
                StockQuantity = productVariantModel.StockQuantity,
                Status = productVariantModel.Status,
                VariantImageURL = productVariantModel.ProductImageUrl != null ? productVariantModel.ProductImageUrl[0].FileName : null
            };
            _productVariantRepository.Add(productVariant);
            return productVariant;
        }

        public void Update(ProductVariantModel productVariantModel, string id)
        {

            var productVariant = GetById(id);

            if (productVariant == null)
            {
                return;
            }

            productVariant.ProductId = productVariantModel.ProductId;
            productVariant.SizeId = productVariantModel.SizeId;
            productVariant.ColorId = productVariantModel.ColorId;
            productVariant.Price = productVariantModel.Price;
            productVariant.StockQuantity = productVariantModel.StockQuantity;
            productVariant.Status = productVariantModel.Status;
            productVariant.VariantImageURL = productVariantModel.ProductImageUrl != null ? productVariantModel.ProductImageUrl[0].FileName : null;

            _productVariantRepository.Update(productVariant);
        }

        public ProductVariant GetById(string id)
        {
            return _productVariantRepository.GetById(id);
        }

        public List<ProductVariant> GetAll()
        {
            return _productVariantRepository.GetAll();
        }

        public void DeleteById(string id)
        {
            _productVariantRepository.DeleteById(id);
        }

        public PaginatedList<ProductVariant> GetPaginatedProductVariants(
            //string searchQuery,
            string sortBy,
            bool? status,
            string colorId,
            string sizeId,
            string productId,
            int pageIndex,
            int pageSize)
        {
            var source = _productVariantRepository.GetDbSet().AsNoTracking();

            // Apply search filter
            //if (!string.IsNullOrEmpty(searchQuery))
            //{
            //    source = source.Where(p => p.ProductName.ToLower().Contains(searchQuery.ToLower()));
            //}

            if (status.HasValue)
            {
                source = source.Where(p => p.Status == status);
            }

            if (!string.IsNullOrEmpty(colorId))
            {
                source = source.Where(p => p.ColorId == colorId);
            }

            if (!string.IsNullOrEmpty(sizeId))
            {
                source = source.Where(p => p.SizeId == sizeId);
            }
            if (!string.IsNullOrEmpty(productId))
            {
                source = source.Where(p => p.ProductId == productId);
            }

            // Apply sorting
            source = sortBy switch
            {
                "price_asc" => source.OrderBy(p => p.Price),
                "price_desc" => source.OrderByDescending(p => p.Price),
                "stock_asc" => source.OrderBy(p => p.StockQuantity),
                "stock_desc" => source.OrderByDescending(p => p.StockQuantity),
                _ => source
            };

            // Apply pagination
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<ProductVariant>(items, count, pageIndex, pageSize);
        }
    }
}
