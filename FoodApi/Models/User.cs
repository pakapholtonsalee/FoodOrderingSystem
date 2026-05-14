using System.ComponentModel.DataAnnotations.Schema;

[Table("Users")]
public class User
{
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    public required string Username { get; set; }

    [Column("role")]
    public required string Role { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}