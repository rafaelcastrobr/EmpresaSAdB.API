using EmpresaSA.Enums;
using EmpresaSA.Models;

namespace EmpresaSA.Entities
{

    public class ColaboradorEntitie : ColaboradorModel
    {

        public ColaboradorEntitie()
        {
            
            Status = StatusEnum.Ativo;
        }

        

        public void Atualizar(string nome, string documento, Guid id_departamento)
        {
            Nome = nome;
            Documento = documento;
            Id_Departamento = id_departamento;
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
