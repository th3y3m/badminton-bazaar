using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class SupplierRepository
    {
        private readonly DbContext _dbContext;

        public SupplierRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Supplier supplier)
        {
            _dbContext.Suppliers.Add(supplier);
            _dbContext.SaveChanges();
        }

        public void Update(Supplier supplier)
        {
            _dbContext.Suppliers.Update(supplier);
            _dbContext.SaveChanges();
        }

        public Supplier GetById(string id) => _dbContext.Suppliers.Find(id);

        public List<Supplier> GetAll()
        {
            return _dbContext.Suppliers.ToList();
        }

        public void Delete(string id) {
            var supplier = GetById(id);
            supplier.Status = "Inactive";
            _dbContext.Suppliers.Update(supplier);
            _dbContext.SaveChanges();
        }
    }
}
