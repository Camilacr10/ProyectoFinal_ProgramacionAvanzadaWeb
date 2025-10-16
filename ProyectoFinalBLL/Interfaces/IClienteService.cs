using ProyectoFinalBLL.DTOs;

namespace ProyectoFinalBLL.Interfaces
{
    public interface IClienteService
    {
        Task<List<ClienteDto>> GetAllAsync();
        Task<ClienteDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(ClienteDto dto);
        Task UpdateAsync(ClienteDto dto);
        Task DeleteAsync(int id);
    }
}
