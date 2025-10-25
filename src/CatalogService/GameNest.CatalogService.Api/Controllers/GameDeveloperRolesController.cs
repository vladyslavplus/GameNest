using GameNest.CatalogService.BLL.DTOs.GameDeveloperRoles;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.CatalogService.Api.Controllers
{
    [ApiController]
    [Route("api/Catalog/[controller]")]
    public class GameDeveloperRolesController : ControllerBase
    {
        private readonly IGameDeveloperRoleService _gameDeveloperRoleService;

        public GameDeveloperRolesController(IGameDeveloperRoleService gameDeveloperRoleService)
        {
            _gameDeveloperRoleService = gameDeveloperRoleService;
        }

        /// <summary>
        /// Get all game developer roles with pagination and filtering.
        /// </summary>
        /// <param name="parameters">Filtering and pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the paginated list of game developer roles</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GameDeveloperRoleDto>>> GetRoles([FromQuery] GameDeveloperRoleParameters parameters, CancellationToken cancellationToken)
        {
            var roles = await _gameDeveloperRoleService.GetRolesPagedAsync(parameters, cancellationToken);
            return Ok(roles);
        }

        /// <summary>
        /// Get a game developer role by Id.
        /// </summary>
        /// <param name="id">Role Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the role details</response>
        /// <response code="404">Role not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GameDeveloperRoleDto>> GetRoleById(Guid id, CancellationToken cancellationToken)
        {
            var role = await _gameDeveloperRoleService.GetRoleByIdAsync(id, cancellationToken);
            if (role == null) return NotFound();
            return Ok(role);
        }

        /// <summary>
        /// Create a new game developer role.
        /// </summary>
        /// <param name="dto">Role creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="201">Role created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GameDeveloperRoleDto>> CreateRole([FromBody] GameDeveloperRoleCreateDto dto, CancellationToken cancellationToken)
        {
            var createdRole = await _gameDeveloperRoleService.CreateRoleAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id }, createdRole);
        }

        /// <summary>
        /// Delete a game developer role by Id.
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
            await _gameDeveloperRoleService.DeleteRoleAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
