using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteStorm.Models
{
    // Clase para representar una mision
    public class Mision
    {
        // Codigo de la mision
        public int codigo { get; set; }
        // Descripcion
        public required string descripcion { get; set; }
        // Estado
        public required string estado { get; set; }
        // Operativo asignado y su ID
        public virtual Operativo? operativo { get; set; }
        public int? idOperativo { get; set; }
        // Lista de equipo asignado
        public virtual List<Equipo>? equipos { get; set; }
    }
}
