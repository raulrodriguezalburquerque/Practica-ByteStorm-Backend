using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteStorm.Models
{
    /*
    // Enumerado para tipo de equipo
    public enum TipoEquipo
    {
        Software,
        Hardware
    }

    // Enumerado para estado de equipo
    public enum EstadoEquipo
    {
        Disponible,
        EnUso
    }
    */

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
    }

}
