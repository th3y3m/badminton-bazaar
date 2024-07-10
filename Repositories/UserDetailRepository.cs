using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserDetailRepository
    {
        private readonly DbContext _dbContext;

        public UserDetailRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(UserDetail userDetail)
        {
            _dbContext.UserDetails.Add(userDetail);
            _dbContext.SaveChanges();
        }

        public void Update(UserDetail userDetail)
        {
            _dbContext.UserDetails.Update(userDetail);
            _dbContext.SaveChanges();
        }

        public UserDetail GetById(string id) => _dbContext.UserDetails.Find(id);

        public List<UserDetail> GetAll()
        {
            return _dbContext.UserDetails.ToList();
        }

    }
}
