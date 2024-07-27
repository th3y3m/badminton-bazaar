using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task Add(IdentityUser user);
        Task Update(IdentityUser user);
        Task<IdentityUser> GetById(string id);
        Task<List<IdentityUser>> GetAll();
        Task Delete(string id);
        Task Ban(string id);
        Task Unban(string id);
        Task<DbSet<IdentityUser>> GetDbSet();
    }
}
