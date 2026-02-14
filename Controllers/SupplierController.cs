using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCategorySupplier.Data;
using ProductCategorySupplier.Models.Domains;

namespace ProductCategorySupplier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;

        public SupplierController(ApplicationDBContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAll(CancellationToken cancellationToken = default)
        {
            var suppliers = await _dbContext.Suppliers.AsNoTracking().ToListAsync(cancellationToken);

            return Ok(suppliers);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Supplier>> Get(Guid id, CancellationToken cancellationToken = default)
        {
            var supplier = await _dbContext.Suppliers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SupplierId == id, cancellationToken);

            if (supplier is null)
                return NotFound();

            return Ok(supplier);
        }

        [HttpPost]
        public async Task<ActionResult<Supplier>> Post([FromBody] Supplier supplier, CancellationToken cancellationToken = default)
        {
            if (supplier is null)
                return BadRequest();

            if (supplier.SupplierId == Guid.Empty)
                supplier.SupplierId = Guid.NewGuid();

            await _dbContext.Suppliers.AddAsync(supplier, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = supplier.SupplierId }, supplier);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Supplier supplier, CancellationToken cancellationToken = default)
        {
            if (supplier is null || id != supplier.SupplierId)
                return BadRequest();

            var exists = await _dbContext.Suppliers.AnyAsync(s => s.SupplierId == id, cancellationToken);
            if (!exists)
                return NotFound();

            _dbContext.Entry(supplier).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                var stillExists = await _dbContext.Suppliers.AnyAsync(s => s.SupplierId == id, cancellationToken);
                if (!stillExists)
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var supplier = await _dbContext.Suppliers.FindAsync(new object[] { id }, cancellationToken);
            if (supplier is null)
                return NotFound();

            _dbContext.Suppliers.Remove(supplier);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
