using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Domain.Entities;
using eCommerce.ShareLibrary.Interface;
using eCommerce.ShareLibrary.Response;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<Response> Register(AppUserDTO appUserDTO);
        Task<Response> Login(LoginDTO loginDTO);
        Task<AppUserDTO?> GetUserDTO<TKey>(TKey userId);
    }
}
