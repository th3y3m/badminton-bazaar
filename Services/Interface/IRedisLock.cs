using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IRedisLock
    {
        void ReleaseLock(string lockKey, string lockValue);
        bool AcquireLock(string lockKey, string lockValue);
    }
}
