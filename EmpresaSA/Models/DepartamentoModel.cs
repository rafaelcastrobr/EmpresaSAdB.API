using EmpresaSA.Entities;
using EmpresaSA.Enums;
using System.Text.Json.Serialization;

namespace EmpresaSA.Models
{
    public class DepartamentoModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Sigla { get; set; }
        public List<ColaboradorModel> Colaboradores { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StatusEnum Status { get; set; }
    }
}
