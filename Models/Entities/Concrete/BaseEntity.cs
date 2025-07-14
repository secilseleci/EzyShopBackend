using Models.Entities.Abstract;

namespace Models.Entities.Concrete;

public class BaseEntity : IBaseEntity, IAuditable
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public  string? DeletedBy { get; set; }

}
