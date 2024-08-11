using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class FreightPriceRepository : IFreightPriceRepository
    {
        private readonly DbContext _dbContext;

        public FreightPriceRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(FreightPrice freightPrice)
        {
            try
            {
                _dbContext.FreightPrices.Add(freightPrice);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while adding the freight price.", ex);
            }
        }

        public async Task Update(FreightPrice freightPrice)
        {
            try
            {
                _dbContext.FreightPrices.Update(freightPrice);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while updating the freight price.", ex);
            }
        }

        public async Task<FreightPrice> GetById(string id)
        {
            try
            {
                return await _dbContext.FreightPrices.FindAsync(id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving the freight price by ID.", ex);
            }
        }

        public async Task<List<FreightPrice>> GetAll()
        {
            try
            {
                return await _dbContext.FreightPrices.ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving all freight prices.", ex);
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var freightPrice = await GetById(id);
                _dbContext.FreightPrices.Remove(freightPrice);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while deleting the freight price.", ex);
            }
        }
    }
}
