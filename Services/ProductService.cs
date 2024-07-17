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
    public class ProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly ProductVariantRepository _productVariantRepository;
        private readonly OrderDetailRepository _orderDetailRepository;

        public ProductService(ProductRepository productRepository, ProductVariantRepository productVariantRepository, OrderDetailRepository orderDetailRepository)
        {
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public PaginatedList<Product> GetPaginatedProducts(
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
            var source = _productRepository.GetDbSet().AsNoTracking();

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

        public Product AddProduct(ProductModel productModel)
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
            _productRepository.Add(product);
            return product;
        }

        public Product UpdateProduct(ProductModel productModel, string productId)
        {
            var product = GetProductById(productId);

            product.ProductName = productModel.ProductName;
            product.CategoryId = productModel.CategoryId;
            product.SupplierId = productModel.SupplierId;
            product.ProductDescription = productModel.ProductDescription;
            product.BasePrice = productModel.BasePrice;
            product.ImageUrl = productModel.ImageUrl;
            product.Status = productModel.Status;

            _productRepository.Update(product);
            return product;
        }

        public void DeleteProduct(string productId) => _productRepository.Delete(productId);

        public Product GetProductById(string productId) => _productRepository.GetById(productId);
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

        public int GetSelledProduct(string productId)
        {
            return _orderDetailRepository.GetAll()
                                         .Where(od => _productVariantRepository.GetAll()
                                                                               .Any(pv => pv.ProductId == productId && pv.ProductVariantId == od.ProductVariantId))
                                         .Sum(od => od.Quantity);
        }

        public List<Product> GetTopSeller(int n)
        {
            var productSales = _productRepository.GetAll()
                                                 .Select(p => new
                                                 {
                                                     Product = p,
                                                     TotalSales = GetSelledProduct(p.ProductId)
                                                 })
                                                 .OrderByDescending(ps => ps.TotalSales)
                                                 .Take(n)
                                                 .Select(ps => ps.Product)
                                                 .ToList();
            return productSales;
        }

        public int ProductRemaining(string productId)
        {
            var productVariants = _productVariantRepository.GetAll().Where(p => p.ProductId == productId).ToList();
            int total = 0;
            foreach (var productVariant in productVariants)
            {
                total += productVariant.StockQuantity;
            }
            return total;
        }
    }
}
