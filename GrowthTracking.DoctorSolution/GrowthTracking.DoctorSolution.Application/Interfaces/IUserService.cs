using GrowthTracking.DoctorSolution.Application.DTOs;
using GrowthTracking.DoctorSolution.Infrastructure.DBContext;

namespace GrowthTracking.DoctorSolution.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> CheckUserExists(string userId);
        Task<Response> Login(LoginDTO loginDTO);
        Task<Response> Register(UserDTO user);
    }
}
