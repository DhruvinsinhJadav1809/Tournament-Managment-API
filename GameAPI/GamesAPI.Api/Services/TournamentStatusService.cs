using GamesAPI.Api.Data;
using GamesAPI.Api.DTOs.TournamentStatuses;
using GamesAPI.Api.Interfaces;
using GamesAPI.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GamesAPI.Api.Services
{
    public class TournamentStatusService : ITournamentStatusService
    {
        private readonly AppDbContext _context;

        public TournamentStatusService(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TournamentStatus>> GetAllAsync()
        {
            return await _context.TournamentStatuses
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<TournamentStatus?> GetByIdAsync(Guid id)
        {
            return await _context.TournamentStatuses
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<TournamentStatus> CreateAsync(CreateTournamentStatusRequest request)
        {
            var status = new TournamentStatus
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                IsActive = request.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.TournamentStatuses.Add(status);
            await _context.SaveChangesAsync();

            return status;
        }

        public async Task<TournamentStatus?> UpdateAsync(Guid id, UpdateTournamentStatusRequest request)
        {
            var status = await _context.TournamentStatuses
                .FirstOrDefaultAsync(x => x.Id == id);

            if (status == null)
            {
                return null;
            }

            status.Name = request.Name.Trim();
            status.IsActive = request.IsActive;

            await _context.SaveChangesAsync();

            return status;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var status = await _context.TournamentStatuses
                .FirstOrDefaultAsync(x => x.Id == id);

            if (status == null)
            {
                return false;
            }

            _context.TournamentStatuses.Remove(status);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
