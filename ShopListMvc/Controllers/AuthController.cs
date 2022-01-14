using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopListMvc.Data;
using ShopListMvc.Models;

namespace ShopListMvc.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public IConfiguration Configuration { get; }

        public AuthController(SignInManager<IdentityUser> signInManager, ApplicationDbContext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            Configuration = configuration;
        }

        [HttpPost, Route("login")]
        public async Task<IActionResult> LogInAsync([FromBody]LoginUser model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, true, false);
            if (result.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, model.Login)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                var now = DateTime.UtcNow;
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: Configuration["AuthOptions:ISSUER"],
                        audience: Configuration["AuthOptions:AUDIENCE"],
                        notBefore: now,
                        claims: claimsIdentity.Claims,
                        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(Configuration["AuthOptions:JWTKEY"]), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    username = claimsIdentity.Name
                };

                return Ok(new { type = "ok", user = response });
            }
            else
            {
                string[] errors = { "Не правильно введены Email или Пароль" };
                return Ok(new { type = "error", errors });
            }
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser model)
        {
            List<string> errors = new List<string>();
            model.Email = model.Email.ToLower();

            if (_context.Users.Count(c => c.Email == model.Email) > 0)
            {
                errors.Add("Такой Email уже зарегистрован! Попробуйте восстановить пароль.");
            }
            if (model.Password != model.ConfirmPassword)
            {
                errors.Add("Пароль и подтверждение паролья не совпадают!");
            }
            if (model.Password.Length < 6)
            {
                errors.Add("Пароль не может быть меньше 6 символов!");
            }

            if (errors.Count > 0)
            {
                return Ok(new { type = "error", errors });
            }

            IdentityUser user = new IdentityUser { Email = model.Email, UserName = model.Email };

            var registerResult = await _userManager.CreateAsync(user, model.Password);
            if (!registerResult.Succeeded)
            {
                return BadRequest();
            }

            Profile profile = new Profile();
            profile.UserId = user.Id;
            profile.Nickname = user.Email;
            _context.Profiles.Add(profile);
            _context.SaveChanges();

            profile.Nickname = model.Email.Substring(0, model.Email.IndexOf("@")) + "_" + profile.Id;
            _context.Profiles.Update(profile);
            _context.SaveChanges();

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
            if (result.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, model.Email)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                var now = DateTime.UtcNow;
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: Configuration["AuthOptions:ISSUER"],
                        audience: Configuration["AuthOptions:AUDIENCE"],
                        notBefore: now,
                        claims: claimsIdentity.Claims,
                        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(Configuration["AuthOptions:KEY"]), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    username = claimsIdentity.Name
                };

                return Ok(new { type = "ok", user = response });
            }

            return BadRequest();
        }
    }
}