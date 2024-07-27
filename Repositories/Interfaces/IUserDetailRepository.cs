using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IUserDetailRepository
    {
        Task Add(UserDetail userDetail);
        Task Update(UserDetail userDetail);
        Task<UserDetail> GetById(string id);
        Task<List<UserDetail>> GetAll();
        Task Delete(string id);
        Task<DbSet<UserDetail>> GetDbSet();
    }
}
