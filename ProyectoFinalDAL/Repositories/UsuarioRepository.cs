using Microsoft.EntityFrameworkCore;
using ProyectoFinalDAL.Entidades;
using ProyectoFinalDAL.Data;


namespace ProyectoFinalDAL.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SgcDbContext _context;

        public UsuarioRepository(SgcDbContext context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> GetByCorreoAsync(string correo)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
        }

        public async Task AddAsync(Usuario entity)
        {
            await _context.Usuarios.AddAsync(entity);
        }

        public async Task UpdateAsync(Usuario entity)
        {
            _context.Usuarios.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await GetByIdAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}