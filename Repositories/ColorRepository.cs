using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class ColorRepository : IColorRepository
    {
        private readonly DbContext _dbContext;

        public ColorRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Color color)
        {
            try
            {
                _dbContext.Colors.Add(color);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while adding the color.", ex);
            }
        }

        public async Task Update(Color color)
        {
            try
            {
                _dbContext.Colors.Update(color);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while updating the color.", ex);
            }
        }

        public async Task<Color> GetById(string id)
        {
            try
            {
                return await _dbContext.Colors.FindAsync(id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving the color by ID.", ex);
            }
        }

        public async Task<List<Color>> GetAll()
        {
            try
            {
                return await _dbContext.Colors.ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving all colors.", ex);
            }
        }

        public async Task<DbSet<Color>> GetDbSet()
        {
            try
            {
                return await Task.FromResult(_dbContext.Colors);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving the DbSet of colors.", ex);
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var color = await GetById(id);
                if (color != null)
                {
                    _dbContext.Colors.Remove(color);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Color not found.");
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while deleting the color.", ex);
            }
        }
    }
}
