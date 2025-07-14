namespace Models.Entities.Abstract;

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
    DateTime? DeletedAt { get; set; }
    bool IsDeleted { get; set; }
    bool IsActive { get; set; }
    string CreatedBy { get; set; } 
    string? ModifiedBy { get; set; }
    string? DeletedBy { get; set; }

}
