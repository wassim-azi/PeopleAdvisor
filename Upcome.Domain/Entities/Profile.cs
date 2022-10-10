using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Upcome.Domain.Entities;

[Table("Profile")]
public class Profile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public DateTime? CreationDate { get; set; } = DateTime.Now;

    public DateTime? UpdateDate { get; set; } = DateTime.Now;

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Degree { get; set; }

    public string School { get; set; }

    public string City { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public string Linkedin { get; set; }

    public int ExperienceDuration { get; set; }

    public string Languages { get; set; }

    public double Salary { get; set; }

    public string ResumeUrl { get; set; }

    public string ParsedResume { get; set; }
}