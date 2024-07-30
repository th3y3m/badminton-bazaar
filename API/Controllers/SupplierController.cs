using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedSuppliers(
            [FromQuery] string? searchQuery = "",
            [FromQuery] string? sortBy = "companyname_asc",
            [FromQuery] bool? status = true,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedCategories = await _supplierService.GetPaginatedSuppliers(searchQuery, sortBy, status, pageIndex, pageSize);
                return Ok(paginatedCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving paginated suppliers: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierById(string id)
        {
            try
            {
                var supplier = await _supplierService.GetSupplierById(id);
                return Ok(supplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving supplier by ID: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier([FromBody] SupplierModel supplier)
        {
            try
            {
                var newSupplier = await _supplierService.AddSupplier(supplier);
                return Ok(newSupplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding supplier: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSupplier([FromBody] SupplierModel supplier, [FromQuery] string id)
        {
            try
            {
                var updatedSupplier = await _supplierService.UpdateSupplier(supplier, id);
                return Ok(updatedSupplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating supplier: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(string id)
        {
            try
            {
                await _supplierService.DeleteSupplier(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting supplier: {ex.Message}");
            }
        }
    }
}
