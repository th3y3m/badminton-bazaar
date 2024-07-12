using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly SupplierService _supplierService;

        public SupplierController(SupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public ActionResult GetPaginatedSuppliers(
            [FromQuery] string? searchQuery = "",
            [FromQuery] string? sortBy = "companyname_asc",
            [FromQuery] bool? status = true,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedCategories = _supplierService.GetPaginatedSuppliers(searchQuery, sortBy, status, pageIndex, pageSize);
            return Ok(paginatedCategories);
        }

        [HttpGet("{id}")]
        public ActionResult GetSupplierById(string id)
        {
            var supplier = _supplierService.GetSupplierById(id);
            return Ok(supplier);
        }

        [HttpPost]
        public ActionResult AddSupplier([FromBody] SupplierModel supplier)
        {
            var newSupplier = _supplierService.AddSupplier(supplier);
            return Ok(newSupplier);
        }

        [HttpPut]
        public ActionResult UpdateSupplier([FromBody] SupplierModel supplier, [FromQuery] string id)
        {
            var updatedSupplier = _supplierService.UpdateSupplier(supplier, id);
            return Ok(updatedSupplier);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteSupplier(string id)
        {
            _supplierService.DeleteSupplier(id);
            return Ok();
        }
    }
}
