using System.Data;
using Azure;
using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Roles.Contracts;
using Home_Service_Finder.Roles.Dtos;

namespace Home_Service_Finder.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _dbContext;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _dbContext = unitOfWork;
        }

        public async Task<APIResponse> AddRoleAsync(RoleRequestDto roleRequestDto)
        {
            var entity = await _dbContext.Roles.FindByNameAsync(roleRequestDto.Name);
            if (entity != null)
            {
                return ResponseHandler.GetBadRequestResponse($"Role {roleRequestDto.Name} already exists");
            }
            Role role = new Role()
            {
                Name = roleRequestDto.Name
            };

            await _dbContext.Roles.AddAsync(role);

            string result = await _dbContext.SaveChangesAsync();

            RoleResponseDto response = new RoleResponseDto()
            {
                Id = role.Id,
                Name = role.Name
            };
            return ResponseHandler.GetSuccessResponse(response, "Role added successfully");
        }

        public async Task<APIResponse> DeleteRoleAysnc(Guid id)
        {
            var role = await _dbContext.Roles.GetByIdAsync(id);

            if (role == null)
            {
                return ResponseHandler.GetNoContentResponse($"Role of {id} not found");
            }
            role.IsDeleted = true;
            role = _dbContext.Roles.UpdateAsync(role);
            string result = await _dbContext.SaveChangesAsync();
            RoleResponseDto response = new RoleResponseDto()
            {
                Id = role.Id,
                Name = role.Name
            };
            return ResponseHandler.GetSuccessResponse(response, "Role delete successfully");
        }

        public async Task<APIResponse> GetAllRoleAsync()
        {
            var roles = await _dbContext.Roles.GetAllAsync();

            // Filter out deleted roles
            var activeRoles = roles.Where(r => !r.IsDeleted).ToList();

            List<RoleResponseDto> response = activeRoles.Select(role => new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name
            }).ToList();

            return ResponseHandler.GetSuccessResponse(response);
        }



        public async Task<APIResponse> GetRoleByIDAsync(Guid id)
        {
            var role = await _dbContext.Roles.GetByIdAsync(id);
            if (role == null )
            {
                return ResponseHandler.GetNotFoundResponse($"Role of Id {id} not found.");
            }
            RoleResponseDto response = new RoleResponseDto()
            {
                Id = role.Id,
                Name = role.Name
            };
            return ResponseHandler.GetSuccessResponse(response);
        }

        public async Task<APIResponse> GetRoleByNameAsync(string name)
        {
            var role = await _dbContext.Roles.FindByNameAsync(name);
            if(role == null || role.IsDeleted)
            {
                return ResponseHandler.GetNotFoundResponse($"Invalid role name");
            }

            RoleResponseDto response = new RoleResponseDto()
            {
                Id = role.Id,
                Name = role.Name
            };

            return ResponseHandler.GetSuccessResponse(response, "Successfully role retrived");

        }

        public async Task<APIResponse> UpdateRoleAysnc(Guid id, RoleRequestDto roleRequestDto)
        {
            var role = await _dbContext.Roles.GetByIdAsync(id);
            if(role == null)
            {
                return ResponseHandler.GetNotFoundResponse($"Role of Id {id} not found");
            }
            if(role.Name.ToLower() != roleRequestDto.Name.ToLower())
            {
                var checkRole =  _dbContext.Roles.FindByNameAsync(roleRequestDto.Name).Result;
                if(checkRole != null){
                    return ResponseHandler.GetBadRequestResponse($"Role of name {roleRequestDto.Name} already exits");
                }
            }
            role.Name = roleRequestDto.Name;
            role = _dbContext.Roles.UpdateAsync(role);
            string result =await _dbContext.SaveChangesAsync();

            RoleResponseDto response = new RoleResponseDto()
            {
                Name = role.Name
            };

            return ResponseHandler.GetSuccessResponse(response, "Role is successfully updated");
        }



    }
}
