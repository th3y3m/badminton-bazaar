using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IColorRepository
    {
        Task Add(Color color);
        Task Update(Color color);
        Task<Color> GetById(string id);
        Task<List<Color>> GetAll();
        Task<DbSet<Color>> GetDbSet();
        Task Delete(string id);
    }
}
