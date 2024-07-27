using Microsoft.AspNetCore.Mvc;
using Services;
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
            var paginatedCategories = await _supplierService.GetPaginatedSuppliers(searchQuery, sortBy, status, pageIndex, pageSize);
            return Ok(paginatedCategories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierById(string id)
        {
            var supplier = await _supplierService.GetSupplierById(id);
            return Ok(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier([FromBody] SupplierModel supplier)
        {
            var newSupplier = await _supplierService.AddSupplier(supplier);
            return Ok(newSupplier);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSupplier([FromBody] SupplierModel supplier, [FromQuery] string id)
        {
            var updatedSupplier = await _supplierService.UpdateSupplier(supplier, id);
            return Ok(updatedSupplier);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(string id)
        {
            await _supplierService.DeleteSupplier(id);
            return Ok();
        }
    }
}
