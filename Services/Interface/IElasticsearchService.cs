using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IElasticsearchService
    {
        Task CreateIndexAsync(string indexName);
        Task IndexProductsAsync(List<Product> products);
        Task<List<Product>> SearchProductsByNameAsync(string name);
    }
}
