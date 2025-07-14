using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Customer;

public class CustomerListViewModel
{
    [Required] 
    public Guid Id { get; set; }

    public string FullName { get; set; } = "N/A";

    public string Address { get; set; } = "N/A";

    public string Phone { get; set; } = "-";
    public string Email { get; set; } = "-";

}