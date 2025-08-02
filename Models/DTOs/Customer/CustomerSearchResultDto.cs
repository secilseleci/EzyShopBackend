namespace Models.DTOs.Customer;

public class CustomerSearchResultDto
{
    public Guid CustomerId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

}
