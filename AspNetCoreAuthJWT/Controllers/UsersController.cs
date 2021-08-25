using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreAuthJWT.Services.User;
using AspNetCoreAuthJWT.Models;

namespace AspNetCoreJWT.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService) => _userService = userService;

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authenticate([FromBody] AuthenticateModel authenticateModel)
        {
            var user = _userService.Authenticate(authenticateModel.Username, authenticateModel.Password);

            if (user == null)
                return BadRequest("Username or password incorrect!");

            return Ok(new { Username = user.Value.username, Token = user.Value.token });
        }

        [HttpGet]
        public IActionResult UserData()
        { 
        return Ok(new { login_status = "ok" });
        }


}
}
