using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteStorm.Models
{
    // Clase para representar un equipo
    public class Equipo
    {
        // ID del equipo
        public int ID { get; set; }
        // Tipo
        public required string tipo { get; set; }
        // Descripcion
        public required string descripcion { get; set; }
        // Estado
        public required string estado { get; set; }
        // Codigo de la mision a la que esta asignado
        public int? codigoMision { get; set; }
        // Mision a la que esta asignado
        public virtual Mision? mision { get; set; }
    }

}
