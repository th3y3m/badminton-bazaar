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

            var dbSet = await _productRepository.GetDbSet();
            var source = dbSet.AsNoTracking();
            // Apply search filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                source = source.Where(p => p.ProductName.ToLower().Contains(searchQuery.ToLower()));
            }

            // Apply price filter check if start and end are not null because i will use react nouislider
            if (start.HasValue && end.HasValue)
            {
                source = source.Where(p => p.BasePrice >= start && p.BasePrice <= end);
            }

            // Apply status filter
            if (status.HasValue)
            {
                source = source.Where(p => p.Status == status);
            }

            // Apply supplier filter
            if (!string.IsNullOrEmpty(supplierId))
            {
                source = source.Where(p => p.SupplierId == supplierId);
            }

            // Apply category filter
            if (!string.IsNullOrEmpty(categoryId))
            {
                source = source.Where(p => p.CategoryId == categoryId);
            }

            // Apply sorting
            source = sortBy switch
            {
                "name_asc" => source.OrderBy(p => p.ProductName),
                "name_desc" => source.OrderByDescending(p => p.ProductName),
                "price_asc" => source.OrderBy(p => p.BasePrice),
                "price_desc" => source.OrderByDescending(p => p.BasePrice),
                "top_seller" => source.OrderByDescending(p => GetSelledProduct(p.ProductId)),
                _ => source
            };

            // Apply pagination
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<Product>(items, count, pageIndex, pageSize);
        }

        public async Task<Product> AddProduct(ProductModel productModel)
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

        public async Task<Product> UpdateProduct(ProductModel productModel, string productId)
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

        public async Task DeleteProduct(string productId) => await _productRepository.Delete(productId);

        public async Task<Product> GetProductById(string productId) => await _productRepository.GetById(productId);
        //public int GetSelledProduct(string productId)
        //{
        //    int total = 0;
        //    List<ProductVariant> productVariants = _productVariantRepository.GetAll().Where(p => p.ProductId == productId).ToList();
        //    foreach (var productVariant in productVariants)
        //    {
        //        List<OrderDetail> orderDetails = _orderDetailRepository.GetAll().Where(p => p.ProductVariantId == productVariant.ProductVariantId).ToList();
        //        foreach (var orderDetail in orderDetails)
        //        {
        //            total += orderDetail.Quantity;
        //        }
        //    }
        //    return total;
        //}

        //public List<Product> GetTopSeller(int n)
        //{
        //    List<Product> products = _productRepository.GetAll().ToList();
        //    products.Sort((x, y) => GetSelledProduct(y.ProductId).CompareTo(GetSelledProduct(x.ProductId)));
        //    return products.GetRange(0, n);
        //}

        public async Task<int> GetSelledProduct(string productId)
        {
            List<OrderDetail> orderDetails = await _orderDetailRepository.GetAll();
            List<ProductVariant> products = await _productVariantRepository.GetAll();
            return orderDetails.Where(od => products.Any(pv => pv.ProductId == productId && pv.ProductVariantId == od.ProductVariantId))
                                        .Sum(od => od.Quantity);
        }

        public async Task<List<Product>> GetTopSeller(int n)
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


        public async Task<int> ProductRemaining(string productId)
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

        public async Task<Product> GetProductByProductVariantId(string productVariantId)
        {
            var productVariant = await _productVariantRepository.GetById(productVariantId);
            return await _productRepository.GetById(productVariant.ProductId);
        }
    }
}
