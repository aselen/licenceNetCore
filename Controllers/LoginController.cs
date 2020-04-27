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
            new User { id = 1, username = "user1", password = "1", name = "User1", surname = "User1", isActive = false },
            new User { id = 2, username = "user2", password = "1", name = "User2", surname = "User2", isActive = false },
            new User { id = 3, username = "user3", password = "1", name = "User3", surname = "User3", isActive = false },
            new User { id = 4, username = "user4", password = "1", name = "User4", surname = "User4", isActive = false },
            new User { id = 5, username = "user5", password = "1", name = "User5", surname = "User5", isActive = false },
            new User { id = 6, username = "user6", password = "1", name = "User6", surname = "User6", isActive = false },
            new User { id = 7, username = "user7", password = "1", name = "User7", surname = "User7", isActive = false },
            new User { id = 8, username = "user8", password = "1", name = "User8", surname = "User8", isActive = false },
            new User { id = 9, username = "user9", password = "1", name = "User9", surname = "User9", isActive = false }
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