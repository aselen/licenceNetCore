using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        public static IEnumerable<User> users = new List<User>
        {   
            new User { id = 1, username = "aselen", password = "1", name = "Alper", surname = "Selen", isActive = false },
            new User { id = 2, username = "bselen", password = "1", name = "Balper", surname = "Selen", isActive = false },
            new User { id = 3, username = "cselen", password = "1", name = "CAlper", surname = "Selen", isActive = false },
            new User { id = 4, username = "dselen", password = "1", name = "DAlper", surname = "Selen", isActive = false },
            new User { id = 5, username = "eselen", password = "1", name = "EAlper", surname = "Selen", isActive = false },
            new User { id = 6, username = "fselen", password = "1", name = "FAlper", surname = "Selen", isActive = false },
            new User { id = 7, username = "gselen", password = "1", name = "GAlper", surname = "Selen", isActive = false },
            new User { id = 8, username = "hselen", password = "1", name = "HAlper", surname = "Selen", isActive = false },
            new User { id = 9, username = "iselen", password = "1", name = "IAlper", surname = "Selen", isActive = false }
        };

        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<User> Post(User paramUser) 
        {
            var user = users.Where(u => u.username.Equals(paramUser.username) && u.password.Equals(paramUser.password)).FirstOrDefault();

            if (user == null)
                return NoContent();


            TokenHandler tokenHandler = new TokenHandler();
            Token token = tokenHandler.CreateAccessToken();

            //Refresh token Users tablosuna işleniyor.
            user.refreshToken = token.RefreshToken;
            user.RefreshTokenEndDate = token.Expiration.AddMinutes(20);
            user.authToken = token.AccessToken;
            user.authTokenExpireTime = token.Expiration;

            return Ok(user);
        }
    }
}