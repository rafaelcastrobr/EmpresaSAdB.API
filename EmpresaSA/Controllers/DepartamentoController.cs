using EmpresaSA.Entities;
using EmpresaSA.Enums;
using EmpresaSA.Models;
using EmpresaSA.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EmpresaSA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartamentoController : ControllerBase
    {

        private readonly EmpresaSADbContext _context;
        public DepartamentoController(EmpresaSADbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Busca departamentos ativos
        /// </summary>
        /// <param name="buscaDepartamento">Campo de busca de nome ou sigla</param>
        /// <returns>Retorna todos departamentos ativos no banco</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpGet("buscar-departamentos-ativos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult BuscarDepartamentosAtivos(string? buscaDepartamento)
        {
            var departamentosAtivos = _context.Departamento.Where(d => d.Status == StatusEnum.Ativo);

            if (buscaDepartamento.IsNullOrEmpty())
            {
                return Ok(departamentosAtivos);

            }
            else
            {
                var depertamentoBuscado = departamentosAtivos.Where(d => d.Nome.ToLower().Contains(buscaDepartamento.ToLower()) || d.Sigla.ToLower().Contains(buscaDepartamento.ToLower())).ToList();

                if (depertamentoBuscado.IsNullOrEmpty())
                {
                    return NotFound();
                }
                else
                {
                    return Ok(depertamentoBuscado);
                }

            }
        }

        /// <summary>
        /// Busca departamentos inativos
        /// </summary>
        /// <param name="buscaDepartamento">Campo de busca de nome ou sigla</param>
        /// <returns>Retorna todos departamentos inativos no banco</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Departamento buscado não encontrado</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("buscar-departamentos-inativos")]
        public IActionResult BuscarDepartamentosInativos(string? buscaDepartamento)
        {
            var departamentosDesativados = _context.Departamento.Where(d => d.Status == StatusEnum.Inativo).ToList();

            if (buscaDepartamento.IsNullOrEmpty())
            {
                return Ok(departamentosDesativados);

            }
            else
            {
                var depertamentoBuscado = departamentosDesativados.Where(d => d.Nome.ToLower().Contains(buscaDepartamento.ToLower()) || d.Sigla.ToLower().Contains(buscaDepartamento.ToLower())).ToList();

                if (depertamentoBuscado.IsNullOrEmpty())
                {
                    return NotFound();
                }
                else
                {
                    return Ok(depertamentoBuscado);
                }

            }
        }


        /// <summary>
        /// Busca departament por id
        /// </summary>
        /// <param name="id">Identificador do departamento</param>
        /// <returns>Retorna o departamento específico</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Id do departamento inativo ou não encontrado</response>
        [HttpGet("buscar-departamento/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult BuscarDepartamentoPorId(Guid id)
        {
            var departamento = _context.Departamento
                .Include(de => de.Colaboradores.Where(c => c.Status == StatusEnum.Ativo))
                .SingleOrDefault(d => d.Id == id);

            if (departamento == null)
            {
                return NotFound();
            }

            return Ok(departamento);

        }

        /// <summary>
        /// Cria um novo departamento
        /// </summary>
        /// <param name="departamento">Objeto de criação do departamento</param>
        /// <returns>Objeto criado</returns>
        /// <response code="201">Sucesso</response>
        /// <response code="400">Item requerido não inserido</response>
        [HttpPost("criar-departamento")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CriarDepartamento(DepartamentoEntitie departamento)
        {

            _context.Departamento.Add(departamento);

            _context.SaveChanges();

            return CreatedAtAction(nameof(BuscarDepartamentoPorId), new { id = departamento.Id }, departamento);
        }


        /// <summary>
        /// Atualiza um departamento ativo
        /// </summary>
        /// <param name="id">Identificador do departamento</param>
        /// <param name="input">Dados do departamento</param>
        /// <response code="204">Sucesso</response>
        /// <response code="400">Item requerido não inserido</response>
        /// <response code="404">Id do departamento inativo ou não encontrado</response>
        [HttpPut("atualizar-departamento/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AtualizarDepartamento(Guid id, DepartamentoEntitie input)
        {
            var departamento = _context.Departamento.SingleOrDefault(d => d.Id == id && d.Status == StatusEnum.Ativo);

            if (departamento == null)
            {
                return NotFound();
            }

            departamento.Atualizar(input.Nome, input.Sigla);

            _context.Departamento.Update(departamento);
            _context.SaveChanges();

            return NoContent();
        }


        /// <summary>
        /// Inativa um departamento ativo
        /// </summary>
        /// <param name="id">Identificador do departamento</param>
        /// <returns>Nada.</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Item não encontrado</response>
        [HttpDelete("inativar-departamento/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult InativarDepartamento(Guid id)
        {
            var departamento = _context.Departamento
                .Include(de => de.Colaboradores.Where(c => c.Status == StatusEnum.Ativo))
                .SingleOrDefault(d => d.Id == id && d.Status == StatusEnum.Ativo);

            if (departamento == null)
            {
                return NotFound("Não encontrado");
            }

            int numeroDeColaboradoresAtivos = departamento.Colaboradores.Count();

            if (numeroDeColaboradoresAtivos > 0)
            {
                return NotFound("Não é possível inativar um departamento com colaboradores ativos");
            }

            departamento.Inativar();
            _context.SaveChanges();

            return NoContent();

        }


    }
}
