﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Repositories.Interfaces;
using Services.Interface;

namespace Services
{
    public class FreightPriceService : IFreightPriceService
    {
        private readonly IFreightPriceRepository _freightPriceRepository;

        public FreightPriceService(IFreightPriceRepository freightPriceRepository)
        {
            _freightPriceRepository = freightPriceRepository;
        }

        public async Task Add(FreightPrice freightPrice)
        {
            try
            {
                await _freightPriceRepository.Add(freightPrice);
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
                return await _freightPriceRepository.GetById(id);
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
