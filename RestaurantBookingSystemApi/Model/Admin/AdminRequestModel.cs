using System.ComponentModel.DataAnnotations;

namespace RestaurantBookingSystemApi.Model.Admin;

public class AdminRequestModel
{
    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,16}$",
        ErrorMessage = "Password must be within 8  and 16 characters long and contain at least an uppercase letter, " +
        "a lowercase letter, a number, and a special character.")]
    public string Password { get; set; }
    public string BranchCode { get; set; }
    public bool IsActive { get; set; }
}