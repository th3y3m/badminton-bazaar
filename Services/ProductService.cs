using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        public ProductService(IProductRepository productRepository, IProductVariantRepository productVariantRepository, IOrderDetailRepository orderDetailRepository)
        {
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<PaginatedList<Product>> GetPaginatedProducts(
            string searchQuery,
            decimal? start,
            decimal? end,
            string sortBy,
            bool? status,
            string supplierId,
            string categoryId,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _productRepository.GetDbSet();
                var source = dbSet.AsNoTracking();

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    source = source.Where(p => p.ProductName.ToLower().Contains(searchQuery.ToLower()));
                }

                if (start.HasValue && end.HasValue)
                {
                    source = source.Where(p => p.BasePrice >= start && p.BasePrice <= end);
                }

                if (status.HasValue)
                {
                    source = source.Where(p => p.Status == status);
                }

                if (!string.IsNullOrEmpty(supplierId))
                {
                    source = source.Where(p => p.SupplierId == supplierId);
                }

                if (!string.IsNullOrEmpty(categoryId))
                {
                    source = source.Where(p => p.CategoryId == categoryId);
                }

                source = sortBy switch
                {
                    "name_asc" => source.OrderBy(p => p.ProductName),
                    "name_desc" => source.OrderByDescending(p => p.ProductName),
                    "price_asc" => source.OrderBy(p => p.BasePrice),
                    "price_desc" => source.OrderByDescending(p => p.BasePrice),
                    "top_seller" => source.OrderByDescending(p => GetSelledProduct(p.ProductId)),
                    _ => source
                };

                var count = source.Count();
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return new PaginatedList<Product>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated products: {ex.Message}");
            }
        }

        public async Task<Product> AddProduct(ProductModel productModel)
        {
            try
            {
                var product = new Product
                {
                    ProductId = "P" + GenerateId.GenerateRandomId(4),
                    ProductName = productModel.ProductName,
                    CategoryId = productModel.CategoryId,
                    SupplierId = productModel.SupplierId,
                    ProductDescription = productModel.ProductDescription,
                    BasePrice = productModel.BasePrice,
                    ImageUrl = productModel.ImageUrl,
                    Status = productModel.Status
                };
                await _productRepository.Add(product);
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding product: {ex.Message}");
            }
        }

        public async Task<Product> UpdateProduct(ProductModel productModel, string productId)
        {
            try
            {
                var product = await GetProductById(productId);

                product.ProductName = productModel.ProductName;
                product.CategoryId = productModel.CategoryId;
                product.SupplierId = productModel.SupplierId;
                product.ProductDescription = productModel.ProductDescription;
                product.BasePrice = productModel.BasePrice;
                product.ImageUrl = productModel.ImageUrl;
                product.Status = productModel.Status;

                await _productRepository.Update(product);
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating product: {ex.Message}");
            }
        }

        public async Task DeleteProduct(string productId)
        {
            try
            {
                await _productRepository.Delete(productId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting product: {ex.Message}");
            }
        }

        public async Task<Product> GetProductById(string productId)
        {
            try
            {
                return await _productRepository.GetById(productId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product by ID: {ex.Message}");
            }
        }

        public async Task<int> GetSelledProduct(string productId)
        {
            try
            {
                var orderDetails = await _orderDetailRepository.GetAll();
                var products = await _productVariantRepository.GetAll();
                return orderDetails.Where(od => products.Any(pv => pv.ProductId == productId && pv.ProductVariantId == od.ProductVariantId))
                                   .Sum(od => od.Quantity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sold product count: {ex.Message}");
            }
        }

        public async Task<List<Product>> GetTopSeller(int n)
        {
            try
            {
                var products = await _productRepository.GetAll();
                var productSales = new List<(Product Product, int TotalSales)>();

                foreach (var product in products)
                {
                    var totalSales = await GetSelledProduct(product.ProductId);
                    product.ProductVariants = null;
                    productSales.Add((product, totalSales));
                }

                var topSellers = productSales.OrderByDescending(ps => ps.TotalSales)
                                             .Take(n)
                                             .Select(ps => ps.Product)
                                             .ToList();

                return topSellers;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving top sellers: {ex.Message}");
            }
        }

        public async Task<int> ProductRemaining(string productId)
        {
            try
            {
                var allProduct = await _productVariantRepository.GetAll();
                var productVariants = allProduct.Where(p => p.ProductId == productId).ToList();
                int total = 0;
                foreach (var productVariant in productVariants)
                {
                    total += productVariant.StockQuantity;
                }
                return total;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product remaining: {ex.Message}");
            }
        }

        public async Task<Product> GetProductByProductVariantId(string productVariantId)
        {
            try
            {
                var productVariant = await _productVariantRepository.GetById(productVariantId);
                return await _productRepository.GetById(productVariant.ProductId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product by product variant ID: {ex.Message}");
            }
        }

        public async Task<List<Product>> GetRelatedProduct(string productId)
        {
            try
            {
                var product = await _productRepository.GetById(productId);
                var products = await _productRepository.GetAll();
                return products.Where(p => p.CategoryId == product.CategoryId && p.ProductId != productId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving related products: {ex.Message}");
            }
        }
    }
}
