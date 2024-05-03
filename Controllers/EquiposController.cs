using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ByteStorm.Models;
using ByteStorm.DTO;

namespace ByteStorm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquiposController : ControllerBase
    {
        private readonly BDContext _context;

        public EquiposController(BDContext context)
        {
            _context = context;
        }

        // GET: api/Equipos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipoDTO>>> GetEquipos()
        {
            return await _context.Equipos.Include(e => e.mision)
                .Select(e => EquipoToDTO(e, true)).ToListAsync();
        }

        // GET: api/Equipos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EquipoDTO>> GetEquipo(int id)
        {
            var equipo = await _context.Equipos.Include(e => e.mision)
                .FirstOrDefaultAsync(e => e.ID == id);

            if (equipo == null)
            {
                return NotFound();
            }

            return EquipoToDTO(equipo, true);
        }

        /// <summary>
        /// GET: api/Equipos/Disponibles
        /// </summary>
        /// <returns> Lista de equipos disponibles </returns>
        [HttpGet("Disponibles")]
        public async Task<ActionResult<IEnumerable<EquipoDTO>>> GetEquiposDisponibles()
        {
            return await _context.Equipos.Where(e => e.estado == "Disponible")
                .Include(e => e.mision).Select(e => EquipoToDTO(e, true)).ToListAsync();
        }

        // PUT: api/Equipos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipo(int id, EquipoDTO equipoDTO)
        {
            if (id != equipoDTO.ID)
            {
                return BadRequest();
            }

            var equipo = await _context.Equipos.Include(e => e.mision)
                .FirstOrDefaultAsync(e => e.ID == id);
            if (equipo == null)
            {
                return NotFound();
            }

            // Modificamos los campos del equipo
            equipo.tipo = equipoDTO.tipo;
            equipo.descripcion = equipoDTO.descripcion;

            // Comprobamos si el DTO del equipo tiene una mision
            if (equipoDTO.misionDTO is not null)
            {
                // Buscamos la mision usando el codigo de la mision DTO
                equipo.mision = await _context.Misiones.Include(m => m.equipos)
                    .Include(m => m.operativo)
                    .FirstOrDefaultAsync(m => m.codigo == equipoDTO.misionDTO.codigo);
                // Asignamos el ID
                if (equipo.mision is not null) equipo.codigoMision = equipo.mision.codigo;
                // Cambiamos el estado del equipo a En Uso
                equipo.estado = "En Uso";
            }
            else
            {
                // Eliminamos la mision del equipo original y
                // cambiamos el estado del equipo
                equipo.codigoMision = null;
                equipo.mision = null;
                equipo.estado = "Disponible";
            }

            _context.Entry(equipo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipoExists(id))
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

        // POST: api/Equipos
        [HttpPost]
        public async Task<ActionResult<EquipoDTO>> PostEquipo(EquipoDTO equipoDTO)
        {
            var equipo = new Equipo
            { 
                tipo = equipoDTO.tipo,
                descripcion = equipoDTO.descripcion,
                estado = equipoDTO.estado
            };
            // Comprobamos si el DTO del equipo tiene una mision
            if (equipoDTO.misionDTO is not null)
            {
                // Buscamos la mision usando el codigo del DTO
                equipo.mision = await _context.Misiones.Include(m => m.equipos)
                    .Include(m => m.operativo)
                    .FirstOrDefaultAsync(m => m.codigo == equipoDTO.misionDTO.codigo);
                // Asignamos el codigo
                if (equipo.mision is not null) equipo.codigoMision = equipo.mision.codigo;
                // Cambiamos el estado del equipo a En Uso
                equipo.estado = "En Uso";
            }
            else
                // Asignamos el estado Disponible
                equipo.estado = "Disponible";

            _context.Equipos.Add(equipo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEquipo), 
                new { id = equipo.ID }, 
                EquipoToDTO(equipo,true));
        }

        // DELETE: api/Equipos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }

            _context.Equipos.Remove(equipo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EquipoExists(int id)
        {
            return _context.Equipos.Any(e => e.ID == id);
        }

        /// <summary>
        /// Funcion para transformar un equipo en su version DTO
        /// </summary>
        /// <param name="equipo">Equipo a transformar en DTO</param>
        /// <param name="misionToDTO">Booleano para saber si transformar 
        /// tambien la mision</param>
        /// <returns> Version DTO del equipo </returns>
        public static EquipoDTO EquipoToDTO(Equipo equipo, bool misionToDTO)
        {
            // Creamos un equipo DTO y asignamos id, tipo, descripcion y estado
            var equipoDTO = new EquipoDTO
            {
                ID = equipo.ID,
                tipo = equipo.tipo,
                descripcion = equipo.descripcion,
                estado = equipo.estado
            };
            
            // Comprobamos si el equipo tiene una mision para crear el DTO
            if (misionToDTO && equipo.mision is not null)
                equipoDTO.misionDTO = MisionesController.MisionToDTO(equipo.mision, true, false);
            
            // Devolvemos el resultado
            return equipoDTO;
        }
    }
}
