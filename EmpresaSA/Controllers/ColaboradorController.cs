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
        /// <response code="200">Retorna os colaboradores</response>
        /// <response code="404">Não encontrado</response>
        [HttpGet("buscar-colaborador-ativo-departamento/{idDepartamento}")]
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
        /// Busca colaborador por id
        /// </summary>
        /// <response code="200">Retorna o colaborador</response>
        /// <response code="404">Não encontrado</response>
        [HttpGet("buscar-colaborador/{id}")]
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
        /// <response code="201">Cadastrado com sucesso</response>
        /// <response code="400">Item requerido não inserido</response>
        [HttpPost("cadastrar-colaborador")]
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
        /// Inativa um colaborador ativo
        /// </summary>
        /// <response code="204">Inativado com sucesso</response>
        [HttpDelete("inativar-colaborador/{id}")]
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
