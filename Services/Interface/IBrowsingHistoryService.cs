using BusinessObjects;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IBrowsingHistoryService
    {
        Task LogBrowsingEvent(string userId, string productId, string sessionId);
        Task<List<BrowsingHistory>> Get();
        Task<List<BrowsingHistory>> GetUserHistories(string userId);
    }
}