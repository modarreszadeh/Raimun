using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Models.Dtos;
using Web.Infrastructure.Api;
using Web.Infrastructure.Model;
using Web.Services.User;

namespace Web.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public AccountController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost]
        [Route("/login")]
        public async Task<ApiResult<TokenModel>> Login(LoginDto dto, CancellationToken cancellationToken)
        {
            return await _userServices.Login(dto, cancellationToken);
        }

        [HttpPost]
        [Route("/register")]
        public async Task<ApiResult> Register(RegisterDto dto, CancellationToken cancellationToken)
        {
            await _userServices.Register(dto, cancellationToken);
            return Ok();
        }
    }
}