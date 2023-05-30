using BotterApi.Authentication;
using BotterApi.Context;
using BotterApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BotterApi.Controllers;

[ApiVersion("1.0")]
[Produces("application/json")]
[Route("v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _con;
    public AuthController(
        IConfiguration config,
        AppDbContext context)
    {
        _config = config;
        _con = context;
    }

    /// <summary>
    /// Register a new user to authetication
    /// </summary>
    /// <param name="user"></param>
    /// <returns>New user to DB</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RegisterAsync([FromBody] User user)
    {
        if (user == null) return BadRequest("Insert a valid body request");
        if (await _con.Users.AnyAsync(u => u.Name == user.Name)) return Conflict("This user name alredy exists");
        if (await _con.Users.AnyAsync(u => u.Email == user.Email)) return Conflict("This user email alredy exists");
        if (await _con.Users.AnyAsync(u => u.Nickname == user.Nickname)) return Conflict("This user nickname alredy exists");

        _con.Users.Add(user);
        await _con.SaveChangesAsync();

        Log.Information($"NEW USER ID {user.UserId} ADDED AT {DateTime.Now}");

        return Created($"/users/{user.UserId}", user);
    }

    /// <summary>
    /// Login with email and nickname
    /// </summary>
    /// <param name="login"></param>
    /// <returns>Token to login</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> LoginAsync([FromBody] Login login)
    {
        if (!login.ConfirmEmail.Equals(login.Email)) return BadRequest("The emails are not equal");
        var search = await _con.Users.Where(u=>u.Email==login.Email).FirstAsync();
        if (search is null) return BadRequest("Not found a user with that email");
        if (!search.Nickname.Equals(login.Nickname)) return BadRequest("Not user founded with that nickname");
        
        Log.Information($"NEW TOKEN FOR A USER (LOGIN): {search.Email} ADDED AT {DateTime.Now}");
        return Ok(GenerateToken(search.UserId));
    }
    private Token GenerateToken(int userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _config["TokenConfiguration:Issuer"],
            audience: _config["TokenConfiguration:Audience"],
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials
            );

        return new Token()
        {
            TokenAuth = new JwtSecurityTokenHandler().WriteToken(token),
            Message = $"New generated token at: {DateTime.UtcNow} - {DateTime.UtcNow.AddDays(1)} - User id: {userId}",
            UserId = userId
        };
    }
}
