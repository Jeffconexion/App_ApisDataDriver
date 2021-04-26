using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHOP.Data;
using SHOP.Models;

namespace SHOP.Controllers
{
    [Route("v1/products")]
    public class ProductController : Controller
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices]DataContext context)
        {
            var products = await context.Products
                            .Include(p => p.Category)
                            .AsNoTracking()
                            .ToListAsync();

            return products;
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById([FromServices]DataContext context, int id)
        {
            var product = await context
                         .Products
                         .Include(p => p.Category)
                         .AsNoTracking()
                         .FirstOrDefaultAsync(p => p.Id == id);

            return product;
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory([FromServices]DataContext context, int id)
        {
            var products = await context
                          .Products
                          .Include(p => p.Category)
                          .AsNoTracking()
                          .Where(p => p.CategoryId == id)
                          .ToListAsync();

            return products;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post([FromServices]DataContext context, [FromBody] Product model)
        {
            if (ModelState.IsValid)
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
