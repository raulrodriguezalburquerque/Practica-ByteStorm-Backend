namespace ByteStorm.Repositorios;
using ByteStorm.Models;
using Microsoft.EntityFrameworkCore;

// Interfaz para el repositorio de misiones
public interface IRepositorioMisiones
{
    // Funcion para obtener una mision por codigo
    Task<Mision> ObtenerMision(int codigo);
}

public class RepositorioMisiones: IRepositorioMisiones
{
    private readonly BDContext _context;

    public RepositorioMisiones(BDContext context)
    {
        _context = context;
    }

    // Funcion para obtener una mision por codigo
    public async Task<Mision> ObtenerMision(int codigo)
    {
        return await _context.Misiones.Include(m => m.equipos)
                    .Include(m => m.operativo)
                    .FirstOrDefaultAsync(m => m.codigo == codigo);
    }
}
