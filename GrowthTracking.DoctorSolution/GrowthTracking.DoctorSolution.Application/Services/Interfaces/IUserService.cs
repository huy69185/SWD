namespace GrowthTracking.DoctorSolution.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> CheckUserExists(string userId);
    }
}
