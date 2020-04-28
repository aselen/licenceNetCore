using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using licenseDemoNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
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
            var redisManager = new RedisCacheManager();
            _logger.LogInformation("refresh istegi geldi...");

            var tokens = new JwtSecurityToken(jwtEncodedString: expiredToken);

            var userId = tokens.Claims.First(c => c.Type == "unique_name").Value;

            var session = redisManager.Get(userId);

            if (session == null)
                return BadRequest();

            var user = LoginController
                .users
                .Where(u =>
                !String.IsNullOrEmpty(u.authToken) &&
                u.authToken.Equals(expiredToken) &&
                !String.IsNullOrEmpty(u.refreshToken) &&
                u.refreshToken.Equals(refreshToken)).FirstOrDefault();

            _logger.LogInformation(user.authToken);

            if (user == null)
                return Forbid();

            if (user?.RefreshTokenEndDate > DateTime.Now)
            {
                TokenHandler tokenHandler = new TokenHandler();
                Token token = tokenHandler.CreateAccessToken(user.id);

                user.authToken = token.AccessToken;
                user.authTokenExpireTime = token.Expiration;

                redisManager.Remove(user.id.ToString());
                redisManager.Set(user.id.ToString(), token.AccessToken, 60);

                return user;
            }

            return Forbid();
        }
    }
}