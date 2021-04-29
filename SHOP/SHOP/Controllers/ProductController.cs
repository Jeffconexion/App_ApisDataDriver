using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHOP.Data;
using SHOP.Models;

namespace SHOP.Controllers
{
    [Route("v1/products")]
    public class ProductController : Controller
    {
        /// <summary>
        /// Action para retorno com uma lista de produtos cadastrados no sistema.
        /// </summary>
        /// <param name="context">Contexto do DbContext</param>
        /// <response code="200">A lista dos produtos foram obtidas com sucesso.</response>
        /// <response code="500">Ocorreu um erro ao obter a lista dos produtos.</response>
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            var products = await context.Products
                            .Include(p => p.Category)
                            .AsNoTracking()
                            .ToListAsync();

            return products;
        }

        /// <summary>
        /// Action para retorno de um produto específico cadastrado no sistema.
        /// </summary>
        /// <param name="context">Contexto do DbContext</param>
        /// <param name="id">ID do produto.</param>
        /// <response code="200">O produto foi obtido com sucesso.</response>
        /// <response code="500">Ocorreu um erro ao obter o produto.</response>
        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Product>> GetById([FromServices] DataContext context, int id)
        {
            var product = await context
                         .Products
                         .Include(p => p.Category)
                         .AsNoTracking()
                         .FirstOrDefaultAsync(p => p.Id == id);

            return product;
        }

        /// <summary>
        /// Action para retorno de produtos por uma categoria cadastrada no sistema.
        /// </summary>
        /// <param name="context">Contexto do DbContext</param>
        /// <param name="id">ID da Categoria.</param>
        /// <response code="200">A lista dos produtos foram obtidas com sucesso.</response>
        /// <response code="500">Ocorreu um erro ao obter a lista dos produtos por categoria.</response>
        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Product>>> GetByCategory([FromServices] DataContext context, int id)
        {
            var products = await context
                          .Products
                          .Include(p => p.Category)
                          .AsNoTracking()
                          .Where(p => p.CategoryId == id)
                          .ToListAsync();

            return products;
        }

        /// <summary>
        /// Action para cadastro de um novo usuário.
        /// </summary>
        /// <param name="context">Contexto do DbContext</param>
        /// <param name="model">Abstração da minha entidade Product.</param>
        /// <response code="200">O produto foi cadastrado com sucesso.</response>
        /// <response code="400">O modelo do produto enviado é inválido.</response>
        /// <response code="500">Ocorreu um erro ao cadastrar o produto.</response>
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Product>> Post([FromServices] DataContext context, [FromBody] Product model)
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
