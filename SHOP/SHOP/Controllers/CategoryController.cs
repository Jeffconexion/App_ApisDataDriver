using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHOP.Data;
using SHOP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SHOP.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : Controller
    {
        /// <summary>
        /// Action para retorno com uma lista de categorias cadastradas no sistema.
        /// </summary>
        /// <param name="context">Contexto do DbContext</param>
        /// <response code="200">A lista de categoria foi obtida com sucesso.</response>
        /// <response code="500">Ocorreu um erro ao obter a lista de categoria.</response>
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Action para retorno de uma categoria cadastrada no sistema.
        /// </summary>
        /// <param name="id">ID da categoria.</param>
        /// <param name="context">Contexto do DbContext</param>
        /// <response code="200">A lista de categoria foi obtida com sucesso.</response>
        /// <response code="500">Ocorreu um erro ao obter a lista de categoria.</response>
        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Category>> GetById(int id, [FromServices] DataContext context)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return category;
        }

        /// <summary>
        /// Action para cadastro de uma nova categoria.
        /// </summary>
        /// <param name="model">Abstração da minha entidade Categoria.</param>
        /// <param name="context">Contexto do DbContext</param>
        /// <response code="200">A categoria foi cadastrada com sucesso.</response>
        /// <response code="400">O modelo da categoria enviado é inválido.</response>
        /// <response code="500">Ocorreu um erro ao cadastrar a categoria.</response>
        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Category>> Post([FromBody] Category model, [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar categoria." });
            }
        }

        /// <summary>
        /// Action para atualizar uma categoria.        
        /// </summary>
        /// <param name="context">Contexto do DbContext</param>
        /// <param name="id">ID da categoria.</param>
        /// <param name="model">Abstração da minha entidade Categoria.</param>
        /// <response code="200">A categoria foi atualizado com sucesso.</response>
        /// <response code="400">O modelo da categoria enviado é inválido.</response>
        /// <response code="404">Categoria não encontrada.</response>
        /// <response code="500">Ocorreu um erro ao cadastrar a categoria.</response>
        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Category>> Put(int id, [FromBody] Category model, [FromServices] DataContext context)
        {
            if (id != model.Id)
                return NotFound(new { message = "Categoria não encontrada." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível atualizar categoria." });
            }
        }

        /// <summary>
        /// Action para excluir fisicamente uma categoria.
        /// </summary>
        /// <param name="id">ID da categoria.</param>
        /// <param name="context">Contexto do DbContext</param>
        /// <response code="200">A categoria foi deletado com sucesso.</response>
        /// <response code="404">Categoria não encontrada.</response>
        /// <response code="500">Ocorreu um erro ao deletar a categoria.</response>
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Category>> Delete(int id, [FromServices] DataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok("Categoria Deletado com sucesso!");
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível remover!!" });
            }
        }
    }
}
