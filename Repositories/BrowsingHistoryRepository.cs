using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class BrowsingHistoryRepository : IBrowsingHistoryRepository
    {
        private readonly DbContext _context;

        public BrowsingHistoryRepository(DbContext context)
        {
            _context = context;
        }

        public async Task Create(BrowsingHistory browsingHistory)
        {
            try
            {
                await _context.BrowsingHistories.AddAsync(browsingHistory);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the browsing history.", ex);
            }
        }

        public async Task<List<BrowsingHistory>> Get()
        {
            try
            {
                return await _context.BrowsingHistories.ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving browsing histories.", ex);
            }
        }

        public async Task<List<BrowsingHistory>> GetUserHistories(string userId)
        {
            try
            {
                return await _context.BrowsingHistories.Where(b => b.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving browsing histories for the user.", ex);
            }
        }
    }
}
