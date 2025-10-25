using Microsoft.EntityFrameworkCore;
using ProyectoFinalDAL.Data;
using ProyectoFinalDAL.Entidades;

namespace ProyectoFinalDAL.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SgcDbContext _ctx;
        public UsuarioRepository(SgcDbContext ctx) => _ctx = ctx;

        public Task<List<Usuario>> GetAllAsync()
            => _ctx.Usuarios.AsNoTracking().OrderBy(u => u.NombreCompleto).ToListAsync();

        public Task<Usuario?> GetByIdAsync(int id)
            => _ctx.Usuarios.FindAsync(id).AsTask();

        public Task<Usuario?> GetByCorreoAsync(string correo)
            => _ctx.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Correo == correo);

        public async Task AddAsync(Usuario entity) => await _ctx.Usuarios.AddAsync(entity);

        public Task UpdateAsync(Usuario entity)
        {
            _ctx.Usuarios.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _ctx.Usuarios.FindAsync(id);
            if (e != null) _ctx.Usuarios.Remove(e);
        }

        public Task<int> SaveChangesAsync() => _ctx.SaveChangesAsync();
    }
}
