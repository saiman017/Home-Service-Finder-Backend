using Home_Service_Finder.Roles.Contracts;
using Home_Service_Finder.Roles.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Home_Service_Finder.Roles
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;          
        }
        [HttpPost]
        public async Task<APIResponse> AddRoleAsync([FromBody] RoleRequestDto roleRequestDto)
        {
            var apiResponse = await _roleService.AddRoleAsync(roleRequestDto);
            return apiResponse;
        }

        [HttpGet]
        public async Task<APIResponse> GetAllRoleAsync()
        {
            var apiResponse = await _roleService.GetAllRoleAsync();
            return apiResponse;
        }

        [HttpGet("/{name}")]
        public async Task<APIResponse> GetRoleByName(string name)
        {
            var apiResponse = await _roleService.GetRoleByNameAsync(name);
            return apiResponse;

        }

        [HttpGet("{id}")]
        public async Task<APIResponse> GetRoleByIdAsync(Guid id)
        {
            var apiResponse = await _roleService.GetRoleByIDAsync(id);
            return apiResponse;
        }

        [HttpDelete("{id}")]
        public async Task<APIResponse> DeleteRoleAsync(Guid id)
        {
            var apiResponse = await _roleService.DeleteRoleAysnc(id);
            return apiResponse;
        }

        [HttpPut("{id}")]
        public async Task<APIResponse> UpdateRoleAsync(Guid id,[FromBody]RoleRequestDto roleRequestDto)
        {
            var apiResponse = await _roleService.UpdateRoleAysnc(id,roleRequestDto);
            return apiResponse;
        }

    }
}
