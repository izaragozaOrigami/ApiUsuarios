using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Origami.Identity.Api.Core.Data;
using Origami.Identity.Api.Core.Infrastructure;
using Origami.Identity.Api.Core.Infrastructure.Security.Request;
using Origami.Identity.Api.Core.Services;
using System.Dynamic;

namespace Origami.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SecurityData _securityData;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;
        public ModulesController(UserManager<ApplicationUser> userManager,
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




        [Authorize(Roles = "BasicAuthorization")]
        [Route("GetAllModules")]
        [HttpPost]
        public IActionResult GetAllModules()
        {
            List<Module> modules = _securityData.GetModules();

            return Ok(modules);
        }


        [Authorize(Roles = "BasicAuthorization")]
        [Route("GetAllModulesNew")]
        [HttpPost]
        public IActionResult GetAllModulesNew()
        {
            List<RolesAccionAlta> modules = _securityData.GetAllModulesNew();

            return Ok(modules);
        }


        [Authorize(Roles = "BasicAuthorization")]
        [Route("GetAllModulesByRoleNew")]
        [HttpPost]
        public IActionResult GetAllModulesByRoleNew(GetPermissonRol RoleId)
        {
            List<RolesAccionAlta> modules = _securityData.GetAllModulesByRoleNew(RoleId.RoleId);

            return Ok(modules);
        }

        [Authorize(Roles = "BasicAuthorization")]
        [Route("GetAllModulesByRoleNewAlong")]
        [HttpPost]
        public IActionResult GetAllModulesByRoleNewAlong(GetPermissonRol RoleId)
        {
          //  SecurityData securityData = new SecurityData();
            List<RolesAccionAlta> modules = _securityData.GetAllModulesByRoleNewAlong(RoleId.RoleId);

            return Ok(modules);
        }


        [Authorize(Roles = "BasicAuthorization")]
        [Route("GetModulePermissonByUserNew")]
        [HttpPost]
        public IActionResult GetModulePermissonByUserNew(User user)
        {
            List<RolesAccionAlta> modules = _securityData.GetAllModulesSpecialNew(user.Id);

            return Ok(modules);
        }

        [Authorize(Roles = "BasicAuthorization")]
        [Route("GetSpecialPermissonExtendedByUserNew")]
        [HttpPost]
        public IActionResult GetSpecialPermissonExtendedByUseNew(User user)
        {
            List<RolesAccionAlta> modules = _securityData.GetSpecialPermissonExtendedByUserNew(user.Id, 11);

            return Ok(modules);
        }

        [Authorize(Roles = "BasicAuthorization")]
        [Route("CreatePermissonSpecial2")]
        [HttpPost]
        public async Task<IActionResult> CreatePermissonSpecial2([FromBody] PermissonSpecialRequest2 model)
        {
          //  SecurityData security = new SecurityData();

            if (model.PermissonsSpecial.Count > 0)
            {
                _securityData.CreatePermissonSpecial2(model);

            }
            else
            {
                return BadRequest("No hay permisos por agregar");
                // responseMsg = StatusCode((int)response.StatusCode, response);
            }
            return Ok("Operación completada con éxito");

        }

        [Authorize(Roles = "BasicAuthorization")]
        [Route("DelSpecialPermissonExtendedByUser")]
        [HttpPost]
        public IActionResult DelSpecialPermissonExtendedByUser(User user)
        {
            ResponseBaseDto response = _securityData.DelSpecialPermissonExtendedByUser(user.Id, 1);

            return Ok(response);
        }

        [Authorize(Roles = "BasicAuthorization")]
        [Route("DelSpecialPermissonExtendedByUserAction")]
        [HttpPost]
        public IActionResult DelSpecialPermissonExtendedByUserAction(PermissonSpecial2 user)
        {
            ResponseBaseDto response = _securityData.DelSpecialPermissonExtendedByUserAccion(user.UserId, 2, user.IdModulo);

            return Ok(response);
        }

    }
}


