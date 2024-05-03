using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ByteStorm.Controllers;
using ByteStorm.Models;
using ByteStorm.DTO;

namespace ByteStorm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MisionesController : ControllerBase
    {
        private readonly BDContext _context;

        public MisionesController(BDContext context)
        {
            _context = context;
        }

        // GET: api/Misiones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MisionDTO>>> GetMisiones()
        {
            return await _context.Misiones.Include(m => m.operativo).Include(m => m.equipos)
                .Select(m => MisionToDTO(m, true, true)).ToListAsync();
        }

        // GET: api/Misiones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MisionDTO>> GetMision(int id)
        {
            var mision = await _context.Misiones.Include(m => m.operativo).Include(m => m.equipos)
                                            .FirstOrDefaultAsync(m => m.codigo == id);

            if (mision == null)
            {
                return NotFound();
            }

            return MisionToDTO(mision, true, true);
        }

        // PUT: api/Misiones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMision(int id, MisionDTO misionDTO)
        {
            // Ademas del codigo compobamos si la mision esta completada
            // Una mision completada no se puede modificar
            if (id != misionDTO.codigo || misionDTO.estado == "Completada")
            {
                return BadRequest();
            }

            var mision = await _context.Misiones.Include(m => m.equipos)
                .FirstOrDefaultAsync(m => m.codigo == id);
            if (mision == null)
            {
                return NotFound();
            }

            // Modificamos los campos de la mision
            mision.descripcion = misionDTO.descripcion;
            mision.estado = misionDTO.estado;

            // Comprobamos si el DTO de la mision tiene operativo
            if (misionDTO.operativoDTO is not null)
            {
                // Buscamos el operativo usando el ID del operativo DTO
                mision.operativo = await _context.Operativos.Include(o => o.misiones)
                                            .FirstOrDefaultAsync(o => o.ID == misionDTO.operativoDTO.ID);
                // Asignamos el ID
                if (mision.operativo is not null)  mision.idOperativo = mision.operativo.ID;
                // Cambiamos el estado de mision a activa
                mision.estado = "Activa";
            }
            else
            {
                // Eliminamos al operativo de la mision original y cambiamos el estado de la mision
                mision.idOperativo = null;
                mision.operativo = null;
                mision.estado = "Planificada";
            }

            // Comprobamos que la lista de equipos no sea nula
            if (misionDTO.equiposDTO is not null && mision.equipos is not null)
            {
                // Si la lista de equipos no esta vacia, actualizamos los equipos de mision
                if(misionDTO.equiposDTO.Count > 0)
                {
                    // Obtenemos los IDs de los equipos del DTO
                    List<int> IDsDTO = misionDTO.equiposDTO.Select(e => e.ID).ToList();
                    // Buscamos los equipos con los IDs obtenidos
                    List<Equipo> equiposDTO = await _context.Equipos.Where(e => IDsDTO.Contains(e.ID))
                        .ToListAsync();
                    // Cambiamos el estado de los equipos asignados a En Uso
                    equiposDTO.ForEach(e => e.estado = "En Uso");
                    
                    // Si la mision original tenia equipos, cambiamos el estado de los equipos que
                    // se eliminan
                    if (mision.equipos.Count > 0)
                    {
                        // Obtenemos los IDs de los equipos de la mision original
                        List<int> IDsOriginal = mision.equipos.Select(e => e.ID).ToList();
                        // Buscamos los equipos que esten en la mision original pero no en el DTO
                        List<Equipo> equiposOriginal = await _context.Equipos
                          .Where(e => IDsOriginal.Contains(e.ID) && !IDsDTO.Contains(e.ID))
                          .ToListAsync();
                        // Cambiamos el estado de los equipos filtrados a Disponible
                        equiposOriginal.ForEach(e => e.estado = "Disponible");
                    }

                    // Asignamos la lista de equipos
                    mision.equipos = equiposDTO;
                }
                // Si la mision actualizada no tiene equipos pero la original si,
                // cambiamos el estado de todos los equipos de la mision original y 
                // vaciamos la lista de equipos
                else if (mision.equipos.Count > 0)
                {
                    // Obtenemos los IDs de los equipos de la mision original
                    List<int> IDsOriginal = mision.equipos.Select(e => e.ID).ToList();
                    // Buscamos los equipos con los IDs obtenidos
                    List<Equipo> equiposOriginal = await _context.Equipos
                        .Where(e => IDsOriginal.Contains(e.ID)).ToListAsync();
                    // Cambiamos el estado de los equipos eliminados a Disponible
                    equiposOriginal.ForEach(e => e.estado = "Disponible");
                    // Vaciamos la lista de equipos
                    mision.equipos.Clear();
                }
            }

            _context.Entry(mision).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MisionExists(id))
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

        // PUT: api/Misiones/Completar/5
        // Funcion para completar una mision con un ID determinado
        [HttpPut("Completar/{id}")]
        public async Task<IActionResult> CompleteMision(int id)
        {
            var mision = await _context.Misiones.Include(m => m.equipos)
                .FirstOrDefaultAsync(m => m.codigo == id);
            if (mision == null)
            {
                return NotFound();
            }

            // Se quita el operativo asignados
            mision.idOperativo = null;
            mision.operativo = null;
            // Si la mision tenia equipos, le cambiamos el estado a disponibles y limpiamos
            // la lista
            if (mision.equipos is not null && mision.equipos.Count > 0)
            {
                // Obtenemos los IDs de los equipos de la mision
                List<int> IDs = mision.equipos.Select(e => e.ID).ToList();
                // Buscamos los equipos con los IDs obtenidos
                List<Equipo> equipos = await _context.Equipos
                    .Where(e => IDs.Contains(e.ID)).ToListAsync();
                // Cambiamos el estado de los equipos eliminados a Disponible
                equipos.ForEach(e => e.estado = "Disponible");
                // Vaciamos la lista de equipos
                mision.equipos.Clear();
            }

            // Se cambia el estado de la mision a completada
            mision.estado = "Completada";

            _context.Entry(mision).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MisionExists(id))
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

        // POST: api/Misiones
        [HttpPost]
        public async Task<ActionResult<MisionDTO>> PostMision(MisionDTO misionDTO)
        {
            // Creamos una nueva mision y asignamos la descripcion y estado del DTO
            var mision = new Mision
            {
                descripcion = misionDTO.descripcion,
                estado = misionDTO.estado,
            };
            // Comprobamos si el DTO de la mision tiene operativo
            if (misionDTO.operativoDTO is not null)
            {
                // Buscamos el operativo usando el ID del operativo DTO
                mision.operativo = await _context.Operativos.Include(o => o.misiones)
                                            .FirstOrDefaultAsync(o => o.ID == misionDTO.operativoDTO.ID);
                // Asignamos el ID
                if (mision.operativo is not null) mision.idOperativo = mision.operativo.ID;
                // Cambiamos el estado de mision a activa
                mision.estado = "Activa";
            }
            else
            {
                // Asignamos el estado planificada
                mision.estado = "Planificada";
            }

            // Comprobamos que la lista de equipos no sea nula
            if (misionDTO.equiposDTO is not null)
            {
                // Obtenemos los IDs de los equipos de la mision DTO
                List<int> IDs = misionDTO.equiposDTO.Select(e => e.ID).ToList();
                // Buscamos los equipos con los IDs obtenidos
                List<Equipo> equipos = await _context.Equipos.Where(e => IDs.Contains(e.ID)).ToListAsync();
                // Cambiamos el estado de los equipos a En Uso
                equipos.ForEach(e => e.estado = "En Uso");
                // Asignamos la lista de equipos
                mision.equipos = equipos;
            }

            _context.Misiones.Add(mision);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMision), 
                new { id = mision.codigo }, 
                MisionToDTO(mision, true, true));
        }

        // DELETE: api/Misiones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMision(int id)
        {
            var mision = await _context.Misiones.Include(m => m.equipos)
                .FirstOrDefaultAsync(m => m.codigo == id);
            if (mision == null)
            {
                return NotFound();
            }

            // Si la mision tenia equipos, les cambiamos el estado a disponible
            if (mision.equipos is not null && mision.equipos.Count > 0)
            {
                // Obtenemos los IDs de los equipos de la mision
                List<int> IDs = mision.equipos.Select(e => e.ID).ToList();
                // Buscamos los equipos con los IDs obtenidos
                List<Equipo> equipos = await _context.Equipos
                    .Where(e => IDs.Contains(e.ID)).ToListAsync();
                // Cambiamos el estado de los equipos a Disponible
                equipos.ForEach(e => e.estado = "Disponible");
            }

            _context.Misiones.Remove(mision);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MisionExists(int id)
        {
            return _context.Misiones.Any(e => e.codigo == id);
        }

        /// <summary>
        /// Funcion para transformar una mision en su version DTO
        /// </summary>
        /// <param name="mision">Mision a transformar en DTO</param>
        /// <param name="operativoToDTO">Booleano para saber si transformar 
        /// tambien el operativo</param>
        /// <param name="equiposToDTO">Booleano para saber si transformar 
        /// tambien los equipos</param>
        /// <returns> Version DTO de la mision </returns>
        public static MisionDTO MisionToDTO(Mision mision, bool operativoToDTO, bool equiposToDTO)
        {
            // Creamos una mision DTO y asignamos el codigo, descripcion y estado
            var misionDTO = new MisionDTO
            {
                codigo = mision.codigo,
                descripcion = mision.descripcion,
                estado = mision.estado
            };
            
            // Comprobamos si la mision tiene un operativo para crear el DTO
            if(operativoToDTO && mision.operativo is not null)
                // Creamos el DTO del operativo pero sin guardar las misiones
                misionDTO.operativoDTO = OperativosController
                    .OperativoToDTO(mision.operativo,false);  
            
            // Comprobamos si la mision tiene equipos para crear el DTO
            if (equiposToDTO && mision.equipos is not null)
                // Convertimos todos los equipos en DTOs
                misionDTO.equiposDTO = mision.equipos
                    .ConvertAll(m => EquiposController.EquipoToDTO(m, false));
            
            // Devolvemos el resultado
            return misionDTO;
        }
    }
}
