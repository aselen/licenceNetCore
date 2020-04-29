using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using licenseDemoNetCore;
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
        private readonly UserContext _userContext;



        public RefreshController(ILogger<RefreshController> logger, UserContext userContext)
        {
            _logger = logger;
            _userContext = userContext;
        }

        [HttpPost]
        public ActionResult<DtoUser> Post(Refresh refresh)
        {
            try
            {
                var redisManager = new RedisCacheManager();

                _logger.LogInformation("refresh istegi geldi...");

                var tokens = new JwtSecurityToken(jwtEncodedString: refresh.authToken);

                var userId = tokens.Claims.First(c => c.Type == "unique_name").Value;

                var session = redisManager.Get(userId);

                if (session == null)
                    return BadRequest();

                if (!refresh.authToken.Equals(session)) 
                    return Forbid();

                var user = _userContext.users.FirstOrDefault(u => u.id.Equals(long.Parse(userId)) && u.refreshtoken.Equals(refresh.refreshToken));

                if (user == null)
                    return Forbid();

                if (user.refreshtokenexpirationdate > DateTime.Now)
                {
                    TokenHandler tokenHandler = new TokenHandler();
                    Token token = tokenHandler.CreateAccessToken(user.id);

                    user.refreshtoken = token.RefreshToken;

                    _userContext.SaveChanges();

                    redisManager.Remove(user.id.ToString());
                    redisManager.Set(user.id.ToString(), token.AccessToken, 300);

                    return Ok(new DtoUser
                    {
                        authToken = token.AccessToken,
                        name = user.name,
                        surname = user.surname,
                        refreshToken = token.RefreshToken
                    });
                }

                return Forbid();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return BadRequest();
            }

        }
    }
}