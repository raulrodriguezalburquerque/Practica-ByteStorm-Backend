using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ByteStorm.Models;
using ByteStorm.DTO;
using ByteStorm.Repositorios;

namespace ByteStorm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquiposController : ControllerBase
    {
        // Repositorio de equipos y misiones
        private readonly IRepositorioEquipos _repositorioEquipos;
        private readonly IRepositorioMisiones _repositorioMisiones;

        private readonly BDContext _context;

        public EquiposController(IRepositorioEquipos repositorioEquipos,
            IRepositorioMisiones repositorioMisiones, BDContext context)
        {
            _repositorioEquipos = repositorioEquipos;
            _repositorioMisiones = repositorioMisiones;

            _context = context;
        }

        // GET: api/Equipos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipoDTO>>> GetEquipos()
        {
            return await _repositorioEquipos.ObtenerEquipos();
        }

        // GET: api/Equipos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EquipoDTO>> GetEquipo(int id)
        {
            return await _repositorioEquipos.ObtenerEquipoDTO(id);
        }

        // PUT: api/Equipos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipo(int id, EquipoDTO equipoDTO)
        {
            if (id != equipoDTO.ID)
            {
                return BadRequest();
            }

            var equipo = await _repositorioEquipos.ObtenerEquipo(id);
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
                equipo.mision = await _repositorioMisiones
                    .ObtenerMision(equipoDTO.misionDTO.codigo);
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

            return await _repositorioEquipos.ModificarEquipo(equipo);
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
                equipo.mision = await _repositorioMisiones
                    .ObtenerMision(equipoDTO.misionDTO.codigo);
                // Asignamos el codigo
                if (equipo.mision is not null) equipo.codigoMision = equipo.mision.codigo;
                // Cambiamos el estado del equipo a En Uso
                equipo.estado = "En Uso";
            }
            else
                // Asignamos el estado Disponible
                equipo.estado = "Disponible";

            await _repositorioEquipos.InsertarEquipo(equipo);

            return CreatedAtAction(
                nameof(GetEquipo),
                new { id = equipo.ID },
                RepositorioEquipos.EquipoToDTO(equipo, true));
        }

        // DELETE: api/Equipos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            return await _repositorioEquipos.BorrarEquipo(id);
        }

    }
}
