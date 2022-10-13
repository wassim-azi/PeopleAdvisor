namespace PeopleAdvisor.Domain.Entities;

public class Experience
{
    public Experience()
    {
        Profile = new Profile();
    }

    public Guid Id { get; set; }
    public string? Company { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsPrimary { get; set; }
    public DateTime? StartDate { get; set; }
    public string? Title { get; set; }

    // relationships IDs
    public Guid ProfileId { get; set; }

    // relationships objects
    public Profile Profile { get; set; }
}