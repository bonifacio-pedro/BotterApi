using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BotterApi.Models;

public class Post
{
    [Key]
    public int PostId { get; set; }
    [Required]
    [StringLength(100)]
    public string? Title { get; set; }
    [Required]
    [StringLength(300)]
    public string? Body { get; set; }
    [Required]
    public bool IsPrivate { get; set; }
    [Required]
    public DateTime PostDate { get; set; }
    [StringLength(300)]
    public string? Files { get; set; }

    // Usuario
    public int UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

}
