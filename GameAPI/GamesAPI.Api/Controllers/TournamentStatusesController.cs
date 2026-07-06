using GamesAPI.Api.DTOs.TournamentStatuses;
using GamesAPI.Api.DTOs;
using GamesAPI.Api.Interfaces;
using GamesAPI.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GamesAPI.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentStatusesController : ControllerBase
    {
        private readonly ITournamentStatusService _service;

        public TournamentStatusesController(
            ITournamentStatusService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(new ApiResponse<List<Models.TournamentStatus>>
            {
                Success = true,
                Message = "Tournament statuses retrieved successfully.",
                Data = data
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);

            if (item == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Tournament status not found." });
            }

            return Ok(new ApiResponse<Models.TournamentStatus>
            {
                Success = true,
                Message = "Tournament status retrieved successfully.",
                Data = item
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTournamentStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = ModelState.Values
                        .Where(v => v!.Errors.Count > 0)
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var created = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                new ApiResponse<Models.TournamentStatus>
                {
                    Success = true,
                    Message = "Tournament status created successfully.",
                    Data = created
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateTournamentStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed.",
                    errors = ModelState
                        .Where(x => x.Value!.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage))
                });
            }

            var updated = await _service.UpdateAsync(id, request);

            if (updated == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Tournament status not found." });
            }

            return Ok(new ApiResponse<Models.TournamentStatus>
            {
                Success = true,
                Message = "Tournament status updated successfully.",
                Data = updated
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Tournament status not found." });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Tournament status deleted successfully."
            });
        }
    }
}
