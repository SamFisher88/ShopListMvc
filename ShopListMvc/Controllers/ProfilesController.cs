using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopListMvc.Data;

namespace ShopListMvc.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfilesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Profiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfiles()
        {
            return await _context.Profiles.Where(x => x.UserId == _userManager.GetUserId(User)).ToListAsync();
        }

        [HttpGet, Route("my")]
        public async Task<ActionResult<Profile>> My()
        {
            return await _context.Profiles.SingleAsync(x => x.UserId == _userManager.GetUserId(User));
        }

        // GET: api/Profiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Profile>> GetProfile(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);

            if (profile == null)
            {
                return NotFound();
            }

            return profile;
        }

        [HttpPut]
        public async Task<IActionResult> PutProfile(Profile profile)
        {
            string userId = _userManager.GetUserId(User);
            Profile oldProfile = await _context.Profiles.SingleAsync(x => x.UserId == userId);
            if (_context.Profiles.Any(x => x.Nickname == profile.Nickname && x.UserId != userId))
            {
                return Ok(new { type = "error", message = "Такой Nickname уже зарегистрирован!" });
            }
            oldProfile.Nickname = profile.Nickname;
            oldProfile.UserName = profile.UserName;
            oldProfile.AboutUser = profile.AboutUser;

            _context.Entry(oldProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new {type = "ok"});
        }

        private bool ProfileExists(int id)
        {
            return _context.Profiles.Any(e => e.Id == id);
        }
    }
}
