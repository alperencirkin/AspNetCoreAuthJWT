using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace AspNetCoreAuthJWT.Services.User
{
    public interface IUserService
    {
        (string username, string token)? Authenticate(string username, string password);
    }
}