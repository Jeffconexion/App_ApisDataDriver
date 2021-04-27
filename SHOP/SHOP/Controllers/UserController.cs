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
        /// Action para retorno com uma lista de usuários cadastrados no sistema
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        //O atributo ProducesResponseType informa ao swagger quais as possíveis respostas que o controller vai enviar
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            var users = await context
                .Users
                .AsNoTracking()
                .ToListAsync();

            return Ok(users);
        }

        /// <summary>
        /// Action para cadastro de um novo usuário
        /// </summary>
        /// <remarks>
        /// Caso queira deixar algo visível ao clicar na action, pode utilizar esta propriedade remarks e adicionar o conteúdo abaixo. Ex:
        ///     
        ///     POST /v1/users
        ///     {
        ///         "username": "string", -- Aqui você vai colocar o nome de usuário
        ///         "password": "string", -- Aqui você vai colocar a senha do usuário
        ///         "role": "string" -- Aqui você vai colocar o cargo do usuário
        ///     }
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        // [Authorize(Roles = "manager")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                return Created($"v1/users/{model.Id}", model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });

            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
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

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
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

        //[HttpGet]
        //[Route("anonimo")]
        //[AllowAnonymous]
        //public string Anonimo() => "Anonimo";

        //[HttpGet]
        //[Route("autenticado")]
        //[Authorize]
        //public string Autenticado() => "autenticado";
    }
}
