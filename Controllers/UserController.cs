using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace licenseDemoNetCore.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserContext _userContext;

        public UserController(ILogger<UserController> logger, UserContext userContext)
        {
            _logger = logger;
            _userContext = userContext;
        }

        [HttpGet]
        public ActionResult<DtoUser> Get() {
            var expiredToken = HttpContext.Request.Headers["Authorization"];

            var tokens = new JwtSecurityToken(jwtEncodedString: expiredToken.ToString().Split(' ')[1]);
            var userId = tokens.Claims.First(c => c.Type == "unique_name").Value;

            var user = _userContext.users.FirstOrDefault(u => u.id.Equals(long.Parse(userId)));

            return Ok(new DtoUser{
                name= user.name,
                surname= user.surname
            });
        }

        [HttpPatch]
        public ActionResult<DtoUser> Patch([FromBody] JsonPatchDocument<User> user)
        {
            // _logger.LogInformation("patch istegi geldi...");

            // var userDb = _userContext.users.FirstOrDefault(u => u.id.Equals(id));

            // if (userDb == null)
            //     return BadRequest();

            // user.ApplyTo(userDb, ModelState);
            // _userContext.SaveChanges();

            // return Ok();

            var expiredToken = HttpContext.Request.Headers["Authorization"];

            _logger.LogInformation(expiredToken);
            //_logger.LogInformation(id.ToString());


            var tokens = new JwtSecurityToken(jwtEncodedString: expiredToken.ToString().Split(' ')[1]);
            var userId = tokens.Claims.First(c => c.Type == "unique_name").Value;

            var userDb = _userContext.users.FirstOrDefault(u => u.id.Equals(long.Parse(userId)));

            if (userDb == null)
                return BadRequest();

            user.ApplyTo(userDb, ModelState);
            _userContext.SaveChanges();

            var userDto = _userContext.users.FirstOrDefault(u => u.id.Equals(long.Parse(userId)));

            return Ok(new DtoUser{
                name= userDto.name,
                surname= userDto.surname
            });
        }

        
    }
}