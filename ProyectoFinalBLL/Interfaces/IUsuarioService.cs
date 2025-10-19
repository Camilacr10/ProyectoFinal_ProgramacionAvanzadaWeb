using ProyectoFinalBLL.Dtos;

namespace ProyectoFinalBLL.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<UsuarioDto>> GetAllAsync();
        Task<UsuarioDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(UsuarioDto dto);
        Task UpdateAsync(UsuarioDto dto);
        Task DeleteAsync(int id);
    }
}
