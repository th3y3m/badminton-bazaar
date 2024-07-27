using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly DbContext _dbContext;

        public SupplierRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Supplier supplier)
        {
            _dbContext.Suppliers.Add(supplier);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Supplier supplier)
        {
            _dbContext.Suppliers.Update(supplier);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Supplier> GetById(string id) => await _dbContext.Suppliers.FindAsync(id);

        public async Task<List<Supplier>> GetAll()
        {
            return await _dbContext.Suppliers.ToListAsync();
        }
        
        public async Task<DbSet<Supplier>> GetDbSet()
        {
            return await Task.FromResult(_dbContext.Suppliers);
        }

        public async Task Delete(string id) {
            var supplier = await GetById(id);
            supplier.Status = false;
            _dbContext.Suppliers.Update(supplier);
            await _dbContext.SaveChangesAsync();
        }
    }
}
