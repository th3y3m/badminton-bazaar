using Microsoft.AspNetCore.Identity;

namespace Repositories
{
    public class UserRepository
    {
        private readonly DbContext _dbContext;

        public UserRepository(DbContext context)
        {
            _dbContext = context;
        }

        public List<IdentityUser> GetAll() {
            return _dbContext.Users.ToList();
        }

        public IdentityUser GetById(string id) {
            return _dbContext.Users.Find(id);
        }

        public void Add(IdentityUser user) {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        public void Update(IdentityUser user) {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
        }

        public void Delete(string id) {
            var user = GetById(id);
            user.LockoutEnabled = false;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
        }
    }
}
