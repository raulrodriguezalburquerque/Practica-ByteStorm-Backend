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
            return await _context.Equipos.Select(e => EquipoToDTO(e)).ToListAsync();
        }

        // GET: api/Equipos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EquipoDTO>> GetEquipo(int id)
        {
            var equipo = await _context.Equipos.FindAsync(id);

            if (equipo == null)
            {
                return NotFound();
            }

            return EquipoToDTO(equipo);
        }

        /// <summary>
        /// GET: api/Equipos/Disponibles
        /// </summary>
        /// <returns> Lista de equipos disponibles </returns>
        [HttpGet("Disponibles")]
        public async Task<ActionResult<IEnumerable<EquipoDTO>>> GetEquiposDisponibles()
        {
            return await _context.Equipos.Where(e => e.estado == "Disponible")
                .Select(e => EquipoToDTO(e)).ToListAsync();
        }

        // PUT: api/Equipos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipo(int id, EquipoDTO equipoDTO)
        {
            if (id != equipoDTO.ID)
            {
                return BadRequest();
            }

            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }

            // Modificamos los campos del equipo
            equipo.tipo = equipoDTO.tipo;
            equipo.descripcion = equipoDTO.descripcion;
            equipo.estado = equipoDTO.estado;

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

            _context.Equipos.Add(equipo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEquipo), 
                new { id = equipo.ID }, 
                EquipoToDTO(equipo));
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

        // Funcion para transformar un equipo en su version DTO
        public static EquipoDTO EquipoToDTO(Equipo equipo) =>
           new EquipoDTO
           {
               ID = equipo.ID,
               tipo = equipo.tipo,
               descripcion = equipo.descripcion,
               estado = equipo.estado
           };
    }
}
