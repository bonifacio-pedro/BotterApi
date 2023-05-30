using AutoMapper;
using BotterApi.Context;
using BotterApi.Mapper;
using BotterApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BotterApi.Controllers;

[ApiVersion("1.0")]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("v1/users")]
[ApiController]
public class UsersController: ControllerBase
{
    private readonly AppDbContext _con;
    private readonly IMapper _mapper;
    public UsersController(AppDbContext context, IMapper mapper)
    {
        _con = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Get one user
    /// </summary>
    /// <param name="id"></param>
    /// <returns>One user</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDTO>> GetOneAsync([FromRoute] int id)
    {
        if (id <= 0) return BadRequest("Enter a valid User ID");

        Log.Information($"GET ONE USER ID: {id} AT {DateTime.Now}");
        return await _con.Users.FindAsync(id) is User user
            ? Ok(_mapper.Map<UserDTO>(user))
            : NotFound($"User with ID: {id} not found");
    }

    /// <summary>
    /// Update one user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userDto"></param>
    /// <returns>Updated user</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDTO>> PutAsync([FromRoute] int id, [FromBody] UserDTO userDto)
    {
        if (id <= 0) return BadRequest("Enter a valid User ID");

        var find = await _con.Users.FindAsync(id);
        if (find is null) return NotFound($"User with ID: {id} not found");

        if (userDto.Name != find.Name || userDto.Email != find.Email) 
            return BadRequest("You cannot change name or email from an user");

        find.UserDescription = userDto.UserDescription;
        find.Nickname = userDto.Nickname;

        _con.Users.Update(find);
        await _con.SaveChangesAsync();

        Log.Information($"UPDATED USER ID: {id} AT {DateTime.Now}");

        return Ok(_mapper.Map<UserDTO>(find));
    }

    /// <summary>
    /// Delete one user
    /// </summary>
    /// <param name="id"></param>
    /// <returns>No content</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        if (id <= 0) return BadRequest("Enter a valid User ID");

        var user = await _con.Users.FindAsync(id);
        if (user is null) return NotFound($"User with ID: {id} not found");

        Log.Information($"DELETED USER ID: {id} AT {DateTime.Now}");

        _con.Users.Remove(user);
        await _con.SaveChangesAsync();

        return NoContent();
    }
}
