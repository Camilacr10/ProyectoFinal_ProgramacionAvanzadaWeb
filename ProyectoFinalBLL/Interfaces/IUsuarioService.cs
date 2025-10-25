using ProyectoFinalBLL.DTOs;

namespace ProyectoFinalBLL.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<UsuarioDto>> GetAllAsync();
        Task<UsuarioDto?> GetByIdAsync(int id);
        Task<UsuarioDto?> GetByCorreoAsync(string correo);
        Task CreateAsync(UsuarioDto dto);
        Task UpdateAsync(UsuarioDto dto);
        Task DeleteAsync(int id);
    }
}
