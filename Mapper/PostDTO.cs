namespace BotterApi.Mapper;

public class PostDTO
{
    public int PostId { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public bool IsPrivate { get; set; }
    public string? Files { get; set; }
    public int UserId { get; set; }
}
