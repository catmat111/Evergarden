using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using ProjetoDW.Data;
using ProjetoDW.Models.ViewModels;

namespace ProjetoDW.Controllers.API {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context,UserManager<IdentityUser> userManager,
                              SignInManager<IdentityUser> signInManager,
                              IConfiguration config) {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login) {

            // procura pelo 'username' na base de dados, 
            // para determinar se o utilizador existe
            var user = await _userManager.FindByEmailAsync(login.Username);
            if (user==null) return Unauthorized();

            // se chego aqui, é pq o 'username' existe
            // mas, a password é válida?
            var result = await _signInManager.CheckPasswordSignInAsync(user,login.Password,false);
            if (!result.Succeeded) return Unauthorized();

            // houve sucesso na autenticação
            // vou gerar o 'token', associado ao utilizador
            var token = GenerateJwtToken(login.Username);

            // devolvo o 'token'
            return Ok(new { token });
        }

        private string GenerateJwtToken(string user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user),
            };
            
                
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(s: _config["Jwt:Key"]));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        
    }
}
