using Microsoft.EntityFrameworkCore;
using GamesAPI.Api.Models;

namespace GamesAPI.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentStatus> TournamentStatuses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Logs> Logs { get; set; }
        public DbSet<TournamentParticipant> TournamentParticipants { get; set; }
        public DbSet<TournamentRound> TournamentRounds { get; set; }
        public DbSet<TournamentMatch> TournamentMatches { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        public DbSet<AnnouncementRecipient>
            AnnouncementRecipients
        { get; set; }
        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }

        public DbSet<ConversationMessage> ConversationMessages { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        protected override void OnModelCreating(
        ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TournamentMatch>()
                .HasOne(x => x.Player1)
                .WithMany(x => x.Player1Matches)
                .HasForeignKey(x => x.Player1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TournamentMatch>()
                .HasOne(x => x.Player2)
                .WithMany(x => x.Player2Matches)
                .HasForeignKey(x => x.Player2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TournamentMatch>()
                .HasOne(x => x.Winner)
                .WithMany(x => x.WonMatches)
                .HasForeignKey(x => x.WinnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TournamentMatch>()
                .HasOne(x => x.PreviousMatch1)
                .WithMany()
                .HasForeignKey(x => x.PreviousMatch1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TournamentMatch>()
                .HasOne(x => x.PreviousMatch2)
                .WithMany()
                .HasForeignKey(x => x.PreviousMatch2Id)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Tournament>()
                .HasOne(x => x.Winner)
                .WithMany()
                .HasForeignKey(x => x.WinnerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Notification>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Announcement>()
            .HasOne(x => x.Tournament)
            .WithMany()
            .HasForeignKey(x => x.TournamentId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Announcement>()
            .HasOne(x => x.Match)
            .WithMany()
            .HasForeignKey(x => x.MatchId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Announcement>()
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AnnouncementRecipient>()
            .HasOne(x => x.Announcement)
            .WithMany(x => x.AnnouncementRecipients)
            .HasForeignKey(x => x.AnnouncementId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnnouncementRecipient>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AnnouncementRecipient>()
            .HasOne(x => x.Announcement)
            .WithMany(x => x.AnnouncementRecipients)
            .HasForeignKey(x => x.AnnouncementId);
            modelBuilder.Entity<Conversation>()
            .HasOne(c => c.CreatedByUser)
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConversationParticipant>()
            .HasOne(cp => cp.Conversation)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => cp.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ConversationParticipant>()
            .HasOne(cp => cp.User)
            .WithMany()
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConversationMessage>()
            .HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ConversationMessage>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConversationParticipant>()
            .HasOne(cp => cp.LastSeenMessage)
            .WithMany(m => m.SeenByParticipants)
            .HasForeignKey(cp => cp.LastSeenMessageId)
            .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Invitation>()
            .HasOne(x => x.InvitedByUser)
            .WithMany()
            .HasForeignKey(x => x.InvitedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Invitation>()
            .Property(x => x.Status)
            .HasConversion<int>();
        }
    }
}