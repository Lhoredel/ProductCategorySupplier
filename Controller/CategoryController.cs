using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCategorySupplier.Data;
using ProductCategorySupplier.Models.Domains;

namespace ProductCategorySupplier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;

        public CategoriesController(ApplicationDBContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll(CancellationToken cancellationToken = default)
        {
            var categories = await _dbContext.Categories.AsNoTracking().ToListAsync(cancellationToken);

            return Ok(categories);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Category>> Get(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id, cancellationToken);

            if (category is null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Post([FromBody] Category category, CancellationToken cancellationToken = default)
        {
            if (category is null)
                return BadRequest();

            if (category.CategoryId == Guid.Empty)
                category.CategoryId = Guid.NewGuid();

            await _dbContext.Categories.AddAsync(category, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = category.CategoryId }, category);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Category category, CancellationToken cancellationToken = default)
        {
            if (category is null || id != category.CategoryId)
                return BadRequest();

            var exists = await _dbContext.Categories.AnyAsync(c => c.CategoryId == id, cancellationToken);
            if (!exists)
                return NotFound();

            _dbContext.Entry(category).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                var stillExists = await _dbContext.Categories.AnyAsync(c => c.CategoryId == id, cancellationToken);
                if (!stillExists)
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _dbContext.Categories.FindAsync(new object[] { id }, cancellationToken);
            if (category is null)
                return NotFound();

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
