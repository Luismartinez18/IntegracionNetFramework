using IntegrationWS.DTOs;
using IntegrationWS.Models;
using IntegrationWS.Utils;
using IntegrationWS.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IntegrationWS.Controllers
{
    [RoutePrefix("api/auth")]    
    public class AuthController : ApiController
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost]
        [Route("register")]
        //[Authorize]
        public async Task<IHttpActionResult> Register([FromBody]UserForRegisterDTO userForRegisterDTO)
        {
            userForRegisterDTO.Username = userForRegisterDTO.Username.Trim().ToLower();

            if (await _authRepo.UserExist(userForRegisterDTO.Username))
                ModelState.AddModelError("Username", "Este nombre de usuario ya ha sido tomado");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userToCreate = new User
            {
                Username = userForRegisterDTO.Username
            };

            var createUser = _authRepo.Register(userToCreate, userForRegisterDTO.Password);

            return StatusCode(HttpStatusCode.Created);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody]UserForLoginDTO userForLoginDTO)
        {
            var userFromRepo = await _authRepo.Login(userForLoginDTO.Username.Trim().ToLower(),
                                                    userForLoginDTO.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var token = TokenGenerator.GenerateTokenJwt(userForLoginDTO.Username);
            return Ok(token);
        }
    }
}
