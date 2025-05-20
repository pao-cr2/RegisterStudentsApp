using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RegisterStudents.API.Data;
using RegisterStudents.API.Dto;
using RegisterStudents.API.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace RegisterStudents.API.Services
{
   
    public interface IAuthService
    {
        Task<LoginResponseDto> AuthenticateAsync(string email, string password);
    }
}
