using BotterApi.Models;

namespace BotterApi.Mapper;

public class UserDTO
{
    public int UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Nickname { get; set; }
    public string? UserDescription { get; set; }
    public ICollection<Post>? Posts { get; set; }
}
