using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApp.DTOs;
using WebApp.Services;

namespace WebApp.Controllers.v1
{
    [ApiController]
    [Route("api/v1/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public CuentasController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService
            )
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            this.dataProtector = dataProtectionProvider.CreateProtector("valor_unico_y_secreto");
        }


        [HttpGet("encriptar")]
        public ActionResult Encriptar()
        {
            var textoPlano = "Yar Marin";
            var textoCifrado = dataProtector.Protect(textoPlano);

            var textoDescriptado = dataProtector.Unprotect(textoCifrado);

            return Ok( new { 
                textoPlano = textoPlano,
                textoCifrado = textoCifrado,
                textoDescriptado = textoDescriptado
                 
            } );
        }

        [HttpGet("hash/{texto}")]
        public ActionResult RealizarHash(string texto)
        {
            var resultado1 = hashService.Hash(texto);
            var resultado2 = hashService.Hash(texto);

            return Ok( new
            {
                texto = texto,
                Hash1 = resultado1,
                Hash2 = resultado2
            } );


        }

        [HttpGet("encriptarPorTiempo")]
        public ActionResult encriptarPorTiempo()
        {

            var protectorPorTiempo = dataProtector.ToTimeLimitedDataProtector();

            var textoPlano = "Yair Marin";
            var textoCifrado = protectorPorTiempo.Protect(textoPlano, lifetime: TimeSpan.FromSeconds(5) );

            Thread.Sleep(6000);

            var textoDescriptado = protectorPorTiempo.Unprotect(textoCifrado);

            return Ok(new
            {
                textoPlano = textoPlano,
                textoCifrado = textoCifrado,
                textoDescriptado = textoDescriptado

            });
        }


        [HttpPost("registrar", Name = "registrarUsuario")]
        public async Task<ActionResult<RespuestaAutentication>> Registrar(CredencialesUsuario credencialUsuario)
        {
            var usuario = new IdentityUser { UserName = credencialUsuario.Email, Email = credencialUsuario.Email };
            var resultado = await userManager.CreateAsync(usuario, credencialUsuario.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpPost("login", Name = "loginUsuario")]
        public async Task<ActionResult<RespuestaAutentication>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager
                .PasswordSignInAsync(
                    credencialesUsuario.Email,
                    credencialesUsuario.Password,
                    isPersistent: false,
                    lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        [HttpGet("renovarToken", Name = "renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutentication>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;

            var credencialesUsuario = new CredencialesUsuario()
            {
                Email = email
            };

            return await ConstruirToken(credencialesUsuario);
        }

        [HttpPost("hacerAdmin", Name = "hacerAdmin")]
        public async Task<ActionResult> HacerAdmin( EditarAdminDTO editarAdminDTO )
        {
            var usuario = await userManager.FindByEmailAsync( editarAdminDTO.Email );

            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }

        [HttpPost("removerAdmin", Name = "removerAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);

            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }

        private async Task<RespuestaAutentication> ConstruirToken(CredencialesUsuario credenciales)
        {
            var claims = new List<Claim>
            {
                new Claim("email", credenciales.Email)
            };

            var usuario = await userManager.FindByEmailAsync( credenciales.Email );
            var claimsDb = await userManager.GetClaimsAsync(usuario);

            claims.AddRange( claimsDb );

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["secret"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddMinutes(60);

            var securityToken = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: expiracion,
                    signingCredentials: creds
                );

            return new RespuestaAutentication()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }

    }
}
