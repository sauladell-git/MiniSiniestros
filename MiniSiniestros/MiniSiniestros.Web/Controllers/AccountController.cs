using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniSiniestros.Dto.Auth;
using MiniSiniestros.Web.Services;

namespace MiniSiniestros.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ISiniestroApiClient _apiClient;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ISiniestroApiClient apiClient, ILogger<AccountController> logger)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginDto());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Password))
            {
                ViewBag.ErrorMessage = "Debe ingresar el nombre de usuario y la contraseña.";
                return View(dto);
            }

            _logger.LogInformation("Solicitando autenticación a la API para usuario '{Nombre}'", dto.Nombre);
            var response = await _apiClient.LoginAsync(dto, cancellationToken);

            if (!response.Success || response.Data == null)
            {
                _logger.LogWarning("Inicio de sesión fallido para usuario '{Nombre}'", dto.Nombre);
                ViewBag.ErrorMessage = response.Errors.FirstOrDefault()?.Message ?? "Credenciales inválidas.";
                return View(dto);
            }

            var authData = response.Data;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, authData.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, $"{authData.Nombre} {authData.Apellido}"),
                new Claim(ClaimTypes.GivenName, authData.Nombre),
                new Claim(ClaimTypes.Surname, authData.Apellido),
                new Claim("jwt_token", authData.Token)
            };

            foreach (var rol in authData.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = authData.Expiration
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            _logger.LogInformation("✅ Usuario '{Nombre}' (Roles: {Roles}) autenticado correctamente en la App Web MVC.",
                authData.Nombre, string.Join(", ", authData.Roles));

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Cerrando sesión de usuario en la App Web.");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
