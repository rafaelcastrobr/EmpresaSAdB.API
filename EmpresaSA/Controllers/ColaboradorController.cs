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
    public class ColaboradorController : ControllerBase
    {

        private readonly EmpresaSADbContext _context;
        public ColaboradorController(EmpresaSADbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Busca colaboradores ativos
        /// </summary>
        /// <param name="buscaColaborador">Campo de busca de nome ou documento</param>
        /// <returns>Retorna os colaboradores</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Colaborador buscado não encontrado</response>
        [HttpGet("buscar-colaboradores-ativos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult BuscarColaboradoresAtivos(string? buscaColaborador)
        {

            var todosColaboradoresAtivos = _context.Colaborador.Where(c => c.Status == StatusEnum.Ativo).ToList();

            if (buscaColaborador.IsNullOrEmpty())
            {
                return Ok(todosColaboradoresAtivos);

            }
            else
            {
                var colaboradorBuscado = todosColaboradoresAtivos.Where(d => d.Nome.ToLower().Contains(buscaColaborador.ToLower()) || d.Documento.Contains(buscaColaborador)).ToList();

                if (colaboradorBuscado.IsNullOrEmpty())
                {
                    return NotFound();
                }
                else
                {
                    return Ok(colaboradorBuscado);

                }
            }


        }

        /// <summary>
        /// Busca colaboradores ativos por departamento
        /// </summary>
        /// <param name="idDepartamento">Id do departamento</param>
        /// <param name="buscaColaborador">Campo de busca de nome ou documento</param>
        /// <returns>Retorna os colaboradores ativos pelo departamento</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Item não encontrado</response>
        [HttpGet("buscar-colaboradores-ativos-departamento/{idDepartamento}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult BuscarColaboradoresAtivosPorDepartamento(Guid idDepartamento, string? buscaColaborador)
        {
            var departamento = _context.Departamento
                .Include(d => d.Colaboradores.Where(c => c.Status == StatusEnum.Ativo))
                .SingleOrDefault(d => d.Id == idDepartamento && d.Status == StatusEnum.Ativo);

            if (departamento == null)
            {
                return NotFound("Departamento inativo ou não encontrado");
            }

            var colaboradoresAtivosDoDepartamento = departamento.Colaboradores.Where(c => c.Status == StatusEnum.Ativo).ToList();

            if (buscaColaborador.IsNullOrEmpty())
            {
                return Ok(colaboradoresAtivosDoDepartamento);

            }
            else
            {
                var colaboradorBuscadoComPesquisa = colaboradoresAtivosDoDepartamento.Where(d => d.Nome.ToLower().Contains(buscaColaborador.ToLower()) || d.Documento.Contains(buscaColaborador)).ToList();

                if (colaboradorBuscadoComPesquisa.IsNullOrEmpty())
                {
                    return NotFound("Colaborador inativo ou não encontrado");
                }
                else
                {
                    return Ok(colaboradorBuscadoComPesquisa);

                }
            }
        }


        /// <summary>
        /// Busca colaboradores inativos
        /// </summary>
        /// <param name="buscaColaborador">Campo de busca de nome ou documento</param>
        /// <returns>Retorna todos colaboradores inativos e também por busca</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Colaborador não encontrado</response>
        [HttpGet("buscar-colaboradores-inativos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult BuscarColaboradorInativo(string? buscaColaborador)
        {

            var colaboradoresInativos = _context.Colaborador.Where(c => c.Status == StatusEnum.Inativo).ToList();

            if (buscaColaborador.IsNullOrEmpty())
            {
                return Ok(colaboradoresInativos);
            }
            else
            {
                var colaboradorBuscado = colaboradoresInativos.Where(ci => ci.Nome == buscaColaborador || ci.Documento == buscaColaborador);

                if (colaboradorBuscado.IsNullOrEmpty())
                {
                    return NotFound();
                }
                else
                {
                    return Ok(colaboradorBuscado);

                }
            }
        }

        /// <summary>
        /// Busca colaborador por id
        /// </summary>
        /// <param name="id">Id do colaborador</param>
        /// <returns>Retorna o colaborador</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Colaborador não encontrado</response>
        [HttpGet("buscar-colaborador/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult BuscarColaboradorPorId(Guid id)
        {
            var colaborador = _context.Colaborador.SingleOrDefault(d => d.Id == id);

            if (colaborador == null)
            {
                return NotFound();
            }

            return Ok(colaborador);

        }

        /// <summary>
        /// Cadastra um colaborador e relaciona ao departamento
        /// </summary>
        /// <param name="idDepartamento">Id do departamento</param>
        /// <param name="colaborador">Objeto de criação do departamento</param>
        /// <returns>Objeto criado</returns>
        /// <response code="201">Sucesso</response>
        /// <response code="400">Item requerido não inserido</response>
        /// <response code="404">Id do departamento inativo ou não encontrado</response>
        [HttpPost("cadastrar-colaborador/{idDepartamento}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CadastrarColaborador(Guid idDepartamento, ColaboradorEntitie colaborador)
        {
            var departamento = _context.Departamento.SingleOrDefault(c => c.Id == idDepartamento && c.Status == StatusEnum.Ativo);

            if (departamento == null)
            {
                return NotFound();
            }

            colaborador.Id_Departamento = idDepartamento;

            _context.Colaborador.Add(colaborador);

            _context.SaveChanges();

            return CreatedAtAction(nameof(BuscarColaboradorPorId), new { id = colaborador.Id }, colaborador);
        }

        /// <summary>
        /// Atualiza um colaborador ativo
        /// </summary>
        /// <param name="id">Identificador do colaborador</param>
        /// <param name="input">Dados do colaborador</param>
        /// <returns>Nada.</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Id do colaborador inativo ou não encontrado</response>
        [HttpPut("atualizar-colaborador/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AtualizarColaborador(Guid id, ColaboradorEntitie input)
        {
            var colaborador = _context.Colaborador.SingleOrDefault(d => d.Id == id && d.Status == StatusEnum.Ativo);

            if (colaborador == null)
            {
                return NotFound();
            }

            colaborador.Atualizar(input.Nome, input.Documento, input.Id_Departamento);

            _context.Colaborador.Update(colaborador);
            _context.SaveChanges();

            return NoContent();
        }



        /// <summary>
        /// Inativa um colaborador ativo
        /// </summary>
        /// <param name="id">Identificador do colaborador</param>
        /// <returns>Nada.</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Id do colaborador inativo ou não encontrado</response>
        [HttpDelete("inativar-colaborador/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult InativarColaborador(Guid id)
        {
            var colaborador = _context.Colaborador.SingleOrDefault(d => d.Id == id && d.Status == StatusEnum.Ativo);

            if (colaborador == null)
            {
                return NotFound();
            }

            colaborador.Inativar();
            _context.SaveChanges();

            return NoContent();

        }
    }
}
