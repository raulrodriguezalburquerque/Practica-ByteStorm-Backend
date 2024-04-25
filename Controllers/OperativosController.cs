using ByteStorm.DTO;
using ByteStorm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ByteStorm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperativosController : ControllerBase
    {
        private readonly BDContext _context;

        public OperativosController(BDContext context)
        {
            _context = context;
        }

        // GET: api/Operativos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperativoDTO>>> GetOperativos()
        {
            return await _context.Operativos.Include(o => o.misiones)
                .Select(o => OperativoToDTO(o,true)).ToListAsync();
        }

        // GET: api/Operativos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OperativoDTO>> GetOperativo(int id)
        {
            var operativo = await _context.Operativos.Include(o => o.misiones)
                                            .FirstOrDefaultAsync(o => o.ID == id);

            if (operativo == null)
            {
                return NotFound();
            }

            return OperativoToDTO(operativo,true);
        }

        // PUT: api/Operativos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOperativo(int id, OperativoDTO operativoDTO)
        {
            if (id != operativoDTO.ID)
            {
                return BadRequest();
            }

            var operativo = await _context.Operativos.Include(o => o.misiones)
                .FirstOrDefaultAsync(o => o.ID == id);
            if (operativo == null)
            {
                return NotFound();
            }

            // Modificamos los campos del operativo
            operativo.nombre = operativoDTO.nombre;
            operativo.rol = operativoDTO.rol;

            // Comprobamos si el DTO del operativo tiene misiones
            if (operativoDTO.misionesDTO is not null && operativo.misiones is not null)
            {
                // Si la lista de misiones no esta vacia, actualizamos las misiones del operativo
                if (operativoDTO.misionesDTO.Count > 0)
                {
                    // Obtenemos los codigos de las misions del operativo DTO
                    List<int> codigosDTO = operativoDTO.misionesDTO.Select(m => m.codigo).ToList();
                    // Buscamos las misiones con los codigos
                    List<Mision> misionesDTO = await _context.Misiones
                        .Where(m => codigosDTO.Contains(m.codigo))
                        .ToListAsync();
                    // Cambiamos el estado de las misiones asignadas a Activa
                    misionesDTO.ForEach(m => m.estado = "Activa");


                    // Si el operativo original tenia misiones, cambiamos el estado de las misiones que
                    // se eliminan
                    if (operativo.misiones.Count > 0)
                    {
                        // Obtenemos los codigos de las misiones del operativo original
                        List<int> codigosOriginal = operativo.misiones.Select(m => m.codigo).ToList();
                        // Buscamos las misiones que esten en el operativo original pero no en el DTO
                        List<Mision> misionesOriginal = await _context.Misiones
                            .Where(m => codigosOriginal.Contains(m.codigo) 
                                && !codigosDTO.Contains(m.codigo))
                            .ToListAsync();
                        // Cambiamos el estado de las misiones a Planificada
                        misionesOriginal.ForEach(m => m.estado = "Planificada");
                    }

                    // Asignamos la lista de misiones
                    operativo.misiones = misionesDTO;
                }
                // Si el operativo actualizado no tiene misiones pero el original si,
                // cambiamos el estado de todas las misiones del operativo original y 
                // vaciamos la lista de misiones
                else if (operativo.misiones.Count > 0)
                {
                    // Obtenemos los codigos de las misiones del operativo original
                    List<int> codigosOriginal = operativo.misiones.Select(m => m.codigo).ToList();
                    // Buscamos las misiones con los codigos obtenidos
                    List<Mision> misionesOriginal = await _context.Misiones
                        .Where(m => codigosOriginal.Contains(m.codigo)).ToListAsync();
                    // Cambiamos el estado de las misiones eliminadas a Planificada
                    misionesOriginal.ForEach(m => m.estado = "Planificada");
                    // Vaciamos la lista de misiones
                    operativo.misiones.Clear();
                }
            }

            _context.Entry(operativo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OperativoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Operativos
        [HttpPost]
        public async Task<ActionResult<OperativoDTO>> PostOperativo(OperativoDTO operativoDTO)
        {
            // Creamos un nuevo operativo y asignamos ID, nombre y rol
            var operativo = new Operativo
            {
                ID = operativoDTO.ID,
                nombre = operativoDTO.nombre,
                rol = operativoDTO.rol
            };
            // Comprobamos si el DTO del operativo tiene misiones
            if(operativoDTO.misionesDTO is not null)
            {
                // Obtenemos los codigos de las misions del operativo DTO
                List<int> codigos = operativoDTO.misionesDTO.Select(m => m.codigo).ToList();
                // Buscamos las misiones con los codigos
                List<Mision> misiones = await _context.Misiones.Where(m => codigos.Contains(m.codigo))
                    .ToListAsync();
                // Cambiamos el estado de las misiones a Planificada
                misiones.ForEach(m => m.estado = "Activa");
                // Asignamos la lista de misiones
                operativo.misiones = misiones;
            }

            _context.Operativos.Add(operativo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetOperativo), 
                new { id = operativo.ID }, 
                OperativoToDTO(operativo, true));
        }

        // DELETE: api/Operativos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOperativo(int id)
        {
            var operativo = await _context.Operativos.Include(o => o.misiones)
                .FirstOrDefaultAsync(o => o.ID == id);
            if (operativo == null)
            {
                return NotFound();
            }

            // Si el operativo tiene misiones asignadas, le cambiamos el estado a Planificada
            if (operativo.misiones is not null && operativo.misiones.Count > 0)
            {
                // Obtenemos los codigos de las misiones del operativo
                List<int> codigos = operativo.misiones.Select(m => m.codigo).ToList();
                // Buscamos las misiones con los IDs obtenidos
                List<Mision> misiones = await _context.Misiones
                    .Where(m => codigos.Contains(m.codigo)).ToListAsync();
                // Cambiamos el estado de las misiones a Planificada
                misiones.ForEach(e => e.estado = "Planificada");
            }

            _context.Operativos.Remove(operativo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OperativoExists(int id)
        {
            return _context.Operativos.Any(e => e.ID == id);
        }

        /// <summary>
        /// Funcion para transformar un operativo en su version DTO
        /// </summary>
        /// <param name="operativo">Operativo que transformar en DTO</param>
        /// <param name="misionsToDTO">Booleano para saber si transformamos tambien las misiones</param>
        /// <returns></returns>
        public static OperativoDTO OperativoToDTO(Operativo operativo, bool misionsToDTO)
        {
            // Creamos un operativo DTO y asignamos el ID, nombre y rol
            var operativoDTO = new OperativoDTO
            {
                ID = operativo.ID,
                nombre = operativo.nombre,
                rol = operativo.rol
            };
            // Comprobamos si tenemos que transformar las misiones a DTO
            // y si el operativo tiene misiones
            if (misionsToDTO && operativo.misiones is not null)
            {
                // Convertimos todas las misiones en DTOs pero sin guardar al operativo
                operativoDTO.misionesDTO = operativo.misiones
                    .ConvertAll(m => MisionesController.MisionToDTO(m,false));
            }
            // Devolvemos el resultado
            return operativoDTO;
        }

    }
}
