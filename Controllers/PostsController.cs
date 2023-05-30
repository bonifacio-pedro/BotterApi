using AutoMapper;
using BotterApi.Authentication;
using BotterApi.Context;
using BotterApi.Mapper;
using BotterApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BotterApi.Controllers;

[ApiVersion("1.0")]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("v1/posts")]
[ApiController]
public class PostsController: ControllerBase
{
    private readonly AppDbContext _con;
    private readonly IMapper _mapper;
    public PostsController(AppDbContext context, IMapper mapper)
    {
        _con = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Create a new post
    /// </summary>
    /// <remarks>
    /// Requires a token (login)
    /// </remarks>
    /// <param name="post"></param>
    /// <returns>A new post to DB</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> PostAsync([FromBody] Post post)
    {
        if (post is null) return BadRequest($"Enter a valid body request from post");
        if (!(await _con.Users.FindAsync(post.UserId) is User)) 
            return BadRequest($"This user does not exist");

        Log.Information($"NEW POST ID: {post.PostId} FROM THE USER ID: {post.UserId}  ADDED AT {DateTime.Now}");

        _con.Posts.Add(post);
        await _con.SaveChangesAsync();
        return Created($"/posts/{post.PostId}",post);
    }

    /// <summary>
    /// Get all posts from one user, with pagination
    /// </summary>
    /// <param name="id"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <returns>All posts from one user</returns>
    [HttpGet("user/{id:int}")]
    [ProducesResponseType(typeof(PostDTO),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PostDTO>>> GetAsyncPostsFromOneUser(
        [FromRoute] int id,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10)
    {
        if (id <= 0) return BadRequest("Enter a valid User ID");

        Log.Information($"GET ALL POSTS FROM ONE USER ID: {id} AT {DateTime.Now}");

        var userFind = await _con.Users.FindAsync(id);
        if (userFind is null) return NotFound($"User with ID: {id} not found");
        var postsFind = await _con.Posts
            .Where(p => p.UserId == id)
            .AsNoTracking()
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return Ok(_mapper.Map<List<PostDTO>>(postsFind));
    }

    /// <summary>
    /// Get all posts, with pagination
    /// </summary>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <returns>All posts</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PostDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PostDTO>>> GetAsyncAllPosts(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10)
    {
        Log.Information($"GET ALL POSTS AT {DateTime.Now}");
        var posts = await _con.Posts
            .AsNoTracking()
            .Skip(skip)
            .Take(take)
            .ToListAsync();
        return Ok(_mapper.Map<List<PostDTO>>(posts));
    }

    /// <summary>
    /// Update one post
    /// </summary>
    /// <param name="id"></param>
    /// <param name="postDto"></param>
    /// <returns>New updated post</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PostDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDTO>> PutAsync([FromRoute] int id, [FromBody] PostDTO postDto)
    {
        if (id <= 0) return BadRequest("Enter a valid User ID");

        var find = await _con.Posts.FindAsync(id);
        if (find is null) return NotFound($"User with ID: {id} not found");

        if (postDto.UserId != find.UserId || postDto.UserId != find.UserId)
            return BadRequest("You cannot change user id");

        find.Title = postDto.Title;
        find.Body = postDto.Body;
        find.IsPrivate = postDto.IsPrivate;
        find.Files = postDto.Files;

        _con.Posts.Update(find);
        await _con.SaveChangesAsync();

        Log.Information($"UPDATED USER ID: {id} AT {DateTime.Now}");

        return Ok(_mapper.Map<PostDTO>(find));
    }

    /// <summary>
    /// Delete one post
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

        var post = await _con.Posts.FindAsync(id);
        if (post is null) return NotFound($"Post with ID: {id} not found");

        Log.Information($"DELETED POST ID: {id} AT {DateTime.Now}");

        _con.Posts.Remove(post);
        await _con.SaveChangesAsync();

        return NoContent();
    }
}
