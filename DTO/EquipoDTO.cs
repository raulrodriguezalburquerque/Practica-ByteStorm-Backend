using ByteStorm.Models;

namespace ByteStorm.DTO
{
    // DTO de equipo
    public class EquipoDTO
    {
        // ID del equipo
        public int ID { get; set; }
        // Tipo
        public required string tipo { get; set; }
        // Descripcion
        public required string descripcion { get; set; }
        // Estado
        public required string estado { get; set; }
        // Mision a la que esta asignado
        public virtual MisionDTO? misionDTO { get; set; }
    }

}
