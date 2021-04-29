using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;
using SHOP.Data;
using SHOP.Models;

namespace SHOP.Controllers
{
    [Route("v1/users")]
    public class UserController : Controller
    {
        /// <summary>
        /// Action para retorno com uma lista de usuários cadastrados no sistema.
        /// </summary>
        /// <param name="context">Contexto do DbContext</param>
        /// <response code="200">A lista de usuários foi obtida com sucesso.</response>
        /// <response code="500">Ocorreu um erro ao obter a lista de usuários.</response>
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            var users = await context
                .Users
                .AsNoTracking()
                .ToListAsync();
            return users;
        }

        /// <summary>
        /// Action para cadastro de um novo usuário.
        /// </summary>
        /// <param name="context">Contexto do DbContext</param>
        /// <param name="model">Abstração da minha entidade Users.</param>
        /// <response code="200">O usuário foi cadastrado com sucesso.</response>
        /// <response code="400">O modelo do usuário enviado é inválido.</response>
        /// <response code="500">Ocorreu um erro ao cadastrar o usuário.</response>
        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> Post(
           [FromServices] DataContext context,
           [FromBody] User model)
        {
            // Verifica se os dados são válidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Força o usuário a ser sempre "funcionário"
                model.Role = "employee";

                context.Users.Add(model);
                await context.SaveChangesAsync();

                // Esconde a senha
                model.Password = "";
                //return model;
                return Created($"v1/users/{model.Id}", model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });

            }
        }

        /// <summary>
        /// Action para atualizar um usuário.        
        /// </summary>
        /// <param name="context">Contexto do DbContext</param>
        /// <param name="id">ID do usuário.</param>
        /// <param name="model">Abstração da minha entidade Users.</param>
        /// <response code="200">O usuário foi atualizado com sucesso.</response>
        /// <response code="400">O modelo do usuário enviado é inválido.</response>
        /// <response code="404">Usuário não encontrado.</response>
        /// <response code="500">Ocorreu um erro ao atualizar o usuário.</response>
        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> Put(
            [FromServices] DataContext context,
            int id,
            [FromBody] User model)
        {
            // Verifica se os dados são válidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifica se o ID informado é o mesmo do modelo
            if (id != model.Id)
                return NotFound(new { message = "Usuário não encontrada" });

            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });

            }
        }

        /// <summary>
        /// Action para autenticação do sistema.
        /// </summary>
        /// <param name="context">Contexto do DbContext</param>
        /// <param name="model">Abstração da minha entidade Users.</param>
        /// <response code="400">O modelo do usuário enviado é inválido.</response>
        /// <response code="404">Usuário não encontrado.</response>
        /// <response code="500">Ocorreu um erro ao cadastrar o usuário.</response>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<dynamic>> Authenticate(
                    [FromServices] DataContext context,
                    [FromBody] User model)
        {
            var user = await context.Users
                .AsNoTracking()
                .Where(x => x.Username == model.Username && x.Password == model.Password)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(user);
            // Esconde a senha
            user.Password = "";
            return new
            {
                user = user,
                token = token
            };
        }
    }
}
