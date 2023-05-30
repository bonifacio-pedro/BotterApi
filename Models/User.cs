using BotterApi.Validations;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BotterApi.Models;

public class User
{
    public User()
    {
        Posts = new Collection<Post>();
    }

    [Key]
    public int UserId { get; set; }
    [Required]
    [StringLength(200)]
    public string? Name { get; set; }
    [Required]
    [StringLength(100)]
    [EmailAddress(ErrorMessage ="Please enter a valid email address")]
    public string? Email { get; set; }
    [Required]
    [StringLength(80)]
    public string? Nickname { get; set; }
    [BirthDayValidation(ErrorMessage ="Enter a valid birthdate")]
    [DataType(DataType.Date)]
    public DateTime BirthDay { get; set; }
    [Required]
    [StringLength(300)]
    public string? Icon { get; set; }
    [Required]
    [StringLength(250)]
    public string? UserDescription { get; set; }

    [JsonIgnore]
    public ICollection<Post>? Posts { get; set; }
}
