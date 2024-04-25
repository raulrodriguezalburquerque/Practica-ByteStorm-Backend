using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteStorm.Models
{
    // Clase para representar un operativo
    public class Operativo
    {
        // ID del operativo
        public int ID { get; set; }
        // Nombre del operativo
        public required string nombre { get; set; }
        // Rol
        public required string rol { get; set; }
        // Lista de misiones asignadas
        public virtual List<Mision>? misiones { get; set; }
    }
}
