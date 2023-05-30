using System.ComponentModel.DataAnnotations;

namespace BotterApi.Authentication;

public class Login
{
    public string? Nickname { get; set; }
    public string? Email { get; set; }
    public string? ConfirmEmail { get; set; }
}
