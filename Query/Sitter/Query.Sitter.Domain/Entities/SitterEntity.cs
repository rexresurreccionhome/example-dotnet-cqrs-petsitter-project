using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Query.Sitter.Domain.Entities;

[Table("Sitter")]
public class SitterEntity
{
    [Key]
    public required Guid SitterId { get; set; }
    public required Guid MemberId { get; set; }
    public required bool IsActive { get; set; }
}