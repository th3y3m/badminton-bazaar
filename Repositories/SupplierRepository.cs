using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

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
            try
            {
                _dbContext.Suppliers.Add(supplier);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding supplier: {ex.Message}");
            }
        }

        public async Task Update(Supplier supplier)
        {
            try
            {
                _dbContext.Suppliers.Update(supplier);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating supplier: {ex.Message}");
            }
        }

        public async Task<Supplier> GetById(string id)
        {
            try
            {
                return await _dbContext.Suppliers.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving supplier by ID: {ex.Message}");
            }
        }

        public async Task<List<Supplier>> GetAll()
        {
            try
            {
                return await _dbContext.Suppliers.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all suppliers: {ex.Message}");
            }
        }

        public async Task<DbSet<Supplier>> GetDbSet()
        {
            try
            {
                return await Task.FromResult(_dbContext.Suppliers);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving DbSet of suppliers: {ex.Message}");
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var supplier = await GetById(id);
                supplier.Status = false;
                _dbContext.Suppliers.Update(supplier);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting supplier: {ex.Message}");
            }
        }
    }
}
