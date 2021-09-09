using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Domain;
using Web.Models.Dtos;
using Web.Infrastructure;
using Web.Infrastructure.Model;

namespace Web.Services.User
{
    public class UserServices : IUserServices
    {
        private readonly RaimunDbContext _context;
        private readonly IJwtHandler _jwtHandler;

        public UserServices(RaimunDbContext context, IJwtHandler jwtHandler)
        {
            _context = context;
            _jwtHandler = jwtHandler;
        }

        public async Task<TokenModel> Login(LoginDto dto, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u =>
                u.Username == dto.Username && u.Password == dto.Password, cancellationToken: cancellationToken);
            return _jwtHandler.GenerateToken(user);
        }

        public async Task Register(RegisterDto dto, CancellationToken cancellationToken)
        {
            var user = new Web.Domain.User
            {
                Name = dto.Name,
                Password = dto.Password,
                Username = dto.Username,
                LastName = dto.Lastname,
            };
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public interface IUserServices
    {
        Task<TokenModel> Login(LoginDto dto, CancellationToken cancellationToken);
        Task Register(RegisterDto dto, CancellationToken cancellationToken);
    }
}