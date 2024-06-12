using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantBookingSystemApi.Model.Admin;

[Table("User")]
public class AdminManagementModel
{
    [Key]
    public long UserId { get; set; }
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,16}$",
        ErrorMessage = "Password must be within 8  and 16 characters long and contain at least an uppercase letter, " +
        "a lowercase letter, a number, and a special character.")]
    public string Password { get; set; }

    [MaxLength(4)]
    [StringLength(4,ErrorMessage = "Branch Code must be 4 characters")]
    public string BranchCode { get; set; }
    public string UserRole {  get; set; }
    public bool IsActive { get; set; }
}