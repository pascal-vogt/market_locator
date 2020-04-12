namespace Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Service;
    using Web.Dtos;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Database;
    using Microsoft.AspNetCore.Mvc;


    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private SessionService SessionService { get; set; }

        public SessionController(
            SessionService sessionService
        )
        {
            this.SessionService = sessionService;
        }

        [HttpPost]
        public async Task<ActionResult> LogIn(LoginRequestDto loginRequest)
        {
            var user = this.SessionService.TryLogin(loginRequest.Email, loginRequest.Password);
            
            if (user == null)
            {
                return this.BadRequest();
            }
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.RoleAssignments.Select(o => o.Role.Code).FirstOrDefault()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true,
                IssuedUtc =  DateTimeOffset.UtcNow
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);

            return this.Ok();
        }
    }
}