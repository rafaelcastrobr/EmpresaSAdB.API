using EmpresaSA.Enums;
using System.Text.Json.Serialization;

namespace EmpresaSA.Models
{
    public class ColaboradorModel
    {

        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Documento { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StatusEnum Status { get; set; }
        public Guid Id_Departamento { get; set; }
    }
}
