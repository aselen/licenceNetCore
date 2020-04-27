using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace backend.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class RefreshController : ControllerBase
    {
        private readonly ILogger<RefreshController> _logger;

        public RefreshController(ILogger<RefreshController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<User> Get(string expiredToken, string refreshToken)
        {
            _logger.LogInformation("refresh istegi geldi...");

            var user1 = LoginController.users.Where(u => u.id.Equals(1)).FirstOrDefault();

            _logger.LogInformation("GELEN ISTEK");


            _logger.LogInformation("expiredToken ->" + expiredToken);
            _logger.LogInformation("refreshToken ->" + refreshToken);

            _logger.LogInformation("KULLANICI");

            _logger.LogInformation("KULLANICI expiredToken ->" + user1.authToken);
            _logger.LogInformation("KULLANICI refreshToken ->" + user1.refreshToken);


            var user = LoginController
                .users
                .Where(u => 
                !String.IsNullOrEmpty(u.authToken) && 
                u.authToken.Equals(expiredToken) && 
                !String.IsNullOrEmpty(u.refreshToken) &&
                u.refreshToken.Equals(refreshToken)).FirstOrDefault();

            _logger.LogInformation(user.authToken);

            if (user == null)
                return NoContent();

            if (user?.RefreshTokenEndDate > DateTime.Now)
            {
                TokenHandler tokenHandler = new TokenHandler();
                Token token = tokenHandler.CreateAccessToken();

                user.authToken = token.AccessToken;
                user.authTokenExpireTime = token.Expiration;

                return user;
            }

            return null;
        }
    }
}