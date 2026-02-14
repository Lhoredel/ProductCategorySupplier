using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCategorySupplier.Data;
using ProductCategorySupplier.Models.Domains;

namespace ProductCategorySupplier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;

        public ProductsController(ApplicationDBContext dbContext)
        {
           this._dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll(CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Product>> Get(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == id, cancellationToken);

            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromBody] Product product, CancellationToken cancellationToken = default)
        {
            if (product is null)
                return BadRequest();

            if (product.ProductId == Guid.Empty)
                product.ProductId = Guid.NewGuid();

            await _dbContext.Products.AddAsync(product, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = product.ProductId }, product);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Product product, CancellationToken cancellationToken = default)
        {
            if (product is null || id != product.ProductId)
                return BadRequest();

            var exists = await _dbContext.Products.AnyAsync(p => p.ProductId == id, cancellationToken);
            if (!exists)
                return NotFound();

            _dbContext.Entry(product).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Re-check existence in case of concurrency race
                var stillExists = await _dbContext.Products.AnyAsync(p => p.ProductId == id, cancellationToken);
                if (!stillExists)
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _dbContext.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product is null)
                return NotFound();

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
