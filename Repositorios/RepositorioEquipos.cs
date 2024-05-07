namespace ByteStorm.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ByteStorm.Models;
using ByteStorm.DTO;
using ByteStorm.Controllers;

// Interfaz para el repositorio de equipos
public interface IRepositorioEquipos
{
    // Funcion para obtener todos los equipos de la base de datos
    Task<ActionResult<IEnumerable<EquipoDTO>>> ObtenerEquipos();
    // Funcion para obtener un equipo por ID
    Task<Equipo> ObtenerEquipo(int id);
    // Funcion para obtener un equipo DTO por ID
    Task<ActionResult<EquipoDTO>> ObtenerEquipoDTO(int id);
    // Funcion para modificar un equipo
    Task<IActionResult> ModificarEquipo(Equipo equipo);
    // Funcion para insertar un equipo en la base de datos
    Task<int> InsertarEquipo(Equipo equipo);
    // Funcion para borrar un equipo de la base de datos
    Task<IActionResult> BorrarEquipo(int id);
    // Funcion para saber si existe un equipo en la base de datos
    bool EquipoExists(int id);
}

// Clase con el repositorio de los equipos
public class RepositorioEquipos : ControllerBase, IRepositorioEquipos
{
    private readonly BDContext _context;

    public RepositorioEquipos(BDContext context)
    {
        _context = context;
    }

    // Funcion para obtener todos los equipos de la base de datos
    public async Task<ActionResult<IEnumerable<EquipoDTO>>> ObtenerEquipos()
    {
        return await _context.Equipos.Include(e => e.mision)
                .Select(e => EquipoToDTO(e, true)).ToListAsync();
    }

    // Funcion para obtener un equipo por ID
    public async Task<Equipo> ObtenerEquipo(int id)
    {
        return await _context.Equipos.Include(e => e.mision)
                .FirstOrDefaultAsync(e => e.ID == id);
    }

    // Funcion para obtener un equipo DTO por ID
    public async Task<ActionResult<EquipoDTO>> ObtenerEquipoDTO(int id)
    {
        var equipo = await ObtenerEquipo(id);

        if (equipo == null)
        {
            return NotFound();
        }

        return EquipoToDTO(equipo, true);
    }

    // Funcion para modificar un equipo
    public async Task<IActionResult> ModificarEquipo(Equipo equipo)
    {
        _context.Entry(equipo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EquipoExists(equipo.ID))
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

    // Funcion para insertar un equipo en la base de datos
    public async Task<int> InsertarEquipo(Equipo equipo)
    {
        _context.Equipos.Add(equipo);
        return await _context.SaveChangesAsync();
    }

    // Funcion para borrar un equipo de la base de datos
    public async Task<IActionResult> BorrarEquipo(int id)
    {
        var equipo = await ObtenerEquipo(id);
        if (equipo == null)
        {
            return NotFound();
        }

        _context.Equipos.Remove(equipo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Funcion para saber si existe un equipo en la base de datos
    public bool EquipoExists(int id)
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

