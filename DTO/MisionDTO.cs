using ByteStorm.Models;

namespace ByteStorm.DTO
{
    // DTO para mision
    public class MisionDTO
    {
        // Codigo de la mision
        public int codigo { get; set; }
        // Descripcion
        public required string descripcion { get; set; }
        // Estado
        public required string estado { get; set; }
        // Operativo asignado
        public virtual OperativoDTO? operativoDTO { get; set; }
        // Lista de equipo asignado
        public virtual List<EquipoDTO>? equiposDTO { get; set; }
    }
}
