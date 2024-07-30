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
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _productVariantRepository;

        public ProductVariantService(IProductVariantRepository productVariantRepository)
        {
            _productVariantRepository = productVariantRepository;
        }

        public async Task<ProductVariant> Add(ProductVariantModel productVariantModel)
        {
            try
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
                await _productVariantRepository.Add(productVariant);
                return productVariant;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding product variant: {ex.Message}");
            }
        }

        public async Task Update(ProductVariantModel productVariantModel, string id)
        {
            try
            {
                var productVariant = await GetById(id);

                if (productVariant == null)
                {
                    throw new Exception("Product variant not found");
                }

                productVariant.ProductId = productVariantModel.ProductId;
                productVariant.SizeId = productVariantModel.SizeId;
                productVariant.ColorId = productVariantModel.ColorId;
                productVariant.Price = productVariantModel.Price;
                productVariant.StockQuantity = productVariantModel.StockQuantity;
                productVariant.Status = productVariantModel.Status;
                productVariant.VariantImageURL = productVariantModel.ProductImageUrl != null ? productVariantModel.ProductImageUrl[0].FileName : null;

                await _productVariantRepository.Update(productVariant);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating product variant: {ex.Message}");
            }
        }

        public async Task<ProductVariant> GetById(string id)
        {
            try
            {
                return await _productVariantRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product variant by ID: {ex.Message}");
            }
        }

        public async Task<List<ProductVariant>> GetAll()
        {
            try
            {
                return await _productVariantRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all product variants: {ex.Message}");
            }
        }

        public async Task DeleteById(string id)
        {
            try
            {
                await _productVariantRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting product variant: {ex.Message}");
            }
        }

        public async Task<PaginatedList<ProductVariant>> GetPaginatedProductVariants(
            string sortBy,
            bool? status,
            string colorId,
            string sizeId,
            string productId,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _productVariantRepository.GetDbSet();
                var source = dbSet.AsNoTracking();

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

                source = sortBy switch
                {
                    "price_asc" => source.OrderBy(p => p.Price),
                    "price_desc" => source.OrderByDescending(p => p.Price),
                    "stock_asc" => source.OrderBy(p => p.StockQuantity),
                    "stock_desc" => source.OrderByDescending(p => p.StockQuantity),
                    _ => source
                };

                var count = source.Count();
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return new PaginatedList<ProductVariant>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated product variants: {ex.Message}");
            }
        }
    }
}
