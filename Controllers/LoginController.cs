using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using licenseDemoNetCore;
using Microsoft.AspNetCore.JsonPatch;

namespace backend.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private const int activeUserLimit = 2;

        private readonly ILogger<LoginController> _logger;
        private readonly UserContext _userContext;

        public LoginController(ILogger<LoginController> logger, UserContext userContext)
        {
            _logger = logger;
            _userContext = userContext;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<DtoUser> Post(DtoUser paramUser)
        {
            var rm = new RedisCacheManager();

            if (rm.Count() >= activeUserLimit)
                return Forbid();

            var user = _userContext.users.Where(u => u.username.Equals(paramUser.username) && u.password.Equals(paramUser.password)).FirstOrDefault();

            if (user == null)
                return Forbid();

            var session = rm.Get(user.id.ToString());

            if (!string.IsNullOrEmpty(session))
                return Forbid();

            TokenHandler tokenHandler = new TokenHandler();
            Token token = tokenHandler.CreateAccessToken(user.id);

            //Refresh token Users tablosuna işleniyor.
            user.refreshtoken = token.RefreshToken;
            user.refreshtokenexpirationdate = token.Expiration.AddMinutes(20);

            _userContext.SaveChanges();

            //write to redis
            rm.Set(user.id.ToString(), token.AccessToken, 300);

            return Ok(new DtoUser
            {
                authToken = token.AccessToken,
                name = user.name,
                surname = user.surname,
                refreshToken = token.RefreshToken
            });
        }
    }
}