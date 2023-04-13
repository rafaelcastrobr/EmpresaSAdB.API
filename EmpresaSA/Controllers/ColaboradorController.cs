using EmpresaSA.Entities;
using EmpresaSA.Enums;
using EmpresaSA.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        /// Busca colaboradores ativos por departamento ativo
        /// </summary>
        /// <param name="idDepartamento">Identificador do departamento</param>
        /// <returns>Retorna os colaboradores</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpGet("buscar-colaborador-ativo/{idDepartamento}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult BuscarColaboradorAtivoPorDepartamento(Guid idDepartamento)
        {
            var departamento = _context.Departamento
                .Include(d => d.Colaboradores.Where(c => c.Status == StatusEnum.Ativo))
                .SingleOrDefault(d => d.Id == idDepartamento && d.Status == StatusEnum.Ativo);

            if (departamento == null)
            {
                return NotFound("Não encontrado");
            }

            var colaboradoresAtivos = departamento.Colaboradores.Where(c => c.Status == StatusEnum.Ativo).ToList();

            return Ok(colaboradoresAtivos);

        }


        /// <summary>
        /// Busca colaboradores inativos
        /// </summary>
        /// <returns>Retorna os colaboradores inativos</returns>
        /// <response code="204">Sucesso</response>
        [HttpGet("buscar-colaboradores-inativos")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult BuscarColaboradorInativo()
        {

            var colaboradoresAtivos = _context.Colaborador.Where(c => c.Status == StatusEnum.Inativo).ToList();

            return Ok(colaboradoresAtivos);

        }

        /// <summary>
        /// Busca colaborador por id
        /// </summary>
        /// <param name="id">Identificador do colaborador</param>
        /// <returns>Retorna o colaborador</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpGet("buscar-colaborador/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult BuscarColaboradorPorId(Guid id)
        {
            var colaborador = _context.Colaborador.SingleOrDefault(d => d.Id == id);

            if (colaborador == null)
            {
                return NotFound("Não encontrado");
            }

            return Ok(colaborador);

        }

        /// <summary>
        /// Cadastrar um colaborador e relacionar ao departamento
        /// </summary>
        /// <param name="id">Identificador do colaborador</param>
        /// <param name="colaborador">Objeto de criação do departamento</param>
        /// <returns>Objeto criado</returns>
        /// <response code="201">Sucesso</response>
        /// <response code="400">Item requerido não inserido</response>
        [HttpPost("cadastrar-colaborador")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CadastrarColaborador(Guid id, ColaboradorEntitie colaborador)
        {
            var departamento = _context.Departamento.SingleOrDefault(c => c.Id == id && c.Status == StatusEnum.Ativo);

            if (departamento == null)
            {
                return NotFound("Não encontrado");
            }

            colaborador.Id_Departamento = id;  

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
        /// <response code="404">Item não encontrado</response>
        [HttpPut("atualizar-colaborador/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AtualizarColaborador(Guid id, ColaboradorEntitie input)
        {
            var colaborador = _context.Colaborador.SingleOrDefault(d => d.Id == id && d.Status == StatusEnum.Ativo);

            if (colaborador == null)
            {
                return NotFound("Não encontrado");
            }

            colaborador.Atualizar(input.Nome, input.Documento, input.Id_Departamento);

            _context.Colaborador.Update(colaborador);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Ativa um colaborador inativo
        /// </summary>
        /// <param name="idColaborador">Identificador do colaborador</param>
        /// <param name="idDepartamento">Identificador do departamento</param>
        /// <returns>Nada.</returns>
        /// <response code="204">Sucesso</response>
        [HttpPatch("ativar-colaborador/{idColaborador}/{idDepartamento}")]
        public IActionResult AtivarColaborador(Guid idColaborador, Guid idDepartamento)
        {
            var colaborador = _context.Colaborador.SingleOrDefault(c => c.Id == idColaborador && c.Status == StatusEnum.Inativo);

            if(colaborador == null)
            {
                return NotFound("Não encontrado");
            }

            colaborador.Id_Departamento = idDepartamento;

            colaborador.Ativar();
            _context.SaveChanges();

            return NoContent();


        }

        /// <summary>
        /// Inativa um colaborador ativo
        /// </summary>
        /// <param name="id">Identificador do colaborador</param>
        /// <returns>Nada.</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Item não encontrado</response>
        [HttpDelete("inativar-colaborador/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult InativarColaborador(Guid id)
        {
            var colaborador = _context.Colaborador.SingleOrDefault(d => d.Id == id && d.Status == StatusEnum.Ativo);

            if (colaborador == null)
            {
                return NotFound("Não encontrado");
            }


            colaborador.Inativar();
            _context.SaveChanges();

            return NoContent();

        }
    }
}
