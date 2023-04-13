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
    [Route("api/empresa")]
    [ApiController]
    public class DepartamentoController : ControllerBase
    {

        private readonly EmpresaSADbContext _context;
        public DepartamentoController(EmpresaSADbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Busca todos departamentos ativos
        /// </summary>
        /// <response code="200">Retorna todos departamentos ativos no banco</response>
        /// <response code="404">Não encontrado</response>
        [HttpGet("buscar-departamentos-ativos")]
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
        /// Busca todos departamentos inativos
        /// </summary>
        /// <response code="200">Retorna todos departamentos inativos no banco</response>
        /// <response code="404">Não encontrado</response>
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
        /// <response code="200">Retorna o item</response>
        /// <response code="404">Não encontrado</response>
        [HttpGet("buscar-departamento/{id}")]
        public IActionResult BuscarDepartamentoPorId(Guid id)
        {
            var departamento = _context.Departamento
                .Include(de => de.Colaboradores.Where(c => c.Status == StatusEnum.Ativo))
                .SingleOrDefault(d => d.Id == id);

            if (departamento == null)
            {
                return NotFound("Não encontrado");
            }

            return Ok(departamento);

        }

        /// <summary>
        /// Cria um novo departamento
        /// </summary>
        /// <response code="201">Cadastrado com sucesso</response>
        /// <response code="400">Item requerido não inserido</response>
        [HttpPost("criar-departamento")]
        public IActionResult CriarDepartamento(DepartamentoEntitie departamento)
        {

            _context.Departamento.Add(departamento);

            _context.SaveChanges();

            return CreatedAtAction(nameof(BuscarDepartamentoPorId), new { id = departamento.Id }, departamento);
        }


        /// <summary>
        /// Atualiza um departamento ativo
        /// </summary>
        /// <response code="204">Atualizado com sucesso</response>
        /// <response code="400">Item requerido não inserido</response>
        [HttpPut("atualizar-departamento/{id}")]
        public IActionResult AtualizarDepartamento(Guid id, DepartamentoEntitie input)
        {
            var departamento = _context.Departamento.SingleOrDefault(d => d.Id == id && d.Status == StatusEnum.Ativo);

            if (departamento == null)
            {
                return NotFound("Não encontrado");
            }

            departamento.Atualizar(input.Nome, input.Sigla);

            _context.Departamento.Update(departamento);
            _context.SaveChanges();

            return NoContent();
        }


        /// <summary>
        /// Ativa um departamento inativo
        /// </summary>
        /// <response code="204">Ativado com sucesso</response>
        /// <response code="404">Item não encontrado</response>
        [HttpPatch("ativar-departamento/{id}")]
        public IActionResult AtivarDepartamento(Guid id)
        {
            var departamento = _context.Departamento.SingleOrDefault(d => d.Id == id && d.Status == StatusEnum.Inativo);

            if (departamento == null)
            {
                return NotFound("Não encontrado");
            }

            departamento.Ativar();
            _context.SaveChanges();

            return NoContent();

        }

        /// <summary>
        /// Inativa um departamento ativo
        /// </summary>
        /// <response code="204">Inativado com sucesso</response>
        /// <response code="404">Item não encontrado</response>
        [HttpDelete("inativar-departamento/{id}")]
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
