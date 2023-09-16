using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using CompagnyTools.Models;
using CompagnyTools.Interface;
using DAL.Entities;
using AutoMapper;
using CompagnyTools.Models.SimpleModels;

namespace CompagnyTools.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogin _iLogin;
        private readonly IMapper _mapper;

        public LoginController(ILogin iLogin, IMapper mapper)
        {
            _iLogin = iLogin;
            _mapper = mapper;
        }

        [HttpPost("userLogin")]
        public async Task<ActionResult<SimpleUserModel>> Login([FromBody] LoginModel model)
        {
            string returnUrl = "/Login";

            if (model != null)
            {
                Users? userChecked = ValidateLogin(model);

                if (userChecked != null)
                {
                    // TODO : remove this
                    UsersRoles? role = userChecked.UsersRoles.First();

                    List<Claim> claims = new()
                    {
                        new Claim("user", userChecked.Username),
                        new Claim("role", role.UserRight)
                    };

                    await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, "user", "role")));

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        SimpleUserModel result = _mapper.Map<SimpleUserModel>(userChecked);
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        private Users? ValidateLogin(LoginModel model)
        {
            Users? checkLogin = _iLogin.Login(model);
            return checkLogin;
        }
    }
}
