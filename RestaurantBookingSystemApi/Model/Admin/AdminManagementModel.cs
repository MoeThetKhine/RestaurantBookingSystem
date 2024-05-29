using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantBookingSystemApi.Model.Admin;

[Table("User")]
public class AdminManagementModel
{
    [Key]
    public long UserId {  get; set; }
    public string UserName { get; set; }
    public string Email {  get; set; }
    public string Password { get; set; }
    public string BranchCode { get; set; }
    public string UserRole {  get; set; }
    public bool IsActive { get; set; }
}