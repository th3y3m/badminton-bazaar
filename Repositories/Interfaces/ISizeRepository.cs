using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface ISizeRepository
    {
        Task Add(Size size);
        Task Update(Size size);
        Task<Size> GetById(string id);
        Task<List<Size>> GetAll();
        Task Delete(string id);
        Task<DbSet<Size>> GetDbSet();
    }
}
