using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task Add(Review review);
        Task Update(Review review);
        Task<Review> GetById(string id);
        Task<List<Review>> GetAll();
        Task Delete(string id);
        Task<DbSet<Review>> GetDbSet();
    }
}
