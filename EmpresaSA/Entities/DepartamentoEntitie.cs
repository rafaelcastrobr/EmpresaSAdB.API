using EmpresaSA.Enums;
using EmpresaSA.Models;

namespace EmpresaSA.Entities
{

    public class DepartamentoEntitie : DepartamentoModel
    {
        public DepartamentoEntitie()
        {
            Colaboradores = new List<ColaboradorModel>();
            Status = StatusEnum.Ativo;

        }

        public void Atualizar(string nome, string sigla)
        {
            Nome = nome;
            Sigla = sigla;
        }

        public void Inativar()
        {
            Status = StatusEnum.Inativo;
        }

        public void Ativar()
        {
            Status = StatusEnum.Ativo;
        }

    }
}
