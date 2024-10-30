using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Nest;
using Newtonsoft.Json;
using Repositories.Interfaces;
using Services.Interface;
using StackExchange.Redis;

namespace Services
{
    public class FreightPriceService : IFreightPriceService
    {
        private readonly IFreightPriceRepository _freightPriceRepository;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;

        public FreightPriceService(IFreightPriceRepository freightPriceRepository, IConnectionMultiplexer redisConnection)
        {
            _freightPriceRepository = freightPriceRepository;
            _redisConnection = redisConnection;
            _redisDb = _redisConnection.GetDatabase();
        }

        public async Task Add(FreightPrice freightPrice)
        {
            try
            {
                await _freightPriceRepository.Add(freightPrice);
                await _redisDb.StringSetAsync($"freightPrice:{freightPrice.PriceId}", JsonConvert.SerializeObject(freightPrice), TimeSpan.FromHours(1));
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while adding the freight price.", ex);
            }
        }

        public async Task Update(FreightPrice freightPrice)
        {
            try
            {
                await _freightPriceRepository.Update(freightPrice);
                await _redisDb.StringSetAsync($"freightPrice:{freightPrice.PriceId}", JsonConvert.SerializeObject(freightPrice), TimeSpan.FromHours(1));
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while updating the freight price.", ex);
            }
        }

        public async Task<FreightPrice> GetById(string id)
        {
            try
            {
                var cachedFreightPrice = await _redisDb.StringGetAsync($"freightPrice:{id}");

                if (!cachedFreightPrice.IsNullOrEmpty)
                    return JsonConvert.DeserializeObject<FreightPrice>(cachedFreightPrice);
                var freightPrice = await _freightPriceRepository.GetById(id);
                await _redisDb.StringSetAsync($"freightPrice:{id}", JsonConvert.SerializeObject(freightPrice), TimeSpan.FromHours(1));
                return freightPrice; 
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving the freight price by ID.", ex);
            }
        }

        public async Task<List<FreightPrice>> GetAll()
        {
            try
            {
                return await _freightPriceRepository.GetAll();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving all freight prices.", ex);
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                await _freightPriceRepository.Delete(id);
                await _redisDb.KeyDeleteAsync($"freightPrice:{id}");
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while deleting the freight price.", ex);
            }
        }

        public async Task<decimal> GetPriceByDistance(decimal km)
        {

            try
            {
                var freightPrices = await GetAll();
                var freightPrice = freightPrices.FirstOrDefault(x => x.MinDistance <= km && x.MaxDistance >= km);
                if (freightPrice == null)
                {
                    throw new Exception("Freight price not found for the specified distance.");
                }
                return freightPrice.PricePerKm;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving the freight price by distance.", ex);
            }
        }
    }
}
