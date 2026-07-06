using System.ComponentModel.DataAnnotations;

public class CreateTournamentRequest
{
    [Required(ErrorMessage = "Tournament name is required.")]
    public string Name
    {
        get;
        set;
    } = string.Empty;

    [Required(ErrorMessage = "Game is required.")]
    public Guid? GameId
    {
        get;
        set;
    }

    [Required(ErrorMessage = "Tournament type is required.")]
    public string TournamentType
    {
        get;
        set;
    } = string.Empty;

    [Required(ErrorMessage = "Maximum participants is required.")]
    [Range(
        1,
        int.MaxValue,
        ErrorMessage =
            "Maximum participants must be greater than 0.")]
    public int MaxParticipants
    {
        get;
        set;
    }
    [Required(ErrorMessage = "Start date is required.")]
    public DateTime? StartDate
    {
        get;
        set;
    }
    [Required(ErrorMessage = "End date is required.")]

    public DateTime? EndDate
    {
        get;
        set;
    }

    [MaxLength(
        500,
        ErrorMessage =
            "Description cannot exceed 500 characters.")]
    public string? Description
    {
        get;
        set;
    }

    public bool IsActive
    {
        get;
        set;
    } = true;
    [Required(ErrorMessage = "Registration start date is required.")]
    public DateTime? RegistrationStartDate { get; set; }

    [Required(ErrorMessage = "Registration end date is required.")]
    public DateTime? RegistrationEndDate { get; set; }
    [Range(1, 1,
         ErrorMessage = "Only single-player tournaments are supported currently.")]
    public int ParticipantsPerEntry { get; set; } = 1;
}