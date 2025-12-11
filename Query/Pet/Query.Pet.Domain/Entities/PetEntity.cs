using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Query.Pet.Domain.Entities;

[Table("Pet")]
public class PetEntity
{
    [Key]
    public required Guid PetId { get; set; }
    public required Guid MemberId { get; set; }
    public required string Name { get; set; }
    public required bool IsActive { get; set; }
}
