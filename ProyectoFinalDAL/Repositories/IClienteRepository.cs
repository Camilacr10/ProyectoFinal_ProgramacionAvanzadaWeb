using ProyectoFinalDAL.Entidades;

namespace ProyectoFinalDAL.Repositories
{
    public interface IClienteRepository
    {
        Task<List<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(int id);
        Task<Cliente?> GetByIdentificacionAsync(string identificacion);
        Task AddAsync(Cliente entity);
        Task UpdateAsync(Cliente entity);
        Task DeleteAsync(int id);
        Task<int> SaveChangesAsync();
    }
}
