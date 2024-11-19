using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IBrowsingHistoryRepository
    {
        Task Create(BrowsingHistory browsingHistory);
        Task<List<BrowsingHistory>> Get();
        Task<List<BrowsingHistory>> GetUserHistories(string userId);
    }
}
