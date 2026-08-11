using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Origami.Identity.Api.Core.Data;
using Origami.Identity.Api.Core.Infrastructure;
using Origami.Identity.Api.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using Azure.Storage.Blobs;
using LoginRequest = Origami.Identity.Api.Core.Infrastructure.LoginRequest;
using Origami.Identity.Api.Core.Infrastructure.Security.Request;
using Origami.Identity.Api.Core.Infrastructure.Security.Response;

namespace Origami.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SecurityData _securityData;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            SecurityData securityData,
            IEmailService emailService,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _securityData = securityData;
            _emailService = emailService;
            _env = env;
        }

        [AllowAnonymous]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateUserBindingModel createUserModel)
        {
            IList<string> accessibilitys = new List<string>();

            foreach (CBU cBU in createUserModel.Accessibility.CBU)
            {
                accessibilitys.Add(cBU.Id);
            }

            var user = new ApplicationUser()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                Level = 3,
                JoinDate = DateTime.Now.Date,
                NoEmployee = createUserModel.NoEmployee,
                PhoneNumber = createUserModel.PhoneNumber,
                PhoneExtension = createUserModel.PhoneExtension,
                PositionId = createUserModel.PositionId,
                Accessibility = accessibilitys,
                PhotoURL = UserStorageResolver.ToRelativePath(createUserModel.URLPhotos),
                Position = createUserModel.Position
            };

            IdentityResult addUserResult =
     await _userManager.CreateAsync(user, createUserModel.Password);

            if (!addUserResult.Succeeded)
            {
                List<string> errores = new List<string>();

                foreach (var error in addUserResult.Errors)
                {
                    if (error.Description.Contains("already", StringComparison.OrdinalIgnoreCase))
                    {
                        string mensaje = $"El usuario {createUserModel.Username} ya existe";
                        errores.Add(mensaje);
                    }
                    else
                    {
                        errores.Add(error.Description);
                    }
                }
                return BadRequest(errores);
            }
            IdentityResult roleResult = null;
            var defaultRole = await _roleManager.FindByNameAsync("BasicAuthorization");
            if (defaultRole == null)
            {
                var role = new IdentityRole("BasicAuthorization");
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                    return BadRequest(result.Errors.Select(e => e.Description));
                defaultRole = role;
            }
            string[] roles = new string[createUserModel.Roles.Count() + createUserModel.RolesFijos.Count() + 1];
            roles[0] = defaultRole.Name;

            int roleIndex = 1;
            foreach (Role rol in createUserModel.Roles)
            {
                var initialRole = await _roleManager.FindByIdAsync(rol.Id);
                roles[roleIndex] = initialRole.Name;
                roleIndex++;
            }
            foreach (Role rol in createUserModel.RolesFijos)
            {
                var initialRole = await _roleManager.FindByIdAsync(rol.Id);
                roles[roleIndex] = initialRole.Name;
                roleIndex++;
            }
            ResponseBaseDto responseBaseDto = _securityData.InsSecurityPermissons(createUserModel.Accessibility, user.Id.ToString());
            roleResult = await _userManager.AddToRolesAsync(user, roles);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return BadRequest(roleResult.Errors.Select(e => e.Description));
            }
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code });
            if (callbackUrl == null)
                return BadRequest("No se pudo generar la URL de confirmación.");
            var filePath = Path.Combine(AppContext.BaseDirectory, "Resources", "resources_views_emails_welcome.html");
            string body = await System.IO.File.ReadAllTextAsync(filePath);
            body = string.Format(body, user.FirstName + " " + user.LastName, createUserModel.Password, callbackUrl, user.Email);
            await _emailService.SendEmailAsync(user.Email, "Confirma tu registro", body);
            return CreatedAtRoute("GetUserById", new { id = user.Id }, user);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }
            try
            {

                var user = await _userManager.FindByIdAsync(userId);

                List<CBU> cbus = _securityData.GetUserAccesibility(user.Id);
                IList<string> accessibilitys = new List<string>();

                foreach (CBU cBU in cbus)
                {
                    accessibilitys.Add(cBU.Id);
                }
                user.Accessibility = accessibilitys;

                IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);

                if (result.Succeeded)
                {

                    string url = _configuration["RedirectUrl"] + "/auth/login";

                    return Redirect(url);
                }
                else
                {
                    return BadRequest(result.Errors);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // =========================
        // 2️ LOGIN
        // =========================
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
            {
                ApplicationUser test = await _userManager.FindByEmailAsync(model.username);
                var user = test;
                if (user == null)
                    return Unauthorized();

                var passwordValid = await _userManager.CheckPasswordAsync(user, model.password);
                if (!passwordValid)
                    return Unauthorized();

                // Claims
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

                var _roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(_roles.Select(r => new Claim(ClaimTypes.Role, r)));

                // JWT
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpireHours"])),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                List<Role> roles = new List<Role>();
                List<CBU> cbus = new List<CBU>();
                List<Modulos> modules = new List<Modulos>();

                PermissonAccess permissonAccess = new PermissonAccess();
                // SecurityData securityData = new SecurityData();
                permissonAccess.perAccess = _securityData.ConstruirEstructuraJerarquica(user.Id);
                roles = _securityData.GetRolesXId(user.Id);
                cbus = _securityData.GetUserAccesibility(user.Id);
                modules = _securityData.GetUserModulo(user.Id);
                var tokenHandler = new JwtSecurityTokenHandler();
                string token2 = tokenHandler.WriteToken(token);

                LoginResponse response = new LoginResponse()
                {
                    //Avatar = "https://sistemascarreteros.s3-us-west-2.amazonaws.com/assets/avatar2.jpg",
                    Created = DateTime.Now.ToString(),
                    Email = user.Email,
                    Fullname = user.FirstName + " " + user.LastName,
                    Id = user.Id,
                    EmailConfirmed = user.EmailConfirmed,
                    Phone = user.PhoneNumber,
                    PhoneExtension = user.PhoneExtension,
                    Picture = UserStorageResolver.ResolveUrl(user.PhotoURL),
                    Roles = roles,
                    CBUs = cbus,
                    Updated = DateTime.Now.ToString(),
                    Token = token2,
                    TimeExpirationToken = token.ValidTo.ToLocalTime(),
                    PositionId = user.PositionId.ToString(),
                    NoEmployee = user.NoEmployee,
                    UserName = user.UserName,
                    Modules = modules,
                    permissonAccess = permissonAccess,
                };

                if (user != null)
                {
                    return Ok(response);
                }
                else
                { return Unauthorized("Credenciales Invalidas"); }
            }
            catch
            {
                return Unauthorized("Credenciales Invalidas");
            }



        }

        [Authorize(Roles = "SuperAdmin,Administrador")]
        [HttpPost("user/{id:guid}", Name = "GetUserById")]
        public async Task<IActionResult> GetUser(string Id)
        {
            User user = await Task.Run(() => _securityData.GetUserById(Id));
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }

        [Authorize(Roles = "BasicAuthorization")]
        [Route("userXId", Name = "GetUserXId")]
        [HttpPost]
        public async Task<IActionResult> GetUserXId(User request)
        {
            //SecurityData securityData = new SecurityData();
            SecurityUser user = await Task.Run(() => _securityData.GetUserXId(request.Id));
            List<Modulos> modulos = new List<Modulos>();
            modulos = _securityData.GetUserModulo(user.Id);
            user.Modulos = modulos;
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }


        [Authorize(Roles = "BasicAuthorization")]
        [HttpPost]
        [Route("Users")]
        public IActionResult Users(UsersRequest usersRequest)
        {
            UsersResponse usersResponse = new UsersResponse();
            List<User> users = _securityData.GetUsers(int.Parse(usersRequest.page), usersRequest.pageLength);
            usersResponse.CountUsers = users.Count.ToString();
            usersResponse.Users = users;

            return Ok(usersResponse);
        }


        /// <summary>
        /// Post UpdateUser
        /// </summary>
        /// <param name="User">New User parameters</param>
        /// <returns>Update an User</returns>
        //  [Authorize(Roles = "SuperAdmin,Administrador")]
        [Route("UpdateUser")]
        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserUpdate request)
        {
            // Crear o obtener el UserManager
            // TODO ASP.NET Identity debe reemplazarse por la identidad de ASP.NET Core. Para obtener más información, consulte https://docs.microsoft.com/aspnet/core/migration/identity.
            //   var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));



            // Obtener el usuario que deseas actualizar
            // var user = await userManager.FindByIdAsync(request.Id);

            var user = await _userManager.FindByIdAsync(request.Id);
            //    userManager.AddToRole(request.Id,);

            if (user != null)
            {
                user.NoEmployee = request.NoEmployee;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.PhoneNumber = request.PhoneNumber;
                user.PhoneExtension = request.PhoneExtension;
                user.PhotoURL = UserStorageResolver.ToRelativePath(request.photoURL);
                user.LockoutEnabled = request.EstatusId;
                user.Position = request.Position;

                //   IdentityUserRole identityUserRole = new IdentityUserRole();

                // var rolesActuales = userManager.GetRoles(request.Id);
                var rolesActuales = await _userManager.GetRolesAsync(user);

                //var context = new ApplicationDbContext();
                //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                // Obtener una lista de roles
                var rolesAll = _roleManager.Roles.ToList();



                //  SecurityData securityData = new SecurityData();
                List<CBU> cBUs = _securityData.GetUserAccesibility(user.Id);


                IList<string> accessibilitys = new List<string>();

                foreach (CBU cBU in cBUs)
                {
                    accessibilitys.Add(cBU.Id);
                }


                user.Accessibility = accessibilitys;

                // Actualizar el usuario en la base de datos
                try
                {
                    var result = await _userManager.UpdateAsync(user);

                    foreach (string role in rolesActuales)
                    {
                        if (!role.Equals("BasicAuthorization"))
                        {
                            //var result2 = await userManager.RemoveFromRoleAsync(user.Id, role);
                            var result2 = await _userManager.RemoveFromRoleAsync(user, role);
                        }

                    }

                    //  var defaultRole = this.AppRoleManager.FindByName("BasicAuthorization");

                    var defaultRole = await _roleManager.FindByNameAsync("BasicAuthorization");
                    //string[] roles = new string[request.Roles.Count() + request.RolesFijos.Count() + 1];
                    //roles[0] = defaultRole.Name;
                    var roles = new List<string>();

                    int roleIndex = 1;
                    foreach (Role rol in request.Roles)
                    {
                        //var initialRole = this.AppRoleManager.FindById(rol.Id);
                        var initialRole = await _roleManager.FindByIdAsync(rol.Id);

                        if (initialRole != null)
                            roles.Add(initialRole.Name);

                        // roles[roleIndex] = initialRole.Name;
                        roleIndex++;
                    }


                    foreach (Role rol in request.RolesFijos)
                    {
                        //var initialRole = this.AppRoleManager.FindById(rol.Id);
                        var initialRole = await _roleManager.FindByIdAsync(rol.Id);

                        if (initialRole != null)
                            roles.Add(initialRole.Name);

                        // roles[roleIndex] = initialRole.Name;
                        roleIndex++;
                    }
                    IdentityResult roleResult = null;
                    //  roleResult = await this.AppUserManager.AddToRolesAsync(user.Id, roles);
                    roleResult = await _userManager.AddToRolesAsync(user, roles);

                    request.Roles = new List<Role>();

                    foreach (var rol in request.Roles)
                    {
                        var initialRole = await _roleManager.FindByIdAsync(rol.Id);
                        if (initialRole != null)
                        {
                            rol.Name = initialRole.Name;
                            request.Roles.Add(rol);
                        }
                    }

                    request.RolesFijos = new List<Role>();

                    foreach (var rol in request.RolesFijos)
                    {
                        var initialRole = await _roleManager.FindByIdAsync(rol.Id);
                        if (initialRole != null)
                        {
                            rol.Name = initialRole.Name;
                            request.RolesFijos.Add(rol);
                        }
                    }
                    foreach (var rolesbyAdd in request.Roles)
                    {
                        var roleToFind = rolesAll.FirstOrDefault(r => r.Id == rolesbyAdd.Id);
                        await _userManager.AddToRoleAsync(user, roleToFind.Name);
                    }
                    foreach (var rolesbyAdd in request.RolesFijos)
                    {
                        var roleToFind = rolesAll.FirstOrDefault(r => r.Id == rolesbyAdd.Id);
                        await _userManager.AddToRoleAsync(user, roleToFind.Name);
                    }

                    return Ok(request);

                }
                catch (Exception ex)
                {
                    string test = ex.Message;
                }
            }


            return Ok();
        }


        /// <summary>
        /// UpdateAccesibility
        /// </summary>
        /// <param name="UpdateAccesibility">New User parameters</param>
        /// <returns>New user</returns>
        [Authorize(Roles = "BasicAuthorization")]
        [HttpPost]
        [Route("updateAccesibility")]
        public async Task<IActionResult> updateAccesibility(RequestAccessibilityUpdate requestAccessibilityUpdate)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ResponseBaseDto responseBaseDto = _securityData.UpdSecurityAccesibility(requestAccessibilityUpdate.accessibility, requestAccessibilityUpdate.UserId);
            return Ok(responseBaseDto);
        }

        /// <summary>
        /// Get UserAccessibility
        /// </summary>
        /// <param name="User">New User parameters</param>
        /// <returns>New user</returns>
        [Authorize(Roles = "BasicAuthorization")]
        [HttpPost]
        [Route("GetUserAccesibility")]
        public IActionResult GetUserAccesibility(User request)
        {
            GetUserAccebilityResponse response = new GetUserAccebilityResponse();
            List<CBU> cBUs = _securityData.GetUserAccesibility(request.Id);

            response.countGetUserAccesibility = cBUs.Count.ToString();
            response.getUserAccesibility = cBUs;

            return Ok(response);
        }

        /// <summary>
        /// Get GetAllPositions
        /// </summary>
        /// <param name="User">New User parameters</param>
        /// <returns>New user</returns>
        [Authorize(Roles = "BasicAuthorization")]
        [HttpGet]
        [Route("GetAllPosition")]
        public IActionResult GetAllPosition()
        {
            PositionResponse response = new PositionResponse();
            List<Position> positions = _securityData.GetPositions();
            response.countPosition = positions.Count.ToString();
            response.positions = positions;

            return Ok(response);
        }

        /// <summary>
        /// Get UserAccessibility
        /// </summary>
        /// <param name="User">New User parameters</param>
        /// <returns>New user</returns>
        [Authorize(Roles = "BasicAuthorization")]
        [HttpGet]
        [Route("GetAccesibility")]
        public IActionResult GetAccesibility()
        {
            //  SecurityData securityData = new SecurityData();
            List<CBU> cBUs = _securityData.GetAccesibility();
            AccebilityResponse accebilityResponse = new AccebilityResponse();
            accebilityResponse.countAccesibility = cBUs.Count.ToString();
            accebilityResponse.accesibility = cBUs;
            return Ok(accebilityResponse);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("GetPasswordRecoveryCode")]
        public async Task<IActionResult> GetPasswordRecoveryCode(RecoveryCodeEmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var filePath = Path.Combine(AppContext.BaseDirectory, "Resources", "resources_views_emails_askPassword.html");
            string url = _configuration["RedirectUrl"] + "/auth/password-change";
            var result = await _userManager.FindByEmailAsync(request.email);

            if (result != null)
            {

                RecoveryCodes currentCode = _securityData.GetRecoveryCode(request.email, null);
                if (currentCode == null)
                {
                    string code = RandomDigits();
                    RecoveryCodes recovery = new RecoveryCodes()
                    {
                        Email = request.email,
                        RecoveryCode = code,
                        ValidityDate = DateTime.Now.AddMinutes(10)
                    };

                    if (_securityData.CreateRecoveryCode(recovery))
                    {
                        string body = await System.IO.File.ReadAllTextAsync(filePath);
                        body = string.Format(body, result.FirstName + " " + result.LastName, code, url);

                        await _emailService.SendEmailAsync(result.Email, "Recupera tu contraseña", body);
                    }
                    ;
                }
                else
                {
                    string body = System.IO.File.ReadAllText(filePath);
                    body = string.Format(body, result.FirstName + " " + result.LastName, currentCode.RecoveryCode, url);
                    await _emailService.SendEmailAsync(result.Email, "Recupera tu contraseña", body);
                }
            }

            return Ok(new { message = "Operación exitosa pero sin contenido" });
        }


        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="model">Recovery pasword model</param>
        /// <returns>Seccess</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            RecoveryCodes currentCode = _securityData.GetRecoveryCode("", model.RecoveryCode);

            var user = await _userManager.FindByEmailAsync(currentCode.Email);

            string newPassword = model.NewPassword;

            //string hashedNewPassword = this.AppUserManager.PasswordHasher.HashPassword(newPassword);


            //  var hashedNewPassword= await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (user != null)
            {
                var tokenPassword = await _userManager.GeneratePasswordResetTokenAsync(user);
                try
                {
                    List<CBU> cbus = _securityData.GetUserAccesibility(user.Id);
                    IList<string> accessibilitys = new List<string>();

                    foreach (CBU cBU in cbus)
                    {
                        accessibilitys.Add(cBU.Id);
                    }
                    user.Accessibility = accessibilitys;

                    IdentityResult result = await _userManager.ResetPasswordAsync(user, tokenPassword, newPassword);
                    if (!result.Succeeded)
                    {
                        return BadRequest(result);
                    }
                }
                catch (DbUpdateException ex)
                {
                    var error = ex.InnerException?.InnerException?.Message ?? ex.Message;

                    // aquí podrías loguearlo
                    Console.WriteLine(error);

                    return BadRequest("Ocurrió un error al tratar de resetear la contraseña.");
                }
            }
            else
            {
                return Ok();
            }
            return Ok(new { Message = "Contraseña cambiada exitosamente." });
        }

        /// <summary>
        /// UploadFile
        /// </summary>
        /// <param name="User">New User parameters</param>
        /// <returns>New user</returns>
        [HttpPost]
        [Authorize(Roles = "BasicAuthorization")]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFileAsync([FromForm] UploadFileRequestDto request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No se recibió archivo.");
            string fileName = GenerateFileName(request.File.FileName);
            string connectionString = _configuration["Storage:ConnectionString"];
            bool consolidated = UserStorageResolver.ConsolidationEnabled;
            string containerName = consolidated
                ? UserStorageResolver.ConsolidatedContainer
                : _configuration["Storage:ContainerName"];
            // Dev: usuarios/<archivo> en el contenedor consolidado. Qa/Main: <archivo> en el contenedor por tipo.
            string blobPath = consolidated
                ? UserStorageResolver.BuildRelativePath(UserStorageResolver.UsersFolder, fileName)
                : fileName;
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient blobContainer = blobServiceClient.GetBlobContainerClient(containerName);

                BlobClient blob = blobContainer.GetBlobClient(blobPath);

                using (var stream = request.File.OpenReadStream())
                {
                    await blob.UploadAsync(stream, overwrite: true);
                }

                UploadFileResponseDto response = new UploadFileResponseDto()
                {
                    // Dev: URL absoluta resuelta para preview; el front la reenvía y se normaliza a ruta al guardar.
                    FileUrl = consolidated ? UserStorageResolver.ResolveUrl(blobPath) : blob.Uri.AbsoluteUri
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Reenvía el correo de bienvenida con una nueva contraseña generada al usuario indicado por email.
        /// </summary>
        /// <param name="request">Payload con el email del usuario</param>
        /// <returns>Ok si el correo se envió correctamente</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("ResendEmail")]
        public async Task<IActionResult> ResendEmail(RecoveryCodeEmailRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.email))
                return BadRequest("El email es requerido.");

            var existingUser = await _userManager.FindByEmailAsync(request.email);
            if (existingUser == null)
                return NotFound("Usuario no encontrado.");

            string password = GenerateRandomPassword(10);

            try
            {
                // Cargar accessibility del usuario (mismo patrón que ResetPassword/ConfirmEmail)
                List<CBU> cbus = _securityData.GetUserAccesibility(existingUser.Id);
                IList<string> accessibilitys = new List<string>();
                foreach (CBU cBU in cbus)
                {
                    accessibilitys.Add(cBU.Id);
                }
                existingUser.Accessibility = accessibilitys;

                // Resetear el password al nuevo generado
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                IdentityResult resetResult = await _userManager.ResetPasswordAsync(existingUser, resetToken, password);
                if (!resetResult.Succeeded)
                    return BadRequest(resetResult.Errors.Select(e => e.Description));

                // Generar el link de confirmación de email
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(existingUser);
                var callbackUrl = Url.Link("ConfirmEmailRoute", new { userId = existingUser.Id, code = code });
                if (callbackUrl == null)
                    return BadRequest("No se pudo generar la URL de confirmación.");

                // Armar el cuerpo del email usando el template de bienvenida
                var filePath = Path.Combine(AppContext.BaseDirectory, "Resources", "resources_views_emails_welcome.html");
                string body = await System.IO.File.ReadAllTextAsync(filePath);
                body = string.Format(body, existingUser.FirstName + " " + existingUser.LastName, password, callbackUrl, existingUser.Email);

                await _emailService.SendEmailAsync(existingUser.Email, "Confirma tu registro", body);

                return Ok(new { message = "Correo reenviado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private string GenerateRandomPassword(int length)
        {
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lower = "abcdefghijkmnpqrstuvwxyz";
            const string digits = "23456789";
            const string special = "!@#$%*";
            const string all = upper + lower + digits + special;

            var random = new Random();
            var chars = new char[length];
            // Aseguramos al menos uno de cada tipo para cumplir políticas comunes
            chars[0] = upper[random.Next(upper.Length)];
            chars[1] = lower[random.Next(lower.Length)];
            chars[2] = digits[random.Next(digits.Length)];
            chars[3] = special[random.Next(special.Length)];
            for (int i = 4; i < length; i++)
                chars[i] = all[random.Next(all.Length)];
            // Shuffle
            for (int i = chars.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }
            return new string(chars);
        }

        private string RandomDigits()
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < 10; i++)
                s = System.String.Concat(s, random.Next(10).ToString());
            return s;
        }

        private string GenerateFileName(string fileName)
        {
            try
            {
                string strFileName = DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff") + "_" + fileName;
                return strFileName;
            }
            catch (Exception ex)
            {
                return fileName;
            }
        }

    }


}