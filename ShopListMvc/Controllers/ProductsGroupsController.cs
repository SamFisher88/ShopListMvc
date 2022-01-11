using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopListMvc.Data;

namespace ShopListMvc.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsGroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductsGroupsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/ProductsGroups
        // Получаем список списков продуктов для текущего пользователя
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductsGroup>>> GetProductsGroups()
        {
            return await _context.ProductsGroups.Where(x => x.UserId == _userManager.GetUserId(User)).ToListAsync();
        }

        // GET: api/ProductsGroups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductsGroup>> GetProductsGroup(int id)
        {
            var productsGroup = await _context.ProductsGroups.Include(x => x.Products).SingleAsync(x => x.Id == id);

            if (productsGroup == null)
            {
                return NotFound();
            }

            if (productsGroup.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return productsGroup;
        }

        // PUT: api/ProductsGroups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductsGroup(int id, ProductsGroup productsGroup)
        {
            if (id != productsGroup.Id)
            {
                return BadRequest();
            }

            var productsGroupOld = await _context.ProductsGroups.SingleAsync(x => x.Id == id);
            if (productsGroupOld == null)
            {
                return NotFound();
            }

            if (productsGroupOld.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            _context.Entry(productsGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductsGroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProductsGroups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductsGroup>> PostProductsGroup(ProductsGroup productsGroup)
        {
            productsGroup.UserId = _userManager.GetUserId(User);
            productsGroup.CreatedAt = DateTime.Now;

            _context.ProductsGroups.Add(productsGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductsGroup", new { id = productsGroup.Id }, productsGroup);
        }

        // DELETE: api/ProductsGroups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductsGroup(int id)
        {
            var productsGroup = await _context.ProductsGroups.FindAsync(id);
            if (productsGroup == null)
            {
                return NotFound();
            }

            if (productsGroup.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            _context.ProductsGroups.Remove(productsGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductsGroupExists(int id)
        {
            return _context.ProductsGroups.Any(e => e.Id == id);
        }
    }
}
