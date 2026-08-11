
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Origami.Identity.Api.Core.Data;
using Origami.Identity.Api.Core.Infrastructure;
using Origami.Identity.Api.Core.Infrastructure.Security.Request;
using Origami.Identity.Api.Core.Services;

namespace Origami.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SecurityData _securityData;

        public RolesController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            SecurityData securityData)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _securityData = securityData;

        }


        [Authorize(Roles = "BasicAuthorization")]
        [Route("GetAllRoles")]
        [HttpPost]
        public IActionResult GetAllRoles()
        {

            List<Role> roles = _securityData.GetRoles();

            RoleResponse roleResponse = new RoleResponse();
            roleResponse.CountRol = roles.Count.ToString();
            roleResponse.roles = roles;

            return Ok(roleResponse);
        }

        [Authorize(Roles = "BasicAuthorization")]
        [HttpPost]
        [Route("CreateRol2")]
        public async Task<IActionResult> CreateRol2(PermissonRolesRequest2 model)
        {
          
            List<string> roleId;
            roleId = _securityData.GetRolId(model.RolName);
            if (roleId.Count <= 0 || roleId[0].Equals(string.Empty))
            {
                var role = new IdentityRole { Name = model.RolName };
                var result = await _roleManager.CreateAsync(role);
                roleId = _securityData.GetRolId(model.RolName);
                _securityData.CreateRol2(model, roleId);
                if (!result.Succeeded)
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest("Error: el rolName: " + model.RolName + " ya existe");
                // responseMsg = StatusCode((int)response.StatusCode, response);
            }
            return Ok("Operación completada con éxito");

        }
        [Authorize(Roles = "BasicAuthorization")]
        [HttpPost]
        [Route("updateRol2")]
        public async Task<IActionResult> updateRol2(PermissonRolesRequest2 model)
        {

            List<string> roleId = new List<string>();
            roleId = _securityData.GetRolIdById(model.RolId);
            if (roleId.Count()>0)
            {
                _securityData.UpdateRol2(model, roleId);
            }
            else
            {
                throw new InvalidOperationException("Error: al tratar de actualizar el rolName: " + model.RolName + "");
            }
            return Ok("Operación completada con éxito");

        }


    }
}


