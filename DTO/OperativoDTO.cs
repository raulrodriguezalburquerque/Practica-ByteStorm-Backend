using ByteStorm.Models;

namespace ByteStorm.DTO
{
    // DTO para operativo
    public class OperativoDTO
    {
        // ID del operativo
        public int ID { get; set; }
        // Nombre del operativo
        public required string nombre { get; set; }
        // Rol
        public required string rol { get; set; }
        // Lista de misiones asignadas
        public virtual List<MisionDTO>? misionesDTO { get; set; }
    }
}
