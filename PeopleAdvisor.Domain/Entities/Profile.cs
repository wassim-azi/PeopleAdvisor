namespace PeopleAdvisor.Domain.Entities;

public class Profile
{
    public Profile()
    {
        Experiences = new HashSet<Experience>();
    }

    public Guid Id { get; set; }
    public int? Age { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTime? CreationDate { get; set; }
    public string? Currency { get; set; }
    public double? CurrentSalary { get; set; }
    public string? Degree { get; set; }
    public string? Email { get; set; }
    public int? ExperienceYears { get; set; }
    public string? FirstName { get; set; }
    public string? Gender { get; set; }
    public string? Languages { get; set; }
    public string? LastName { get; set; }
    public string? Linkedin { get; set; }
    public string? NoticePeriod { get; set; }
    public string? ParsedResume { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? RemoteOnly { get; set; }
    public string? Role { get; set; }
    public string? School { get; set; }
    public string? Sector { get; set; }
    public string? Skills { get; set; }
    public string? StudyLevel { get; set; }
    public DateTime? UpdateDate { get; set; }

    // relationships
    public ICollection<Experience> Experiences { get; set; }
}