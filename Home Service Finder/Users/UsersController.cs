using Home_Service_Finder.ServiceCategories.Dtos;
using Home_Service_Finder.Users.Contracts;
using Home_Service_Finder.Users.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Home_Service_Finder.Users
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        //[Authorize(Roles = "admin")]
        public async Task<APIResponse> GetAllUser()
        {
            var apiResponse = await _userService.GetAllUsers();
            return apiResponse;
        }


        [HttpGet("{id}")]
        public async Task<APIResponse> GetUserById(Guid id)
        {
            var apiResponse = await _userService.GetUserById(id);
            return apiResponse;
        }

        [HttpPost]
        public async Task<APIResponse> AddUser([FromBody] UserRequestDto userRequestDto)
        {
            var apiResponse = await _userService.AddUser(userRequestDto);
            return apiResponse;
        }

        [HttpDelete("{id}")]
        public async Task<APIResponse> DeleteUser(Guid id)
        {
            var apiResponse = await _userService.DeleteUser(id);
            return apiResponse;

        }

        [HttpPut("{id}")]
        public async Task<APIResponse> UpdateServiceCategoryAsync(Guid id, [FromBody] UserUpdateRequestDto userUpdateRequestDto)
        {
            var apiResponse = await _userService.UpdateUser(id, userUpdateRequestDto);
            return apiResponse;
        }
    }


}


