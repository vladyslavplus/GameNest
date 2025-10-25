using GameNest.CatalogService.BLL.DTOs.Roles;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.CatalogService.Api.Controllers
{
    [ApiController]
    [Route("api/Catalog/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Get all roles with pagination and filtering.
        /// </summary>
        /// <param name="parameters">Filtering and pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the paginated list of roles</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles([FromQuery] RoleParameters parameters, CancellationToken cancellationToken)
        {
            var roles = await _roleService.GetRolesPagedAsync(parameters, cancellationToken);
            return Ok(roles);
        }

        /// <summary>
        /// Get a role by Id.
        /// </summary>
        /// <param name="id">Role Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the role details</response>
        /// <response code="404">Role not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDto>> GetRoleById(Guid id, CancellationToken cancellationToken)
        {
            var role = await _roleService.GetRoleByIdAsync(id, cancellationToken);
            if (role == null) return NotFound();
            return Ok(role);
        }

        /// <summary>
        /// Create a new role.
        /// </summary>
        /// <param name="dto">Role creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="201">Role created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] RoleCreateDto dto, CancellationToken cancellationToken)
        {
            var createdRole = await _roleService.CreateRoleAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id }, createdRole);
        }

        /// <summary>
        /// Update an existing role.
        /// </summary>
        /// <param name="id">Role Id</param>
        /// <param name="updateDto">Role update data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Role updated successfully</response>
        /// <response code="400">Validation error or ID mismatch</response>
        /// <response code="404">Role not found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoleDto>> UpdateRole(Guid id, [FromBody] RoleUpdateDto updateDto, CancellationToken cancellationToken)
        {
            var updatedRole = await _roleService.UpdateRoleAsync(id, updateDto, cancellationToken);
            return Ok(updatedRole);
        }

        /// <summary>
        /// Delete a role by Id.
        /// </summary>
        /// <param name="id">Role Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">Role deleted successfully</response>
        /// <response code="404">Role not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRole(Guid id, CancellationToken cancellationToken)
        {
            await _roleService.DeleteRoleAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
